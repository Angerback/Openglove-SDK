using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenGloveSDK
{
    public partial class Form1 : Form
    {
        private String[] actuators = {"", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10"};

        private Dictionary<int, int> mappings;

        public Form1()
        {
            InitializeComponent();
            this.comboBox1.Items.AddRange(actuators);
            this.comboBox2.Items.AddRange(actuators);
            this.comboBox3.Items.AddRange(actuators);
            this.comboBox4.Items.AddRange(actuators);
            this.comboBox5.Items.AddRange(actuators);
            this.comboBox6.Items.AddRange(actuators);
            this.comboBox7.Items.AddRange(actuators);
            this.comboBox8.Items.AddRange(actuators);
            this.comboBox9.Items.AddRange(actuators);
            this.comboBox10.Items.AddRange(actuators);
            this.comboBox11.Items.AddRange(actuators);
            this.comboBox12.Items.AddRange(actuators);
            this.comboBox13.Items.AddRange(actuators);
            this.comboBox14.Items.AddRange(actuators);
            this.comboBox15.Items.AddRange(actuators);
            this.comboBox16.Items.AddRange(actuators);
            this.comboBox17.Items.AddRange(actuators);
            this.comboBox18.Items.AddRange(actuators);
            this.comboBox19.Items.AddRange(actuators);
            this.comboBox20.Items.AddRange(actuators);
            this.comboBox21.Items.AddRange(actuators);
            this.comboBox22.Items.AddRange(actuators);
            this.comboBox23.Items.AddRange(actuators);
            this.comboBox24.Items.AddRange(actuators);
            this.comboBox25.Items.AddRange(actuators);
            this.comboBox26.Items.AddRange(actuators);
            this.comboBox27.Items.AddRange(actuators);
            this.comboBox28.Items.AddRange(actuators);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String selection = (String)((ComboBox) sender).SelectedItem;
            String region = (String)((ComboBox)sender).AccessibleName;
            this.listBox1.Items.Add("Actuator " + selection + " assigned to " + region);
        }
    }
}
