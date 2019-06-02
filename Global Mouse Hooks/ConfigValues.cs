using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Global_Mouse_Hooks
{

    public class ConfigValues
    {
        public static string appIdStr = "OWn Your Time";
        public static int alertExpirationTimeSeconds = 3;
        public static int idleTimeToUpdateBreakMiliseconds = 15000;
        public static int workingTimeBeforeAlertMiliseconds = 25000;

        public static long lastActivityMillis;
        public static long startActivityMillis;
        public static long continousWorkingTimeMillis = 0;
        public static long elapsedMinutes;
        public static long lastBreakMillis;
        public static long lastNotificationMillis;
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

        public static Boolean validarDatos()
        {
            long currentTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);

            //Console.WriteLine($"Validando ... currentTime {currentTime} - continousWorkingTimeMillis {continousWorkingTimeMillis} ");

            if ( continousWorkingTimeMillis == 0)
            {
                //JUST STARTED OR RETURNED FROM BREAK
                //SEND NOTIFICATION TO TAKE BREAKS EVERY HOUR


                new NotificationManager().sendNotificationMessage("Keep in mind: It is recommended to take at least 5 min break each hour.", "", false, "");
                lastBreakMillis = currentTime;
                continousWorkingTimeMillis = 1;
                lastNotificationMillis = currentTime;
                return true;
            }

            continousWorkingTimeMillis = currentTime - lastBreakMillis;
            long idleTime = currentTime - lastActivityMillis;
            long elapsedTime = continousWorkingTimeMillis;

            Console.WriteLine($"Validando ... elapsedTime {elapsedTime / 60000} - idleTime {idleTime / 60000} ");

            if (idleTime > idleTimeToUpdateBreakMiliseconds) //15 segundos idle
            {
                //ENVIAR 
                Console.WriteLine($"BREAK REPORT DUE TO {idleTimeToUpdateBreakMiliseconds} IDLE!!");
                continousWorkingTimeMillis = 1;
                lastBreakMillis = currentTime;
                return false;
            }

            if (elapsedTime > workingTimeBeforeAlertMiliseconds ) //X tiempo trabajado
            {

                //CREATE WEB OBBECT WITH OLIVER WYMAN PROXY
                WebProxy proxyObj = new WebProxy("http://usdal1-03pr02-vip.mgd.mrshmc.com:8888");
                proxyObj.Credentials = CredentialCache.DefaultCredentials;
                WebClient myWebClient = new WebClient();
                myWebClient.Proxy = proxyObj;

                // GETTING JSON
                var json = myWebClient.DownloadString("https://spreadsheets.google.com/feeds/list/1xRGex8sYPd6P4OaxLbSkTgoTFkK2KWU2U8M9NmaTrd8/od6/public/basic?alt=json&pli=1");

                dynamic spreadsheets = Newtonsoft.Json.JsonConvert.DeserializeObject(json.ToString());
                JArray messages = (JArray)spreadsheets["feed"]["entry"];
                int messagesCount = messages.Count - 1;

                Random random = new Random();
                int rInt = random.Next(0, messagesCount);
                String messageFullToWrite = spreadsheets["feed"]["entry"][rInt]["content"]["$t"];
                String messageToWrite = messageFullToWrite.Substring(0, messageFullToWrite.IndexOf(", imageurl: "));

                String[] arrString = messageFullToWrite.Split(',');
                Console.WriteLine(arrString[1]);



                String imageHero = arrString[1];
                imageHero = imageHero.Replace(" imageurl: ", "");

                Console.WriteLine(imageHero);
                new NotificationManager().sendNotificationMessage($"We noticed that you been working for to long, Please take a break!", messageToWrite.Replace("message: ", ""),  true, imageHero);
                lastNotificationMillis = currentTime;
                lastBreakMillis = currentTime;
                return true;

            }
            return false;
        }
        
       
    }
}
