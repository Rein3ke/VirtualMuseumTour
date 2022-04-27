using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Utility
{
    /// <summary>
    /// Helper class for downloading AssetBundles from the internet.
    /// </summary>
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

            
            Debug.Log($"{nameof(DownloadAssetBundle)}: Web request sent!");
            yield return webRequest.SendWebRequest(); // Wait for completion...

            if (webRequest.result != UnityWebRequest.Result.Success) // If the request failed, throw an exception and stop the coroutine.
            {
                Debug.LogError($"{nameof(DownloadAssetBundle)}: {webRequest.error}");
                yield break;
            }
            Debug.Log($"{nameof(DownloadAssetBundle)}: {webRequest.result}");

            var bundle = DownloadHandlerAssetBundle.GetContent(webRequest); // Get the AssetBundle from the web request.
            callbackMethod(bundle); // Pass the AssetBundle to the callback method.
        
            // Unload the bundle and dispose the web request to free up memory.
            bundle.Unload(false);
            webRequest.Dispose();
        }
    }
}
