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
            //Debugger.Launch();

            if (m_svcHost != null) m_svcHost.Close();

            string strAdrHTTP = "http://localhost:9001/OGService";
            string strAdrTCP = "net.tcp://localhost:9002/OGService";

            Uri[] adrbase = { new Uri(strAdrHTTP), new Uri(strAdrTCP) };
            m_svcHost = new ServiceHost(typeof(OGService), adrbase);

            ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
            m_svcHost.Description.Behaviors.Add(mBehave);

            BasicHttpBinding httpb = new BasicHttpBinding();
            m_svcHost.AddServiceEndpoint(typeof(IOGService), httpb, strAdrHTTP);
            m_svcHost.AddServiceEndpoint(typeof(IMetadataExchange),
            MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            NetTcpBinding tcpb = new NetTcpBinding();
            m_svcHost.AddServiceEndpoint(typeof(IOGService), tcpb, strAdrTCP);
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
    public interface IOGService
    {
        [OperationContract]
        int[] GetMappings();

        [OperationContract]
        OpenGloveSDKCore GetCore();
    }

    public class OGService : IOGService
    {
        private OpenGloveSDKCore core = OpenGloveSDKCore.GetCore();

        public int[] GetMappings()
        {
            int[] mappingsList = new int[core.profileCfg.AreaCount];

            foreach (KeyValuePair<string,string> mapping in core.profileCfg.Mappings.ToList())
            {
                mappingsList[Int32.Parse(mapping.Key)] = Int32.Parse(mapping.Value);
            }

            return mappingsList;
        }

        public OpenGloveSDKCore GetCore() {
            if (this.core == null) {
                this.core = OpenGloveSDKCore.GetCore();
            }
            return this.core;
        }
    }
}
