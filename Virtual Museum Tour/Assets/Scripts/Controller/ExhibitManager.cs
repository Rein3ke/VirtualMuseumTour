using System;
using System.Collections.Generic;
using System.Linq;
using com.rein3ke.virtualtour.core;
using Events;
using JetBrains.Annotations;
using UnityEngine;
using Utility;
using EventType = Events.EventType;

namespace Controller
{
    /// <summary>
    /// Stores the URL to a directory of AssetBundles.
    /// One entry in the AssetBundle name array represents one AssetBundle in that directory.
    /// </summary>
    [Serializable]
    public struct AssetPackage {
        public string URL;
        public string[] AssetBundleNames;
    }
    
    /// <summary>
    /// Starts all downloads of all AssetPackages defined in the Inspector at the beginning.
    /// After downloading an AssetBundle, the exhibits are configured and stored in a dictionary.
    /// If an AssetBundle contains an ExhibitData object, it is assigned to the corresponding exhibit.
    /// </summary>
    public class ExhibitManager : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private AssetPackage[] assetPackages;

        #endregion

        #region Properties

        /// <summary>
        /// Public singleton instance. Instance of the script must exist only once.
        /// </summary>
        public static ExhibitManager Instance { get; private set; }

        /// <summary>
        /// A directory that stores all exhibits with an associated key.
        /// </summary>
        public static Dictionary<string, Exhibit> ExhibitDictionary { get; } = new Dictionary<string, Exhibit>();

        #endregion

        #region Unity Methods

        /// <summary>
        /// Sets the singleton instance. Instance of the script must exist only once.
        /// Otherwise, the script is destroyed.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        /// <summary>
        /// Starts a coroutine to download an associated AssetBundle (set via SerializedField). 
        /// </summary>
        private void Start()
        {
            // start the download of all AssetBundles defined in the Inspector
            foreach (AssetPackage assetPackage in assetPackages)
            {
                foreach (var bundleString in assetPackage.AssetBundleNames)
                {
                    StartCoroutine(BundleWebLoader.DownloadAssetBundle(LoadAndUnpackAssetBundle, assetPackage.URL + bundleString));
                }
            }
        }

        /// <summary>
        /// Subscribes to the EventExhibitionsPlaced, as well as the EventAssetBundleLoaded event. Both events call the OnExhibitionsPlaced method.
        /// Whenever the exhibits are placed in the main scene or when an AssetBundle is successfully loaded, the exhibits should be instantiated. 
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventExhibitionsPlaced, OnExhibitionsPlaced);
            EventManager.StartListening(EventType.EventAssetBundleLoaded, OnExhibitionsPlaced);
        }

        /// <summary>
        /// Unsubscribes from all events.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventExhibitionsPlaced, OnExhibitionsPlaced);
            EventManager.StopListening(EventType.EventAssetBundleLoaded, OnExhibitionsPlaced);
        }

        #endregion

        #region Exhibit Download, Configuration and Storage

        /// <summary>
        /// Receives an AssetBundle and iterates through it until all objects in it could be assigned to a type.
        /// The method therefore filters out all GameObjects and ExhibitData scripts.
        /// </summary>
        /// <param name="assetBundle">The AssetBundle to unpack.</param>
        private void LoadAndUnpackAssetBundle([NotNull] AssetBundle assetBundle)
        {
            if (assetBundle == null) // return, if an error occured (e.g. AssetBundle is corrupted)
            {
                Debug.LogError($"{nameof(LoadAndUnpackAssetBundle)}: AssetBundle can't be null!", this);
                return;
            }

            Debug.Log("Load and unpack asset bundle...", this);
            foreach (var o in assetBundle.LoadAllAssets()) // iterate through each object inside the AssetBundle
            {
                switch (o)
                {
                    case GameObject exhibitAsset: // if the object is a GameObject, register it as an Exhibit object
                    {
                        RegisterExhibit(exhibitAsset);
                        break;
                    }
                    case ExhibitData exhibitData: // if the object is an ExhibitData, register it as an ExhibitData object
                    {
                        RegisterExhibit(exhibitData);
                        break;
                    }
                }
            }
            Debug.Log("Asset bundle loaded and unpacked!", this);
            
            EventManager.TriggerEvent(EventType.EventAssetBundleLoaded, new EventParam()); // Trigger the EventAssetBundleLoaded event, so that the exhibit(s) can be instantiated
        }
        
        /// <summary>
        /// Receives an GameObject and registers it as an Exhibit object in the ExhibitDictionary.
        /// If the id of the Exhibit is already in the dictionary, the method will assign the GameObject to the existing Exhibit.
        /// Otherwise the method will create a new Exhibit by the given GameObject.
        /// </summary>
        /// <param name="exhibitAsset">GameObject, which stands for an Exhibit.</param>
        private void RegisterExhibit([NotNull] GameObject exhibitAsset)
        {
            var exhibitID = exhibitAsset.name; // get the name of the GameObject and use it as the Exhibit ID to iterate through the dictionary
            Debug.Log($"Register new exhibit: {exhibitID}");

            if (ExhibitDictionary.TryGetValue(exhibitID, out var exhibit)) // try to get the Exhibit from the dictionary by its ID
            {
                exhibit.Asset = exhibitAsset; // if found, assign the new GameObject to the existing Exhibit
            }
            else // if not found, create a new Exhibit object, assign the ID and the asset and add it to the ExhibitDictionary
            {
                exhibit = new Exhibit
                {
                    Name = exhibitID,
                    Asset = exhibitAsset
                };
                ExhibitDictionary.Add(exhibitID, exhibit);
            }
        }

        /// <summary>
        /// Receives an ExhibitData object and either registers it as an new Exhibit object in the ExhibitDictionary or sets the ExhibitData of an existing Exhibit object.
        /// </summary>
        /// <param name="exhibitData">Object, that contains the data of an Exhibit.</param>
        private void RegisterExhibit([NotNull] ExhibitData exhibitData)
        {
            var exhibitID = exhibitData.exhibitName; // get the name of the ExhibitData object and use it as the Exhibit ID to iterate through the dictionary
            Debug.Log($"Register new exhibit data: {exhibitID}");

            if (ExhibitDictionary.TryGetValue(exhibitID, out var exhibit)) // try to get the Exhibit from the dictionary by its ID
            {
                exhibit.ExhibitData = exhibitData; // if found, assign the ExhibitData to the existing Exhibit
            }
            else // if not found, create a new Exhibit object, assign the ID as well as the ExhibitData object to it and add it to the ExhibitDictionary
            {
                exhibit = new Exhibit
                {
                    ExhibitData = exhibitData,
                    // Anchor = ExhibitAnchor.GetExhibitAnchor(exhibitID)?.gameObject,
                    Name = exhibitID
                };
                ExhibitDictionary.Add(exhibitID, exhibit);
            }
        }

        #endregion

        #region Exhibit Placement 

        /// <summary>
        /// Receives a GameObject (exhibit asset), as well as an ExhibitAnchor (as GameObject in the scene).
        /// Configures the asset so that it can be detected by the SelectionManager.cs via a RayCast.
        /// Instantiates the asset at the position of the ExhibitAnchor GameObject.
        /// </summary>
        /// <param name="anchor">The GameObject associated with the ExhibitAnchor in the scene.</param>
        /// <param name="exhibitGameObject">The asset stored in the exhibit object.</param>
        private void InstantiateExhibit([NotNull] GameObject anchor, [NotNull] GameObject exhibitGameObject)
        {
            if (anchor == null || exhibitGameObject == null) // return, if anchor or exhibit asset is null
            {
                var errorMessage = anchor == null ? "anchor" : "exhibit";
                Debug.LogError($"InstantiateExhibit: {errorMessage} can't be null!");
                return;
            }

            // add "Exhibit" tag to the exhibit and its children (SelectionManager.cs will handle the selection of the exhibit)
            exhibitGameObject.tag = "Exhibit";
            for (var index = 0; index < exhibitGameObject.transform.childCount; index++)
            {
                var childGameObject = exhibitGameObject.transform.GetChild(index).gameObject;
                
                childGameObject.tag = "Exhibit";
                childGameObject.layer = LayerMask.NameToLayer("RaycastTarget"); // set the layer of the child GameObject to "RaycastTarget"
                
                // if the child GameObject has a Renderer but no Collider attached, add a MeshCollider to it
                if (childGameObject.GetComponent<Renderer>() != null && childGameObject.GetComponent<Collider>() == null)
                {
                    // add a MeshCollider to the child GameObject, since it cannot be assumed what geometric shape the asset has
                    childGameObject.AddComponent<MeshCollider>();
                }
                
                // Disable animations if the gameObject has an Animator attached
                var animator = childGameObject.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.enabled = false;
                }
            }

            var exhibit = Instantiate(exhibitGameObject, anchor.transform); // finally, instantiate the exhibit asset at the anchor's position
            var exhibitAnchor = anchor.GetComponent<ExhibitAnchor>(); // get the ExhibitAnchor component to get the exhibit direction and scale
            
            exhibit.transform.LookAt(exhibitAnchor.ExhibitDirection); // rotate the exhibit to look at the Vector3 that is defined in the ExhibitAnchor
            exhibit.transform.localScale = new Vector3(exhibitAnchor.ExhibitScale, exhibitAnchor.ExhibitScale, exhibitAnchor.ExhibitScale); // scale the exhibit to the size defined in the ExhibitAnchor

            Debug.Log($"Instantiate {exhibitGameObject.name} as a exhibitGameObject from [{anchor.GetType()}]{exhibitAnchor.ExhibitID}.");
        }

        /// <summary>
        /// Iterates through each exhibit in the dictionary, determines a corresponding ExhibitAnchor, and assigns that anchor to the exhibit.
        /// If no anchor is found, assign null.
        /// </summary>
        private static void RefreshExhibitAnchors()
        {
            // refresh exhibit anchors
            foreach (var exhibit in ExhibitDictionary.Select(exhibitEntry => exhibitEntry.Value))
            {
                exhibit.Anchor = ExhibitAnchor.GetExhibitAnchor(exhibit.Name)?.gameObject;
            }
        }

        /// <summary>
        /// Iterates through each exhibit in the dictionary and instantiates the exhibit if it has not been instantiated yet.
        /// </summary>
        private void PlaceExhibitsInScene()
        {
            // place exhibits on anchors
            foreach (var exhibit in ExhibitDictionary.Select(exhibitEntry => exhibitEntry.Value))
            {
                if (exhibit.Anchor == null) continue; // if no anchor is found, skip this exhibit
                
                if (exhibit.Anchor.GetComponent<ExhibitAnchor>().ContainsExhibit) continue; // if anchor already contains an exhibit, skip this exhibit
                
                InstantiateExhibit(exhibit.Anchor, exhibit.Asset); // instantiate exhibit at anchor position as child of this anchor
            }
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Gets called when all exhibitions are placed in the scene or when an AssetBundle is loaded successfully.
        /// Ensures that all exhibits are placed in the scene. Whether at the beginning of the main scene or after the download is complete.
        /// </summary>
        /// <param name="eventParam">(Obsolete).</param>
        private void OnExhibitionsPlaced(EventParam eventParam)
        {
            RefreshExhibitAnchors(); // assign all anchors to their exhibits
            PlaceExhibitsInScene(); // call this method to place all exhibits in the scene
        }

        #endregion
    }
}