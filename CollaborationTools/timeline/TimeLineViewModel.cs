using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CollaborationTools.team;

namespace CollaborationTools.timeline
{
    public class TimeLineViewModel : INotifyPropertyChanged
    {
        private Team currentTeam;
        private DateTime startDate = DateTime.Today.AddDays(-30);
        private DateTime endDate = DateTime.Today.AddDays(30);
        private ObservableCollection<TimelineItem> timelineItems;

        public TimeLineViewModel()
        {
            timelineItems = new ObservableCollection<TimelineItem>();
        }

        public Team CurrentTeam
        {
            get => currentTeam;
            set
            {
                if (currentTeam != value)
                {
                    currentTeam = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                if (startDate != value)
                {
                    startDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime EndDate
        {
            get => endDate;
            set
            {
                if (endDate != value)
                {
                    endDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<TimelineItem> TimelineItems
        {
            get => timelineItems;
            set
            {
                if (timelineItems != value)
                {
                    timelineItems = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
