using System.Collections.Generic;
using System.Linq;
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
        BundleWebLoader bundleWebLoader = new BundleWebLoader();
        Coroutine downloadAssetBundleCoroutine = StartCoroutine(bundleWebLoader.DownloadAssetBundle(LoadAssetBundle, bundleUrl));
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

    private void LoadAssetBundle(AssetBundle assetBundle)
    {
        foreach (var o in assetBundle.LoadAllAssets())
        {
            var asset = (GameObject) o;
            InstantiateAssetAsChildFrom(GetExhibitAnchorWithName(asset.name), asset);
        }
    }

    private void InstantiateAssetAsChildFrom(GameObject parent, GameObject asset)
    {
        if (parent == null)
        {
            Debug.LogError("InstantiateAssetAsChildFrom: No parent for " + asset.name + " found!");
            return;
        }
        
        Instantiate(asset, parent.transform);
    }
}
