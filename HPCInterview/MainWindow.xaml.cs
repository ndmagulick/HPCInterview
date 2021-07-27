using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

namespace HPCInterview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            editButton.IsEnabled = false;
            deleteButton.IsEnabled = false;
            submitButton.IsEnabled = false;
            cancelButton.IsEnabled = false;

            setEnableTextBoxes(false);
        }

        #region Event handlers

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            shipsListView.SelectedIndex = -1;
            setEnableTextBoxes(true);

            setEnableAddEditDeleteButtons(false);
            setEnableSubmitCancelButtons(true);
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            setEnableTextBoxes(true);

            setEnableAddEditDeleteButtons(false);
            setEnableSubmitCancelButtons(true);
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            //Prompt user before deleting
            ShipSpec selectedShip = (ShipSpec)shipsListView.Items[shipsListView.SelectedIndex];
            string promptMessage = "Are you sure you want to delete the ship " + selectedShip.Name + " (" + selectedShip.Code + ")?";

            MessageBoxResult result = MessageBox.Show(promptMessage, "Confirm Ship Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if(result == MessageBoxResult.Yes)
            {
                shipsListView.Items.RemoveAt(shipsListView.SelectedIndex--); //Post decrement the selected index so we get the previous item in the list to display
            }

            handleAddEditDeleteAfterSubmitOrCancel();
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMessages = validateFields();

            if(!string.IsNullOrEmpty(errorMessages))
            {
                MessageBox.Show(errorMessages, "Error(s) saving ship", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            ShipSpec s = new ShipSpec
            (
                shipNameTextBox.Text,
                shipCodeTextBox.Text.ToUpper(),
                int.Parse(lengthTextBox.Text),
                int.Parse(widthTextBox.Text)
            );

            if (shipsListView.SelectedIndex >= 0)
            {
                //Selected index gets cleared out after assignment, so save it here
                int savedIndex = shipsListView.SelectedIndex;

                shipsListView.Items[shipsListView.SelectedIndex] = s;
                shipsListView.SelectedIndex = savedIndex;
            }
            else
            {
                shipsListView.SelectedIndex = shipsListView.Items.Add(s);
            }

            handleAddEditDeleteAfterSubmitOrCancel();
            setEnableTextBoxes(false);
            setEnableSubmitCancelButtons(false);
        }

        private void shipsListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (shipsListView.SelectedIndex >= 0)
            {
                ShipSpec selectedShip = (ShipSpec)shipsListView.Items[shipsListView.SelectedIndex];

                shipNameTextBox.Text = selectedShip.Name;
                shipCodeTextBox.Text = selectedShip.Code;
                lengthTextBox.Text = selectedShip.Length.ToString();
                widthTextBox.Text = selectedShip.Width.ToString();
            }
            else
            {
                clearTextBoxes();
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(shipsListView.SelectedIndex == -1)
            {
                clearTextBoxes();

                //Select the first item in the list (if one exists) if we decide to not add a new ship
                shipsListView.SelectedIndex = shipsListView.Items.Count > 0 ? 0 : -1;
            }

            setEnableTextBoxes(false);
            handleAddEditDeleteAfterSubmitOrCancel();
            setEnableSubmitCancelButtons(false);
        }

        #endregion

        #region Control Enable/Disable Logic 

        private void clearTextBoxes()
        {
            shipNameTextBox.Clear();
            shipCodeTextBox.Clear();
            widthTextBox.Clear();
            lengthTextBox.Clear();
        }

        private void setEnableTextBoxes(bool enable)
        {
            shipNameTextBox.IsEnabled = enable;
            shipCodeTextBox.IsEnabled = enable;
            widthTextBox.IsEnabled = enable;
            lengthTextBox.IsEnabled = enable;
        }

        private void setEnableAddEditDeleteButtons(bool enable)
        {
            editButton.IsEnabled = enable;
            deleteButton.IsEnabled = enable;
            addButton.IsEnabled = enable;
        }

        private void setEnableSubmitCancelButtons(bool enable)
        {
            submitButton.IsEnabled = enable;
            cancelButton.IsEnabled = enable;
        }

        private void handleAddEditDeleteAfterSubmitOrCancel()
        {
            addButton.IsEnabled = true;

            // Only enable edit and delete buttons if we have ships to work with
            editButton.IsEnabled = shipsListView.Items.Count > 0;
            deleteButton.IsEnabled = shipsListView.Items.Count > 0;
        }

        #endregion

        #region Field validation

        private void numberValidation(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            //Only allow positive integers in our textboxes that are for length/width
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private string validateFields()
        {
            //Regex for ship code. Ship codes must be of the form AAAA-1111-A1 (A -> any character of the alphabet, 1 -> any number)
            Regex regex = new Regex("[a-zA-Z]{4}-[0-9]{4}-[a-zA-Z]{1}[0-9]{1}");
            string errorMessages = "";

            //Ship name validation
            if(string.IsNullOrEmpty(shipNameTextBox.Text))
            {
                errorMessages += "Please enter a ship name.\n";
            }

            // Ship code validation
            if(string.IsNullOrEmpty(shipCodeTextBox.Text))
            {
                errorMessages += "Please enter a ship code.\n";
            }
            else if(!regex.IsMatch(shipCodeTextBox.Text))
            {
                errorMessages += "Please enter a valid ship code.\n";
            }
            else if(shipCodeExists(shipCodeTextBox.Text))
            {
                errorMessages += "Ship code already exists.\n";
            }

            //Ship dimensions validation
            if(string.IsNullOrEmpty(lengthTextBox.Text))
            {
                errorMessages += "Please enter a ship length.\n";
            }
            else if(int.Parse(lengthTextBox.Text) == 0)
            {
                errorMessages += "Please enter a valid ship length.\n";
            }

            if (string.IsNullOrEmpty(widthTextBox.Text))
            {
                errorMessages += "Please enter a ship length.\n";
            }
            else if (int.Parse(widthTextBox.Text) == 0)
            {
                errorMessages += "Please enter a valid ship length.\n";
            }

            return errorMessages;
        }

        private bool shipCodeExists(string shipCode)
        {
            for(int i = 0; i < shipsListView.Items.Count; i++)
            {
                if(i != shipsListView.SelectedIndex)
                {
                    string currentCode = ((ShipSpec)shipsListView.Items[i]).Code;

                    if (shipCode.Equals(currentCode))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}
