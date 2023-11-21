#Region "#viewmodel"
Imports System
Imports System.Collections.ObjectModel
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Mvvm

Namespace DXScheduler_PopUpMenuCustomization

    <POCOViewModel>
    Public Class MainViewModel

        Private ReadOnly data As SportChannelsData = New SportChannelsData(2)

        Protected Sub New()
            WorkTimeStart = TimeSpan.FromHours(6)
            WorkTimeEnd = TimeSpan.FromHours(21)
        End Sub

        Public Overridable Property WorkTimeStart As TimeSpan

        Public Overridable Property WorkTimeEnd As TimeSpan

        Public Overridable Property WorkTime As TimeSpanRange

        Public Overridable ReadOnly Property SportEvents As ObservableCollection(Of SportEvent)
            Get
                Return data.Events
            End Get
        End Property

        Public Overridable ReadOnly Property SportChannels As ObservableCollection(Of SportChannel)
            Get
                Return data.Channels
            End Get
        End Property

        Public Overridable ReadOnly Property SportGroups As ObservableCollection(Of SportGroup)
            Get
                Return data.Groups
            End Get
        End Property

        Public Overridable Property CancelledEvents As ObservableCollection(Of SportEvent)

        Protected Sub OnWorkTimeStartChanged()
            Dim [end] = If(WorkTimeEnd < WorkTimeStart, WorkTimeStart.Add(TimeSpan.FromHours(1)), WorkTimeEnd)
            WorkTime = New TimeSpanRange(WorkTimeStart, [end])
        End Sub

        Protected Sub OnWorkTimeEndChanged()
            Dim start = If(WorkTimeEnd < WorkTimeStart, WorkTimeEnd.Add(TimeSpan.FromHours(-1)), WorkTimeStart)
            WorkTime = New TimeSpanRange(start, WorkTimeEnd)
        End Sub
    End Class
End Namespace
#End Region  ' #viewmodel
