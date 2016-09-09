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
    }

        
    }
