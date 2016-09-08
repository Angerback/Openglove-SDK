using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace COMTEST
{
    class Program
    {
        static  void Main(string[] args)
        {
            OpenGloveService.OGServiceClient ogClient = new OpenGloveService.OGServiceClient("BasicHttpBinding_IOGService");
            OpenGloveService.Glove[] gloves =  ogClient.GetGloves();

            foreach (var glove in gloves)
            {
                Console.WriteLine("GLOVE: " + glove.Name + ", " + glove.Port + ", " + glove.BluetoothAddress);
            }
            Console.ReadLine();
        }
    }
}
