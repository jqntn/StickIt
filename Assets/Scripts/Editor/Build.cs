using System;
using System.IO;
using UnityEditor;
class ScriptBatch
{
    static void BuildPlayer(BuildOptions bo = BuildOptions.None)
    {
        string path = "Build";
        Directory.Delete(path, true);
        Directory.CreateDirectory(path);
        BuildPipeline.BuildPlayer(
            Directory.GetFiles("Assets/Scenes", "*.unity"), string.Format("{0}/{1}.exe", path, DateTime.Now.ToString("dd.MM.yy HH.mm")),
            BuildTarget.StandaloneWindows64,
            BuildOptions.CompressWithLz4HC | bo
        );
    }
    [MenuItem("BuildPlayer/Build")]
    static void Build() { BuildPlayer(); }
    [MenuItem("BuildPlayer/BuildRun")]
    static void BuildRun() { BuildPlayer(BuildOptions.AutoRunPlayer); }
    [MenuItem("BuildPlayer/BuildShow")]
    static void BuildShow() { BuildPlayer(BuildOptions.ShowBuiltPlayer); }
}