using BoxKite.Models;

namespace Dodo.Logic.Extensions
{
    public static class SettingsExtensions
    {
        const string Credentials = "Credentials";

        public static bool HasUserSettings(this ISettingsService settingsService)
        {
            return settingsService.ContainsKey(Credentials);
        }

        public static TwitterCredentials GetUserCredentials(this ISettingsService settingsService)
        {
            return settingsService.Get<TwitterCredentials>(Credentials);
        }

        public static void StoreUserCredentials(this ISettingsService settingsService, TwitterCredentials credentials)
        {
            settingsService.Set(Credentials, credentials);
        }
    }
}
