using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class BundleWebLoader
{
    /// <summary>
    /// Accepts a callback method and an AssetBundle URL and executes the callback method after an AssetBundle is successfully downloaded.
    /// </summary>
    /// <param name="callbackMethod">Action (callback method) of type AssetBundle to which the downloaded AssetBundle should be passed.</param>
    /// <param name="bundleUrl">The URL of the AssetBundle.</param>
    /// <returns>Nothing (Coroutine)</returns>
    public static IEnumerator DownloadAssetBundle(Action<AssetBundle> callbackMethod, string bundleUrl)
    {
        var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);

        // Wait for completion...
        yield return webRequest.SendWebRequest();
        Debug.Log($"{nameof(DownloadAssetBundle)}: Web request sent!");

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{nameof(DownloadAssetBundle)}: {webRequest.error}");
            yield break;
        }
        Debug.Log($"{nameof(DownloadAssetBundle)}: {webRequest.result}");

        var bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
        callbackMethod(bundle);
        
        bundle.Unload(false);
        webRequest.Dispose();
    }
    
}
