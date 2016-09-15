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

        private const bool DEBUGGING = false;

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
                if (gloves == null) {
                    gloves = ScanGloves();
                    
                }
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
                    foundGlove.LegacyGlove = new LegacyOpenGlove();

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
        public Configuration GloveConfiguration;

        public LegacyOpenGlove LegacyGlove { get; set; }

        
        [DataContract]
        public class Configuration
        {

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

            [DataMember]
            public Profile GloveProfile;

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
            }
        }


    }

    [DataContract(Name = "Side")]
    public enum Sides
    {
        [EnumMember]
        Right,
        [EnumMember]
        Left
    }

    [ServiceContract]
    public interface IOGService
    {
        [OperationContract]
        List<Glove> GetGloves();

        [OperationContract]
        void SaveGlove(Glove glove);

        [OperationContract]
        int Activate(Glove glove, int region, int intensity);

        [OperationContract]
        int ActivateTimed(Glove glove, int region, int intensity, int time);

        [OperationContract]
        int Connect(Glove glove);

        [OperationContract]
        int Disconnect(Glove glove);
    }

    public class OGService : IOGService
    {
        private const bool DEBUGGING = false;

        private const int AREACOUNT = 58;

        private BackgroundWorker bgw;

        public int Activate(Glove glove, int actuator, int intensity)
        {
            if (glove != null)
            {
                if (intensity < 0)
                {
                    intensity = 0;
                }
                else if (intensity > 255)
                {
                    intensity = 255;
                }

                if (actuator < 0)
                {
                    return 1;
                }
                else if (actuator >= AREACOUNT)
                {
                    return 1;
                }

                if (glove.Connected)
                {
                    foreach (Glove g in Glove.Gloves)
                    {
                        if (g.BluetoothAddress.Equals(glove.BluetoothAddress))
                        {
                            try
                            {
                                bgw = new BackgroundWorker();
                                bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
                                bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
                                bgw.RunWorkerAsync(new List<object>() { g, new List<int> { actuator }, new List<string> { intensity.ToString() } });
                                return 0;
                            }
                            catch (Exception)
                            {
                                g.Connected = false;
                                glove.LegacyGlove = new LegacyOpenGlove();
                                return 1;// CANT ACTIVATE
                            }
                        }
                    }
                    
                }
            }
            return 0; //OK
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            Glove g = (Glove)(((List<object>)e.Argument)[0]);
            IEnumerable<int> actuator = (IEnumerable<int>)(((List<object>)e.Argument)[1]);
            IEnumerable<string> intensity = (IEnumerable<string>)(((List<object>)e.Argument)[2]);
            //Your time taking work. Here it's your data query method.
            g.LegacyGlove.ActivateMotor(actuator, intensity);
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //After completing the job.
        }

        public int ActivateTimed(Glove glove, int region, int intensity, int time)
        {
            if (glove != null)
            {
                if (intensity < 0)
                {
                    intensity = 0;
                }
                else if (intensity > 255)
                {
                    intensity = 255;
                }

                if (region < 0)
                {
                    return 1;
                }
                else if (region >= AREACOUNT)
                {
                    return 1;
                }

                if (glove.Connected)
                {
                    foreach (Glove g in Glove.Gloves)
                    {
                        if (g.BluetoothAddress.Equals(glove.BluetoothAddress))
                        {
                            try
                            {
                                g.LegacyGlove.ActivateMotor(new List<int> { region }, new List<string> { intensity.ToString() });
                                Thread.Sleep(time);
                                g.LegacyGlove.ActivateMotor(new List<int> { region }, new List<string> { "0" });
                                return 0;
                            }
                            catch (Exception)
                            {
                                g.Connected = false;
                                glove.LegacyGlove = new LegacyOpenGlove();
                                return 1;// CANT ACTIVATE
                            }
                        }
                    }

                }
            }
            return 0; //OK
        }

        public List<Glove> GetGloves()
        {
            if (DEBUGGING) Debugger.Launch();
            return Glove.Gloves;
        }

        public void SaveGlove(Glove glove) {
            foreach (Glove g in Glove.Gloves)
            {
                if (g.BluetoothAddress.Equals(glove.BluetoothAddress))
                {
                    Glove.Gloves.Remove(g);
                    Glove.Gloves.Add(glove);
                    break;
                }
            }
        }

        public int Connect(Glove glove)
        {
            if (DEBUGGING) Debugger.Launch();
            foreach (Glove g in Glove.Gloves)
            {
                if (g.BluetoothAddress.Equals(glove.BluetoothAddress))
                {
                    if (g.GloveConfiguration != null)
                    {
                        g.LegacyGlove = new LegacyOpenGlove();
                        g.LegacyGlove.OpenPort(g.Port, g.GloveConfiguration.BaudRate);
                        g.LegacyGlove.InitializeMotor(g.GloveConfiguration.PositivePins);
                        g.LegacyGlove.InitializeMotor(g.GloveConfiguration.NegativePins);
                        g.LegacyGlove.ActivateMotor(g.GloveConfiguration.NegativePins, g.GloveConfiguration.NegativeInit);
                        g.Connected = true;
                    }
                    else {
                        return 1; // NO CONFIG
                    }
                    return 0;
                }
            }
            return 0; //OK
        }

        public int Disconnect(Glove glove) {
            if (DEBUGGING) Debugger.Launch();
            foreach (Glove g in Glove.Gloves)
            {
                if (g.BluetoothAddress.Equals(glove.BluetoothAddress))
                {
                    try
                    {
                        g.LegacyGlove.ClosePort();
                    }
                    catch (Exception)
                    {
                        
                    }
                    g.Connected = false;
                    return 0;
                }
            }
            return 0;
        }
    }
}
