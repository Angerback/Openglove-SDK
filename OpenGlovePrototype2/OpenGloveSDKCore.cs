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
        //Simple singleton pattern for SDKCore
        private static OpenGloveSDKCore core;

        private OpenGlove.OpenGlove openGlove;


        //Datos de desarrollo temprano
        private List<int> positivePins = new List<int>() { 10, 9, 6, 5, 3 };
        private List<int> negativePins = new List<int>() { 15, 14, 12, 8, 2 };
        private List<string> negativeInit = new List<string>() { "LOW", "LOW", "LOW", "LOW", "LOW" };
        private List<string> positiveInit = new List<string>() { "HIGH", "HIGH", "HIGH", "HIGH", "HIGH" };

        public void Connect(string port)
        {
            GetOpenGlove().OpenPort(port, 57600); //Baud rate deberia seleccionarse
            GetOpenGlove().InitializeMotor(positivePins); //Positive pins deberian definirse
            GetOpenGlove().InitializeMotor(negativePins); //Negative pins deberian definirse
            GetOpenGlove().ActivateMotor(negativePins, negativeInit);
        }

        public void Disconnect() {
            GetOpenGlove().ClosePort();
        }

        public void StartTest() {
            GetOpenGlove().ActivateMotor(positivePins, positiveInit);
        }

        public void StopTest()
        {
            GetOpenGlove().ActivateMotor(positivePins, negativeInit);
        }

        public static OpenGloveSDKCore getCore()
        {
            if (core == null)
            {
                core = new OpenGloveSDKCore();
            }
            return core;
        }

        private OpenGloveSDKCore()
        {
            Mappings = new Dictionary<string, string>();
        }

        private int ACTUATORS = 20;

        //Solucion temporal
        private String[] actuators = { "", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" };

        

        /// <summary>
        /// Mappings for region (key) and actuators (value).
        /// </summary>
        public Dictionary<String, String> Mappings { get; set; }

        public int getActuatorCount()
        {
            return this.ACTUATORS;
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
            foreach (KeyValuePair<string, string> mapping in this.Mappings)
            {
                XElement mappingXML = new XElement("mapping", new XElement("region", mapping.Key), new XElement("actuator", mapping.Value));
                rootXML.Add(mappingXML);
            }
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
            openedConfiguration = xml.Root.Elements("mapping")
                               .ToDictionary(c => (string)c.Element("region"),
                                             c => (string)c.Element("actuator"));
            this.Mappings = openedConfiguration;
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
