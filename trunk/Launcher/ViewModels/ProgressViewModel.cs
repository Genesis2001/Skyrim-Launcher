namespace Launcher.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    public class ProgressViewModel : INotifyPropertyChanged
    {
        #region Fields
        #endregion

        #region Properties

        private long m_maxValue = 100;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.Int64" /> value representing the maximum value of the progress bar.</para>
        /// </summary>
        public long Maximum
        {
            get { return m_maxValue; }
            set
            {
                if (m_maxValue == value)
                    return;

                m_maxValue = value;
                OnPropertyChanged("Maximum");
            }
        }

        /// <summary>
        ///     <para>Gets or sets a <see cref="System.Boolean" /> value representing whether the progress bar is shown.</para>
        /// </summary>
        public bool IsShown { get; set; }

        private string m_statusLabel;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.String" /> value representing the status text on the progressbar.</para>
        /// </summary>
        public string Status
        {
            get { return m_statusLabel; }
            set
            {
                if (m_statusLabel == value)
                    return;

                m_statusLabel = value;
                OnPropertyChanged("Status");
            }
        }

        private long m_value = 0;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.Int64" /> value representing the value of the progress bar.</para>
        /// </summary>
        public long Value
        {
            get { return m_value; }
            set
            {
                if (m_value == value)
                    return;

                m_value = value;
                OnPropertyChanged("Value");
            }
        }

        #endregion

        #region Methods
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
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
