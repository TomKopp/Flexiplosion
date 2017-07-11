using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CommonClassesLib.Model;

namespace FlexiWallUI.Converter
{
    [ValueConversion(typeof(LoggingLevel), typeof(Color))]
    public class LoggingLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                Log.LogException(this, new StackTrace().GetFrame(1).GetMethod().Name, new NullReferenceException("No value for conversion from LoggingLevel to Color provided."));
                return null;
            }
            var lvl = (LoggingLevel)value;

            //TODO: error prone in case of refactoring: find cleaner way to implement
            switch (lvl)
            {
                case LoggingLevel.Debug:
                    return Application.Current.Resources["LoggingDebugColor"];
                case LoggingLevel.Trace:
                    return Application.Current.Resources["LoggingTraceColor"];
                case LoggingLevel.Notification:
                    return Application.Current.Resources["LoggingNotificationColor"];
                case LoggingLevel.Warning:
                    return Application.Current.Resources["LoggingWarningColor"];
                case LoggingLevel.Error:
                    return Application.Current.Resources["LoggingErrorColor"];
                case LoggingLevel.Exception:
                    return Application.Current.Resources["LoggingExceptionColor"];
            }

            Log.LogMessage("Provided LoggingLevel is not implemented. At " + GetType().FullName + "." + new StackTrace().GetFrame(1).GetMethod().Name + ".", LoggingLevel.Warning);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                Log.LogException(this, new StackTrace().GetFrame(1).GetMethod().Name, new NullReferenceException("No value for conversion from Color to LoggingLevel provided."));
                return null;
            }

            var c = (Color)value;

            //TODO: error prone in case of refactoring: find cleaner way to implement
            if (c.Equals(Application.Current.Resources["LoggingDebugColor"]))
                return LoggingLevel.Debug;

            if (c.Equals(Application.Current.Resources["LoggingTraceColor"]))
                return LoggingLevel.Trace;

            if (c.Equals(Application.Current.Resources["LoggingNotificationColor"]))
                return LoggingLevel.Notification;

            if (c.Equals(Application.Current.Resources["LoggingWarningColor"]))
                return LoggingLevel.Warning;

            if (c.Equals(Application.Current.Resources["LoggingErrorColor"]))
                return LoggingLevel.Error;

            if (c.Equals(Application.Current.Resources["LoggingExceptionColor"]))
                return LoggingLevel.Exception;

            Log.LogMessage("Provided Color is not associated to LoggingLevel. At " + GetType().FullName + "." + new StackTrace().GetFrame(1).GetMethod().Name + ".", LoggingLevel.Warning);
            return null;

        }
    }
}