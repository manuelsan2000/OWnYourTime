using System.Threading;
using System.Windows.Forms;

namespace OWn_Your_Time
{
    static class Program
    {
        static void Main(string[] args)
        {
            ClickDetector.ListenForMouseEvents();

            ListenerThread obj = new ListenerThread();
            Thread thr = new Thread(new ThreadStart(obj.DO ));
            thr.Start();
            //Run the app as a windows forma application
            Application.Run(new ApplicationContext());
            
        }
    }
}
