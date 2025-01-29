using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHG.Common.Runtime
{
    public class LogHandler : MonoBehaviour, ILogHandler
    {
        [Header("Log as Exception:")]
        [SerializeField] private bool errors;
        [SerializeField] private bool asserts;
        [SerializeField] private bool warnings;
        [SerializeField] private bool logs;

        private ILogHandler defaultLogHandler = Debug.unityLogger.logHandler;

        private void OnEnable()
        {
            Debug.unityLogger.logHandler = this;
        }

        private void OnDisable()
        {
            Debug.unityLogger.logHandler = defaultLogHandler;
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            string formatted = FormatLog(string.Format(format, args), context);
            
            switch (logType)
            {
                case LogType.Error when errors:
                case LogType.Assert when asserts:
                case LogType.Warning when warnings:
                case LogType.Log when logs:
                    System.Exception exception = new System.Exception(formatted);
                    defaultLogHandler.LogException(exception, context);
                    break;
                default:
                    defaultLogHandler.LogFormat(logType, context, formatted, args);
                    break;
            }
        }

        public void LogException(System.Exception exception, Object context)
        {
            string formatted = FormatLog($"{exception.Message}\n{exception.StackTrace}", context);
            defaultLogHandler.LogFormat(LogType.Exception, context, formatted);
        }

        private string FormatLog(string format, Object context)
        {
            string scene = SceneManager.GetActiveScene().name;

            string path = context switch
            {
                GameObject g => g.GetPath(),
                Component c => c.GetPath(),
                _ => string.Empty
            };

            return $"\n{format}\nScene: {scene}\nPath: {path}";
        }
    }
}