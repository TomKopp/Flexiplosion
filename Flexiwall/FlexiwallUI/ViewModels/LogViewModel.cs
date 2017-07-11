using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using CommonClassesLib.Event;
using CommonClassesLib.Model;
using Prism.Commands;
using Prism.Mvvm;

namespace FlexiWallUI.ViewModels
{
    public class LogViewModel : BindableBase
    {
        #region Fields

        private readonly Window _logWindow;

        #endregion


        private static readonly String NoFilter = "None";

        private string _selectedFilter = NoFilter;

        public List<String> Filters { get; set; }

        public String SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                if (_selectedFilter == value)
                    return;

                _selectedFilter = value;
                RaisePropertyChanged(nameof(SelectedFilter));
                ApplyFilter();
            }
        }

        public ObservableCollection<LogEntry> LogEntries { get; private set; }

        public ObservableCollection<LogEntry> FilteredLogEntries { get; private set; }

        public ICommand LogWindowCmd { get; private set; }

        public ICommand ClearLogCmd { get; private set; }

        public ICommand SaveLogCmd { get; private set; }

        public LogViewModel(Window logWindow)
        {
            LogEntries = new ObservableCollection<LogEntry>();
            FilteredLogEntries = new ObservableCollection<LogEntry>();
            
            Filters = new List<string> { NoFilter };
            Enum.GetNames(typeof (LoggingLevel)).ToList().ForEach(i => Filters.Add(i));
            SelectedFilter = Filters[0];

            Log.Instance.LogUpdated += UpdateLogEntries;
            _logWindow = logWindow;
            LogWindowCmd = new DelegateCommand<object>(ToggleLogWindowVisibility);
            ClearLogCmd = new DelegateCommand<object>(ClearLog);
            SaveLogCmd = new DelegateCommand<object>(SaveLog);
        }

        private void UpdateLogEntries(object sender, LogUpdatedEventArgs e)
        {
            LogEntries.Add(e.Entry);
            if (_selectedFilter == NoFilter || e.Entry.Level == (LoggingLevel)Enum.Parse(typeof(LoggingLevel), _selectedFilter))
                FilteredLogEntries.Add(e.Entry);
        }

        private void ApplyFilter()
        {
            FilteredLogEntries.Clear();
            if (_selectedFilter == NoFilter)
            {
                LogEntries.ToList().ForEach(e => FilteredLogEntries.Add(e));
            }
            else
            {
                var filterLvl = (LoggingLevel) Enum.Parse(typeof (LoggingLevel), _selectedFilter);
                LogEntries.ToList().ForEach(e =>
                {
                    if (e.Level == filterLvl)
                        FilteredLogEntries.Add(e);
                });
            }

            RaisePropertyChanged(nameof(FilteredLogEntries));
        }

        #region Command Implementations

        public void ToggleLogWindowVisibility(object parameter)
        {
            var s = parameter as String;

            if (string.IsNullOrWhiteSpace(s))
            {
                Log.LogCommandExecuteFailedNoParam(LogWindowCmd);
                return;
            }

            if (s.Equals("ToggleVisibility"))
            {


                if (_logWindow.WindowState != WindowState.Minimized)
                {
                    _logWindow.Hide();
                    _logWindow.WindowState = WindowState.Minimized;
                }
                else
                {
                    _logWindow.WindowState = WindowState.Normal;
                    _logWindow.Show();
                }

                Log.LogCommandSucessfullyExecuted(LogWindowCmd, s);
                return;
            }

            throw new NotImplementedException("Value " + s + " is not connected with an implemented action.");
        }

        public void ClearLog(object parameter)
        {
            LogEntries.Clear();
            Log.LogCommandSucessfullyExecuted(ClearLogCmd, null, "Sucessfully cleared log.");
        }

        public void SaveLog(object parameter)
        {
            var ser = new XmlSerializer(typeof(List<LogEntry>));
            var fileName = $"log_{DateTime.Now.Date.ToShortDateString()}.xml";
            var writer = XmlWriter.Create(fileName);
            ser.Serialize(writer, Log.Instance.LogEntries);

            Log.LogCommandSucessfullyExecuted(SaveLogCmd, null, $"Sucessfully saved log to \"{fileName}\".", LoggingLevel.Notification);
        }

        #endregion
    }
}
