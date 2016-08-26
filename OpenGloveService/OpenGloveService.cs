using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Configuration;
using System.Configuration.Install;
using System.Threading;
using System.ServiceModel.Description;

namespace OpenGloveService
{
    public partial class OpenGloveService : ServiceBase
    {
        private ServiceHost m_svcHost = null;

        public OpenGloveService()
        {
            InitializeComponent();
            // Name the Windows Service
            ServiceName = "OpenGloveService";
        }

        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            Debugger.Launch();

            if (m_svcHost != null) m_svcHost.Close();

            string strAdrHTTP = "http://localhost:9001/CalcService";
            string strAdrTCP = "net.tcp://localhost:9002/CalcService";

            Uri[] adrbase = { new Uri(strAdrHTTP), new Uri(strAdrTCP) };
            m_svcHost = new ServiceHost(typeof(CalcService), adrbase);

            ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
            m_svcHost.Description.Behaviors.Add(mBehave);

            BasicHttpBinding httpb = new BasicHttpBinding();
            m_svcHost.AddServiceEndpoint(typeof(ICalcService), httpb, strAdrHTTP);
            m_svcHost.AddServiceEndpoint(typeof(IMetadataExchange),
            MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            NetTcpBinding tcpb = new NetTcpBinding();
            m_svcHost.AddServiceEndpoint(typeof(ICalcService), tcpb, strAdrTCP);
            m_svcHost.AddServiceEndpoint(typeof(IMetadataExchange),
            MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

            m_svcHost.Open();
        }

        protected override void OnStop()
        {
            if (m_svcHost != null)
            {
                m_svcHost.Close();
                m_svcHost = null;
            }
        }
    }


    [ServiceContract]
    public interface ICalcService
    {
        [OperationContract]
        double Add(double dblNum1, double dblNum2);
        [OperationContract]
        double Subtract(double dblNum1, double dblNum2);
        [OperationContract]
        double Multiply(double dblNum1, double dblNum2);
        [OperationContract]
        double Divide(double dblNum1, double dblNum2);
    }

    public class CalcService : ICalcService
    {
        public double Add(double dblNum1, double dblNum2)
        {
            return (dblNum1 + dblNum2);
        }

        public double Subtract(double dblNum1, double dblNum2)
        {
            return (dblNum1 - dblNum2);
        }

        public double Multiply(double dblNum1, double dblNum2)
        {
            return (dblNum1 * dblNum2);
        }

        public double Divide(double dblNum1, double dblNum2)
        {
            return ((dblNum2 == 0) ? 0 : (dblNum1 / dblNum2));
        }
    }

}
