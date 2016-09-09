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

        private Core.OpenGloveService.Glove selectedGlove;

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

        private List<Core.OpenGloveService.Glove> updateGloves() {
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
            this.selectedGlove = (Core.OpenGloveService.Glove) ((ListView)sender).SelectedItem;

            if (selectedGlove.GloveConfiguration == null)
            {
                this.labelProfile.Content = "None";
                this.labelGloveConfig.Content = "None";
                this.buttonCreateGloveConfig.IsEnabled = true;
                this.buttonOpenGloveConfig.IsEnabled = true;
                this.buttonCreateProfileConfig.IsEnabled = false;
                this.buttonOpenProfileConfig.IsEnabled = false;
            }
            else {
                this.labelGloveConfig.Content = this.selectedGlove.GloveConfiguration.GloveName;

                if (this.selectedGlove.GloveConfiguration.GloveProfile == null)
                {
                    this.labelProfile.Content = "None";
                    this.buttonCreateGloveConfig.IsEnabled = true;
                    this.buttonOpenGloveConfig.IsEnabled = true;
                    this.buttonCreateProfileConfig.IsEnabled = false;
                    this.buttonOpenProfileConfig.IsEnabled = false;
                }
                else
                {
                    this.labelGloveConfig.Content = this.selectedGlove.GloveConfiguration.GloveName;
                }
            }
        }

        private void buttonCreateGloveConfig_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedGlove.GloveConfiguration == null) {
                this.selectedGlove.GloveConfiguration = new Core.OpenGloveService.Glove.Configuration();
            }

            PinsConfiguration pins = new PinsConfiguration(this.selectedGlove);
            pins.ShowDialog();
        }

        private void buttonRefreshGloves_Click(object sender, RoutedEventArgs e)
        {
            this.ReloadGloves();
        }
    }
}
