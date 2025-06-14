using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CollaborationTools.meeting_schedule
{
    public class AvailableTimeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Schedule> _allPersonalSchedules;
        private ObservableCollection<DateTime> _dates;
        private int _meetingId;
        public ObservableCollection<string> Hours { get; set; }
        public ObservableCollection<string> HourStr { get; set; }
        

        public AvailableTimeViewModel(int teamId)
        {
            Hours = new ObservableCollection<string>(
                Enumerable.Range(0, 24).Select(h => $"{h:D2}:00")
            );
            HourStr = new ObservableCollection<string>(
                Enumerable.Range(0, 24).Select(h => $"{h:D2}:00 ~")
            );
            
            var meetingService = new MeetingService();
            (AllPersonalSchedules, MeetingId) = meetingService.GetAllPersonalSchedule(teamId);
            
            var dateItems = meetingService.GetMeetingDates(MeetingId);
            Dates = new ObservableCollection<DateTime>(dateItems.Select(d => d.Date));
        }


        public ObservableCollection<Schedule> AllPersonalSchedules
        {
            get => _allPersonalSchedules;
            set
            {
                _allPersonalSchedules = value;
                OnPropertyChanged();
            }
        }
        
        public ObservableCollection<DateTime> Dates
        {
            get => _dates;
            set
            {
                _dates = value;
                OnPropertyChanged();
            }
        }

        public int MeetingId
        {
            get => _meetingId;
            set => _meetingId = value;
        }
        

        // 속성 변경 알림을 위한 메서드
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    
}