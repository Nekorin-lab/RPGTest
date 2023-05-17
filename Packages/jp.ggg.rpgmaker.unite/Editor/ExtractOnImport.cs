using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

internal static class ExtractOnImport
{
    private static string kAssetPackagePath = "Packages/jp.ggg.rpgmaker.unite/rpgmaker_assets.unitypackage";
    private static string kRPGMakerFolderPath = "Assets/RPGMaker";
    
    [InitializeOnLoadMethod]
    private static void ImportRPGMakerAssetsOnImport()
    {
        if (File.Exists(kAssetPackagePath) && !Directory.Exists(kRPGMakerFolderPath))
        {
            AssetDatabase.ImportPackage(kAssetPackagePath, false);
        }
        else if (File.Exists(kAssetPackagePath) && Directory.Exists(kRPGMakerFolderPath))
        {
            File.Delete(kAssetPackagePath);
        }
    }
}
