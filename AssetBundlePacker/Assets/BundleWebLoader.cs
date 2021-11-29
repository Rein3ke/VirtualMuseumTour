using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class BundleWebLoader : MonoBehaviour
{
    public string bundleUrl = "http://localhost/assetbundles/testbundle";
    public string assetName = "BundledSpriteObject";

    /*private IEnumerator Start()
    {
        using (WWW web = new WWW(bundleUrl))
        {
            yield return web;

            AssetBundle remoteAssetBundle = web.assetBundle;

            if (remoteAssetBundle == null)
            {
                Debug.LogError("Failed to download AssetBundle!");
                yield break;
            }

            Instantiate(remoteAssetBundle.LoadAsset(assetName));
            remoteAssetBundle.Unload(false);
        }
    }*/

    private IEnumerator Start()
    {
        UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);

        // Wait for completion...
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(webRequest.error);
            yield break;
        }
        else
        {
            Debug.Log(webRequest.result);
        }

        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);

        if (bundle == null)
        {
            Debug.LogError("Failed to download AssetBundle!");
            yield break;
        }

        Instantiate(bundle.LoadAsset(assetName, typeof(GameObject)));
        bundle.Unload(false);
    }
}
