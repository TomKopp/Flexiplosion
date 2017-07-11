using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommonClassesLib.Event;

namespace CommonClassesLib.Model
{
    public class Log
    {
        private readonly List<LogEntry> _logEntries = new List<LogEntry>();

        public static readonly Log Instance = new Log();

        public event EventHandler<LogUpdatedEventArgs> LogUpdated;

        public delegate void LogUpdatedHandler(object sender, LogUpdatedEventArgs args);

        public List<LogEntry> LogEntries
        {
            get { return _logEntries; }
        }

        public static void LogMessage(LogEntry e)
        {
            Instance.LogEntries.Add(e);
            Instance.OnLogUpdated(e);
        }

        public static void LogMessage(string msg, LoggingLevel level=LoggingLevel.Notification)
        {
            var e = new LogEntry {Level = level, Message = msg};
            Instance.LogEntries.Add(e);
            Instance.OnLogUpdated(e);
        }

        public static void LogCommandExecuteFailedNoParam(ICommand cmd)
        {
            LogMessage("Tried to execute " + cmd.GetType().FullName + " with invalid parameter. Parameter is null or Whitespace.", LoggingLevel.Warning);
        }

        public static void LogCommandSucessfullyExecuted(ICommand cmd, object parameter = null, string message = "", LoggingLevel level = LoggingLevel.Trace)
        {
            String param = (parameter != null) ? parameter.ToString() : "";

            if (parameter == null)
            {
                LogMessage("Sucessfully executed " + cmd.GetType().FullName + "." + message, level);
            }
            else
            {
                LogMessage("Sucessfully executed " + cmd.GetType().FullName + " with Parameter: \"" + param + "\"." +
                           message, level);
            }

        }

        public static void LogException(object sender, string methodName, Exception exc, LoggingLevel level = LoggingLevel.Exception, string message="")
        {
            LogMessage("Error in " + sender.GetType().FullName + "( " + methodName + "). " + message + "Exception is :" + exc.Message, level);
        }

        private void OnLogUpdated(LogEntry e)
        {
            if (LogUpdated != null)
                LogUpdated(null, new LogUpdatedEventArgs { Entry = e});
        }

    }
}
