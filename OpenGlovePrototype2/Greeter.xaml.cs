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
using OpenGloveSDK;
using System.ComponentModel;

namespace OpenGlovePrototype2
{

    /// <summary>
    /// Interaction logic for Greeter.xaml
    /// </summary>
    public partial class Greeter : Window
    {
        private TaskbarIcon tbi;

        //Destruir
        private OGCore sdkCore;

        private Core.Gloves gloves = Core.Gloves.GetInstance();

        private BackgroundWorker bgw;

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
            this.listViewGloves.ItemsSource = (List<Core.OpenGloveService.Glove>) e.Result;
        }

        public Greeter()
        {
            InitializeComponent();

            bgw = new BackgroundWorker();
            bgw.WorkerReportsProgress = true;
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            

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

        private List<Core.OpenGloveService.Glove> updateGloves() {
            return gloves.Devices;
        }

        LoadingBar bar;
        private void updateControls() {
            //var config = sdkCore.gloveCfg;
            /*
            this.buttonConnectGlove.IsEnabled = false;
            */
            bgw.RunWorkerAsync();
            bar = new LoadingBar(); 
            bar.ShowDialog();
            
            /*
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
            }*/

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

        private void sysTrayItemClicked(object sender, RoutedEventArgs e)
        {
            if (((MenuItem)sender).Header.Equals("Exit"))
            {
                this.Close();
            }
        }
    }
}
