using Microsoft.MixedReality.WorldLocking.Tools;
using UnityEngine;

namespace Logging
{
    public class HoloLensLogCapturer : MonoBehaviour
    {
        void OnEnable()
        {
            Application.logMessageReceived += LogMessage;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= LogMessage;
        }

        private static void LogMessage(string message, string stackTrace, LogType type)
        {
            SimpleConsole.AddLine(1, message);
            SimpleConsole.AddLine(1, stackTrace);
        }
    }
}