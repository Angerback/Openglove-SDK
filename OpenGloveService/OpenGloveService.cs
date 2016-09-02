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
        int[] GetMappingsArray();

        [OperationContract]
        OGCore GetCore();

        [OperationContract]
        void SetConfiguration(int BaudRate, int[] positivePins, int[] negativePins, string[] positiveInit, string[] negativeInit, string gloveHash, string gloveName);

        [OperationContract]
        int GetBaudRate();

        [OperationContract]
        int[] GetPositivePins();

        [OperationContract]
        int[] GetNegativePins();

        [OperationContract]
        string[] GetPositiveInit();

        [OperationContract]
        string[] GetNegativeInit();

        [OperationContract]
        string GetGloveHash();

        [OperationContract]
        string GetGloveName();

        [OperationContract]
        void SetProfile(string profileName, string gloveHash, Dictionary<string, string> mappings);

        [OperationContract]
        string GetProfileName();

        [OperationContract]
        string GetProfileGloveHash();

        [OperationContract]
        Dictionary<string, string> GetMappingsDictionary();



        [OperationContract]
        Dictionary<string, string> getGloves();

        [OperationContract]
        string[] GetPortNames();

        [OperationContract]
        string Connect(string port, bool right);

        [OperationContract]
        void Disconnect(string gloveName);

        [OperationContract]
        void StartTest(string gloveName);

        [OperationContract]
        void StopTest(string gloveName);
    }

    public class OGService : IOGService
    {
        private OGCore core;

        public OGService() {
            core = OGCore.GetCore();
        }

        public int[] GetMappingsArray()
        {
            int[] mappingsList = new int[core.profileCfg.AreaCount];

            foreach (KeyValuePair<string,string> mapping in core.profileCfg.Mappings.ToList())
            {
                mappingsList[Int32.Parse(mapping.Key)] = Int32.Parse(mapping.Value);
            }

            return mappingsList;
        }

        public OGCore GetCore() {
            if (this.core == null) {
                this.core = OGCore.GetCore();
            }
            return this.core;
        }

        public void SetConfiguration(int BaudRate, int[] positivePins, int[] negativePins, string[] positiveInit, string[] negativeInit, string gloveHash, string gloveName) {
            
            this.core.gloveCfg.BaudRate = BaudRate;
            this.core.gloveCfg.positivePins = positivePins.ToList();
            this.core.gloveCfg.negativePins = negativePins.ToList();
            this.core.gloveCfg.positiveInit = positiveInit.ToList();
            this.core.gloveCfg.negativeInit = negativeInit.ToList();
            this.core.gloveCfg.gloveHash = gloveHash;
            this.core.gloveCfg.gloveName = gloveName;
        }

        public void SetProfile(string profileName, string gloveHash, Dictionary<string,string> mappings) {
            Debugger.Launch();
            this.core.profileCfg.profileName = profileName;
            this.core.profileCfg.gloveHash = gloveHash;
            this.core.profileCfg.Mappings = mappings;
        }

        public int GetBaudRate()
        {
            return this.core.gloveCfg.BaudRate;
        }

        public int[] GetPositivePins()
        {
            return this.core.gloveCfg.positivePins.ToArray();
        }

        public int[] GetNegativePins()
        {
            return this.core.gloveCfg.negativePins.ToArray();
        }

        public string[] GetPositiveInit()
        {
            return this.core.gloveCfg.positiveInit.ToArray();
        }

        public string[] GetNegativeInit()
        {
            return this.core.gloveCfg.negativeInit.ToArray();
        }

        public string GetGloveHash()
        {
            return this.core.gloveCfg.gloveHash;
        }

        public string GetGloveName()
        {
            return this.core.gloveCfg.gloveName;
        }

        public string GetProfileName()
        {
            return this.core.profileCfg.profileName;
        }

        public string GetProfileGloveHash()
        {
            return this.core.profileCfg.gloveHash;
        }

        public Dictionary<string, string> GetMappingsDictionary()
        {
            return this.core.profileCfg.Mappings;
        }

        public Dictionary<string, string> getGloves()
        {
            Dictionary<string, string> gloves = new Dictionary<string, string>();

            var bluetoothClient = new BluetoothClient();

            var devices = bluetoothClient.DiscoverDevices();

            foreach (var device in devices)

            {
                if (device.DeviceName.Contains("OpenGlove")) {
                    string address = device.DeviceAddress.ToString();
                    string name = device.DeviceName;
                    gloves.Add(address,name);
                }
            }
            
            return gloves;
        }

        public string[] GetPortNames()
        {
            throw new NotImplementedException();
        }

        public string Connect(string port, bool right)
        {
            Debugger.Launch();
            string result = "ERROR";
            //Establecer comunicacion
            OpenGlove.OpenGlove glove = null;
            core.gloves.TryGetValue(port, out glove);
            if (glove == null)
            {
                core.gloves[port] = new OpenGlove.OpenGlove();
                core.gloves[port].OpenPort(port, core.gloveCfg.BaudRate);
                core.gloves[port].InitializeMotor(core.gloveCfg.positivePins); //Positive pins deberian definirse
                core.gloves[port].InitializeMotor(core.gloveCfg.negativePins); //Negative pins deberian definirse
                core.gloves[port].ActivateMotor(core.gloveCfg.negativePins, core.gloveCfg.negativeInit);
                core.gloves[port].Name = port;
                core.gloves[port].IsRightHand = right;
                result = port;
            }
            return result;
        }

        public void Disconnect(string gloveName)
        {
            Debugger.Launch();
            OpenGlove.OpenGlove glove = null;
            core.gloves.TryGetValue(gloveName, out glove);
            if (glove != null) {
                core.gloves[gloveName].ClosePort();
            }
                

        }

        public void StartTest(string gloveName)
        {
            Debugger.Launch();
            OpenGlove.OpenGlove glove = null;
            core.gloves.TryGetValue(gloveName, out glove);
            if (glove != null)
            {
                core.gloves[gloveName].ActivateMotor(core.gloveCfg.positivePins, core.gloveCfg.positiveInit);
            }


        }

        public void StopTest(string gloveName)
        {
            Debugger.Launch();
            OpenGlove.OpenGlove glove = null;
            core.gloves.TryGetValue(gloveName, out glove);
            if (glove != null)
            {
                core.gloves[gloveName].ActivateMotor(core.gloveCfg.positivePins, core.gloveCfg.negativeInit);
            }

        }
    }
}
