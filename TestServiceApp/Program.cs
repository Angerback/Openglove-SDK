using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServiceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceReference1.CalcServiceClient proxy = new ServiceReference1.CalcServiceClient("BasicHttpBinding_ICalcService");
            Console.WriteLine("Client is running at " + DateTime.Now.ToString());
            Console.WriteLine("Sum of two numbers. 5 + 5 =" + proxy.Add(5, 5));
            Console.ReadLine();

        }
    }
}
