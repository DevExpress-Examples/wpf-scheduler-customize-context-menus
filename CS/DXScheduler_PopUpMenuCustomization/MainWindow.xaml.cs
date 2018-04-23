using DevExpress.Xpf.Bars;
using System.Windows;
using DevExpress.Xpf.Scheduling;

namespace DXScheduler_PopUpMenuCustomization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void allDayCheck_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            AppointmentItem selectedItem = scheduler.SelectedAppointments[0];
            selectedItem.AllDay = (allDayCheck.IsChecked == true) ? true : false;
        }
    }
}
