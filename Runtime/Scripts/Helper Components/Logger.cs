using System;
using UnityEngine;

namespace HHG.Common.Runtime
{
    public class Logger : MonoBehaviour
    {
        [Header("Log as Exception:")]
        [SerializeField] private bool Errors;
        [SerializeField] private bool Asserts;
        [SerializeField] private bool Warnings;
        [SerializeField] private bool Logs;

        private void OnEnable()
        {
            if (Application.isEditor)
            {
                return;
            }

            Application.logMessageReceived += OnLogMessageReceived;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }

        private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error when Errors:
                case LogType.Assert when Asserts:
                case LogType.Warning when Warnings:
                case LogType.Log when Logs:
                    Exception exception = new Exception($"{logString}\n{stackTrace}");
                    Debug.LogException(exception);
                    break;
            }
        }
    }
}