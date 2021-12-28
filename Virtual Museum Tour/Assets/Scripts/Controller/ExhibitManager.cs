using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Controller
{
    public class ExhibitManager : MonoBehaviour
    {
        public static ExhibitManager Instance { get; private set; }

        [SerializeField] private string bundleUrl = "http://localhost/assetbundles/testbundle";
    
        private List<GameObject> _exhibitAnchors;
        public Dictionary<string, Exhibit> ExhibitDictionary { get; private set; }

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

        private void Start()
        {
            ExhibitDictionary = new Dictionary<string, Exhibit>();
            
            // first, load all exhibit anchors...
            _exhibitAnchors = new List<GameObject>(GetAllExhibitAnchors());

            // secondly, start asset bundle download...
            StartCoroutine(BundleWebLoader.DownloadAssetBundle(LoadAndUnpackAssetBundle, bundleUrl));
        }

        private GameObject[] GetAllExhibitAnchors()
        {
            return GameObject.FindGameObjectsWithTag("ExhibitAnchor");
        }

        private GameObject GetExhibitAnchorWithName(string assetName)
        {
            return _exhibitAnchors.FirstOrDefault(anchor => anchor.GetComponent<ExhibitAnchor>().ExhibitID.Equals(assetName));
        }

        private void LoadAndUnpackAssetBundle(AssetBundle assetBundle)
        {
            if (assetBundle == null)
            {
                Debug.LogError("LoadAndUnpackAssetBundle: AssetBundle can't be null!");
                return;
            }

            foreach (var o in assetBundle.LoadAllAssets())
            {
                switch (o)
                {
                    case GameObject exhibitAsset:
                    {
                        RegisterExhibit(exhibitAsset);
                        /*var anchor = GetExhibitAnchorWithName(exhibitAsset.name);
                        InstantiateAssetAsChildFrom(anchor, exhibitAsset);*/
                        break;
                    }
                    case ExhibitData exhibitData:
                    {
                        RegisterExhibit(exhibitData);
                        /*var anchor = GetExhibitAnchorWithName(exhibitData.exhibitName);
                        if (anchor == null) break;
                        exhibitAnchor.GetComponent<ExhibitAnchor>().ExhibitData = exhibitData;*/
                        break;
                    }
                }
            }

            foreach (var valuePair in ExhibitDictionary)
            {
                InstantiateAssetAsChildFrom(valuePair.Value.Anchor, valuePair.Value.Asset);
            }
        }

        private void RegisterExhibit(GameObject exhibitAsset)
        {
            var exhibitID = exhibitAsset.name;

            if (ExhibitDictionary.TryGetValue(exhibitID, out var exhibit))
            {
                exhibit.Asset = exhibitAsset;
            }
            else
            {
                exhibit = new Exhibit
                {
                    Anchor = GetExhibitAnchorWithName(exhibitID),
                    Name = exhibitID,
                    Asset = exhibitAsset
                };
                ExhibitDictionary.Add(exhibitID, exhibit);
            }
        }

        private void RegisterExhibit(ExhibitData exhibitData)
        {
            var exhibitID = exhibitData.exhibitName;

            if (ExhibitDictionary.TryGetValue(exhibitID, out var exhibit))
            {
                exhibit.ExhibitData = exhibitData;
            }
            else
            {
                exhibit = new Exhibit
                {
                    ExhibitData = exhibitData,
                    Anchor = GetExhibitAnchorWithName(exhibitID),
                    Name = exhibitID
                };
                ExhibitDictionary.Add(exhibitID, exhibit);
            }
        }

        private void InstantiateAssetAsChildFrom(GameObject parent, GameObject child)
        {
            if (parent == null || child == null)
            {
                Debug.LogError("InstantiateAssetAsChildFrom: Parent or child can't be null!");
                return;
            }

            // add tag to exhibit and its children
            child.tag = "Exhibit";
            for (var index = 0; index < child.transform.childCount; index++)
            {
                var childOfChild = child.transform.GetChild(index).gameObject;
                childOfChild.tag = "Exhibit";
                if (childOfChild.GetComponent<Renderer>() != null)
                {
                    if (childOfChild.GetComponent<Collider>() == null) childOfChild.AddComponent<MeshCollider>();
                }
            }

            Instantiate(child, parent.transform);
            Debug.Log($"Instantiate {child.name} as a child from [{parent.GetType()}]{parent.GetComponent<ExhibitAnchor>().ExhibitID}.");
        }
    }
}
