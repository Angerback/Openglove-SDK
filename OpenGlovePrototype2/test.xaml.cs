using OpenGlove;
using System.Windows;

namespace OpenGlovePrototype2
{
    /// <summary>
    /// Interaction logic for welcome.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        private OpenGloveSDKCore sdkCore;

        public TestWindow()
        {
            InitializeComponent();
            sdkCore = OpenGloveSDKCore.GetCore();
            //this.connectionBar.Visibility = Visibility.Hidden;
            /*
            OpenFileDialog openConfigurationDialog = new OpenFileDialog();
            openConfigurationDialog.Filter = "XML-File | *.xml";
            openConfigurationDialog.Title = "Open a configuration file";
            openConfigurationDialog.ShowDialog();
            sdkCore.openConfiguration(openConfigurationDialog.FileName);
            */
        }

        private void buttonRefreshPorts_Click(object sender, RoutedEventArgs e)
        {
            this.listViewPorts.Items.Clear();
            string[] ports = sdkCore.GetPortNames();

            foreach (var port in ports)
            {
                this.listViewPorts.Items.Add(port);
            }
            this.buttonActivate.IsEnabled = true;

        }

        
        private void buttonActivate_Click(object sender, RoutedEventArgs e)
        {
            string port = (string)listViewPorts.SelectedItem;
            if (port == null) {
                string message = "You must select a COM port.";
                string caption = "COM Port Error";
                MessageBoxButton button = MessageBoxButton.OK;

                System.Windows.MessageBox.Show(message, caption, button, MessageBoxImage.Information);
                return;
            }
            //Establecer comunicacion
            sdkCore.Connect(port);
            this.buttonActivate.IsEnabled = false;
            this.buttonStop.IsEnabled = true;
            this.buttonVibrate.IsEnabled = true;
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            sdkCore.Disconnect();
            this.buttonActivate.IsEnabled = true;
            this.buttonStop.IsEnabled = false;
            this.buttonVibrate.IsEnabled = false;
        }

        private void buttonVibrate_Click(object sender, RoutedEventArgs e)
        {
            //Activar motores
            sdkCore.StartTest();
            this.buttonVibrate.IsEnabled = false;
            this.buttonStopVibrate.IsEnabled = true;
        }

        private void buttonStopVibrate_Click(object sender, RoutedEventArgs e)
        {
            //Desactivar motores
            sdkCore.StopTest();
            this.buttonVibrate.IsEnabled = true;
            this.buttonStopVibrate.IsEnabled = false;
        }
    }
}
