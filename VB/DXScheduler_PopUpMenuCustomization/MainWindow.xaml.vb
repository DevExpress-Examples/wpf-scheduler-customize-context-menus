Imports DevExpress.Xpf.Bars
Imports System.Windows
Imports DevExpress.Xpf.Scheduling

Namespace DXScheduler_PopUpMenuCustomization
    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    Partial Public Class MainWindow
        Inherits Window

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub allDayCheck_CheckedChanged(ByVal sender As Object, ByVal e As ItemClickEventArgs)
            Dim selectedItem As AppointmentItem = scheduler.SelectedAppointments(0)
            selectedItem.AllDay = If(allDayCheck.IsChecked = True, True, False)
        End Sub
    End Class
End Namespace
