using Windows.Storage;

namespace Dodo.Logic
{
    public class SettingsService : ISettingsService
    {
        readonly ApplicationDataContainer _appSettings;

        public SettingsService()
        {
            var data = ApplicationData.Current.LocalSettings;

            _appSettings = data.CreateContainer("Settings", ApplicationDataCreateDisposition.Existing);
        }

        public bool ContainsKey(string key)
        {
            return _appSettings.Values.ContainsKey(key);
        }

        public T Get<T>(string key)
        {
            return (T)_appSettings.Values[key];
        }

        public void Set<T>(string key, T value)
        {
            _appSettings.Values[key] = value;
        }
    }
}