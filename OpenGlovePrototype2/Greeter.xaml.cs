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
using System.ComponentModel;
using OpenGlove_API_C_Sharp_HL;
using OpenGlove_API_C_Sharp_HL.ServiceReference1;

namespace OpenGlovePrototype2
{

    /// <summary>
    /// Interaction logic for Greeter.xaml
    /// </summary>
    public partial class Greeter : Window
    {
        private TaskbarIcon tbi;

        private OpenGloveAPI gloves;

        private Glove selectedGlove;

        private BackgroundWorker bgw;

        private ConfigManager configManager;

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            //Your time taking work. Here it's your data query method.
            e.Result = gloves.Devices;
        }

        void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Progress bar.
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //After completing the job.
            this.bar.Close();
            this.listViewGloves.ItemsSource = (List<Glove>) e.Result;
        }

        public Greeter()
        {

            InitializeComponent();
            configManager = new ConfigManager();
            gloves = OpenGloveAPI.GetInstance();
            bgw = new BackgroundWorker();
            bgw.WorkerReportsProgress = true;
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            
            ReloadGloves();

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

        private List<Glove> updateGloves() {
            return gloves.Devices;
        }

        LoadingBar bar;
        private void ReloadGloves() {
            bgw.RunWorkerAsync();
            bar = new LoadingBar(); 
            bar.ShowDialog();
        }
       
        private void onTrayClick(object sender, RoutedEventArgs e)
        {
            toggleVisibility();
            ReloadGloves();
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

        private void sysTrayItemClicked(object sender, RoutedEventArgs e)
        {
            if (((MenuItem)sender).Header.Equals("Exit"))
            {
                this.Close();
            }
        }

        private void listViewGloves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            selectedGlove = (Glove)((ListView)sender).SelectedItem;
            refreshControls();
        }

        private void refreshControls()
        {          
            if (selectedGlove.GloveConfiguration == null)
            {
                this.labelProfile.Content = "None";
                this.labelGloveConfig.Content = "None";
                this.buttonCreateGloveConfig.IsEnabled = true;
                this.buttonOpenGloveConfig.IsEnabled = true;
                this.buttonCreateProfileConfig.IsEnabled = false;
                this.buttonOpenProfileConfig.IsEnabled = false;
                this.ConnectMenuItem.IsEnabled = false;
                this.CurrentProfileMenuItem.IsEnabled = false;
            }
            else
            {
                this.labelGloveConfig.Content = this.selectedGlove.GloveConfiguration.GloveName;
                this.buttonCreateGloveConfig.IsEnabled = true;
                this.buttonOpenGloveConfig.IsEnabled = true;
                this.buttonCreateProfileConfig.IsEnabled = true;
                this.buttonOpenProfileConfig.IsEnabled = true;
                this.ConnectMenuItem.IsEnabled = true;

                if (this.selectedGlove.GloveConfiguration.GloveProfile == null)
                {
                    this.labelProfile.Content = "None";
                    
                }
                else
                {
                    this.CurrentProfileMenuItem.IsEnabled = true;
                    this.labelProfile.Content = this.selectedGlove.GloveConfiguration.GloveProfile.ProfileName;
                }

                if (selectedGlove.Connected)
                {
                    this.ConnectMenuItem.Header = "Disconnect";
                }
                else {
                    this.ConnectMenuItem.Header = "Connect";
                }
            }

        }

        private void buttonCreateGloveConfig_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedGlove.GloveConfiguration == null) {
                this.selectedGlove.GloveConfiguration = new Glove.Configuration();
            }

            PinsConfiguration pins = new PinsConfiguration(this.selectedGlove);
            pins.ShowDialog();
        }

        private void buttonRefreshGloves_Click(object sender, RoutedEventArgs e)
        {
            this.ReloadGloves();
        }

        private void buttonOpenGloveConfig_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("This will close the current profile. Are you sure?", "New configuration confirmation", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                OpenFileDialog openConfigurationDialog = new OpenFileDialog();
                openConfigurationDialog.Filter = "XML-File | *.xml";
                openConfigurationDialog.Title = "Open a glove configuration file";
                openConfigurationDialog.ShowDialog();

                if (openConfigurationDialog.FileName != null)
                {
                    if (openConfigurationDialog.FileName != "")
                    {
                        configManager.OpenGloveConfiguration(openConfigurationDialog.FileName, selectedGlove);
                        refreshControls();
                    }
                }
            }
        }

        private void buttonOpenProfileConfig_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("This will close the current profile. Are you sure?", "New configuration confirmation", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                OpenFileDialog openConfigurationDialog = new OpenFileDialog();
                openConfigurationDialog.Filter = "XML-File | *.xml";
                openConfigurationDialog.Title = "Open a glove profile file";
                openConfigurationDialog.ShowDialog();

                if (openConfigurationDialog.FileName != null)
                {
                    if (openConfigurationDialog.FileName != "")
                    {
                        configManager.OpenProfileConfiguration(openConfigurationDialog.FileName, selectedGlove);
                        refreshControls();
                    }
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CurrentProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {

            if (selectedGlove.GloveConfiguration.GloveProfile.Mappings.Count != 0)
            {
                ConfigurationTool config = new ConfigurationTool(selectedGlove);
                config.Show();
            }
            else
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("No profile loaded.", "No profile", System.Windows.MessageBoxButton.OK);

            }
        }

        private void ConnectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGlove.Connected)
            {
                int result = gloves.Disconnect(selectedGlove);
                
                if (result == 0)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Glove " + selectedGlove.Name + " successfully disconnected.", "Connection", MessageBoxButton.OK);
                    selectedGlove.Connected = false;
                }
            }
            else
            {
                int result = gloves.Connect(selectedGlove);
                if (result == 0)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Glove " + selectedGlove.Name + " successfully connected.", "Connection", MessageBoxButton.OK);
                    selectedGlove.Connected = true;
                }
                else
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Can't connect to" + selectedGlove.Name + ". Try repairing it.", "Connection", MessageBoxButton.OK);
                }
            }
            refreshControls();
        }

        private void buttonCreateProfileConfig_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("This will close the current profile. Are you sure?", "New configuration confirmation", MessageBoxButton.YesNo);

            ConfigurationTool config = new ConfigurationTool(this.selectedGlove);
            config.ShowDialog();
            refreshControls();
        }
    }
}
