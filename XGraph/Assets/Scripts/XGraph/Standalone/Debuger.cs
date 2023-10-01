namespace XGraph
{
    public class XGraphDebuger
    {
        public delegate void DebugLogEvent(string message);

        public static event DebugLogEvent OnDebugLogEvent;
        public static event DebugLogEvent OnDebugLogWarningEvent;
        public static event DebugLogEvent OnDebugLogErrorEvent;
        
        public static void Log(string message)
        {
            OnDebugLogEvent?.Invoke(message);
        }
        
        public static void LogWarning(string message)
        {
            OnDebugLogWarningEvent?.Invoke(message);
        }
        
        public static void LogError(string message)
        {
            OnDebugLogErrorEvent?.Invoke(message);
        }
        
    }
}