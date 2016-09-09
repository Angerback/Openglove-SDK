using Core.OpenGloveService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core
{
    public class Gloves
    {
        private static Gloves instance;

        private OGServiceClient serviceClient;

        private Gloves() {

            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress address = new EndpointAddress("http://localhost:9001/OGService");
            serviceClient = new OGServiceClient(binding, address);
        }

        public static Gloves GetInstance() {
            if (instance == null)
            {
                instance = new Gloves();
            }
            return instance;
        }

        private List<Glove> devices;

        public List<Glove> Devices
        {
            get
            {
                devices = serviceClient.GetGloves().ToList();
                return devices;
            }
        }

        public void saveGloveConfiguration(string fileName, Glove selectedGlove)
        {
            XElement rootXML = new XElement("hand");
            XElement boardPins = new XElement("boardPins");
            rootXML.Add(boardPins);
            foreach (int pin in selectedGlove.GloveConfiguration.PositivePins)
            {
                XElement positivePinXML = new XElement("positivePin");
                positivePinXML.SetAttributeValue("pin", pin);
                boardPins.Add(positivePinXML);
            }

            foreach (int pin in selectedGlove.GloveConfiguration.NegativePins)
            {
                XElement negativePinXML = new XElement("negativePin");
                negativePinXML.SetAttributeValue("pin", pin);
                boardPins.Add(negativePinXML);
            }

            selectedGlove.GloveConfiguration.GloveHash = selectedGlove.GloveConfiguration.PositivePins.GetHashCode().ToString();
            rootXML.SetAttributeValue("baudRate", selectedGlove.GloveConfiguration.BaudRate);
            rootXML.SetAttributeValue("gloveHash", selectedGlove.GloveConfiguration.GloveHash);
            rootXML.SetAttributeValue("gloveName", fileName);
            selectedGlove.GloveConfiguration.GloveName = fileName;

            rootXML.Save(fileName);

            serviceClient.SaveGlove(selectedGlove);
        }

        public void OpenGloveConfiguration(string fileName, Glove selectedGlove)
        {
            selectedGlove.GloveConfiguration = new Glove.Configuration();

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
            selectedGlove.GloveConfiguration.PositivePins = positivePins.ToArray();
            selectedGlove.GloveConfiguration.NegativePins = negativePins.ToArray();
            selectedGlove.GloveConfiguration.BaudRate = baudRate;
            selectedGlove.GloveConfiguration.GloveHash = (string) xml.Root.Attribute("gloveHash");
            selectedGlove.GloveConfiguration.GloveName = (string) xml.Root.Attribute("gloveName");

            List<string> positiveInit = new List<string>();
            List<string> negativeInit = new List<string>();

            for (int i = 0; i < positivePins.Count; i++)
            {
                positiveInit.Add("HIGH");
                negativeInit.Add("LOW");
            }

            selectedGlove.GloveConfiguration.PositiveInit = positiveInit.ToArray();
            selectedGlove.GloveConfiguration.NegativeInit = negativeInit.ToArray();

            //Tell the service to update the glove configuration
            serviceClient.SaveGlove(selectedGlove);
        }

        public void OpenProfileConfiguration(string fileName, Glove selectedGlove)
        {
            selectedGlove.GloveConfiguration.GloveProfile = new Glove.Configuration.Profile();

            Dictionary<String, String> openedConfiguration;

            XDocument xml = XDocument.Load(fileName);

            if (!xml.Root.Attribute("gloveHash").Equals(selectedGlove.GloveConfiguration.GloveHash))
            {
                //avisar
            }

            openedConfiguration = xml.Root.Element("mappings").Elements("mapping")
                               .ToDictionary(c => (string)c.Element("region"),
                                             c => (string)c.Element("actuator"));
            selectedGlove.GloveConfiguration.GloveProfile.Mappings = openedConfiguration;

            //Aqui deberia comprobarse que sean todos valores validos
            selectedGlove.GloveConfiguration.GloveProfile.ProfileName = fileName;
            selectedGlove.GloveConfiguration.GloveProfile.GloveHash = selectedGlove.GloveConfiguration.GloveHash;
            serviceClient.SaveGlove(selectedGlove);
        }

        public void Activate(Glove selectedGlove, int region, int intensity) { 
            this.serviceClient.ActivateAsync(selectedGlove, region, intensity);
        }

        public int Connect(Glove selectedGlove) {
            try
            {
                return this.serviceClient.Connect(selectedGlove);
            }
            catch (Exception)
            {

                return -1;
            }
        }

        public int Disconnect(Glove selectedGlove)
        {
            try
            {
                return this.serviceClient.Disconnect(selectedGlove);
            }
            catch (Exception)
            {

                return -1;
            }
        }
    }

        
    }
