namespace Launcher.Common
{
    using System.ComponentModel;

    public class Progress : INotifyPropertyChanged
    {
        #region Properties

        protected int m_Maximum = 100;
        public int Maximum
        {
            get { return m_Maximum; }
            set
            {
                if (m_Maximum.Equals(value))
                {
                    return;
                }

                m_Maximum = value;
                OnPropertyChanged("Maximum");
            }
        }

        protected int m_Value = 0;
        public int Value
        {
            get { return m_Value; }
            set
            {
                if (m_Value.Equals(value))
                {
                    return;
                }

                m_Value = value;
                OnPropertyChanged("Value");
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
