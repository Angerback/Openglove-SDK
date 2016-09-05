using OpenGlovePrototype2.ServiceReference1;
using OpenGloveSDK;
using System.Windows;

namespace OpenGlovePrototype2
{
    /// <summary>
    /// Interaction logic for welcome.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        private OGCore sdkCore;

        private OGServiceClient sdkClient;

        public TestWindow(OGServiceClient sdkClient)
        {
            InitializeComponent();
            sdkCore = OGCore.GetCore();

            this.sdkClient = sdkClient;
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
            if (port == null)
            {
                string message = "You must select a COM port.";
                string caption = "COM Port Error";
                MessageBoxButton button = MessageBoxButton.OK;

                System.Windows.MessageBox.Show(message, caption, button, MessageBoxImage.Information);
                return;
            }
            //Establecer comunicacion

            try
            {
                sdkClient.Connect(port, true);
            }
            catch (System.UnauthorizedAccessException)
            {
                string message = "Cannot access Device. Try repairing it.";
                string caption = "COM Port Error";
                MessageBoxButton button = MessageBoxButton.OK;

                System.Windows.MessageBox.Show(message, caption, button, MessageBoxImage.Information);
                return;
            }
            catch (System.IO.IOException)
            {
                string message = "Cannot access Device. Try connecting again.";
                string caption = "Device error";
                MessageBoxButton button = MessageBoxButton.OK;

                System.Windows.MessageBox.Show(message, caption, button, MessageBoxImage.Information);
                return;
            }


            this.buttonActivate.IsEnabled = false;
            this.buttonStop.IsEnabled = true;
            this.buttonVibrate.IsEnabled = true;
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            sdkClient.Disconnect((string)listViewPorts.SelectedItem);
            this.buttonActivate.IsEnabled = true;
            this.buttonStop.IsEnabled = false;
            this.buttonVibrate.IsEnabled = false;
        }

        private void buttonVibrate_Click(object sender, RoutedEventArgs e)
        {
            //Activar motores
            sdkClient.StartTest((string)listViewPorts.SelectedItem);
            this.buttonVibrate.IsEnabled = false;
            this.buttonStopVibrate.IsEnabled = true;
        }

        private void buttonStopVibrate_Click(object sender, RoutedEventArgs e)
        {
            //Desactivar motores
            sdkClient.StopTest((string)listViewPorts.SelectedItem);
            this.buttonVibrate.IsEnabled = true;
            this.buttonStopVibrate.IsEnabled = false;
        }
    }
}
