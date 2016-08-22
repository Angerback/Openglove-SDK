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
using OpenGloveSDKBackend;
using OpenGloveSDKConfigurationPrototype2;

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
        List<int> pinNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
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

            this.comboBoxBaudRate.ItemsSource = OpenGloveSDKCore.getCore().allowedBaudRates;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            OpenGloveSDKCore core = OpenGloveSDKCore.getCore();

            if (this.comboBoxBaudRate.SelectedItem != null)
            {
                core.BaudRate = Int32.Parse(this.comboBoxBaudRate.SelectedItem.ToString());
                core.positivePins = new List<int>();
                core.negativePins = new List<int>();

                foreach (PinRow pin in pins)
                {
                    if (pin.Polarity != null)
                    {
                        if (pin.Polarity.Equals("Positive"))
                        {
                            core.positivePins.Add(pin.Pin);
                        }
                        else
                        {
                            core.negativePins.Add(pin.Pin);
                        }
                    }
                }

                MainWindow mw = new MainWindow();

                mw.Show();

                this.Close();
            }
            else {
                string message = "Must select BaudRate";
                string caption = "BaudRate";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBox.Show(message, caption, button, MessageBoxImage.Error);
            }
 
        }
    }
}
