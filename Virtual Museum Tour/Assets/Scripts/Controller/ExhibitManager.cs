using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Controller
{
    public class ExhibitManager : MonoBehaviour
    {
        public static ExhibitManager Instance { get; private set; }

        [Header("Asset Bundle")]
        [SerializeField] private string bundleUrl = "http://localhost/assetbundles/";
        [SerializeField] private string bundleName = "testbundle";
        
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

            // start asset bundle download...
            StartCoroutine(BundleWebLoader.DownloadAssetBundle(LoadAndUnpackAssetBundle, bundleUrl + bundleName));
        }

        private void LoadAndUnpackAssetBundle([NotNull] AssetBundle assetBundle)
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
                        InstantiateAsChildFrom(anchor, exhibitAsset);*/
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

            // instantiate each exhibit in its own anchor
            foreach (var valuePair in ExhibitDictionary)
            {
                InstantiateAsChildFrom(valuePair.Value.Anchor, valuePair.Value.Asset);
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
                    Anchor = ExhibitAnchor.GetExhibitAnchor(exhibitID)?.gameObject,
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
                    Anchor = ExhibitAnchor.GetExhibitAnchor(exhibitID)?.gameObject,
                    Name = exhibitID
                };
                ExhibitDictionary.Add(exhibitID, exhibit);
            }
        }

        private void InstantiateAsChildFrom([NotNull] GameObject parent, [NotNull] GameObject child)
        {
            if (parent == null || child == null)
            {
                Debug.LogError("InstantiateAsChildFrom: Parent or child can't be null!");
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
