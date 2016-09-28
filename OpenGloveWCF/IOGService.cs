using InTheHand.Net.Sockets;
using OpenGlove;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace OpenGloveWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IOGService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
                    ResponseFormat = WebMessageFormat.Json,
                    RequestFormat = WebMessageFormat.Json,
                    UriTemplate = "GetGloves")]
        List<Glove> GetGloves();

        [OperationContract]
        List<Glove> RefreshGloves();

        [OperationContract]
        void SaveGlove(Glove glove);

        [OperationContract]
        [WebInvoke(Method = "POST",
                    ResponseFormat = WebMessageFormat.Json,
                    RequestFormat = WebMessageFormat.Json,
                    UriTemplate = "Activate?gloveAddress={gloveAddress}&actuator={actuator}&intensity={intensity}")]
        int Activate(string gloveAddress, int actuator, int intensity);

        /*
        [OperationContract]
        int ActivateTimed(Glove glove, int region, int intensity, int time);
        */

        [OperationContract]
        int Connect(Glove glove);

        [OperationContract]
        int Disconnect(Glove glove);
    }

    [DataContract]
    public class Glove
    {
        /// <summary>
        /// Private singleton field containing all active gloves, connected or disconnected in the system.
        /// </summary>
        private static List<Glove> gloves;

        /// <summary>
        /// Gets the current list of gloves connected to the system. If it is the first 
        /// execution since the service start, it will refresh the list.
        /// </summary>
        public static List<Glove> Gloves
        {
            get
            {
                if (gloves == null)
                {
                    gloves = ScanGloves();

                }
                return gloves;
            }
        }

        /// <summary>
        /// Same behaviour as Gloves, but always refreshes the glove list.
        /// </summary>
        /// <returns></returns>
        public static List<Glove> RefreshGloves()
        {
            gloves = ScanGloves();
            return gloves;
        }

        /// <summary>
        /// Scans the system using 32Feet.NET for OpenGlove devices. Currently it only filters by the bluetooth device Name, so
        /// any device containing "OpenGlove" on their name would be picked. Hardware limitation.
        /// </summary>
        /// <returns></returns>
        private static List<Glove> ScanGloves()
        {

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
}
