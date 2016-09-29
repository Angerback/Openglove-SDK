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
using OpenGlove;
using InTheHand.Net.Sockets;
using System.Runtime.Serialization;
using System.Management;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace OpenGloveService
{
    public partial class OpenGloveService : ServiceBase
    {
        private ServiceHost m_svcHost = null;

        private const bool DEBUGGING = true;

        public OpenGloveService()
        {
            InitializeComponent();
            // Name the Windows Service
            ServiceName = "OpenGloveService";
        }

        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            if (DEBUGGING) Debugger.Launch();

            if (m_svcHost != null) m_svcHost.Close();

            /*
            string strAdrHTTP = "http://localhost:9001/OGService";
            string strAdrTCP = "net.tcp://localhost:9002/OGService";

            Uri[] adrbase = { new Uri(strAdrHTTP), new Uri(strAdrTCP) };
            m_svcHost = new ServiceHost(typeof(OGService), adrbase);

            ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
            mBehave.HttpGetEnabled = true;
            m_svcHost.Description.Behaviors.Add(mBehave);

            BasicHttpBinding httpb = new BasicHttpBinding();

            m_svcHost.AddServiceEndpoint(typeof(IOGService), httpb, strAdrHTTP);
            m_svcHost.AddServiceEndpoint(typeof(IMetadataExchange),
            MetadataExchangeBindings.CreateMexHttpBinding(), "mex");


            NetTcpBinding tcpb = new NetTcpBinding();
            m_svcHost.AddServiceEndpoint(typeof(IOGService), tcpb, strAdrTCP);
            m_svcHost.AddServiceEndpoint(typeof(IMetadataExchange),
            MetadataExchangeBindings.CreateMexTcpBinding(), "mex");
            */

            m_svcHost = new ServiceHost(typeof(OpenGloveWCF.OGService));
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
}
