using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenGlove
{
    public class OGCore
    {
        public class GloveConfiguration {
            public int BaudRate { get; set; }

            public List<int> allowedBaudRates { get; } = new List<int> { 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200 };
            //Datos de desarrollo temprano
            public List<int> positivePins { get; set; } //Aspectos del guante, hacer configuracion especial para esto
            public List<int> negativePins { get; set; } //Aspectos del guante, hacer configuracion especial para esto

            public List<string> negativeInit { get; set; }
            public List<string> positiveInit { get; set; }

            public String gloveHash { get; set; }

            public String gloveName { get; set; }

            public void openGloveConfiguration(String fileName)
            {

                XDocument xml = XDocument.Load(fileName);
                List<XElement> Xpins = xml.Root.Element("boardPins").Elements("positivePin").ToList();
                List<int> positivePins = new List<int>();
                foreach (XElement xpin in Xpins)
                {
                    int pinNumber = Int32.Parse(xpin.Attribute("pin").Value);
                    positivePins.Add(pinNumber);
                }

                Xpins = xml.Root.Element("boardPins").Elements("negativePin").ToList();
                List<int> negativePins = new List<int>();
                foreach (XElement xpin in Xpins)
                {
                    int pinNumber = Int32.Parse(xpin.Attribute("pin").Value);
                    negativePins.Add(pinNumber);
                }

                int baudRate = Int32.Parse(xml.Root.Attribute("baudRate").Value);
                this.positivePins = positivePins;
                this.negativePins = negativePins;
                this.BaudRate = baudRate;
                this.gloveHash = (string)xml.Root.Attribute("gloveHash");
                this.gloveName = (string)xml.Root.Attribute("gloveName");

                this.positiveInit = new List<string>();
                this.negativeInit = new List<string>();

                for (int i = 0; i < positivePins.Count; i++)
                {
                    this.positiveInit.Add("HIGH");
                    this.negativeInit.Add("LOW");
                }
            }

            /// <summary>
            /// Saves the current glove pins configuration on an XML file.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public int saveGloveConfiguration(String name)
            {
                XElement rootXML = new XElement("hand");
                XElement boardPins = new XElement("boardPins");
                rootXML.Add(boardPins);
                foreach (int pin in positivePins)
                {
                    XElement positivePinXML = new XElement("positivePin");
                    positivePinXML.SetAttributeValue("pin", pin);
                    boardPins.Add(positivePinXML);
                }

                foreach (int pin in negativePins)
                {
                    XElement negativePinXML = new XElement("negativePin");
                    negativePinXML.SetAttributeValue("pin", pin);
                    boardPins.Add(negativePinXML);
                }
                gloveHash = positivePins.GetHashCode().ToString();
                rootXML.SetAttributeValue("baudRate", BaudRate);
                rootXML.SetAttributeValue("gloveHash", gloveHash);
                rootXML.SetAttributeValue("gloveName", name);
                this.gloveName = name;
                rootXML.Save(name);

                return 0;
            }
        }

        public class ProfileConfiguration {
            public String profileName { get; set; }

            public String gloveHash { get; set; }

            public int AreaCount { get; } = 58;

            /// <summary>
            /// Mappings for region (key) and actuators (value).
            /// </summary>
            public Dictionary<String, String> Mappings { get; set; } = new Dictionary<string, string>();

            /// <summary>
            /// Saves a Dictionary in a XML file with the provided name. Used to save mappings config.
            /// </summary>
            /// <param name="mappings"></param>
            /// <param name="name"></param>
            public int saveProfileConfiguration(String name, String currentGloveHash)
            {
                XElement rootXML = new XElement("hand");
                rootXML.SetAttributeValue("gloveHash", currentGloveHash);
                XElement mappings = new XElement("mappings");
                rootXML.Add(mappings);
                foreach (KeyValuePair<string, string> mapping in this.Mappings)
                {
                    XElement mappingXML = new XElement("mapping", new XElement("region", mapping.Key), new XElement("actuator", mapping.Value));
                    mappings.Add(mappingXML);
                }
                rootXML.Save(name);
                this.profileName = name;
                this.gloveHash = currentGloveHash;
                return 0;
            }

            /// <summary>
            /// Opens an OpenGlove XML Configuration File and creates a mappings dictionary.
            /// </summary>
            /// <param name="fileName"></param>
            /// <returns></returns>
            public Dictionary<String, String> openProfileConfiguration(String fileName, String currentGloveHash)
            {
                Dictionary<String, String> openedConfiguration;

                XDocument xml = XDocument.Load(fileName);

                if(!xml.Root.Attribute("gloveHash").Equals(currentGloveHash))
                { 
                    //avisar
                }

                openedConfiguration = xml.Root.Element("mappings").Elements("mapping")
                                   .ToDictionary(c => (string)c.Element("region"),
                                                 c => (string)c.Element("actuator"));
                this.Mappings = openedConfiguration;

                //Aqui deberia comprobarse que sean todos valores validos
                this.profileName = fileName;
                this.gloveHash = currentGloveHash;
                return openedConfiguration;
            }
        }

        //Simple singleton pattern for SDKCore
        private static OGCore core;

        public ProfileConfiguration profileCfg { get; set; }

        public GloveConfiguration gloveCfg { get; set; }

        private OpenGlove openGlove;

        public OGCore() {
            this.openGlove = new OpenGlove();
            this.profileCfg = new ProfileConfiguration();
            this.gloveCfg = new GloveConfiguration();
        }

        public void resetProfile()
        {
            this.profileCfg = new ProfileConfiguration();
        }

        /// <summary>
        /// Starts a connection with an OpenGlove on the desired port.
        /// </summary>
        /// <param name="port"></param>
        public void Connect(string port)
        {
            openGlove.OpenPort(port, this.gloveCfg.BaudRate);
            openGlove.InitializeMotor(this.gloveCfg.positivePins); //Positive pins deberian definirse
            openGlove.InitializeMotor(this.gloveCfg.negativePins); //Negative pins deberian definirse
            openGlove.ActivateMotor(this.gloveCfg.negativePins, this.gloveCfg.negativeInit);
        }

        /// <summary>
        /// Closes the current active connection.
        /// </summary>
        public void Disconnect() {
            openGlove.ClosePort();
        }

        /// <summary>
        /// Sets all the configured actuators to HIGH state.
        /// </summary>
        public void StartTest() {
            openGlove.ActivateMotor(this.gloveCfg.positivePins, this.gloveCfg.positiveInit);
        }

        /// <summary>
        /// Sets all the configured actuators to LOW state.
        /// </summary>
        public void StopTest()
        {
            openGlove.ActivateMotor(this.gloveCfg.positivePins, this.gloveCfg.negativeInit);
        }

        /// <summary>
        /// Returns the instance of the SDKCore.
        /// </summary>
        /// <returns></returns>
        public static OGCore GetCore()
        {
            if (core == null)
            {
                core = new OGCore();
            }
            return core;
        }

        /// <summary>
        /// Gets how many actuators are in the current configuration of the glove.
        /// </summary>
        /// <returns></returns>
        public int GetActuatorCount()
        {
            return this.gloveCfg.positivePins.Count;
        }

        public List<int> GetActuators() {
            return this.gloveCfg.positivePins;
        }

        /// <summary>
        /// Assings an actuator to a desired region.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void assignActuator(String region, String actuator)
        {
            if (!actuator.Equals(""))
            {
                try
                {
                    this.profileCfg.Mappings.Add(region, actuator);
                }
                catch (Exception)
                {
                    this.profileCfg.Mappings[region] = actuator;
                }
            }
            else
            {
                String liberatedActuator;
                this.profileCfg.Mappings.TryGetValue(region, out liberatedActuator);
                if (liberatedActuator != null)
                {
                    this.profileCfg.Mappings.Remove(region);
                }

            }
        }

        public string[] GetPortNames() {
            return this.openGlove.GetPortNames();
        }

        

    }
}
