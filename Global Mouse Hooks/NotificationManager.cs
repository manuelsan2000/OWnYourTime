using System;
using System.Collections.Generic;
using System.Linq;
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
        public void sendDesktopBreakNotification(string message)
        {
            //Console.WriteLine("BREAK NOTIFICATION!!");
            //PopUp("STRING1", "STRING2", "STRING3");
            sendNotificationMessage(message);
            //SEND NOTIFICATION CODE
        }

        public void sendDesktopInitialNotification(string message)
        {
            try
            {
                //PopUp("INI STRING1", "INI STRING2", "INI STRING3");
                sendNotificationMessage(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //SEND NOTIFICATION CODE
        }

        public void sendNotificationMessage(string message)
        {
            try
            {
                //PopUp("INI STRING1", "INI STRING2", "INI STRING3");
                XmlDocument tileXml = new XmlDocument();

                tileXml.LoadXml( createToasterTemplate( message ) );
                var toast = new ToastNotification(tileXml);

                toast.ExpirationTime = DateTime.Now.AddSeconds( ConfigValues.alertExpirationTimeSeconds );
                toast.Tag = ConfigValues.appIdStr + ConfigValues.notificationId++;
                toast.Group = ConfigValues.appIdStr;

                ToastNotificationManager.CreateToastNotifier(ConfigValues.appIdStr).Show(toast);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public string createToasterTemplate(string message)
        {
            string Toast = "<toast launch=\"app-defined-string\"><visual><binding template=\"ToastGeneric\">";
            Toast += "<text>" + ConfigValues.appIdStr + "</text>";
            Toast += "<text>" + message + "</text>";
            Toast += "</binding></visual>";
            Toast += "<actions>";
            //Toast += "<action content=\"check\" arguments =\"check\" imageUri =\"check.png\" />";
            Toast += "<action content=\"cancel\" arguments=\"cancel\" />";
            Toast += "</actions>";
            Toast += "</toast>";

            /*string Toast = "<toast launch=\"app-defined-string\">";
            Toast +=        "<visual>";
            Toast +=            "<binding template=\"ToastGeneric\">";
            Toast +=                "<text>" + message + "</text>";
            Toast +=                "<image placement=\"AppLogoOverride\" src =\"oneAlarm.png\" />";
            Toast +=            "</binding>";
            Toast +=        "</visual>";
            Toast +=        "<actions>";
            Toast +=            "<action content=\"check\" arguments =\"check\" imageUri =\"check.png\" />";
            Toast +=            "<action content=\"cancel\" arguments=\"cancel\" />";
            Toast +=        "</actions>";
            Toast +=        "<audio src=\"ms -winsoundevent:Notification.Reminder\"/>";
            Toast +=       "</toast>";
            */
            return Toast;

        }

    }
}
