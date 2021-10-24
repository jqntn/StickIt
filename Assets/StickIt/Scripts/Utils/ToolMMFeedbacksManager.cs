using System.IO;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(MMFeedbacksManager))]
public class ToolMMFeedbacksManager : MonoBehaviour
{
    private string fileMMFeedbacksManager = "MMFeedbacksManager.cs";
    private string fileGameEvents = "GameEvents.cs";
    private string sourceFileName = "CopyGameEvents.cs";
    private string subFolder = "/StickIt/Scripts/Utils/"; 
    private string startListeners = "// | Listeners";
    private string endListeners = "// | End Listeners";
    private string startCalls= "// | Calls";
    private string endCalls= "// | End Calls";
    private string startEvents= "// | Events";
    private string endEvents= "// | End Events";
    private List<MMFeedbacks> feedbacksList;
    public void GenerateFiles()
    {
        string pathMMFeedbacksManager = GetFilePath(fileMMFeedbacksManager);
        string pathGameEvents = GetFilePath(fileGameEvents);
        string pathSource = GetFilePath(sourceFileName);

        if (!File.Exists(pathGameEvents))
        {
            File.Create(pathGameEvents);
            File.Copy(pathSource, pathGameEvents);
        }

        MMFeedbacksManager manager = GetComponent<MMFeedbacksManager>();
        feedbacksList = manager.feedbacksList;

        WriteInGameEvents(pathGameEvents);
        WriteInMMFeedbacksManager(pathMMFeedbacksManager);
        Debug.Log("File Generated");
    }

    private string GetFilePath(string filename)
    {
        return Application.dataPath + subFolder + filename;
    }

    private void DeleteOldText(string path, ref string text, string startText, string endText)
    {
        int start = text.IndexOf(startText) + startText.Length;
        int end = text.IndexOf(endText, start);
        text = text.Remove(start, end - start);
        File.WriteAllText(path, text);
    }
    private void WriteInMMFeedbacksManager(string path)
    {
        string text = File.ReadAllText(path);
        DeleteOldText(path, ref text, startListeners, endListeners);
        DeleteOldText(path, ref text, startCalls, endCalls);
        int index = text.IndexOf(startListeners) + startListeners.Length;
        foreach (MMFeedbacks feedbacks in feedbacksList)
        {
            text = text.Insert(index,
                "\n\t\tGameEvents." +
                feedbacks.name +
                "Event.AddListener(" +
                feedbacks.name +
                "Call" +
                ");"
             );
            File.WriteAllText(path, text);
        }

        text = text.Insert(text.IndexOf(endListeners), "\n\t\t");
        File.WriteAllText(path, text);

        index = text.IndexOf(startCalls) + startCalls.Length;
        for (int i = 0; i < feedbacksList.Count; i++)
        {
            text = text.Insert(index,
                "\n\tpublic void " + feedbacksList[i].name + "Call()\n\t{" +
                "\n\t\tif (!feedbacksList[" + i + "].IsPlaying){" +
                "\n\t\t\tfeedbacksList[" + i + "].PlayFeedbacks();" +
                "\n\t\t}" +
                "\n\t}"
             );
            File.WriteAllText(path, text);
        }

        text = text.Insert(text.IndexOf(endCalls), "\n\t");
        File.WriteAllText(path, text);
    }

    private void WriteInGameEvents(string path)
    {
        string text = File.ReadAllText(path);
        DeleteOldText(path, ref text, startEvents, endEvents);
        int index = text.IndexOf(startEvents) + startEvents.Length;
        foreach (MMFeedbacks feedbacks in feedbacksList)
        {
            text = text.Insert(index,
                "\n\tpublic static UnityEvent " +
                 feedbacks.name +
                "Event = new UnityEvent();"
                );
            File.WriteAllText(path, text);
        }

        text = text.Insert(text.IndexOf(endEvents), "\n\t");
        File.WriteAllText(path, text);
    }
}
