using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using System;
using System.Collections.ObjectModel;

namespace DXScheduler_PopUpMenuCustomization {
    [POCOViewModel]
    public class MainViewModel {
        readonly SportChannelsData data = new SportChannelsData(2);
        protected MainViewModel() {
            WorkTimeStart = TimeSpan.FromHours(6);
            WorkTimeEnd = TimeSpan.FromHours(21);
        }
        public virtual TimeSpan WorkTimeStart { get; set; }
        public virtual TimeSpan WorkTimeEnd { get; set; }
        public virtual TimeSpanRange WorkTime { get; set; }
        public virtual ObservableCollection<SportEvent> SportEvents { get { return data.Events; } }
        public virtual ObservableCollection<SportChannel> SportChannels { get { return data.Channels; } }
        public virtual ObservableCollection<SportGroup> SportGroups { get { return data.Groups; } }
        public virtual ObservableCollection<SportEvent> CancelledEvents { get; set; }

        protected void OnWorkTimeStartChanged() {
            var end = (WorkTimeEnd < WorkTimeStart) ? WorkTimeStart.Add(TimeSpan.FromHours(1)) : WorkTimeEnd;
            WorkTime = new TimeSpanRange(WorkTimeStart, end);
        }
        protected void OnWorkTimeEndChanged() {
            var start = (WorkTimeEnd < WorkTimeStart) ? WorkTimeEnd.Add(TimeSpan.FromHours(-1)) : WorkTimeStart;
            WorkTime = new TimeSpanRange(start, WorkTimeEnd);
        }
    }
}
