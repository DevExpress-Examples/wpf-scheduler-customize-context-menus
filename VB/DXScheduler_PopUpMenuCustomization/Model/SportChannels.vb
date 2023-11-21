Imports DevExpress.Mvvm
Imports DevExpress.XtraScheduler
Imports System
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Linq
Imports System.Windows.Media

Namespace DXScheduler_PopUpMenuCustomization

    Public Class SportEvent

        Public Property Id As Integer

        Public Property StartTime As Date

        Public Property EndTime As Date

        Public Property Caption As String

        Public Property ChannelId As Integer

        Public Property SportId As Integer

        Public Property Type As Integer?

        Public Property AllDay As Boolean

        Public Property Location As String

        Public Property Description As String

        Public Property RecurrenceInfo As String

        Public Property ReminderInfo As String

        Public Property CustomText As String
            Get
                Return String.Format("[{0}] - {1}", Location, Caption)
            End Get

            Set(ByVal value As String)
            End Set
        End Property
    End Class

    Public Class SportChannel

        Public Property Id As Integer

        Public Property Caption As String

        Public ReadOnly Property HasNews As Boolean
            Get
                Return Not NewsTimeRange.IsZero
            End Get
        End Property

        Public Property IsVisible As Boolean

        Public Property NewsTimeRange As TimeSpanRange
    End Class

    Public Class SportGroup

        Public Property Id As Integer

        Public Property Caption As String

        Public Property Color As Color

        Public Property Image As ImageSource
    End Class

    Public Class SportChannelsData

        Private _Groups As ObservableCollection(Of DXScheduler_PopUpMenuCustomization.SportGroup), _Channels As ObservableCollection(Of DXScheduler_PopUpMenuCustomization.SportChannel), _Events As ObservableCollection(Of DXScheduler_PopUpMenuCustomization.SportEvent), _CancelledEvents As ObservableCollection(Of DXScheduler_PopUpMenuCustomization.SportEvent)

        Public Shared ReadOnly SportGroups As SportGroup() = {New SportGroup() With {.Id = 0, .Caption = "SportNews", .Color = Color.FromRgb(255, 255, 255)}, New SportGroup() With {.Id = 1, .Caption = "Basketball", .Color = Color.FromRgb(&HFF, &HC2, &HBE)}, New SportGroup() With {.Id = 2, .Caption = "Boxing", .Color = Color.FromRgb(&HA8, &HD5, &HFF)}, New SportGroup() With {.Id = 3, .Caption = "Tennis", .Color = Color.FromRgb(&HC1, &HF4, &H9C)}, New SportGroup() With {.Id = 4, .Caption = "Soccer", .Color = Color.FromRgb(&HF3, &HE4, &HC7)}, New SportGroup() With {.Id = 5, .Caption = "Artistic Gymnastics", .Color = Color.FromRgb(&HF4, &HCE, &H93)}, New SportGroup() With {.Id = 6, .Caption = "Canoe", .Color = Color.FromRgb(&HC7, &HF4, &HFF)}, New SportGroup() With {.Id = 7, .Caption = "Kayak", .Color = Color.FromRgb(&HCF, &HDB, &H98)}, New SportGroup() With {.Id = 8, .Caption = "Sailing", .Color = Color.FromRgb(&HE0, &HCF, &HE9)}, New SportGroup() With {.Id = 9, .Caption = "Swimming", .Color = Color.FromRgb(&H8D, &HE9, &HDF)}, New SportGroup() With {.Id = 10, .Caption = "Shooting", .Color = Color.FromRgb(&HFF, &HF7, &HA5)}}

        Private Shared Function GenerateEvents(ByVal dayCount As Integer) As ObservableCollection(Of SportEvent)
            Dim result As ObservableCollection(Of SportEvent) = New ObservableCollection(Of SportEvent)()
            Dim aptId As Integer = 0
            For Each ch As SportChannel In sportChannels
                Dim start As Date = Date.Today - TimeSpan.FromDays(dayCount)
                Dim [end] As Date = Date.Today + TimeSpan.FromDays(dayCount)
                If ch.HasNews Then result.Add(GetRandomRecurrenceSportEvent(Math.Min(Threading.Interlocked.Increment(aptId), aptId - 1), start, ch, dayCount * 2 - 1))
                Dim i As Date = start
                While i < [end]
                    If i.DayOfWeek = DayOfWeek.Saturday OrElse i.DayOfWeek = DayOfWeek.Sunday Then
                        result.Add(GetRandomSportEvent(Math.Min(Threading.Interlocked.Increment(aptId), aptId - 1), i, i + TimeSpan.FromDays(1), ch.Id))
                        Continue While
                    End If

                    Dim airtimeStart As Date = New DateTime(i.Year, i.Month, i.Day, 6, 0, 0)
                    Dim airtimeEnd As Date = New DateTime(i.Year, i.Month, i.Day, 21, 0, 0)
                    Dim aptStartTime As Date = airtimeStart
                    While aptStartTime < airtimeEnd
                        Dim aptEndTime As Date
                        Dim duration As TimeSpan = TimeSpan.FromMinutes(rnd.Next(20, 180))
                        Dim newsTimeRange As TimeSpanRange = ch.NewsTimeRange
                        If ch.HasNews Then
                            Dim timeBeforeNews As Long =(newsTimeRange.Start - aptStartTime.TimeOfDay).Ticks
                            If timeBeforeNews > 0 AndAlso (timeBeforeNews < TimeSpan.FromMinutes(30).Ticks OrElse timeBeforeNews <= duration.Ticks) Then
                                aptEndTime = aptStartTime.Date + newsTimeRange.Start
                                result.Add(GetRandomSportEvent(Math.Min(Threading.Interlocked.Increment(aptId), aptId - 1), aptStartTime, aptEndTime, ch.Id))
                                aptStartTime = aptEndTime.Date + newsTimeRange.End
                                Continue While
                            End If
                        End If

                        aptEndTime = aptStartTime + duration
                        result.Add(GetRandomSportEvent(Math.Min(Threading.Interlocked.Increment(aptId), aptId - 1), aptStartTime, aptEndTime, ch.Id))
                        aptStartTime = aptEndTime
                    End While

                    i += TimeSpan.FromDays(1)
                End While
            Next

            Return result
        End Function

        Private Shared recurrenceInfoFormat As String = "<RecurrenceInfo Start=""{0}"" End=""{1}"" WeekDays=""{2}"" OccurrenceCount=""{3}"" Range=""{4}"" Type=""{5}"" Id=""{6}""/>"

        Private Shared Function GetRandomRecurrenceSportEvent(ByVal id As Integer, ByVal startDate As Date, ByVal channel As SportChannel, ByVal dayCount As Integer) As SportEvent
            Dim pattern As SportEvent = New SportEvent()
            pattern.Id = id
            Dim newsTimeRange As TimeSpanRange = channel.NewsTimeRange
            pattern.StartTime = startDate + newsTimeRange.Start
            pattern.EndTime = startDate + newsTimeRange.End
            pattern.SportId = 0
            pattern.ChannelId = channel.Id
            pattern.Caption = "Sport News"
            pattern.Location = "New York City, USA"
            pattern.Type = CInt(AppointmentType.Pattern)
            Dim recInfo As RecurrenceInfo = New RecurrenceInfo()
            recInfo.Start = pattern.StartTime
            recInfo.End = pattern.EndTime.AddDays(dayCount)
            recInfo.WeekDays = WeekDays.WorkDays
            recInfo.Range = RecurrenceRange.EndByDate
            recInfo.Type = RecurrenceType.Daily
            pattern.RecurrenceInfo = String.Format(CultureInfo.InvariantCulture, recurrenceInfoFormat, recInfo.Start, recInfo.End, CInt(recInfo.WeekDays), recInfo.OccurrenceCount, CInt(recInfo.Range), CInt(recInfo.Type), recInfo.Id.ToString())
            Return pattern
        End Function

        Private Shared Function GetRandomSportEvent(ByVal id As Integer, ByVal start As Date, ByVal [end] As Date, ByVal channelId As Integer) As SportEvent
            Dim res = New SportEvent()
            res.Id = id
            res.StartTime = start
            res.EndTime = [end]
            res.SportId = rnd.Next(1, 10)
            res.ChannelId = channelId
            res.Caption = GetRandomString(GetEvents(res.SportId))
            res.Location = GetRandomString(GetLocations(res.SportId))
            res.Description = GetRandomString(GetDescriptions(res.SportId))
            res.Type = 0
            Return res
        End Function

        Private Shared Function GetRandomString(ByVal events As String()) As String
            Return events(rnd.Next(0, events.Count()))
        End Function

        Private Shared Function GetEvents(ByVal sportId As Integer) As String()
            Select Case sportId
                Case 1
                    Return basketballEvents
                Case 2
                    Return boxingEvents
                Case 3
                    Return tennisEvents
                Case 4
                    Return soccerEvents
                Case 5
                    Return artisticGymnasticsEvents
                Case 6
                    Return canoeEvents
                Case 7
                    Return kayakEvents
                Case 8
                    Return sailingEvents
                Case 9
                    Return swimmingEvents
                Case 10
                    Return shootingEvents
            End Select

            Return Nothing
        End Function

        Private Shared Function GetLocations(ByVal sportId As Integer) As String()
            Select Case sportId
                Case 1
                    Return basketballLocations
                Case 2
                    Return boxingLocations
                Case 3
                    Return tennisLocations
                Case 4
                    Return soccerLocations
                Case 5
                    Return artisticGymnasticsLocations
                Case 6
                    Return canoeLocations
                Case 7
                    Return kayakLocations
                Case 8
                    Return sailingLocations
                Case 9
                    Return swimmingLocations
                Case 10
                    Return shootingLocations
            End Select

            Return Nothing
        End Function

        Private Shared Function GetDescriptions(ByVal sportId As Integer) As String()
            Select Case sportId
                Case 1
                    Return basketballDescriptions
                Case 2
                    Return boxingDescriptions
                Case 3
                    Return tennisDescriptions
                Case 4
                    Return soccerDescriptions
                Case 5
                    Return artisticGymnasticsDescriptions
                Case 6
                    Return canoeDescriptions
                Case 7
                    Return kayakDescriptions
                Case 8
                    Return sailingDescriptions
                Case 9
                    Return swimmingDescriptions
                Case 10
                    Return shootingDescriptions
            End Select

            Return Nothing
        End Function

        Private Shared rnd As Random = New Random()

        Private Shared Function GetRandomNewsTimeRange(ByVal start As TimeSpan) As TimeSpanRange
            Return New TimeSpanRange(start, start + TimeSpan.FromMinutes(rnd.Next(30, 60)))
        End Function

#Region "Sample Data"
        Private Shared ReadOnly sportChannels As SportChannel() = {New SportChannel() With {.Id = 0, .Caption = "SPORT TV 1", .NewsTimeRange = GetRandomNewsTimeRange(TimeSpan.FromHours(11))}, New SportChannel() With {.Id = 1, .Caption = "SPORT TV 2", .NewsTimeRange = GetRandomNewsTimeRange(TimeSpan.FromHours(19))}, New SportChannel() With {.Id = 2, .Caption = "SPORT TV 3"}, New SportChannel() With {.Id = 3, .Caption = "SPORT TV 4"}, New SportChannel() With {.Id = 4, .Caption = "TV 5"}, New SportChannel() With {.Id = 5, .Caption = "TV 6"}, New SportChannel() With {.Id = 6, .Caption = "TV 7"}, New SportChannel() With {.Id = 7, .Caption = "TV 8"}}

        Private Shared ReadOnly basketballEvents As String() = {"Basketball First Group Phase - Men", "Basketball First Group Phase - Women"}

        Private Shared ReadOnly boxingEvents As String() = {"Boxing - ((Showtime) Jeff Lacy (20-0) vs. Joe Calzaghe (39-0) (IBF, IBO and WBO super middleweight belts)", "Boxing - ((WBC and WBO lightweight belts) (PPV) Carlos Hernandez vs. Bobby Pacquiao", "Boxing - (Danilo Haussler (25-3) vs. TBA", "Boxing - (PPV) Antonio Tarver (23-3) vs. Roy Jones (49-3) (IBO light heavyweight belt)"}

        Private Shared ReadOnly tennisEvents As String() = {"Tennis - BNP Paribas Masters", "Tennis - Tennis Masters Hamburg", "Tennis - Tennis Masters Monte Carlo", "Tennis - Australian Open"}

        Private Shared ReadOnly soccerEvents As String() = {"Soccer Quarter final - *Women's Quarterfinal - Women", "Soccer 1st Round - *Men's Preliminaries - Men", "Soccer places 3/4 - *Women's Bronze Medal Match - Women"}

        Private Shared ReadOnly artisticGymnasticsEvents As String() = {"Artistic Gymnastics - Vault - Women - Final", "Artistic Gymnastics - Floor Exercise - Men - Final"}

        Private Shared ReadOnly canoeEvents As String() = {"Canoe - Slalom C2 - Men - Heats", "Canoe - Slalom C1 - Men - Heats"}

        Private Shared ReadOnly kayakEvents As String() = {"Kayak - Slalom K2 - Men - Heats", "Kayak - Slalom K1 - Women - Semi-finals"}

        Private Shared ReadOnly sailingEvents As String() = {"Sailing - Finn - Race 1", "Sailing - Women's Mistral - Race 01"}

        Private Shared ReadOnly swimmingEvents As String() = {"Swimming - Men's 100m Breaststroke - Heat 2", "Swimming - Women's 400m Individual Medley - Heat 2", "Swimming - Women's 100m Butterfly - Heat 1"}

        Private Shared ReadOnly shootingEvents As String() = {"Shooting - Men's 50m Pistol Qualification", "Shooting - Women's 25m Pistol Final", "Shooting - Men's 10m Air Pistol Qualification"}

        Private Shared ReadOnly basketballLocations As String() = {"Brooklyn, NY, USA", "Charlottesville, VA, USA"}

        Private Shared ReadOnly basketballDescriptions As String() = {"Baseline Leaners vs Rimshots", "Lady Mustangs vs Houston Rockettes"}

        Private Shared ReadOnly boxingLocations As String() = {"Hamburg, Germany", "Chattanooga, Tennessee, USA", "Salt Lake City, Utah, USA", "Albuquerque, New Mexico, USA"}

        Private Shared ReadOnly boxingDescriptions As String() = {"IBF, IBO and WBO super middleweight belts", "WBC and WBO lightweight belts", "WBC super middleweight belt", "IBO light heavyweight belt"}

        Private Shared ReadOnly tennisLocations As String() = {"Paris, France", "Hamburg, Germany", "Monte Carlo, Monaco", "Melbourne, Victoria, Australia"}

        Private Shared ReadOnly tennisDescriptions As String() = {"One Hit Wonders vs Big Hitters", "Love Hurts vs Queens of the Court", "Alley Gators vs String Nation", "Tennis the Menace vs Heavy Duty Felt"}

        Private Shared ReadOnly soccerLocations As String() = {"Philadelphia, PA, USA", "San Francisco, CA, USA", "New York City, USA"}

        Private Shared ReadOnly soccerDescriptions As String() = {"Cheetahs vs Lady Eagles", "Amigos vs Terminators", "Spitting Cobras vs Red Dragons"}

        Private Shared ReadOnly artisticGymnasticsLocations As String() = {"Hong Kong Open Championships", "World Artistic Gymnastics Championships Glasgo, UK"}

        Private Shared ReadOnly artisticGymnasticsDescriptions As String() = {"Complicated vaults in different body positions, such as tucked, piked or stretched.", "Express the personalities through their music choice and choreography."}

        Private Shared ReadOnly canoeLocations As String() = {"Račice, Czech republic", "Milan, Italy"}

        Private Shared ReadOnly canoeDescriptions As String() = {"Row hard or row home.", "Recovery is not an option."}

        Private Shared ReadOnly kayakLocations As String() = {"Puerto Deportivo el Puntal, Spain", "Danson Lake, Bexleyheath, UK"}

        Private Shared ReadOnly kayakDescriptions As String() = {"After the Gold Rush vs Humpbacks", "Drag-On vs Shark Week"}

        Private Shared ReadOnly sailingLocations As String() = {"Marseille, France", "Savannah, Georgia, United States"}

        Private Shared ReadOnly sailingDescriptions As String() = {"Grab life by the tail, set sail", "Do or do not.  There is no try."}

        Private Shared ReadOnly swimmingLocations As String() = {"Budapest, Hungary", "Kazan, Russia", "Barcelona, Spain"}

        Private Shared ReadOnly swimmingDescriptions As String() = {"Dashing Dolphins and Krazy Krakens", "Golden Gators and Stingrays", "Wave Makers and Murray Marlins"}

        Private Shared ReadOnly shootingLocations As String() = {"Munich, Germany", "Zagreb, Croatia", "Lahti, Finland"}

        Private Shared ReadOnly shootingDescriptions As String() = {"Shoot over a distance of 50 meters / 54.68 yards in standing position, using a 5.6 millimeters / 0.22 inches caliber pistol with no weight restriction.", "Shoot over a distance of 25 meters / 27.34 yards in standing position, using a 5.6 millimeters / 0.22 inches caliber pistol with a maximum weight of 1.4 kilograms / 3.09 libbers.", "Shoot over a distance of 10 meters / 10.94 yards in standing position, using a 4.5 millimeters / 0.177 inches caliber air pistol with a maximum weight of 1.5 kilograms / 3,31 libbers."}

#End Region
        Public Sub New(ByVal dayCount As Integer)
            Groups = New ObservableCollection(Of SportGroup)(SportGroups)
            Channels = New ObservableCollection(Of SportChannel)(sportChannels)
            Events = GenerateEvents(dayCount)
        End Sub

        Public Property Groups As ObservableCollection(Of SportGroup)
            Get
                Return _Groups
            End Get

            Private Set(ByVal value As ObservableCollection(Of SportGroup))
                _Groups = value
            End Set
        End Property

        Public Property Channels As ObservableCollection(Of SportChannel)
            Get
                Return _Channels
            End Get

            Private Set(ByVal value As ObservableCollection(Of SportChannel))
                _Channels = value
            End Set
        End Property

        Public Property Events As ObservableCollection(Of SportEvent)
            Get
                Return _Events
            End Get

            Private Set(ByVal value As ObservableCollection(Of SportEvent))
                _Events = value
            End Set
        End Property

        Public Property CancelledEvents As ObservableCollection(Of SportEvent)
            Get
                Return _CancelledEvents
            End Get

            Private Set(ByVal value As ObservableCollection(Of SportEvent))
                _CancelledEvents = value
            End Set
        End Property
    End Class
End Namespace
