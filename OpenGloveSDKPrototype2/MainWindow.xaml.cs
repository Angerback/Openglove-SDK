using OpenGloveSDKConfigurationBackend;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            int actuatorsCount = sdkCore.getActuatorCount();
            
            actuators = new List<string>();

            foreach (ComboBox selector in selectors)
            {
                selector.Items.Add("");
            }

            for (int i = 0; i < actuatorsCount; i++)
            {
                actuators.Add(i.ToString());
                foreach (ComboBox selector in selectors)
                {
                    selector.Items.Add(i.ToString());
                }
            }
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
            foreach (ComboBox selector in this.selectors)
            {
                selector.SelectedIndex = 0;
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {

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

            //refreshMappingsList(this.sdkCore.Mappings);
        }
    }
}
