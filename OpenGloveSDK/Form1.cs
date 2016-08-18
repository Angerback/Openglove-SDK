using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

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

        /// <summary>
        /// On a ComboBox index change, takes care of refreshing all the other combobox available on the form
        /// so the user can't select an actuator multiple times. Also handles when an actuator is released from
        /// one region of the hand by selecting a blank option "".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                
            }
            else {
                
                String liberatedActuator;
                this.mappings.TryGetValue(owner, out liberatedActuator);
                if (liberatedActuator != null) {
                    liberateActuator(liberatedActuator, sender);
                    this.mappings.Remove(owner);
                }
                
            }

            refreshMappingsList(this.mappings);

        }

        /// <summary>
        /// Takes a dictionary and refreshes a list based on the changes made to the dictionary using
        /// therminology of hand regions.
        /// </summary>
        /// <param name="mappings"></param>
        private void refreshMappingsList(Dictionary<string, string> mappings)
        {
            this.mappingsList.Items.Clear();
            foreach (KeyValuePair<string, string> mapping in mappings)
            {
                this.mappingsList.Items.Add("Actuator " + mapping.Value + " assigned to region " + mapping.Key);
            }


        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Saves a Dictionary in a XML file with the provided name. 
        /// In upcoming releases it should recieve a "Hand" object of some kind.
        /// </summary>
        /// <param name="mappings"></param>
        /// <param name="name"></param>
        private void saveConfiguration(Dictionary<String, String> mappings, String name) {
            XElement rootXML = new XElement("palm", mappings.Select(kv => new XElement("region_" + kv.Key , kv.Value)));
            rootXML.Save(name + ".xml");
        }

        /// <summary>
        /// Opens a save file dialog and saves the configuration made on the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSaveConfig_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveConfigurationDialog = new SaveFileDialog();
            saveConfigurationDialog.Filter = "XML-File | *.xml";
            saveConfigurationDialog.Title = "Save your configuration file";
            saveConfigurationDialog.ShowDialog();

            if (saveConfigurationDialog.FileName != "")
            {
                Console.WriteLine(saveConfigurationDialog.FileName);
                this.saveConfiguration(this.mappings, saveConfigurationDialog.FileName);
            }
        }
    }
}
