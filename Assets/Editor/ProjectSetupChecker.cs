using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ProjectSetupChecker
{
    private const string RavenfieldAssetsPath = "Assets/Ravenfield";
    private const string ManifestPath = "Packages/manifest.json";
    private const string ProjectVersionPath = "ProjectSettings/ProjectVersion.txt";
    private const string TextMeshProPackage = "com.unity.textmeshpro";
    private const string UguiPackage = "com.unity.ugui";
    private static readonly string[] GeneratedFolders = { "Library", "Temp", "obj", "Build", "Logs" };

    [MenuItem("Tools/Ravenfield/Run Project Setup Check")]
    public static void RunProjectSetupCheck()
    {
        Debug.Log("Ravenfield project setup check started.");

        bool success = true;

        success &= CheckFolderExists(RavenfieldAssetsPath);
        success &= CheckFileExists(ManifestPath);
        success &= CheckFileExists(ProjectVersionPath);

        if (File.Exists(ManifestPath))
        {
            string manifestText = File.ReadAllText(ManifestPath);
            success &= CheckPackageListed(manifestText, TextMeshProPackage, "TextMeshPro");
            success &= CheckPackageListed(manifestText, UguiPackage, "UGUI");
        }

        success &= CheckNoGeneratedFoldersInAssets();
        success &= CheckSteamworksUsage();

        if (success)
        {
            Debug.Log("Project setup check completed: no issues found.");
        }
        else
        {
            Debug.LogWarning("Project setup check completed: some issues were detected. Review the warnings and errors above.");
        }
    }

    private static bool CheckFolderExists(string path)
    {
        if (Directory.Exists(path))
        {
            Debug.Log($"OK: Found folder '{path}'.");
            return true;
        }

        Debug.LogError($"Missing folder: '{path}' not found.");
        return false;
    }

    private static bool CheckFileExists(string path)
    {
        if (File.Exists(path))
        {
            Debug.Log($"OK: Found file '{path}'.");
            return true;
        }

        Debug.LogError($"Missing file: '{path}' not found.");
        return false;
    }

    private static bool CheckPackageListed(string manifestText, string packageName, string friendlyName)
    {
        if (manifestText.Contains(packageName))
        {
            Debug.Log($"OK: {friendlyName} package '{packageName}' is listed in manifest.json.");
            return true;
        }

        Debug.LogError($"Missing package: {friendlyName} package '{packageName}' is not listed in manifest.json.");
        return false;
    }

    private static bool CheckNoGeneratedFoldersInAssets()
    {
        bool success = true;
        foreach (string folder in GeneratedFolders)
        {
            string path = Path.Combine("Assets", folder);
            if (Directory.Exists(path))
            {
                Debug.LogWarning($"Generated folder found under Assets: '{path}'. This folder should not be committed to Git.");
                success = false;
            }
        }

        if (success)
        {
            Debug.Log("OK: No generated folders found inside Assets/.");
        }

        return success;
    }

    private static bool CheckSteamworksUsage()
    {
        string manifestText = File.Exists(ManifestPath) ? File.ReadAllText(ManifestPath) : string.Empty;
        bool steamworksDefined = IsSteamworksDefinePresent();
        bool foundSteamworksUsage = false;

        string[] csFiles = Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories);
        foreach (string file in csFiles)
        {
            string[] lines = File.ReadAllLines(file);
            if (lines.Any(line => line.TrimStart().StartsWith("using Steamworks;")))
            {
                Debug.LogWarning($"Steamworks usage found in file: {file}");
                foundSteamworksUsage = true;
            }
        }

        if (foundSteamworksUsage && !steamworksDefined)
        {
            Debug.LogWarning("STEAMWORKS is not defined, but Steamworks references exist. Add a compile define or stub Steamworks code to avoid errors.");
            return false;
        }

        if (!foundSteamworksUsage)
        {
            Debug.Log("OK: No Steamworks using directives found in Assets/.");
            return true;
        }

        Debug.Log("OK: Steamworks references found and STEAMWORKS define is present.");
        return true;
    }

    private static bool IsSteamworksDefinePresent()
    {
        BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        bool present = symbols.Split(';').Select(s => s.Trim()).Any(s => s == "STEAMWORKS");
        Debug.Log($"STEAMWORKS scripting define present: {present}");
        return present;
    }
}
