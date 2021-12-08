using System.IO;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(MMFeedbacksManager))]
public class ToolMMFeedbacksManager : MonoBehaviour
{
    private string fileMMFeedbacksManager = "MMFeedbacksManager.cs";
    private string fileGameEvents = "GameEvents.cs";
    private string subFolder = "/StickIt/Scripts/Utils/"; 
    private string startListeners = "// | Listeners";
    private string startCalls= "// | Calls";
    private string startEvents= "// | Events";
    private List<MMFeedbacks> feedbacksList;

    /*
    private string sourceFileName = "CopyGameEvents.cs";
    private string endListeners = "// | End Listeners";
    private string endCalls = "// | End Calls";
    private string endEvents= "// | End Events";
    */
    public void UpdateFiles()
    {
        MMFeedbacksManager manager = GetComponent<MMFeedbacksManager>();
        feedbacksList = manager.feedbacksList;
        string pathMMFeedbacksManager = GetFilePath(fileMMFeedbacksManager);
        string pathGameEvents = GetFilePath(fileGameEvents);
        UpdateGameEvents(pathGameEvents);
        UpdateMMFeedbacksManager(pathMMFeedbacksManager);
    }

    private void UpdateGameEvents(string path)
    {
        string text = File.ReadAllText(path);
        int index = text.IndexOf(startEvents) + startEvents.Length;
        foreach (MMFeedbacks feedbacks in feedbacksList)
        {
            int indexSearch = text.IndexOf(feedbacks.name);
            // Feedbacks Not Found Add it
            if(indexSearch == -1)
            {
                 text = text.Insert(index,
                    "\n\tpublic static FeelEvent " +
                     feedbacks.name +
                    "Event = new FeelEvent();"
                    );
                File.WriteAllText(path, text);
            }
        }
    }

    private void UpdateMMFeedbacksManager(string path)
    {
        string text = File.ReadAllText(path);
        int indexListeners = text.IndexOf(startListeners) + startListeners.Length;

        for (int i = 0; i < feedbacksList.Count; i++)
        {
            int indexSearch = text.IndexOf(feedbacksList[i].name);
            if (indexSearch == -1)
            {
                text = text.Insert(indexListeners,
                "\n\t\tGameEvents." +
                feedbacksList[i].name +
                "Event.AddListener(" +
                feedbacksList[i].name +
                "Call" +
                ");"
                );
                File.WriteAllText(path, text);

                int indexCalls = text.IndexOf(startCalls) + startCalls.Length;
                text = text.Insert(indexCalls,
                    "\n\tpublic void " + feedbacksList[i].name + "Call(float duration, float intensity)\n\t{" +
                    "\n\t\tif (!feedbacksList[" + i + "].IsPlaying){" +
                    "\n\t\t\tfloat durationMultiplier = duration / feedbacksList[" + i + "].TotalDuration;" +
                    "\n\t\t\tfeedbacksList[" + i + "].FeedbacksIntensity = intensity;" +
                    "\n\t\t\tfeedbacksList[" + i + "].DurationMultiplier = durationMultiplier;" +
                    "\n\t\t\tfeedbacksList[" + i + "].PlayFeedbacks();" +
                    "\n\t\t}" +
                    "\n\t}"
                );
                File.WriteAllText(path, text);
            }
        }
    }
    private string GetFilePath(string filename)
    {
        return Application.dataPath + subFolder + filename;
    }

    /*
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
    */
}
