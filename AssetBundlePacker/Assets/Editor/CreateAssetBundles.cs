using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Editor script to create an AssetBundle.
    /// </summary>
    public static class CreateAssetBundles
    {
        #region Constants

        /// <summary>
        /// Default path to save the AssetBundles to.
        /// </summary>
        private const string AssetBundleDirectory = "Assets/StreamingAssets";

        #endregion

        #region Static Members
        
        /// <summary>
        /// Contains a list of all supported build targets.
        /// </summary>
        private static readonly BuildTarget[] SupportedBuildTargets =
        {
            BuildTarget.StandaloneWindows,
            BuildTarget.WebGL,
            BuildTarget.NoTarget
        };

        #endregion

        /// <summary>
        /// Receives a given build target and creates an AssetBundle for each GameObject that has an AssetBundle tag.
        /// </summary>
        /// <param name="target">Build target platform.</param>
        private static void BuildAssetBundle(BuildTarget target)
        {
            Debug.LogWarning($"Building AssetBundles for target: {target}");

            var assetBundleDirectory = AssetBundleDirectory + "/" + target.ToString(); // Path to save the AssetBundles to.
            if (!Directory.Exists(assetBundleDirectory)) // Create the directory if it doesn't exist.
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, target); // Build the AssetBundles using the build pipeline with standard settings.

            Debug.LogWarning($"AssetBundles built to dir: {assetBundleDirectory}");
        }

        #region Menu Items

        /// <summary>
        /// Adds a menu item to the Unity Editor to build AssetBundles for the PC Standalone platform.
        /// </summary>
        [MenuItem("Assets/Build AssetBundles/Windows")]
        private static void BuildAssetBundlesWindows()
        {
            BuildAssetBundle(BuildTarget.StandaloneWindows);
        }

        /// <summary>
        /// Adds a menu item to the Unity Editor to build AssetBundles for the WebGL platform.
        /// </summary>
        [MenuItem("Assets/Build AssetBundles/WebGL")]
        private static void BuildAssetBundlesWebGl()
        {
            BuildAssetBundle(BuildTarget.WebGL);
        }

        /// <summary>
        /// Adds a menu item to the Unity Editor to build AssetBundles for all supported platforms.
        /// </summary>
        [MenuItem("Assets/Build AssetBundles/All")]
        private static void BuildAssetBundlesAll()
        {
            foreach (BuildTarget target in SupportedBuildTargets)
            {
                BuildAssetBundle(target);
            }
        }

        #endregion
    }
}