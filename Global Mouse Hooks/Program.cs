using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Global_Mouse_Hooks
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
