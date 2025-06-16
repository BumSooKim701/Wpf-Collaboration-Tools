using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CollaborationTools.timeline
{
    public enum TimelineItemType
    {
        Schedule,
        File,
        Memo
    }

    public class TimelineItem : INotifyPropertyChanged
    {
        private DateTime dateTime;
        private string title;
        private string description;
        private TimelineItemType itemType;
        private string teamId;
        private object originalItem;
        private string createdBy;
        
        private double xPosition;
        private double yPosition;

        public double XPosition
        {
            get => xPosition;
            set
            {
                if (Math.Abs(xPosition - value) > 0.01)
                {
                    xPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        public double YPosition
        {
            get => yPosition;
            set
            {
                if (Math.Abs(yPosition - value) > 0.01)
                {
                    yPosition = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public DateTime DateTime
        {
            get => dateTime;
            set
            {
                if (dateTime != value)
                {
                    dateTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title
        {
            get => title;
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimelineItemType ItemType
        {
            get => itemType;
            set
            {
                if (itemType != value)
                {
                    itemType = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TeamId
        {
            get => teamId;
            set
            {
                if (teamId != value)
                {
                    teamId = value;
                    OnPropertyChanged();
                }
            }
        }

        public object OriginalItem
        {
            get => originalItem;
            set
            {
                if (originalItem != value)
                {
                    originalItem = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public string CreatedBy
        {
            get => createdBy;
            set { createdBy = value; OnPropertyChanged(); }
        }

        // UI 표시용 속성들
        public string ItemTypeText => ItemType switch
        {
            TimelineItemType.Schedule => "일정",
            TimelineItemType.File => "파일",
            TimelineItemType.Memo => "메모",
            _ => "알 수 없음"
        };

        public string ItemTypeColor => ItemType switch
        {
            TimelineItemType.Schedule => "#4CAF50",
            TimelineItemType.File => "#2196F3", 
            TimelineItemType.Memo => "#FF9800",
            _ => "#9E9E9E"
        };
        
        public string FormattedDateTime => DateTime.ToString("MM/dd HH:mm");

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
