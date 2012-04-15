using BoxKite.Models;

namespace Dodo.Logic
{
    public interface ISettingsService
    {
        bool ContainsKey(string key);
        T Get<T>(string key);
        void Set<T>(string key, T value);
    }
}
