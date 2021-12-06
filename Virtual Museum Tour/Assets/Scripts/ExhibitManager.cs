using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class ExhibitManager : MonoBehaviour
{
    public ExhibitManager Instance => this;

    public string bundleUrl = "http://localhost/assetbundles/testbundle";
    private List<GameObject> _exhibitAnchors;

    private void Start()
    {
        // First, load all exhibit anchors...
        LoadAllExhibitAnchors();

        // Start asset bundle download...
        Coroutine downloadAssetBundleCoroutine = StartCoroutine(BundleWebLoader.DownloadAssetBundle(LoadAndUnpackAssetBundle, bundleUrl));
    }

    private void LoadAllExhibitAnchors()
    {
        // If null, declare field...
        _exhibitAnchors ??= new List<GameObject>();
        
        _exhibitAnchors.AddRange(GameObject.FindGameObjectsWithTag("ExhibitAnchor"));
        Debug.Log("LoadAllExhibitAnchors: " + _exhibitAnchors.Count + " exhibit anchors loaded.");
    }

    private GameObject GetExhibitAnchorWithName(string assetName)
    {
        return _exhibitAnchors.FirstOrDefault(exhibitAnchor => exhibitAnchor.GetComponent<ExhibitAnchor>().ExhibitID.Equals(assetName));
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
                case GameObject exhibitGameObject:
                {
                    InstantiateAssetAsChildFrom(GetExhibitAnchorWithName(exhibitGameObject.name), exhibitGameObject);
                    break;
                }
                case ExhibitData exhibitData:
                    Debug.Log($"ExhibitData found. Name: {exhibitData.exhibitName}");
                    GameObject exhibitAnchor = GetExhibitAnchorWithName(exhibitData.exhibitName);
                    if (exhibitAnchor == null) break;
                    exhibitAnchor.GetComponent<ExhibitAnchor>().ExhibitData = exhibitData;
                    break;
            }
        }
    }

    private void InstantiateAssetAsChildFrom(GameObject parent, GameObject child)
    {
        if (parent == null || child == null)
        {
            Debug.LogError("InstantiateAssetAsChildFrom: Parent or child can't be null!");
            return;
        }

        Instantiate(child, parent.transform);
        Debug.Log($"Instantiate {child.name} as a child from [{parent.GetType()}]{parent.GetComponent<ExhibitAnchor>().ExhibitID}.");
    }
}
