using System;
using Dodo.Logic.Shared;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Data.Json;
using Windows.UI.Xaml.Media.Imaging;

namespace Dodo.Modules.Share
{
    public class ShareViewModel : BindableBase
    {
        private string _dataPackageDescription;
        private string _dataPackageTitle;

        // TODO: Swap out "Windows-8-Preview-Book" for "http://schema.org/Book". The Beta manifest schema is unable to support URLs as data formats
        const string DataFormatName = "Windows-8-Preview-Book";

        public void SetShareTarget(ShareTargetActivatedEventArgs args)
        {
            var shareOperation = args.ShareOperation;
            DataPackageTitle = shareOperation.Data.Properties.Title;
            DataPackageDescription = shareOperation.Data.Properties.Description;

            DetectContents(shareOperation);
        }

        private async void DetectContents(ShareOperation shareOperation)
        {
            if (shareOperation.Data.Contains(StandardDataFormats.Uri))
            {
                var uri = await shareOperation.Data.GetUriAsync();
                if (uri != null)
                {
                    
                }
            }
            
            if (shareOperation.Data.Contains(StandardDataFormats.Text))
            {
                string text = await shareOperation.Data.GetTextAsync();
                if (text != null)
                {
                  
                }
            }
            
            if (shareOperation.Data.Contains(StandardDataFormats.StorageItems))
            {
                var storageItems = await shareOperation.Data.GetStorageItemsAsync();
                var fileList = String.Empty;
                for (var index = 0; index < storageItems.Count; index++)
                {
                    fileList += storageItems[index].Name;
                    if (index < storageItems.Count - 1)
                    {
                        fileList += ", ";
                    }
                }
            }
            
            if (shareOperation.Data.Contains(DataFormatName))
            {
                var receivedStrings = await shareOperation.Data.GetTextAsync(DataFormatName);

                JsonObject customObject = null;
                if (JsonObject.TryParse(receivedStrings, out customObject))
                {
                    if (customObject.ContainsKey("type"))
                    {
                        if (customObject["type"].GetString() == "http://schema.org/Book")
                        {
                            // This sample expects the custom format to be of type http://schema.org/Book
                            receivedStrings = "Type: " + customObject["type"].Stringify();
                            JsonObject properties = customObject["properties"].GetObject();
                            receivedStrings += Environment.NewLine + "Image: " + properties["image"].Stringify()
                                            + Environment.NewLine + "Name: " + properties["name"].Stringify()
                                            + Environment.NewLine + "Book Format: " + properties["bookFormat"].Stringify()
                                            + Environment.NewLine + "Author: " + properties["author"].Stringify()
                                            + Environment.NewLine + "Number of Pages: " + properties["numberOfPages"].Stringify()
                                            + Environment.NewLine + "Publisher: " + properties["publisher"].Stringify()
                                            + Environment.NewLine + "Date Published: " + properties["datePublished"].Stringify()
                                            + Environment.NewLine + "In Language: " + properties["inLanguage"].Stringify()
                                            + Environment.NewLine + "ISBN: " + properties["isbn"].Stringify();
                        }
                    }
                }
            }
            
            if (shareOperation.Data.Contains(StandardDataFormats.Html))
            {
                var htmlFormat = await shareOperation.Data.GetHtmlFormatAsync();
                var htmlFragment = HtmlFormatHelper.GetStaticFragment(htmlFormat);
            }

            if (shareOperation.Data.Contains(StandardDataFormats.Bitmap))
            {
                var imageReceived = await shareOperation.Data.GetBitmapAsync();
                var stream = await imageReceived.OpenReadAsync();
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(stream);
            }
        }

        public string DataPackageTitle
        {
            get { return _dataPackageTitle; }
            set { SetProperty(ref _dataPackageTitle, value); }
        }

        public string DataPackageDescription
        {
            get { return _dataPackageDescription; }
            set { SetProperty(ref _dataPackageDescription, value); }
        }
    }
}
