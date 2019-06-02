using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public void sendDesktopInitialNotification(string message, Boolean hasButtons, String heroImag)
        {
            try
            {
                sendNotificationMessage(message, "", hasButtons, heroImag);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //SEND NOTIFICATION CODE
        }

        public async void sendNotificationMessage(string message, String message2, Boolean hasButtons, String heroImage)
        {
            try
            {
                XmlDocument tileXml = new XmlDocument();
                String xmlContent;
                if (hasButtons)
                {
                    xmlContent = await createToasterTemplateButtonsAsync(message, message2, heroImage);
                }
                else {
                    xmlContent = await createToasterTemplateAsync(message, message2);
                }

                tileXml.LoadXml(xmlContent);
                var toast = new ToastNotification(tileXml);             
                //toast.ExpirationTime = DateTime.Now.AddSeconds(ConfigValues.alertExpirationTimeSeconds);
                toast.Tag = ConfigValues.appIdStr + ConfigValues.notificationId++;
                toast.Group = ConfigValues.appIdStr;

                ToastNotificationManager.CreateToastNotifier(ConfigValues.appIdStr).Show(toast);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<string> createToasterTemplateButtonsAsync(string message, String message2, String heroImage)
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
                            Source = DownloadImageToDisk(heroImage)
                        },

                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text =message
                            },

                      

                             new AdaptiveText()
                            {
                                Text = message2
                            }


                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = System.IO.Path.GetFullPath("1005 -64x64.jpg"),
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

                       new ToastButton("Let's go", new QueryString()
                        {
                            { "action", "like" },
                            { "conversationId" }

                        }.ToString()),
                    }
                }
            };
      

            return toastContent.GetContent();
        }

        public async Task<string> createToasterTemplateAsync(string message, String message2)
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


        private static  string DownloadImageToDisk(string httpImage)
        {
            WebProxy proxyObj = new WebProxy("http://usdal1-03pr02-vip.mgd.mrshmc.com:8888");
            proxyObj.Credentials = CredentialCache.DefaultCredentials;

            int imageLenghtName = httpImage.Length - httpImage.LastIndexOf("/");
            String httpImageName = httpImage.Substring(httpImage.LastIndexOf("/") + 1 , imageLenghtName - 1);

            string localFilename = @"c:\\TempImages\"+ httpImageName;
            using (WebClient client = new WebClient())
            {
                client.Proxy = proxyObj;
                client.DownloadFile(httpImage, localFilename);
            }
            Console.WriteLine(localFilename);
            return localFilename;
        }
    }
}
