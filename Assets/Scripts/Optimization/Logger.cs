using System;
using UnityEngine;

public static class Logger {
    [System.Diagnostics.Conditional("ENABLE_LOG")]
    static public void Log(object message) {
        UnityEngine.Debug.Log(message);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    static public void LogWarning(object message) {
        UnityEngine.Debug.LogWarning(message);
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    static public void LogError(object message) {
        UnityEngine.Debug.LogError(message);
    }
}