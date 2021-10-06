using System;
using System.IO;
using UnityEditor;
class ScriptBatch
{
    static void BuildPlayer(BuildOptions bo = BuildOptions.None)
    {
        string path = "Build_" + DateTime.Now.ToString("dd.MM.yy_HH.mm") + "/";
        Directory.CreateDirectory(path);
        string[] dirs = Directory.GetDirectories(".", "Build_*", SearchOption.TopDirectoryOnly);
        if (dirs != null) Directory.Delete(dirs[0], true);
        else Directory.CreateDirectory(path);
        BuildPipeline.BuildPlayer(
            Directory.GetFiles("Assets/Scenes", "*.unity"), path + "Plat.exe",
            BuildTarget.StandaloneWindows64,
            BuildOptions.CompressWithLz4HC | bo
        );
        File.Delete(path + "UnityCrashHandler64.exe");
    }
    [MenuItem("BuildPlayer/Build")]
    static void Build() { BuildPlayer(); }
    [MenuItem("BuildPlayer/BuildRun")]
    static void BuildRun() { BuildPlayer(BuildOptions.AutoRunPlayer); }
    [MenuItem("BuildPlayer/BuildShow")]
    static void BuildShow() { BuildPlayer(BuildOptions.ShowBuiltPlayer); }
}