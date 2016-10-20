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
using Hardcodet.Wpf.TaskbarNotification;
using OpenGlovePrototype2.ServiceReference1;
using OpenGloveSDK;

namespace OpenGlovePrototype2
{
    /// <summary>
    /// Interaction logic for Greeter.xaml
    /// </summary>
    public partial class Greeter : Window
    {
        private TaskbarIcon tbi;

        private OGCore sdkCore;

        private OGServiceClient sdkClient;

        public Greeter()
        {
            InitializeComponent();

            sdkClient = new OGServiceClient("BasicHttpBinding_IOGService");

            sdkCore = OGCore.GetCore();
            sdkCore.gloveCfg.BaudRate = sdkClient.GetBaudRate();
            sdkCore.gloveCfg.gloveHash = sdkClient.GetGloveHash();
            sdkCore.gloveCfg.gloveName = sdkClient.GetGloveName();
            try
            {
                sdkCore.gloveCfg.positivePins = sdkClient.GetPositivePins().ToList();
                sdkCore.gloveCfg.negativePins = sdkClient.GetNegativePins().ToList();
                sdkCore.gloveCfg.positiveInit = sdkClient.GetPositiveInit().ToList();
                sdkCore.gloveCfg.negativeInit = sdkClient.GetNegativeInit().ToList();
            }
            catch (Exception)
            {
                sdkCore.gloveCfg.positivePins = null;
                sdkCore.gloveCfg.negativePins = null;
                sdkCore.gloveCfg.positiveInit = null;
                sdkCore.gloveCfg.negativeInit = null;
            }
            
            try
            {
                sdkCore.profileCfg.Mappings = sdkClient.GetMappingsDictionary();
                sdkCore.profileCfg.profileName = sdkClient.GetProfileName();
                sdkCore.profileCfg.gloveHash = sdkClient.GetProfileGloveHash();
            }
            catch (Exception)
            {

            }

            updateControls();

            tbi = new TaskbarIcon();
            tbi.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location); ;
            tbi.ToolTipText = "OpenGlove";

            MenuItem mi1 = new MenuItem();
            mi1.Header = "Exit";
            mi1.Click += sysTrayItemClicked;

            tbi.ContextMenu = new ContextMenu();

            tbi.ContextMenu.Items.Add(mi1);

            tbi.TrayLeftMouseUp += this.onTrayClick;

        }

        private void updateControls() {
            var config = sdkCore.gloveCfg;
            this.buttonConnectGlove.IsEnabled = false;
            if (config.positivePins == null)
            {
                Console.WriteLine("No config");
                this.buttonOpenProfile.IsEnabled = false;
                this.buttonNewProfile.IsEnabled = false;
                this.buttonConnectGlove.IsEnabled = false;
                this.labelGloveConfig.Content = "None. Please select or create a new glove configuration.";
            }
            else {
                this.buttonOpenProfile.IsEnabled = true;
                this.buttonNewProfile.IsEnabled = true;

                this.labelGloveConfig.Content = config.gloveName;
            }

            var profile = sdkCore.profileCfg;
            if (profile.Mappings.Count == 0)
            {
                this.labelProfile.Content = "None.";
            }
            else {
                this.buttonConnectGlove.IsEnabled = true;
                this.labelProfile.Content = profile.profileName;
            }

        }
       
        private void onTrayClick(object sender, RoutedEventArgs e)
        {
            toggleVisibility();
            updateControls();
        }

        private void toggleVisibility() {
            if (this.Visibility == Visibility.Visible)
            {
                this.Visibility = Visibility.Hidden;
            }
            else {
                this.Visibility = Visibility.Visible;
            }
        }

        private void buttonCreateConfiguration_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("This will close the current profile. Are you sure?", "New configuration confirmation", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes) {
                PinsConfiguration pinsConfig = new PinsConfiguration();
                pinsConfig.Show();
                sdkCore.resetProfile();
                sdkClient.SetProfile(sdkCore.profileCfg.profileName, sdkCore.profileCfg.gloveHash, sdkCore.profileCfg.Mappings);
                //this.Visibility = Visibility.Hidden;
            }
              
        }

        private void buttonOpenProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("This will close the current profile. Are you sure?", "New configuration confirmation", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes) {
                OpenFileDialog openConfigurationDialog = new OpenFileDialog();
                openConfigurationDialog.Filter = "XML-File | *.xml";
                openConfigurationDialog.Title = "Open a configuration file";
                openConfigurationDialog.ShowDialog();

                if (openConfigurationDialog.FileName != null)
                {
                    if (openConfigurationDialog.FileName != "")
                    {
                        sdkCore.profileCfg.openProfileConfiguration(openConfigurationDialog.FileName, sdkCore.gloveCfg.gloveHash);
                        sdkClient.SetProfile(sdkCore.profileCfg.profileName, sdkCore.profileCfg.gloveHash, sdkCore.profileCfg.Mappings);

                        ConfigurationTool config = new ConfigurationTool(false);
                        config.Show();
                        this.updateControls();
                        //this.Visibility = Visibility.Hidden;
                    }
                }
            }
            
        }

        private void createProfile_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("This will close the current profile. Are you sure?", "New configuration confirmation", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes) {
                ConfigurationTool config = new ConfigurationTool(true);
                config.Show();
                //this.Visibility = Visibility.Hidden;
                this.updateControls();
            }
            
        }

        private void buttonLoadConfiguration_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("This will close the current profile. Are you sure?", "New configuration confirmation", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes) {
                OpenFileDialog openConfigurationDialog = new OpenFileDialog();
                openConfigurationDialog.Filter = "XML-File | *.xml";
                openConfigurationDialog.Title = "Open a glove configuration file";
                openConfigurationDialog.ShowDialog();

                if (openConfigurationDialog.FileName != null)
                {
                    if (openConfigurationDialog.FileName != "")
                    {
                        sdkCore.gloveCfg.openGloveConfiguration(openConfigurationDialog.FileName);
                        sdkClient.SetConfiguration(sdkCore.gloveCfg.BaudRate, sdkCore.gloveCfg.positivePins.ToArray(), sdkCore.gloveCfg.negativePins.ToArray(), sdkCore.gloveCfg.positiveInit.ToArray(), sdkCore.gloveCfg.negativeInit.ToArray(), sdkCore.gloveCfg.gloveHash, sdkCore.gloveCfg.gloveName);
                        sdkCore.resetProfile();
                        sdkClient.SetProfile(sdkCore.profileCfg.profileName, sdkCore.profileCfg.gloveHash, sdkCore.profileCfg.Mappings);
                    }
                }
                updateControls();
            }
        }

        private void buttonConnectGlove_Click(object sender, RoutedEventArgs e)
        {
            TestWindow test = new TestWindow(sdkClient);
            test.Show();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.toggleVisibility();
        }

        private void sysTrayItemClicked(object sender, RoutedEventArgs e) {
            if (((MenuItem)sender).Header.Equals("Exit")) {
                this.Close();
            }
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> test = await sdkClient.getGlovesAsync();
            foreach (var item in test)
            {
                Console.WriteLine(item.Key + ": " + item.Value);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (sdkCore.profileCfg.Mappings.Count != 0)
            {
                ConfigurationTool config = new ConfigurationTool(false);
                config.Show();
            }
            else
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("No profile loaded.", "No profile", System.Windows.MessageBoxButton.OK);

            }
        }
    }
}
