using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace BoxKite.Tests
{
    public class BaseContext
    {
        public static async Task<string> GetTestData(string filePath)
        {
            var folder = Package.Current.InstalledLocation;
            var file = await folder.GetFileAsync(filePath);
            var openFile = await file.OpenReadAsync();
            var reader = new StreamReader(WindowsRuntimeStreamExtensions.AsStreamForRead(openFile));
            var contents = await reader.ReadToEndAsync();
            return contents;
        }
    }
}