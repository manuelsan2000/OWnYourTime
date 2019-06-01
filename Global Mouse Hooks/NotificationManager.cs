using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Global_Mouse_Hooks
{
    class NotificationManager
    {
        public NotificationManager()
        {
            //Constructor
        }

        public void sendDesktopInitialNotification(string message, Boolean hasButtons)
        {
            try
            {
                //PopUp("INI STRING1", "INI STRING2", "INI STRING3");
                sendNotificationMessage(message, hasButtons);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //SEND NOTIFICATION CODE
        }

        public async void sendNotificationMessage(string message, Boolean hasButtons)
        {
            try
            {
                //PopUp("INI STRING1", "INI STRING2", "INI STRING3");
                XmlDocument tileXml = new XmlDocument();
                String xmlContent;
                if (hasButtons)
                {
                    xmlContent = await createToasterTemplateButtonsAsync(message);
                }
                else {
                    xmlContent = await createToasterTemplateAsync(message);
                }

                tileXml.LoadXml(xmlContent);
                var toast = new ToastNotification(tileXml);

                toast.ExpirationTime = DateTime.Now.AddSeconds(ConfigValues.alertExpirationTimeSeconds);
                toast.Tag = ConfigValues.appIdStr + ConfigValues.notificationId++;
                toast.Group = ConfigValues.appIdStr;

                ToastNotificationManager.CreateToastNotifier(ConfigValues.appIdStr).Show(toast);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<string> createToasterTemplateButtonsAsync(string message)
        {

            string image = "https://picsum.photos/364/202?image=883";

            // Construct the toast content
            ToastContent toastContent = new ToastContent()
            {
                // Arguments when the user taps body of toast
                Launch = new QueryString()
                {
                    { "action", "viewConversation" }

                }.ToString(),

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        HeroImage = new ToastGenericHeroImage()
                        {
                            Source = "1043-360x180.jpg"
                        },

                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = ConfigValues.appIdStr
                            },

                            new AdaptiveText()
                            {
                                Text = message
                            },

                             new AdaptiveImage()
                            {
                                // Non-Desktop Bridge apps cannot use HTTP images, so
                                // we download and reference the image locally
                                Source = await DownloadImageToDisk(image)
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = "1005-64x64.jpg",
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },   
                    }
                },
                Duration = ToastDuration.Long,

                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastSelectionBox("snoozeTime")
                        {
                            DefaultSelectionBoxItemId = "15",
                            Items =
                            {
                                new ToastSelectionBoxItem("5", "5 minutes"),
                                new ToastSelectionBoxItem("15", "15 minutes"),
                            }
                        }
                    },

                    Buttons =
                    {
                        new ToastButtonSnooze()
                        {
                            SelectionBoxId = "snoozeTime"
                        },

                        new ToastButtonDismiss()
                    }
                }
            };
      

            return toastContent.GetContent();
        }

        public async Task<string> createToasterTemplateAsync(string message)
        {

            string image = "https://picsum.photos/364/202?image=883";

            // Construct the toast content
            ToastContent toastContent = new ToastContent()
            {
                // Arguments when the user taps body of toast
                Launch = new QueryString()
                {
                    { "action", "viewConversation" }

                }.ToString(),

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = ConfigValues.appIdStr
                            },

                            new AdaptiveText()
                            {
                                Text = message
                            },

                             new AdaptiveImage()
                            {
                                // Non-Desktop Bridge apps cannot use HTTP images, so
                                // we download and reference the image locally
                                Source = await DownloadImageToDisk(image)
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = "file:///C:/Users/fernando.cervantes/source/OWnYourTime/Global%20Mouse%20Hooks/1005-64x64.jpg",
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                
            };


            return toastContent.GetContent();
        }


        private static bool _hasPerformedCleanup;
        private static async Task<string> DownloadImageToDisk(string httpImage)
        {
            // Toasts can live for up to 3 days, so we cache images for up to 3 days.
            // Note that this is a very simple cache that doesn't account for space usage, so
            // this could easily consume a lot of space within the span of 3 days.

            try
            {
                if (DesktopNotificationManagerCompat.CanUseHttpImages)
                {
                    return httpImage;
                }

                var directory = Directory.CreateDirectory(System.IO.Path.GetTempPath() + "WindowsNotifications.DesktopToasts.Images");

                if (!_hasPerformedCleanup)
                {
                    // First time we run, we'll perform cleanup of old images
                    _hasPerformedCleanup = true;

                    foreach (var d in directory.EnumerateDirectories())
                    {
                        if (d.CreationTimeUtc.Date < DateTime.UtcNow.Date.AddDays(-3))
                        {
                            d.Delete(true);
                        }
                    }
                }

                var dayDirectory = directory.CreateSubdirectory(DateTime.UtcNow.Day.ToString());
                string imagePath = dayDirectory.FullName + "\\" + (uint)httpImage.GetHashCode();

                if (File.Exists(imagePath))
                {
                    return imagePath;
                }

                HttpClient c = new HttpClient();
                using (var stream = await c.GetStreamAsync(httpImage))
                {
                    using (var fileStream = File.OpenWrite(imagePath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }

                return imagePath;
            }
            catch { return ""; }
        }

    }
}
