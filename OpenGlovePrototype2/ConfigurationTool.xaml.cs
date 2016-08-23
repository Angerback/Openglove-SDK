using Microsoft.Win32;
using OpenGlovePrototype2;
using OpenGloveSDKBackend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenGloveSDKConfigurationPrototype2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ConfigurationTool : Window
    {
        public class Mapping {
            public String Actuator { get; set; }
            public String Region { get; set; }
        }

        private OpenGloveSDKCore sdkCore;

        private List<ComboBox> selectors;

        private IEnumerable<int> actuators;

        public ConfigurationTool()
        {
            InitializeComponent();
            this.sdkCore = OpenGloveSDKCore.getCore();
            this.initializeSelectors();
            this.updateView();
            foreach (ComboBox selector in this.selectors)
            {
                selector.SelectionChanged -= new SelectionChangedEventHandler(selectorsSelectionChanged);
            }
        }

        /// <summary>
        /// Instantiates and populates all view's comboboxes with the available amount of actuators.
        /// </summary>
        private void initializeSelectors()
        {
            selectors = new List<ComboBox>();

            foreach (var selector in selectorsGrid.Children.OfType<ComboBox>())
            {
                selectors.Add(selector);
            }
           
            actuators = new List<int>();
        }

        /// <summary>
        /// Erases an actuator from the selectors (ComboBox) that doesn't own it.
        /// </summary>
        /// <param name="actuator"></param>
        /// <param name="owner"></param>
        private void removeActuator(String actuator, object owner)
        {
            Console.WriteLine("Removiendo actuador.");
            foreach (ComboBox selector in this.selectors)
            {
                if (((ComboBox)owner) != selector)
                {
                    selector.Items.Remove(Int32.Parse(actuator));
                }
            }
        }

        /// <summary>
        /// Repopulates an actuator on all selectors (ComboBox) except for it's owner.
        /// </summary>
        /// <param name="liberatedActuator"></param>
        /// <param name="preowner"></param>
        private void liberateActuator(String liberatedActuator, Object preowner)
        {
            foreach (ComboBox selector in this.selectors)
            {
                if ( ! selector.Equals(preowner) ) {
                    selector.Items.Add(Int32.Parse(liberatedActuator));
                }
            }
        }

        /// <summary>
        /// Resets all selectors to it's initial selection (index 0) and repopulates with available actuators.
        /// </summary>
        private void resetSelectors()
        {

            foreach (ComboBox selector in this.selectors)
            {
                selector.SelectionChanged -= new SelectionChangedEventHandler(selectorsSelectionChanged);
                selector.SelectedIndex = 0;
            }

            foreach (ComboBox selector in selectors)
            {
                selector.Items.Clear();
                selector.Items.Add("");
            }
            
            actuators = sdkCore.GetActuators();

            foreach (int actuator in actuators)
            {
                foreach (ComboBox selector in selectors)
                {
                    selector.Items.Add(actuator);
                }
            }
            
            foreach (ComboBox selector in selectors)
            {
                selector.SelectionChanged += new SelectionChangedEventHandler(selectorsSelectionChanged);
            }
        }

        /// <summary>
        /// Takes a dictionary and refreshes a list based on the changes made to the dictionary using
        /// therminology of hand regions.
        /// DEV: Should go to backend.
        /// </summary>
        /// <param name="mappings"></param>
        private void refreshMappingsList(Dictionary<string, string> mappings)
        {
            this.mappingsList.Items.Clear();
            foreach (KeyValuePair<string, string> mapping in mappings.ToList())
            {
                //Console.WriteLine("MAPPING: "+ mapping.Key + ", " + mapping.Value);
                this.mappingsList.Items.Add(new Mapping() { Actuator = mapping.Value, Region = mapping.Key});
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveConfigurationDialog = new SaveFileDialog();
            saveConfigurationDialog.Filter = "XML-File | *.xml";
            saveConfigurationDialog.Title = "Save your configuration file";
            saveConfigurationDialog.ShowDialog();

            if (saveConfigurationDialog.FileName != "")
            {
                Console.WriteLine(saveConfigurationDialog.FileName);
                this.sdkCore.saveConfiguration(saveConfigurationDialog.FileName);
                this.statusBarItemProfile.Content = saveConfigurationDialog.FileName;

                string message = "File saved.";
                string caption = "Save";
                MessageBoxButton button = MessageBoxButton.OK;

                MessageBox.Show(message, caption, button, MessageBoxImage.Information);

            }
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openConfigurationDialog = new OpenFileDialog();
            openConfigurationDialog.Filter = "XML-File | *.xml";
            openConfigurationDialog.Title = "Open a configuration file";
            openConfigurationDialog.ShowDialog();

            this.open(openConfigurationDialog);

        }

        private void open(OpenFileDialog openConfigurationDialog) {
            if (openConfigurationDialog.FileName != "")
            {
                this.sdkCore.openConfiguration(openConfigurationDialog.FileName);

                this.updateView();
            }
        }

        private void updateView() {
            
            foreach (ComboBox selector in selectors)
            {
                selector.SelectionChanged -= new SelectionChangedEventHandler(selectorsSelectionChanged);
            }
            
            if (this.sdkCore.Mappings != null)
            {
                //Actualizar vista
                this.refreshMappingsList(this.sdkCore.Mappings);
                this.resetSelectors();

                foreach (KeyValuePair<string, string> mapping in this.sdkCore.Mappings.ToList())
                {
                    this.selectors[Int32.Parse(mapping.Key)].SelectedItem = Int32.Parse(mapping.Value);
                    this.removeActuator(mapping.Value, this.selectors[Int32.Parse(mapping.Key)]);
                }
                this.statusBarItemProfile.Content = this.sdkCore.profileName;
            }
            else
            {
                string message = "File not found.";
                string caption = "File not found";
                MessageBoxButton button = MessageBoxButton.OK;

                MessageBox.Show(message, caption, button, MessageBoxImage.Error);

            }
            
            foreach (ComboBox selector in selectors)
            {
                selector.SelectionChanged += new SelectionChangedEventHandler(selectorsSelectionChanged);
            }
        }

        private void selectorsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem != null) {
                String selection = ((ComboBox)sender).SelectedItem.ToString();
                //String region = (String)((ComboBox)sender).AccessibleName;

                //Si el selector usado poseia otro actuador con anterioridad, este debe ser liberado y añadido a los otros selectores, en general, a todos.

                // Si se selecciona un actuador, la idea es que no se pueda volver a seleccionar en otro punto de la mano.

                String owner = ((ComboBox)sender).TabIndex.ToString();
                if (selection != null)
                {
                    if (!selection.Equals(""))
                    {
                        removeActuator(selection, sender);
                        try
                        {
                            this.sdkCore.Mappings.Add(owner, selection);
                        }
                        catch (Exception)
                        {
                            String liberatedActuator = this.sdkCore.Mappings[owner];
                            liberateActuator(liberatedActuator, sender);
                            this.sdkCore.Mappings[owner] = selection;
                        }
                    }
                    else
                    {
                        String liberatedActuator;
                        this.sdkCore.Mappings.TryGetValue(owner, out liberatedActuator);
                        if (liberatedActuator != null)
                        {
                            liberateActuator(liberatedActuator, sender);
                            this.sdkCore.Mappings.Remove(owner);
                        }

                    }
                }
                refreshMappingsList(this.sdkCore.Mappings);
            }
        }

        private void buttonTestConfig_Click(object sender, RoutedEventArgs e)
        {
            TestWindow test = new TestWindow();
            test.Show();
        }
    }
}
