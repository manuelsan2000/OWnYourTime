using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Global_Mouse_Hooks
{
    class ListenerThread
    {
        public Boolean abortthread = false;
        public void DO()
        {
            while (true)
            {
                Console.WriteLine("Validando datos...");
                Boolean notificacionEnviada = ConfigValues.validarDatos();
                Console.WriteLine("Durmiendo thread 5 segundos");
                Thread.Sleep(5000);
            }
        }

    }
}
