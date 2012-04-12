using Windows.Storage;

namespace Dodo.Logic
{
    public class SettingsService : ISettingsService
    {
        private ApplicationDataContainer appSettings;

        public SettingsService()
        {
            var data = ApplicationData.Current.LocalSettings;

            appSettings = data.CreateContainer("Settings", ApplicationDataCreateDisposition.Existing);
        }

        public bool ContainsKey(string key)
        {
            return appSettings.Values.ContainsKey(key);
        }

        public T Get<T>(string key)
        {
            return (T)appSettings.Values[key];
        }

        public void Set<T>(string key, T value)
        {
            appSettings.Values[key] = value;
        }
    }
}