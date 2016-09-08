using Core.OpenGloveService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
    }

        
    }
