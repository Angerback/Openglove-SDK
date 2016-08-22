using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenGloveSDKBackend
{
    class OpenGloveSDKCore
    {
        public OpenGloveSDKCore() {
            Mappings = new Dictionary<string, string>();
            if (BaudRate == 0) {
                BaudRate = 57600;
            }
        }

        //Simple singleton pattern for SDKCore
        private static OpenGloveSDKCore core;

        private OpenGlove.OpenGlove openGlove;

        public int BaudRate { get; set; }

        public List<int> allowedBaudRates { get; } = new List<int> { 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200 };

        //Datos de desarrollo temprano
        public List<int> positivePins { get; set; } = new List<int>() { 10, 9, 6, 5, 3 }; //Aspectos del guante, hacer configuracion especial para esto
        public List<int> negativePins { get; set; } = new List<int>() { 15, 14, 12, 8, 2 }; //Aspectos del guante, hacer configuracion especial para esto

        private List<string> negativeInit = new List<string>() { "LOW", "LOW", "LOW", "LOW", "LOW" };
        private List<string> positiveInit = new List<string>() { "HIGH", "HIGH", "HIGH", "HIGH", "HIGH" };

        /// <summary>
        /// Starts a connection with an OpenGlove on the desired port.
        /// </summary>
        /// <param name="port"></param>
        public void Connect(string port)
        {
            GetOpenGlove().OpenPort(port, BaudRate);
            GetOpenGlove().InitializeMotor(positivePins); //Positive pins deberian definirse
            GetOpenGlove().InitializeMotor(negativePins); //Negative pins deberian definirse
            GetOpenGlove().ActivateMotor(negativePins, negativeInit);
        }

        /// <summary>
        /// Closes the current active connection.
        /// </summary>
        public void Disconnect() {
            GetOpenGlove().ClosePort();
        }

        /// <summary>
        /// Sets all the configured actuators to HIGH state.
        /// </summary>
        public void StartTest() {
            GetOpenGlove().ActivateMotor(positivePins, positiveInit);
        }

        /// <summary>
        /// Sets all the configured actuators to LOW state.
        /// </summary>
        public void StopTest()
        {
            GetOpenGlove().ActivateMotor(positivePins, negativeInit);
        }

        /// <summary>
        /// Returns the instance of the SDKCore.
        /// </summary>
        /// <returns></returns>
        public static OpenGloveSDKCore getCore()
        {
            if (core == null)
            {
                core = new OpenGloveSDKCore();
            }
            return core;
        }

        /// <summary>
        /// Mappings for region (key) and actuators (value).
        /// </summary>
        public Dictionary<String, String> Mappings { get; set; }

        /// <summary>
        /// Gets how many actuators are in the current configuration of the glove.
        /// </summary>
        /// <returns></returns>
        public int GetActuatorCount()
        {
            return this.positivePins.Count;
        }

        public List<int> GetActuators() {
            return this.positivePins;
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
                    this.Mappings.Add(region, actuator);
                }
                catch (Exception)
                {
                    this.Mappings[region] = actuator;
                }
            }
            else
            {
                String liberatedActuator;
                this.Mappings.TryGetValue(region, out liberatedActuator);
                if (liberatedActuator != null)
                {
                    this.Mappings.Remove(region);
                }

            }
        }


        /// <summary>
        /// Saves a Dictionary in a XML file with the provided name. 
        /// In upcoming releases it should recieve a "Hand" object of some kind.
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="name"></param>
        public int saveConfiguration(String name)
        {
            XElement rootXML = new XElement("hand");
            XElement mappings = new XElement("mappings");
            rootXML.Add(mappings);
            foreach (KeyValuePair<string, string> mapping in this.Mappings)
            {
                XElement mappingXML = new XElement("mapping", new XElement("region", mapping.Key), new XElement("actuator", mapping.Value));
                mappings.Add(mappingXML);
            }

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

            rootXML.SetAttributeValue("baudRate", BaudRate);

            rootXML.Save(name);

            return 0;
        }

        /// <summary>
        /// Opens an OpenGlove XML Configuration File and creates a mappings dictionary.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Dictionary<String, String> openConfiguration(String fileName)
        {
            Dictionary<String, String> openedConfiguration;

            XDocument xml = XDocument.Load(fileName);
            openedConfiguration = xml.Root.Element("mappings").Elements("mapping")
                               .ToDictionary(c => (string)c.Element("region"),
                                             c => (string)c.Element("actuator"));
            this.Mappings = openedConfiguration;

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

            //Aqui deberia comprobarse que sean todos valores validos

            this.positivePins = positivePins;
            this.negativePins = negativePins;
            this.BaudRate = baudRate;

            return openedConfiguration;
        }

        public OpenGlove.OpenGlove GetOpenGlove()
        {
            if (this.openGlove == null) {
                this.openGlove = new OpenGlove.OpenGlove();
            }
            return this.openGlove;
        }


    }
}
