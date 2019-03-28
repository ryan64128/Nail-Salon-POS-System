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
using System.Windows.Threading;
using DatabaseController;
using BusinessLogicController;
using PrinterModule;

namespace NailSalonWPF
{

    /// <summary>
    /// Interaction logic for NailSalonHome.xaml
    /// </summary>
    /// 
    public enum ButtonState { MAIN_PANEL_STATE, MANICURE_PANEL_STATE, PEDICURE_PANEL_STATE, WAX_PANEL_STATE, OTHER_PANEL_STATE }
    public partial class NailSalonHome : Window
    {
        private DispatcherTimer timer;
        private DatabaseAccessor databaseAccessor;
        private BusinessLogic mainClass;
        private PrinterConnector pm;
        private IEmployee currentEmployee;
        private List<Button> serviceButtonList;
        private List<Button> manicureButtonList;
        private List<Button> pedicureButtonList;
        private List<Button> waxButtonList;
        private List<Button> otherButtonList;
        private List<IServiceItem> serviceList;
        private const int BUTTON_LIST_MAX_SIZE = 12;
        private ButtonState buttonState;
        public NailSalonHome()
        {
            InitializeComponent();
            buttonState = ButtonState.MAIN_PANEL_STATE;
            serviceButtonList = new List<Button>();
            manicureButtonList = new List<Button>();
            pedicureButtonList = new List<Button>();
            waxButtonList = new List<Button>();
            otherButtonList = new List<Button>();
            this.Topmost = false;
            this.Loaded += new RoutedEventHandler(window_Loaded);
            databaseAccessor = new DatabaseAccessor();
            pm = new PrinterConnector(databaseAccessor);
            mainClass = new BusinessLogic(databaseAccessor, pm);
            currentEmployee = null;
            showMainButtonPanel();
        }

        public void showMainButtonPanel()
        {
            clearItemPanel();
            Button manicureButton = new Button();
            addServiceButton(manicureButton, "Manicure Menu", ButtonState.MANICURE_PANEL_STATE);
            Button pedicureButton = new Button();
            addServiceButton(pedicureButton, "Pedicure Menu", ButtonState.PEDICURE_PANEL_STATE);
            Button waxButton = new Button();
            addServiceButton(waxButton, "Wax Menu", ButtonState.WAX_PANEL_STATE);
            Button otherButton = new Button();
            addServiceButton(otherButton, "Other", ButtonState.OTHER_PANEL_STATE);

            makeButtonLists();
            buildServiceButtonPanel();
        }

        public void addServiceButton(Button b, string buttonContent, ButtonState state)
        {

            b.Background = Brushes.DodgerBlue;
            b.Foreground = Brushes.White;
            b.FontSize = 25;
            b.FontWeight = FontWeights.UltraBold;
            b.FontFamily = new FontFamily("Verdana");
            b.FontWeight = FontWeights.Medium;
            b.Margin = new Thickness(0, 0, 7, 7);
            b.Content = new TextBlock() { TextWrapping = TextWrapping.Wrap, Text = buttonContent };
            b.Click += delegate {
                buttonState = state;
                clearItemPanel();
                makeButtonLists();
                buildServiceButtonPanel();
            };
            serviceButtonList.Add(b);
        }

        public void makeButtonLists()
        {
            serviceList = mainClass.getServiceItemList();
            manicureButtonList = new List<Button>();
            pedicureButtonList = new List<Button>();
            waxButtonList = new List<Button>();
            for (int i = 0; i < serviceList.Count; i++)
            {
                IServiceItem item = serviceList.ElementAt(i);
                Button b = new Button();
                //b.Content = item.getName().Replace("_", " ");
                //b.Width = 180;
                //b.Height = 130;
                b.Background = Brushes.Violet;
                b.Foreground = Brushes.White;
                b.FontSize = 15;
                b.FontWeight = FontWeights.UltraBold;
                b.FontFamily = new FontFamily("Verdana");
                b.FontWeight = FontWeights.Medium;
                b.Margin = new Thickness(0, 0, 7, 7);
                b.Content = new TextBlock() { TextWrapping = TextWrapping.Wrap, Text = item.getName().Replace("_", " ") };
                b.Click += delegate
                {
                    if (isInvoiceReady())
                    {
                        handleAddItemToOrder(item.getIdAsString());
                    }
                };
                { };
                if (item.getCategory().Equals("MANICURE"))
                    manicureButtonList.Add(b);
                else if (item.getCategory().Equals("PEDICURE"))
                    pedicureButtonList.Add(b);
                else if (item.getCategory().Equals("WAX"))
                    waxButtonList.Add(b);
                else
                    serviceButtonList.Add(b);
            }
            Button backButton = new Button();
            backButton.Background = Brushes.PaleVioletRed;
            backButton.Foreground = Brushes.White;
            backButton.FontSize = 15;
            backButton.FontWeight = FontWeights.UltraBold;
            backButton.FontFamily = new FontFamily("Verdana");
            backButton.FontWeight = FontWeights.Medium;
            backButton.Margin = new Thickness(0, 0, 7, 7);
            backButton.Content = new TextBlock() { TextWrapping = TextWrapping.Wrap, Text = "Back" };
            backButton.Click += delegate
            {
                buttonState = ButtonState.MAIN_PANEL_STATE;
                showMainButtonPanel();
            };
            if (buttonState == ButtonState.MANICURE_PANEL_STATE)
                manicureButtonList.Add(backButton);
            if (buttonState == ButtonState.PEDICURE_PANEL_STATE)
                pedicureButtonList.Add(backButton);
            if (buttonState == ButtonState.WAX_PANEL_STATE)
                waxButtonList.Add(backButton);
        }
        public void buildServiceButtonPanel()
        {
            ServiceButtonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ServiceButtonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ServiceButtonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ServiceButtonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ServiceButtonGrid.RowDefinitions.Add(new RowDefinition());
            ServiceButtonGrid.RowDefinitions.Add(new RowDefinition());
            ServiceButtonGrid.RowDefinitions.Add(new RowDefinition());
            ServiceButtonGrid.RowDefinitions.Add(new RowDefinition());
            ServiceButtonGrid.RowDefinitions.Add(new RowDefinition());
            ServiceButtonGrid.RowDefinitions.Add(new RowDefinition());
            ServiceButtonGrid.RowDefinitions.Add(new RowDefinition());
            int index = 0;
            for (int i = 0; i < ServiceButtonGrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < ServiceButtonGrid.ColumnDefinitions.Count; j++)
                {
                    if (buttonState == ButtonState.MAIN_PANEL_STATE)
                    {
                        if (index < serviceButtonList.Count)
                        {
                            Grid.SetColumn(serviceButtonList.ElementAt(index), (j * 2) % 4);
                            Grid.SetColumnSpan(serviceButtonList.ElementAt(index), 2);
                            Grid.SetRow(serviceButtonList.ElementAt(index), (index / 2) * 2);
                            Grid.SetRowSpan(serviceButtonList.ElementAt(index), 2);
                            ServiceButtonGrid.Children.Add(serviceButtonList.ElementAt(index));
                            index++;
                        }
                    }
                    else if (buttonState == ButtonState.MANICURE_PANEL_STATE)
                    {
                        if (index < manicureButtonList.Count)
                        {
                            Grid.SetColumn(manicureButtonList.ElementAt(index), j);
                            Grid.SetRow(manicureButtonList.ElementAt(index), i);
                            ServiceButtonGrid.Children.Add(manicureButtonList.ElementAt(index));
                            index++;
                        }
                    }
                    else if (buttonState == ButtonState.PEDICURE_PANEL_STATE)
                    {
                        if (index < pedicureButtonList.Count)
                        {
                            Grid.SetColumn(pedicureButtonList.ElementAt(index), j);
                            Grid.SetRow(pedicureButtonList.ElementAt(index), i);
                            ServiceButtonGrid.Children.Add(pedicureButtonList.ElementAt(index));
                            index++;
                        }
                    }
                    else if (buttonState == ButtonState.WAX_PANEL_STATE)
                    {
                        if (index < waxButtonList.Count)
                        {
                            Grid.SetColumn(waxButtonList.ElementAt(index), j);
                            Grid.SetRow(waxButtonList.ElementAt(index), i);
                            ServiceButtonGrid.Children.Add(waxButtonList.ElementAt(index));
                            index++;
                        }
                    }
                }
            }

        }



        private void window_Loaded(Object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(Object sender, EventArgs e)
        {
            TextBlockTime.Text = DateTime.Now.ToString();
        }
        private void Button_Click_0(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text += 0;
            var scope = FocusManager.GetFocusScope(Button0); // elem is the UIElement to unfocus
            FocusManager.SetFocusedElement(scope, null); // remove logical focus
            Keyboard.ClearFocus(); // remove keyboard focus
            Button1.Focus();
            Keyboard.Focus(Button1);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text += 1;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text += 2;
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text += 3;
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text += 4;
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text += 5;
        }
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text += 6;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text += 7;
        }
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text += 8;
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text += 9;
        }

        private void Button_Click_00(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text += "00";
        }

        private void Button_Click_Period(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text += ".";
        }

        private void Button_Click_Clear(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text = "";
        }

        private void Button_Click_BackSpace(object sender, RoutedEventArgs e)
        {
            TextBoxNumbers.Text = TextBoxNumbers.Text.Substring(0, TextBoxNumbers.Text.Length - 1);
        }

        private void Button_Click_NewOrder(object sender, RoutedEventArgs e)
        {
            if (currentEmployee != null)
            {
                ListBoxOrder.Items.Clear();
                ListBoxOrder.Items.Add(mainClass.processCreateInvoice(currentEmployee.getIdAsString()));
                TextBoxTotal.Text = mainClass.getCurrentInvoiceTotals();
            }
        }

        private void Button_Click_Manicure(object sender, RoutedEventArgs e)
        {
            if (isInvoiceReady())
            {
                handleAddItemToOrder("3");
            }
        }

        private void Button_Click_Pedicure(object sender, RoutedEventArgs e)
        {
            if (isInvoiceReady())
            {
                handleAddItemToOrder("4");
            }
        }

        public void handleAddItemToOrder(string itemNumber)
        {
            ListBoxOrder.Items.Add(mainClass.processAddLineItem(itemNumber, sanitizeInt(TextBoxNumbers.Text)));
            TextBoxTotal.Text = mainClass.getCurrentInvoiceTotals();
            clearTextBoxNumbers();
        }

        public string sanitizeInt(string rawNumber)
        {
            if (rawNumber == "")
                return "1";
            int valueAsInt = Convert.ToInt32(Math.Floor(Convert.ToDouble(rawNumber)));
            return valueAsInt.ToString();
        }
        private void Button_Click_Void(object sender, RoutedEventArgs e)
        {
            if (isInvoiceReady())
            {
                mainClass.processVoidLineItem((ILineItem)ListBoxOrder.SelectedItem);
                ListBoxOrder.Items.Remove(ListBoxOrder.SelectedItem);
                TextBoxTotal.Text = mainClass.getCurrentInvoiceTotals();
            }
        }

        private void Button_Click_PayCash(object sender, RoutedEventArgs e)
        {
            if (isInvoiceReady())
            {
                handlePayment("CASH");
            }
        }

        private void Button_Click_PayCredit(object sender, RoutedEventArgs e)
        {
            if (isInvoiceReady())
            {
                handlePayment("CREDIT");
            }
        }

        private void Button_Click_PayGift(object sender, RoutedEventArgs e)
        {
            if (isInvoiceReady())
            {
                handlePayment("GIFT");
            }
        }

        private void Button_Click_Sell_Gift(object sender, RoutedEventArgs e)
        {
            if (isInvoiceReady() && TextBoxNumbers.Text != null && !TextBoxNumbers.Text.Equals(""))
            {
                ListBoxOrder.Items.Add(mainClass.processAddLineItemGiftCard(TextBoxNumbers.Text));
                TextBoxTotal.Text = mainClass.getCurrentInvoiceTotals();
                clearTextBoxNumbers();
            }
        }

        private void handlePayment(string paymentType)
        {
            string amount = "";
            if (TextBoxNumbers.Text == "")
            {
                amount = mainClass.getBalance();
            }
            else if (paymentType.Equals("CREDIT") && Convert.ToDecimal(TextBoxNumbers.Text) > Convert.ToDecimal(mainClass.getBalance()))
            {
                amount = mainClass.getBalance();
            }
            else if (paymentType.Equals("GIFT") && Convert.ToDecimal(TextBoxNumbers.Text) > Convert.ToDecimal(mainClass.getBalance()))
            {
                amount = mainClass.getBalance();
            }
            else
            {
                amount = TextBoxNumbers.Text;
            }
            clearTextBoxNumbers();
            mainClass.processMakePayment(paymentType, amount);
            TextBoxTotal.Text = mainClass.getCurrentInvoiceTotals();
            if (isInvoicePaid())
            {
                mainClass.printReceipt();
                clearTextBoxNumbers();
            }
            if (isInvoicePaid())
            {
                ListBoxOrder.Items.Add("\n*** ORDER CLOSED ***");
            }
        }

        private bool isInvoicePaid()
        {
            return mainClass.currentInvoiceIsPaid();
        }

        private bool isInvoiceReady()
        {
            return !mainClass.currentInvoiceIsPaid() && mainClass.getCurrentInvoice() != null;
        }

        public void clearTextBoxNumbers()
        {
            TextBoxNumbers.Text = "";
        }
        private void validate(object sender, TextChangedEventArgs e)
        {
            try
            {
                Convert.ToDecimal(TextBoxNumbers.Text);

            }
            catch (FormatException exception)
            {
                if (TextBoxNumbers.Text != "")
                {
                    if (TextBoxNumbers.Text == ".")
                    {
                        TextBoxNumbers.Text = "0.";
                    }
                    else
                    {
                        Console.Beep();
                        TextBoxNumbers.Text = TextBoxNumbers.Text.Substring(0, TextBoxNumbers.Text.Length - 1);
                    }
                }
            }
        }

        private void Button_Click_OpenReportsPage(object sender, RoutedEventArgs e)
        {
            openDialog(new NailSalonReportPage(mainClass));
        }

        private void Button_Click_LogOnCashDrawer(object sender, RoutedEventArgs e)
        {
            LogOnCashDrawer logOnDrawerDialog = new LogOnCashDrawer(mainClass);
            openDialog(logOnDrawerDialog);
            string PIN = logOnDrawerDialog.PasswordBoxPIN.Password;
            currentEmployee = (IEmployee)logOnDrawerDialog.EmployeeComboBox.SelectedItem;
            if (PIN != null && currentEmployee != null)
            {
                if (PIN.Equals(currentEmployee.getPIN()))
                {
                    mainClass.logOnCashDrawer(currentEmployee.getIdAsString(), "100");
                    TextBlockEmployeeName.Text = "\t" + currentEmployee.ToString();
                }
                else
                    System.Media.SystemSounds.Asterisk.Play();
            }
        }
        private void Button_Click_AddItem(object sender, RoutedEventArgs e)
        {
            openDialog(new AddItemDialog(mainClass));
            showMainButtonPanel();
        }
        public void openDialog(Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var darkWindow = new Window
            {
                Background = Brushes.Black,
                Opacity = 0.4,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                WindowState = WindowState.Maximized,
                Topmost = true
            };
            this.IsEnabled = false;
            darkWindow.Show();
            darkWindow.Topmost = false;
            window.ShowDialog();
            darkWindow.Close();
            this.IsEnabled = true;
        }
        private void Button_Click_CloseCashDrawer(object sender, RoutedEventArgs e)
        {
            if (currentEmployee != null)
            {
                currentEmployee = null;
                TextBlockEmployeeName.Text = "";
                mainClass.printDrawerCashReport();
            }
        }

        private void clearItemPanel()
        {
            ServiceButtonGrid.Children.Clear();
            serviceButtonList = new List<Button>();
            ServiceButtonGrid.ColumnDefinitions.Clear();
            ServiceButtonGrid.RowDefinitions.Clear();
            //for (int i=0; i<ServiceButtonGrid.Children.Count; i++)
            //{
            //    ServiceButtonGrid.Children.RemoveRange(0, ServiceButtonGrid.Children.Count);
            //}
        }

        private void Button_Click_Return(object sender, RoutedEventArgs e)
        {
            openDialog(new ReturnDialog(mainClass));
        }
    }
}