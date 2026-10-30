using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public int _maxLineInStack = 16;
    public TMP_Text textLogger;

    private readonly Queue<string> _logLines = new Queue<string>();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // ngine shit
        string cleanLog = logString.Split("UnityEngine")[0].TrimEnd();

        _logLines.Enqueue(cleanLog);

        while (_logLines.Count > _maxLineInStack)
        {
            _logLines.Dequeue();
        }

        var reversedLines = new List<string>(_logLines);
        reversedLines.Reverse();
        textLogger.text = "> " + string.Join("\n", reversedLines);
    }

    public void ClearConsole()
    {
        _logLines.Clear();
        if (textLogger != null) textLogger.text = "";
        print("\nConsole Cleared");
    }
}