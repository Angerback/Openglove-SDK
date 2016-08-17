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
        private int ACTUATORS = 20;

        private String[] actuators = {"", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20"};

        private Dictionary<String, String> mappings;

        private List<ComboBox> selectors;

        public Form1()
        {
            InitializeComponent();
            this.mappings = new Dictionary<string, string>();
            this.selectors = new List<ComboBox>();
            this.selectors.Add(this.comboBox1);
            this.selectors.Add(this.comboBox2);
            this.selectors.Add(this.comboBox3);
            this.selectors.Add(this.comboBox4);
            this.selectors.Add(this.comboBox5);
            this.selectors.Add(this.comboBox6);
            this.selectors.Add(this.comboBox7);
            this.selectors.Add(this.comboBox8);
            this.selectors.Add(this.comboBox9);
            this.selectors.Add(this.comboBox10);
            this.selectors.Add(this.comboBox11);
            this.selectors.Add(this.comboBox12);
            this.selectors.Add(this.comboBox13);
            this.selectors.Add(this.comboBox14);
            this.selectors.Add(this.comboBox15);
            this.selectors.Add(this.comboBox16);
            this.selectors.Add(this.comboBox17);
            this.selectors.Add(this.comboBox18);
            this.selectors.Add(this.comboBox19);
            this.selectors.Add(this.comboBox20);

            foreach (ComboBox selector in this.selectors)
            {
                selector.Items.AddRange(actuators);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void removeActuator(String actuator, object owner) {
            foreach (ComboBox selector in this.selectors)
            {
                if (((ComboBox)owner) != selector) {
                    selector.Items.Remove(actuator);
                }
            }
        }

        private void liberateActuator(String liberatedActuator, Object preowner) {
            foreach (ComboBox selector in this.selectors)
            {
                if (selector != ((ComboBox)preowner)) {
                    selector.Items.Add(liberatedActuator);
                }
                
            }
        }

        private void actuatorSelectionComboboxIndexChanged(object sender, EventArgs e)
        {
            String selection = (String)((ComboBox)sender).SelectedItem;
            String region = (String)((ComboBox)sender).AccessibleName;

            //Si el selector usado poseia otro actuador con anterioridad, este debe ser liberado y añadido a los otros selectores, en general, a todos.

            // Si se selecciona un actuador, la idea es que no se pueda volver a seleccionar en otro punto de la mano.

            String owner = ((ComboBox)sender).TabIndex.ToString();
            if (!selection.Equals(""))
            {
                removeActuator(selection, sender);
                try
                {
                    this.mappings.Add(owner, selection);
                }
                catch (Exception)
                {
                    String liberatedActuator = this.mappings[owner];
                    liberateActuator(liberatedActuator, sender);
                    this.mappings[owner] = selection;
                }

                int index = this.listBox1.Items.Add("Actuator " + selection + " assigned to " + region);
            }
            else {
                String liberatedActuator = this.mappings[owner];
                liberateActuator(liberatedActuator, sender);
                this.mappings.Remove(owner);
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
