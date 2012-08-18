namespace Launcher.Models
{
    using Launcher.Linq;
    using Launcher.ViewModels;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;

    public class LogModel : IDisposable
    {
        #region Constructor(s)

        public LogModel(MasterViewModel viewModel)
        {
            m_ViewModel = viewModel;
        }

        #endregion

        #region Fields

        protected string m_LogFile;
        protected Stream m_LogStream;
        protected StreamWriter m_LogWriter;

        protected MasterViewModel m_ViewModel;

        #endregion

        #region Properties

        protected string m_Directory = String.Empty;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.String" /> value representing the log directory.</para>
        /// </summary>
        public string Directory
        {
            get { return m_Directory; }
            set
            {
                if (m_Directory.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                m_Directory = value;
                OnPropertyChanged("Directory");
            }
        }

        protected bool m_IsError = false;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.Boolean" /> value representing whether an error has been recorded.</para>
        /// </summary>
        public bool IsError
        {
            get { return m_IsError; }
            set
            {
                if (m_IsError.Equals(value))
                {
                    return;
                }

                m_IsError = value;
                OnPropertyChanged("IsError");
            }
        }

        protected string m_Message = String.Empty;
        public string Message
        {
            get { return m_Message; }
            set
            {
                if (m_Message.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                m_Message = value;
                OnPropertyChanged("Message");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     <para></para>
        /// </summary>
        public void Initialize()
        {
            if (!System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }

            m_LogFile = Path.Combine(Directory, String.Format(@"launcher.{0}.log", DateTime.Now.ToString("dd-MM-yyyy")));
            try
            {
                m_LogStream = File.Open(m_LogFile, FileMode.Append, FileAccess.Write, FileShare.Read);
                m_LogWriter = new StreamWriter(m_LogStream, Encoding.UTF8);
            }
            catch (System.Security.SecurityException)
            {
                IsError = true;
                Message = "Cannot open log file.";
            }

            Write("Application starting up.");
        }

        public void Write(string format, params object[] args)
        {
            if (m_IsDisposed)
            {
                throw new ObjectDisposedException("The stream has been disposed of and cannot be written to.");
            }

            if (m_LogWriter != null)
            {
                StringBuilder message = new StringBuilder();
                message.Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss - "));
                message.AppendFormat(format, args);

                m_LogWriter.WriteLine(message.ToString());
                m_LogWriter.Flush();
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

        #region IDisposable Members

        protected bool m_IsDisposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_LogStream != null && m_LogStream.CanWrite)
                {
                    m_LogWriter.Close();
                }

                m_IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Write("Application shutting down.");

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
