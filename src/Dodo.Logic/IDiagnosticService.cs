using System;
using System.Text;
using Windows.Storage;

namespace Dodo.Logic
{
    public interface IDiagnosticService
    {
        void LogError(string page, Exception exception);
    }

    public class DiagnosticService : IDiagnosticService
    {
        readonly ApplicationDataContainer _errors;

        public DiagnosticService()
        {
            var data = ApplicationData.Current.LocalSettings;

            _errors = data.CreateContainer("Errors", ApplicationDataCreateDisposition.Always);   
        }

        public void LogError(string page, Exception exception)
        {
            string existing;

            if (_errors.Values.ContainsKey(page))
            {
                existing = _errors.Values[page].ToString();
            }
            else
            {
                existing = "";
            }

           
            var sb = new StringBuilder(existing);
            LogErrorInternal("Error:", exception, sb);
            _errors.Values[page] = sb.ToString();
        }

        private static void LogErrorInternal(string type, Exception exception, StringBuilder sb)
        {
            sb.AppendFormat(type + " {1}{0} Timestamp {2}{0} Stack {3}{0}", Environment.NewLine, exception.Message, DateTimeOffset.Now, exception.StackTrace);

            if (exception.InnerException != null)
            {
                LogErrorInternal("Inner:", exception.InnerException, sb);
            }
        }
    }
}
