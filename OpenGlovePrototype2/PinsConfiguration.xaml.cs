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

namespace OpenGlovePrototype2
{
    /// <summary>
    /// Interaction logic for PinsConfiguration.xaml
    /// </summary>
    public partial class PinsConfiguration : Window
    {

        //LilyPad. Desconozco aun todos los posibles pines.
        List<int> pins = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };

        public PinsConfiguration()
        {
            InitializeComponent();
            int y = 0;
            foreach (int pin in pins)
            {
                Label label = new Label { Content = pin.ToString() + ":" };
                this.pinsGrid.Children.Add(label);
                ComboBox comboBox = new ComboBox { Name = "pin" + pin };
                this.pinsGrid.Children.Add(comboBox);
                comboBox.Margin = new Thickness(comboBox.Margin.Left + 40, comboBox.Margin.Top + y, comboBox.Margin.Right, comboBox.Margin.Bottom);
                label.Margin = new Thickness(label.Margin.Left, label.Margin.Top + y, label.Margin.Right, comboBox.Margin.Bottom);
                comboBox.Width = 40;
                comboBox.Height = 20;

                label.Height = 20;

                y = y + 40;
            }
        }

        
    }
}
