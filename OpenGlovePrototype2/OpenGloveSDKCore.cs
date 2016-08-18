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
        private int saveConfiguration(Dictionary<String, String> mappings, String name)
        {
            XElement rootXML = new XElement("hand");
            foreach (KeyValuePair<string, string> mapping in mappings)
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
        Dictionary<String, String> openConfiguration(String fileName)
        {
            Dictionary<String, String> openedConfiguration;

            XDocument xml = XDocument.Load(fileName);
            openedConfiguration = xml.Root.Elements("mapping")
                               .ToDictionary(c => (string)c.Element("region"),
                                             c => (string)c.Element("actuator"));

            return openedConfiguration;
        }


    }
}
