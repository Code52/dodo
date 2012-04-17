using Newtonsoft.Json;
using Windows.Storage;

namespace Dodo.Logic
{
    public class SettingsService : ISettingsService
    {
        readonly ApplicationDataContainer _appSettings;

        public SettingsService()
        {
            var data = ApplicationData.Current.LocalSettings;

            _appSettings = data.CreateContainer("Settings", ApplicationDataCreateDisposition.Always);
        }

        public bool ContainsKey(string key)
        {
            return _appSettings.Values.ContainsKey(key);
        }

        public T Get<T>(string key)
        {
            var json = _appSettings.Values[key].ToString();
            var obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        public void Set<T>(string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            _appSettings.Values[key] = json;
        }
    }
}