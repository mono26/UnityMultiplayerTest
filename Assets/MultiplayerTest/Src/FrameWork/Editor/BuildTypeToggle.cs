using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static class BuildTypeToggle
{
    [MenuItem("Tools/Build Type/Client", false, 20)]
    public static void SetIsClientBuild()
    {
        BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
        string rawSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        List<string> symbols = rawSymbols.Split(new String[] { ";" }, StringSplitOptions.None).ToList();

        if (symbols.Contains("GAME_SERVER")) {
            symbols.Remove("GAME_SERVER");
        }

        if (!symbols.Contains("GAME_CLIENT")) {
            symbols.Add("GAME_CLIENT");
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", symbols));
    }

    [MenuItem("Tools/Build Type/Client", true)]
    public static bool ValidateSetIsClientBuild()
    {
#if GAME_CLIENT
        Menu.SetChecked("Tools/Build Type/Client", true);

        return false;
#else
        Menu.SetChecked("Tools/Build Type/Client", false);

        return !EditorApplication.isPlaying;
#endif
    }

    [MenuItem("Tools/Build Type/Server", false, 21)]
    public static void SetIsServerBuild()
    {
        BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
        string rawSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        List<string> symbols = rawSymbols.Split(new String[] { ";" }, StringSplitOptions.None).ToList();

        if (symbols.Contains("GAME_CLIENT")) {
            symbols.Remove("GAME_CLIENT");
        }

        if (!symbols.Contains("GAME_SERVER")) {
            symbols.Add("GAME_SERVER");
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", symbols));
    }

    [MenuItem("Tools/Build Type/Server", true)]
    public static bool ValidEnableMenuItem()
    {
#if GAME_SERVER
        Menu.SetChecked("Tools/Build Type/Server", true);

        return false;
#else
        Menu.SetChecked("Tools/Build Type/Server", false);

        return !EditorApplication.isPlaying;
#endif
    }
}
