using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class CreateAssetBundles
    {
        private const string AssetBundleDirectory = "Assets/StreamingAssets";

        private static readonly BuildTarget[] SupportedBuildTargets =
        {
            BuildTarget.StandaloneWindows,
            BuildTarget.WebGL,
            BuildTarget.NoTarget
        };

        private static void BuildAssetBundle(BuildTarget target)
        {
            Debug.LogWarning($"Building AssetBundles for target: {target}");

            var assetBundleDirectory = AssetBundleDirectory + "/" + target.ToString();
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, target);

            Debug.LogWarning($"AssetBundles built to dir: {assetBundleDirectory}");
        }

        [MenuItem("Assets/Build AssetBundles/Windows")]
        private static void BuildAssetBundlesWindows()
        {
            BuildAssetBundle(BuildTarget.StandaloneWindows);
        }

        [MenuItem("Assets/Build AssetBundles/WebGL")]
        private static void BuildAssetBundlesWebGl()
        {
            BuildAssetBundle(BuildTarget.WebGL);
        }

        [MenuItem("Assets/Build AssetBundles/All")]
        private static void BuildAssetBundlesAll()
        {
            foreach (BuildTarget target in SupportedBuildTargets)
            {
                BuildAssetBundle(target);
            }
        }
    }
}