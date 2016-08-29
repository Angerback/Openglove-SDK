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
using OpenGlove;
using OpenGloveSDKConfigurationPrototype2;
using System.Windows.Forms;

namespace OpenGlovePrototype2
{
    /// <summary>
    /// Interaction logic for PinsConfiguration.xaml
    /// </summary>
    public partial class PinsConfiguration : Window
    {
        public class PinRow{
            public int Pin { get; set; }
            public String Polarity { get; set; }

            public PinRow(int pin)
            {
                this.Pin = pin;
            }
        }

        public List<PinRow> pins;
        //LilyPad. Desconozco aun todos los posibles pines.
        List<int> pinNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        List<String> Polarities;

        public PinsConfiguration()
        {
            InitializeComponent();
            this.pins = new List<PinRow>();
            foreach (int pin in pinNumbers)
            {
                this.pins.Add(new PinRow(pin));
            }
            this.dataGridPins.ItemsSource = this.pins;

            Polarities  = new List<string>() { "Positive", "Negative" };

            Polarity.ItemsSource = Polarities;

            this.comboBoxBaudRate.ItemsSource = OGCore.GetCore().gloveCfg.allowedBaudRates;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            OGCore core = OGCore.GetCore();

            SaveFileDialog saveConfigurationDialog = new SaveFileDialog();
            saveConfigurationDialog.Filter = "XML-File | *.xml";
            saveConfigurationDialog.Title = "Save your configuration file";
            saveConfigurationDialog.ShowDialog();

            if (saveConfigurationDialog.FileName != "") {
                if (this.comboBoxBaudRate.SelectedItem != null)
                {
                    core.gloveCfg.BaudRate = Int32.Parse(this.comboBoxBaudRate.SelectedItem.ToString());
                    core.gloveCfg.positivePins = new List<int>();
                    core.gloveCfg.negativePins = new List<int>();

                    foreach (PinRow pin in pins)
                    {
                        if (pin.Polarity != null)
                        {
                            if (pin.Polarity.Equals("Positive"))
                            {
                                core.gloveCfg.positivePins.Add(pin.Pin);
                            }
                            else
                            {
                                core.gloveCfg.negativePins.Add(pin.Pin);
                            }
                        }
                    }
                    /*
                    ConfigurationTool mw = new ConfigurationTool();

                    mw.Show();

                    this.Close();*/
                    core.gloveCfg.saveGloveConfiguration(saveConfigurationDialog.FileName);
                }
                else
                {
                    string message = "Must select BaudRate";
                    string caption = "BaudRate";
                    MessageBoxButton button = MessageBoxButton.OK;
                    System.Windows.MessageBox.Show(message, caption, button, MessageBoxImage.Error);
                }
            }
 
        }
    }
}
