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
            var jsonOutput = _appSettings.Values[key].ToString();
            var content = JsonConvert.DeserializeObject<T>(jsonOutput);
            return content;
        }

        public void Set<T>(string key, T value)
        {
            var jsonValue = JsonConvert.SerializeObject(value);
            _appSettings.Values[key] = jsonValue;
        }
    }
}