using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenGloveSDKConfigurationPrototype2;
using Microsoft.Win32;
using OpenGloveSDKBackend;

namespace OpenGlovePrototype2
{
    /// <summary>
    /// Interaction logic for Greeter.xaml
    /// </summary>
    public partial class Greeter : Window
    {
        public Greeter()
        {
            InitializeComponent();
        }

        private void buttonCreateProfile_Click(object sender, RoutedEventArgs e)
        {
            PinsConfiguration pinsConfig = new PinsConfiguration();
            pinsConfig.Show();
            this.Close();
        }

        private void buttonOpenProfile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openConfigurationDialog = new OpenFileDialog();
            openConfigurationDialog.Filter = "XML-File | *.xml";
            openConfigurationDialog.Title = "Open a configuration file";
            openConfigurationDialog.ShowDialog();

            if (openConfigurationDialog.FileName != null) {
                if (openConfigurationDialog.FileName != "") {
                    OpenGloveSDKCore core = OpenGloveSDKCore.getCore();
                    core.openConfiguration(openConfigurationDialog.FileName);

                    ConfigurationTool config = new ConfigurationTool();
                    config.Show();
                    this.Close();
                }
            }
        }
    }
}
