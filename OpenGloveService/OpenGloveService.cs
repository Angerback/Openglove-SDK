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

    [DataContract]
    public class Glove
    {
        /// <summary>
        /// Private singleton field containing all active gloves, connected or disconnected in the system.
        /// </summary>
        private static List<Glove> gloves;

        public static List<Glove> Gloves
        {
            get
            {
                gloves = ScanGloves();
                return gloves;
            }
        }

        /// <summary>
        /// Scans the system using 32Feet.NET for OpenGlove devices. Currently it only filters by the bluetooth device Name, so
        /// any device containing "OpenGlove" on their name would be picked. Hardware limitation.
        /// </summary>
        /// <returns></returns>
        private static List<Glove> ScanGloves() {

            List<Glove> scannedGloves = new List<Glove>();

            var bluetoothClient = new BluetoothClient();

            var devices = bluetoothClient.DiscoverDevices();

            foreach (var device in devices)

            {
                if (device.DeviceName.Contains("OpenGlove"))
                {
                    string deviceAddress = device.DeviceAddress.ToString();
                    string comPort = GetBluetoothPort(deviceAddress);
                    string address = device.DeviceAddress.ToString();
                    string name = device.DeviceName;
                    
                    Glove foundGlove = new Glove();
                    foundGlove.BluetoothAddress = deviceAddress;
                    foundGlove.Port = comPort;
                    foundGlove.Name = name;
                    foundGlove.Connected = false;
                    foundGlove.legacyGlove = new OpenGlove.OpenGlove();

                    scannedGloves.Add(foundGlove);
                }
            }
            return scannedGloves;
        }

        /// <summary>
        /// Gets the outgoing COM Serial Port of a bluetooth device.
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <returns></returns>
        private static string GetBluetoothPort(string deviceAddress)
        {
            const string Win32_SerialPort = "Win32_SerialPort";
            SelectQuery q = new SelectQuery(Win32_SerialPort);
            ManagementObjectSearcher s = new ManagementObjectSearcher(q);
            foreach (object cur in s.Get())
            {
                ManagementObject mo = (ManagementObject)cur;
                string pnpId = mo.GetPropertyValue("PNPDeviceID").ToString();

                if (pnpId.Contains(deviceAddress))
                {
                    object captionObject = mo.GetPropertyValue("Caption");
                    string caption = captionObject.ToString();
                    int index = caption.LastIndexOf("(COM");
                    if (index > 0)
                    {
                        string portString = caption.Substring(index);
                        string comPort = portString.
                                      Replace("(", string.Empty).Replace(")", string.Empty);
                        return comPort;
                    }
                }
            }
            return null;
        }

        [DataMember]
        public string Name;

        [DataMember]
        public string Port;

        [DataMember]
        public Sides Side;

        [DataMember]
        public string BluetoothAddress;

        [DataMember]
        public bool Connected;

        [DataMember]
        public Profile Profile;

        private OpenGlove.OpenGlove legacyGlove;
    }

    [DataContract(Name = "Side")]
    public enum Sides
    {
        [EnumMember]
        Right,
        [EnumMember]
        Left
    }

    [DataContract]
    public class Profile
    {
        [DataMember]
        public String ProfileName;

        [DataMember]
        public String GloveHash;

        [DataMember]
        public int AreaCount = 58;

        [DataMember]
        public Dictionary<string, string> Mappings = new Dictionary<string, string>();

        [DataMember]
        public Configuration Configuration;
    }

    [DataContract]
    public class Configuration {

        [DataMember]
        public int BaudRate;

        [DataMember]
        public List<int> AllowedBaudRates = new List<int> { 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200 };

        [DataMember]
        public List<int> PositivePins;

        [DataMember]
        public List<int> NegativePins;

        [DataMember]
        public List<string> NegativeInit;

        [DataMember]
        public List<string> PositiveInit;

        [DataMember]
        public String GloveHash;

        [DataMember]
        public String GloveName;
    }

    [ServiceContract]
    public interface IOGService
    {
        [OperationContract]
        List<Glove> GetGloves();
    }

    public class OGService : IOGService
    {
        public List<Glove> GetGloves()
        {
            Debugger.Launch();
            return Glove.Gloves;
        }

    }
}
