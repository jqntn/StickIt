using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;
class BuildPlayer
{
    static void StandaloneWindows64(BuildOptions bo = BuildOptions.None)
    {
        string path = "Build_" + DateTime.Now.ToString("dd.MM.yy_HH.mm") + "/";
        Directory.CreateDirectory(path);
        string[] dirs = Directory.GetDirectories(".", "Build*", SearchOption.TopDirectoryOnly);
        if (dirs != null)
            foreach (var i in dirs) Directory.Delete(i, true);
        else Directory.CreateDirectory(path);
        BuildPipeline.BuildPlayer(
            Directory.GetFiles("Assets/Scenes", "*.unity"), path + "Plat.exe",
            BuildTarget.StandaloneWindows64,
            BuildOptions.CompressWithLz4HC | bo
        );
        File.Delete(path + "UnityCrashHandler64.exe");
        ZipFile.CreateFromDirectory(path, path.Remove(path.Length - 1) + ".zip");
    }
    [MenuItem("BuildPlayer/Build")]
    static void Build() { StandaloneWindows64(); }
    [MenuItem("BuildPlayer/BuildRun")]
    static void BuildRun() { StandaloneWindows64(BuildOptions.AutoRunPlayer); }
    [MenuItem("BuildPlayer/BuildShow")]
    static void BuildShow() { StandaloneWindows64(BuildOptions.ShowBuiltPlayer); }
}