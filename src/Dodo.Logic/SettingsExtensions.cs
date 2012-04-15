using BoxKite.Models;

namespace Dodo.Logic
{
    public static class SettingsExtensions
    {
        private const string SettingsKey = "Settings";

        public static bool HasSettings(this ISettingsService settingsService)
        {
            return settingsService.ContainsKey(SettingsKey);
        }

        public static TwitterCredentials GetSettings(this ISettingsService settingsService)
        {
            return settingsService.Get<TwitterCredentials>(SettingsKey);
        }

        public static void SaveSettings(this ISettingsService settingsService, TwitterCredentials credentials)
        {
            settingsService.Set(SettingsKey, credentials);
        }
    }
}