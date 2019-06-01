using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Global_Mouse_Hooks
{

    public class ConfigValues
    {
        public static string appIdStr = "OWn Your Time";
        public static int alertExpirationTimeSeconds = 3;
        public static long lastActivityMillis;
        public static long startActivityMillis;
        public static long continousWorkingTimeMillis = 0;
        public static long elapsedMinutes;
        public static long lastBreakMillis;
        public static long currentActivityMillis = 0;
        public static int notificationId = 0;

        static ConfigValues()
        {
            lastBreakMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            lastActivityMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            startActivityMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            elapsedMinutes = 0;
            Console.WriteLine($"Starting time: {lastActivityMillis}");
        }

        public static void validarDatos()
        {
            long currentTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);

            //Console.WriteLine($"Validando ... currentTime {currentTime} - continousWorkingTimeMillis {continousWorkingTimeMillis} ");

            if ( continousWorkingTimeMillis == 0)
            {
                //JUST STARTED OR RETURNED FROM BREAK
                //SEND NOTIFICATION TO TAKE BREAKS EVERY HOUR
                new NotificationManager().sendDesktopInitialNotification("Recuerda que es importante tomar breaks de al menos 5 minutos cada hora " +
                    "por salud");
                lastBreakMillis = currentTime;
                continousWorkingTimeMillis = 1;
                return;
            }

            continousWorkingTimeMillis = currentTime - lastBreakMillis;
            long idleTime = currentTime - lastActivityMillis;
            //decimal elapsedMinutes = continousWorkingTimeMillis / (60000);
            long elapsedTime = continousWorkingTimeMillis;

            Console.WriteLine($"Validando ... elapsedTime {elapsedTime} - idleTime {idleTime} ");

            
            if (idleTime > 15000) //15 segundos idle
            {
                //ENVIAR 
                Console.WriteLine("BREAK DUE TO 15000 IDLE!!");
                continousWorkingTimeMillis = 0;
                lastBreakMillis = currentTime;
                return;
            }

            if (elapsedTime > (60000) ) //1 minuto de tiempo trabajado
            {
                new NotificationManager().sendDesktopBreakNotification("Hemos detectado que llevas x tiempo sin descansar, por favor toma un break!");
                return;
            }

        }

        
        public static void PopUp(String msg1, String msg2, String msg3)
        {
            string title = msg1;
            string content = msg2;
            string image = "https://picsum.photos/364/202?image=883";
           int conversationId = 5;

            // Construct the toast content
            ToastContent toastContent = new ToastContent()
            {
                // Arguments when the user taps body of toast
                Launch = new QueryString()
               {
                   { "action", "viewConversation" },
                   { "conversationId", conversationId.ToString() }

               }.ToString(),

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                       {
                           new AdaptiveText()
                           {
                               Text = title
                           },

                           new AdaptiveText()
                           {
                               Text = content
                           },

                           new AdaptiveImage()
                           {
                               // Non-Desktop Bridge apps cannot use HTTP images, so
                               // we download and reference the image locally
                               //Source = await DownloadImageToDisk(image)
                           }
                       },

                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            //Source = await DownloadImageToDisk("https://unsplash.it/64?image=1005"),
 
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Inputs =
                   {
                       new ToastTextBox("tbReply")
                       {
                           PlaceholderContent = "Type a response"
                       }
                   },

                    Buttons =
                   {
                       // Note that there's no reason to specify background activation, since our COM
                       // activator decides whether to process in background or launch foreground window
                       new ToastButton("Reply", new QueryString()
                       {
                           { "action", "reply" },
                           { "conversationId", conversationId.ToString() }

                       }.ToString()),

                       new ToastButton("Like", new QueryString()
                       {
                           { "action", "like" },
                           { "conversationId", conversationId.ToString() }

                       }.ToString()),

                       new ToastButton("View", new QueryString()
                       {
                           { "action", "viewImage" },
                           { "imageUrl", image }

                       }.ToString())
                   }
                }
            };

            // Make sure to use Windows.Data.Xml.Dom
            var doc = new XmlDocument();
            doc.LoadXml(toastContent.GetContent());

            // And create the toast notification
            var toast = new ToastNotification(doc);

            // And then show it
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
