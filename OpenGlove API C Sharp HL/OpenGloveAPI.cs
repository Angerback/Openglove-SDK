using OpenGlove_API_C_Sharp_HL.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Xml.Linq;

namespace OpenGlove_API_C_Sharp_HL
{
    public class OpenGloveAPI
    {
        private static OpenGloveAPI instance;

        private OGServiceClient serviceClient;

        OpenGloveAPI()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress address = new EndpointAddress("http://localhost:9001/OGService");
            serviceClient = new OGServiceClient(binding, address);
            
        }

        public static OpenGloveAPI GetInstance()
        {
            if (instance == null)
            {
                instance = new OpenGloveAPI();
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

        public List<Glove> UpdateDevices()
        {
            return serviceClient.RefreshGloves().ToList();
        }

        public int Connect(Glove selectedGlove)
        {
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
        /*
        public void Activate(Glove selectedGlove, int region, int intensity, int time)
        {
            this.serviceClient.ActivateTimed(selectedGlove, region, intensity, time);
        }
        */

        public void Activate(Glove selectedGlove, int region, int intensity)
        {
            int actuator = -1;
            foreach (var item in selectedGlove.GloveConfiguration.GloveProfile.Mappings)
            {
                if (item.Key.Equals(region.ToString()))
                {
                    actuator = Int32.Parse(item.Value);
                    //Console.WriteLine("REGION: " + item.Key + ", ACTUATOR: " + item.Value);
                    break;
                }
            }
            if (actuator == -1)
            {
                return;
            }
            this.serviceClient.Activate(selectedGlove.BluetoothAddress, actuator, intensity);

        }
    }


    public enum PalmRegion
    {
        FingerSmallDistal,
        FingerRingDistal,
        FingerMiddleDistal,
        FingerIndexDistal,

        FingerSmallMiddle,
        FingerRingMiddle,
        FingerMiddleMiddle,
        FingerIndexMiddle,

        FingerSmallProximal,
        FingerRingProximal,
        FingerMiddleProximal,
        FingerIndexProximal,

        PalmSmallDistal,
        PalmRingDistal,
        PalmMiddleDistal,
        PalmIndexDistal,

        PalmSmallProximal,
        PalmRingProximal,
        PalmMiddleProximal,
        PalmIndexProximal,

        HypoThenarSmall,
        HypoThenarRing,
        ThenarMiddle,
        ThenarIndex,

        FingerThumbProximal,
        FingerThumbDistal,

        HypoThenarDistal,
        Thenar,

        HypoThenarProximal
    }

    public enum DorsoRegion
    {
        FingerSmallDistal = 29,
        FingerRingDistal,
        FingerMiddleDistal,
        FingerIndexDistal,

        FingerSmallMiddle,
        FingerRingMiddle,
        FingerMiddleMiddle,
        FingerIndexMiddle,

        FingerSmallProximal,
        FingerRingProximal,
        FingerMiddleProximal,
        FingerIndexProximal,

        PalmSmallDistal,
        PalmRingDistal,
        PalmMiddleDistal,
        PalmIndexDistal,

        PalmSmallProximal,
        PalmRingProximal,
        PalmMiddleProximal,
        PalmIndexProximal,

        HypoThenarSmall,
        HypoThenarRing,
        ThenarMiddle,
        ThenarIndex,

        FingerThumbProximal,
        FingerThumbDistal,

        HypoThenarDistal,
        Thenar,

        HypoThenarProximal
    }
}
