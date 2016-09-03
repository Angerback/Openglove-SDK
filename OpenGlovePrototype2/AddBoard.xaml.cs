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
    /// Interaction logic for AddBoard.xaml
    /// </summary>
    public partial class AddBoard : Window
    {
        public string BoardName { get; set; } = null;

        public int Pins { get; set; } = 0;

        public AddBoard()
        {

            InitializeComponent();
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            BoardName = this.textBoxBoardName.Text;
            Pins = (int) this.pinsSelector.Value;
        }
    }
}
