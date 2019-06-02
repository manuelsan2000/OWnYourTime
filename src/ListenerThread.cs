using System;
using System.Threading;

namespace OWn_Your_Time
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
