using System;
using CommonClassesLib.Model;

namespace CommonClassesLib.Event
{
    public class LogUpdatedEventArgs : EventArgs
    {
        public LogEntry Entry { get; set; }
    }
}
