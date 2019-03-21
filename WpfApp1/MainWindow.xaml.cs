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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            actionComboBox.ItemsSource = controller.ActionName;
            actionComboBox.SelectedIndex = controller.CurrentAction;

            depositsGrid.ItemsSource = controller.Deposits;

            // I could do control bindings for the label and combobox
            // here as well, however I've prefered using events, they
            // don't add too much code and demonstrate another way of
            // communication between app components.
            //
            controller.OnMaturityUpdateEvent += (total) =>
            {
                totalMaturity.Content = total.ToString();
            };

            controller.OnActionUpdateEvent += (selected) =>
            {
                if (actionComboBox.SelectedIndex != selected)
                {
                    actionComboBox.SelectedIndex = selected;
                }
            };

            controller.PrePopulateDeposits();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            controller.CurrentAction = actionComboBox.SelectedIndex;
        }

        private Controller controller = new Controller();
    }
}
