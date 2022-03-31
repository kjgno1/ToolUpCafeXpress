using System.ComponentModel;

namespace ToolUpCafeXpress
{
    public class Notification : INotifyPropertyChanged
    {
        protected string action;
        protected string resize;

        public string ActionNotifi
        {
            get { return action; }
            set
            {
                if (action != value)
                {
                    action = value;
                    OnPropertyChanged("ActionNotifi");

                }
            }
        }

        public string ActionResize
        {
            get { return resize; }
            set
            {
                if (resize != value)
                {
                    resize = value;
                    OnPropertyChanged("ActionResize");

                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
            }

        }



    }
}