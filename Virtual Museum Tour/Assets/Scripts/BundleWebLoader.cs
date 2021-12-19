using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class BundleWebLoader
{
    public static IEnumerator DownloadAssetBundle(Action<AssetBundle> assetBundleCallback, string bundleUrl)
    {
        UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);

        // Wait for completion...
        yield return webRequest.SendWebRequest();
        Debug.Log("DownLoadAssetBundle: Web request sent!");

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("DownLoadAssetBundle: " + webRequest.error);
            yield break;
        }
        else
        {
            Debug.Log("DownLoadAssetBundle: " + webRequest.result);
        }

        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);

        assetBundleCallback(bundle);
        
        bundle.Unload(false);
        webRequest.Dispose();
    }
    
}
