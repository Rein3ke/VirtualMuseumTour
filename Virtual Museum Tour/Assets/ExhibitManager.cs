using UnityEngine;

public class ExhibitManager : MonoBehaviour
{
    public ExhibitManager Instance => this;

    public string bundleUrl = "http://localhost/assetbundles/testbundle";
    private void Start()
    {
        GameObject[] exhibitAnchors = GameObject.FindGameObjectsWithTag("ExhibitAnchor");

        BundleWebLoader bundleWebLoader = new BundleWebLoader();
        Coroutine downloadAssetBundleCoroutine = StartCoroutine(bundleWebLoader.DownloadAssetBundle(LoadAssetBundle, bundleUrl));
    }

    private void LoadAssetBundle(AssetBundle assetBundle)
    {
        foreach (var o in assetBundle.LoadAllAssets())
        {
            var asset = (GameObject) o;
            InstantiateAsset(asset);
        }
    }

    private void InstantiateAsset(GameObject asset)
    {
        Instantiate(asset);
    }
}
