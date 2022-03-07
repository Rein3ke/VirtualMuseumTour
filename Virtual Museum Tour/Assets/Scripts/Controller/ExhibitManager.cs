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
    public class ExhibitManager : MonoBehaviour
    {
        [Header("Asset Bundle")] [SerializeField]
        private string bundleUrl = "http://localhost/assetbundles/";

        [SerializeField] private string bundleName = "testbundle";

        /// <summary>
        /// Public singleton instance. Instance of the script must exist only once.
        /// </summary>
        public static ExhibitManager Instance { get; private set; }

        /// <summary>
        /// A directory that stores all exhibits with an associated key.
        /// </summary>
        public static Dictionary<string, Exhibit> ExhibitDictionary { get; } = new Dictionary<string, Exhibit>();

        /// <summary>
        /// Set Instance to this if Instance isn't null. Otherwise destroy gameObject.
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
            // register events
            // SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
            
            // start asset bundle download...
            StartCoroutine(BundleWebLoader.DownloadAssetBundle(LoadAndUnpackAssetBundle, bundleUrl + bundleName));
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventExhibitionsPlaced, OnExhibitionsPlaced);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventExhibitionsPlaced, OnExhibitionsPlaced);
        }

        /// <summary>
        /// Receives an AssetBundle and iterates through it until all objects in it could be assigned to a type.
        /// The method therefore filters out all GameObjects and ExhibitData scripts.
        /// </summary>
        /// <param name="assetBundle">The AssetBundle to unpack.</param>
        private void LoadAndUnpackAssetBundle([NotNull] AssetBundle assetBundle)
        {
            if (assetBundle == null)
            {
                Debug.LogError($"{nameof(LoadAndUnpackAssetBundle)}: AssetBundle can't be null!", this);
                return;
            }

            foreach (var o in assetBundle.LoadAllAssets())
            {
                switch (o)
                {
                    case GameObject exhibitAsset:
                    {
                        RegisterExhibit(exhibitAsset);
                        break;
                    }
                    case ExhibitData exhibitData:
                    {
                        RegisterExhibit(exhibitData);
                        break;
                    }
                }
            }
        }

        private void RegisterExhibit([NotNull] GameObject exhibitAsset)
        {
            var exhibitID = exhibitAsset.name;
            Debug.Log($"Register new exhibit: {exhibitID}");

            if (ExhibitDictionary.TryGetValue(exhibitID, out var exhibit))
            {
                exhibit.Asset = exhibitAsset;
            }
            else
            {
                exhibit = new Exhibit
                {
                    // Anchor = ExhibitAnchor.GetExhibitAnchor(exhibitID)?.gameObject,
                    Name = exhibitID,
                    Asset = exhibitAsset
                };
                ExhibitDictionary.Add(exhibitID, exhibit);
            }
        }

        private void RegisterExhibit([NotNull] ExhibitData exhibitData)
        {
            var exhibitID = exhibitData.exhibitName;
            Debug.Log($"Register new exhibit data: {exhibitID}");

            if (ExhibitDictionary.TryGetValue(exhibitID, out var exhibit))
            {
                exhibit.ExhibitData = exhibitData;
            }
            else
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

        private void InstantiateAsChildFrom([NotNull] GameObject parent, [NotNull] GameObject child)
        {
            if (parent == null || child == null)
            {
                var errorMessage = parent == null ? "parent" : "child";
                Debug.LogError($"InstantiateAsChildFrom: {errorMessage} can't be null!");
                return;
            }

            // add tag to exhibit and its children
            child.tag = "Exhibit";
            for (var index = 0; index < child.transform.childCount; index++)
            {
                var childOfChild = child.transform.GetChild(index).gameObject;
                childOfChild.tag = "Exhibit";
                if (childOfChild.GetComponent<Renderer>() != null && childOfChild.GetComponent<Collider>() == null)
                {
                    childOfChild.AddComponent<MeshCollider>();
                }
            }

            Instantiate(child, parent.transform);
            Debug.Log($"Instantiate {child.name} as a child from [{parent.GetType()}]{parent.GetComponent<ExhibitAnchor>().ExhibitID}.");
        }

        private static void RefreshExhibitAnchors()
        {
            // refresh exhibit anchors
            foreach (var exhibit in ExhibitDictionary.Select(exhibitEntry => exhibitEntry.Value))
            {
                exhibit.Anchor = ExhibitAnchor.GetExhibitAnchor(exhibit.Name)?.gameObject;
            }
        }

        private void PlaceExhibitsInScene()
        {
            // place exhibits on anchors
            foreach (var exhibit in ExhibitDictionary.Select(exhibitEntry => exhibitEntry.Value))
            {
                if (exhibit.Anchor == null) continue;
                InstantiateAsChildFrom(exhibit.Anchor, exhibit.Asset);
            }
        }

        #region Event Handling

        private void OnExhibitionsPlaced(EventParam eventParam)
        {
            RefreshExhibitAnchors();
            PlaceExhibitsInScene();
        }

        #endregion
    }
}