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
using BusinessLogicController;

namespace NailSalonWPF
{
    /// <summary>
    /// Interaction logic for AddItemDialog.xaml
    /// </summary>
    public partial class AddItemDialog : Window
    {
        private List<IServiceItem> itemList;
        private BusinessLogic bl;
        public AddItemDialog(BusinessLogic bl)
        {
            this.bl = bl;
            InitializeComponent();
            itemList = bl.getServiceItemList();
            for (int i = 0; i < itemList.Count; i++)
            {
                ComboBoxItems.Items.Add(itemList.ElementAt(i));
            }
            //categoryComboBox.SelectedItem = categoryComboBox.Items.GetItemAt(2);
        }

        private int getCategoryIndex(string category)
        {
            switch (category)
            {
                case "MANICURE":
                    return 0;
                case "PEDICURE":
                    return 1;
                case "WAX":
                    return 2;
            }
            return -1;
        }

        private void ComboBoxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxItems.SelectedItem != null)
            {
                IServiceItem item = (IServiceItem)ComboBoxItems.SelectedItem;
                idTextBox.Text = item.getIdAsString();
                nameTextBox.Text = item.getName();
                priceTextBox.Text = item.getPrice().ToString();
                categoryComboBox.SelectedItem = categoryComboBox.Items.GetItemAt(getCategoryIndex(item.getCategory()));
            }
        }

        public void updateComboBox()
        {
            ComboBoxItems.Items.Clear();
            itemList = bl.getServiceItemList();
            for (int i = 0; i < itemList.Count; i++)
            {
                ComboBoxItems.Items.Add(itemList.ElementAt(i));
            }
        }

        private void Button_Click_NewItem(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("******************* " + categoryComboBox.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", "") + " *****************************");
            IServiceItem newItem = bl.processAddServiceItem(sanitizeItemName(nameTextBox.Text), Convert.ToDecimal(priceTextBox.Text), categoryComboBox.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", ""));
            idTextBox.Text = newItem.getIdAsString();
            nameTextBox.Text = newItem.getName();
            priceTextBox.Text = newItem.getPrice().ToString();
            updateComboBox();
            ComboBoxItems.SelectedIndex = ComboBoxItems.Items.Count - 1;
        }
        private string sanitizeItemName(string name)
        {
            return name.Replace(" ", "_");
        }
        private void Button_Click_UpdateItem(object sender, RoutedEventArgs e)
        {
            IServiceItem newItem = bl.processUpdateServiceItem(Convert.ToInt32(idTextBox.Text), sanitizeItemName(nameTextBox.Text), Convert.ToDecimal(priceTextBox.Text), categoryComboBox.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", ""));
            idTextBox.Text = newItem.getIdAsString();
            nameTextBox.Text = newItem.getName();
            priceTextBox.Text = newItem.getPrice().ToString();
            updateComboBox();
            //MessageBox.Show("Index of new item: " + ComboBoxItems.Items.IndexOf(getItemInComboBox(newItem)).ToString());
            getItemInComboBox(newItem);
            ComboBoxItems.SelectedIndex = ComboBoxItems.Items.IndexOf(getItemInComboBox(newItem));
        }

        private IServiceItem getItemInComboBox(IServiceItem item)
        {
            for (int i = 0; i < ComboBoxItems.Items.Count; i++)
            {
                IServiceItem listItem = (IServiceItem)ComboBoxItems.Items.GetItemAt(i);
                //MessageBox.Show("ListItem = " + listItem.getName());
                if (listItem.getIdAsString().Equals(item.getIdAsString()))
                {
                    return listItem;
                }
            }
            return null;
        }

        private void Button_Click_DeleteItem(object sender, RoutedEventArgs e)
        {
            bl.setItemInactive((IServiceItem)ComboBoxItems.SelectedItem);
            updateComboBox();
            clearForms();
        }

        private void clearForms()
        {
            ComboBoxItems.SelectedItem = null;
            idTextBox.Text = "";
            nameTextBox.Text = "";
            priceTextBox.Text = "";
            categoryComboBox.SelectedItem = null;
        }

        private void Button_Click_ClearForm(object sender, RoutedEventArgs e)
        {
            clearForms();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
