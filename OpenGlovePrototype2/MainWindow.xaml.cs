﻿using Microsoft.Win32;
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
    public partial class MainWindow : Window
    {
        private OpenGloveSDKCore sdkCore;

        private List<ComboBox> selectors;

        private List<String> actuators;

        public MainWindow()
        {
            InitializeComponent();
            this.sdkCore = OpenGloveSDKCore.getCore();
            this.initializeSelectors();
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
           
            actuators = new List<string>();

            this.resetSelectors();
        }

        /// <summary>
        /// Erases an actuator from the selectors (ComboBox) that doesn't own it.
        /// </summary>
        /// <param name="actuator"></param>
        /// <param name="owner"></param>
        private void removeActuator(String actuator, object owner)
        {
            foreach (ComboBox selector in this.selectors)
            {
                if (((ComboBox)owner) != selector)
                {
                    selector.Items.Remove(actuator);
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
                    selector.Items.Add(liberatedActuator);
                }
            }
        }

        /// <summary>
        /// Resets all selectors to it's initial selection (index 0).
        /// </summary>
        private void resetSelectors()
        {
            /*
            foreach (ComboBox selector in this.selectors)
            {
                selector.SelectedIndex = 0;
            }
            */
            foreach (ComboBox selector in selectors)
            {
                selector.Items.Clear();
                selector.Items.Add("");
            }

            for (int i = 0; i < sdkCore.getActuatorCount(); i++)
            {
                actuators.Add(i.ToString());
                foreach (ComboBox selector in selectors)
                {
                    selector.Items.Add(i.ToString());
                }
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
            foreach (KeyValuePair<string, string> mapping in mappings)
            {
                this.mappingsList.Items.Add("Actuator " + mapping.Value + " assigned to region " + mapping.Key);
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

            if (openConfigurationDialog.FileName != "")
            {
                this.sdkCore.openConfiguration(openConfigurationDialog.FileName);
                if (this.sdkCore.Mappings != null)
                {
                    //Actualizar vista
                    this.refreshMappingsList(this.sdkCore.Mappings);
                    this.resetSelectors();
                    
                    foreach (KeyValuePair<string, string> mapping in this.sdkCore.Mappings.ToList())
                    {
                        this.selectors[Int32.Parse(mapping.Key)].SelectedItem = mapping.Value;
                        this.removeActuator(mapping.Value, this.selectors[Int32.Parse(mapping.Key)]);
                    }
                    this.statusBarItemProfile.Content = openConfigurationDialog.FileName;
                }
                else
                {
                    string message = "File not found.";
                    string caption = "File not found";
                    MessageBoxButton button = MessageBoxButton.OK;

                    MessageBox.Show(message, caption, button, MessageBoxImage.Error);

                }

            }
        }

        private void selectorsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String selection = (String)((ComboBox)sender).SelectedItem;
            //String region = (String)((ComboBox)sender).AccessibleName;

            //Si el selector usado poseia otro actuador con anterioridad, este debe ser liberado y añadido a los otros selectores, en general, a todos.

            // Si se selecciona un actuador, la idea es que no se pueda volver a seleccionar en otro punto de la mano.

            String owner = ((ComboBox)sender).TabIndex.ToString();
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

            refreshMappingsList(this.sdkCore.Mappings);
        }
    }
}
