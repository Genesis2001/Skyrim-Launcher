namespace Launcher.ViewModels
{
    using Launcher.Commands;
    using Launcher.Linq;
    using Launcher.Models;

    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Xml.Linq;

    public class MasterViewModel : INotifyPropertyChanged
    {
        #region Constructor(s)

        /// <summary>
        ///     <para>Instantiates the MainViewModel class</para>
        /// </summary>
        /// <param name="app"></param>
        public MasterViewModel(App app)
        {
            m_App = app;
            m_TrayIcon = new NotifyIcon();
            m_TrayIcon.Icon = Properties.Resources.skyrimicon;


            ExitCommand = new RelayCommand((o) => Shutdown(), (o) => true);
        }

        #endregion

        #region Fields

        protected App m_App;
        protected XDocument m_Config;
        protected string m_ConfigPath;
        protected NotifyIcon m_TrayIcon;

        #endregion

        #region Commands

        /// <summary>
        ///     <para>Gets a representation of the command to exit the launcher (and Skyrim if necessary)</para>
        /// </summary>
        public ICommand ExitCommand { get; protected set; }

        #endregion

        #region Properties

        protected bool m_AutoSave = false;
        public bool AutoSave
        {
            get { return m_AutoSave; }
            set
            {
                if (m_AutoSave == value)
                    return;

                m_AutoSave = value;
                OnPropertyChanged("AutoSave");
            }
        }

        protected bool m_KeepOpen = true;
        public bool KeepOpen
        {
            get { return m_KeepOpen; }
            set
            {
                if (m_KeepOpen.Equals(value))
                {
                    return;
                }

                m_KeepOpen = value;
                OnPropertyChanged("KeepOpen");
            }
        }

        protected CharacterModel m_CharacterModel;
        public CharacterModel Character
        {
            get { return m_CharacterModel; }
            protected set { m_CharacterModel = value; }
        }

        protected LogModel m_LogModel;
        public LogModel Log
        {
            get { return m_LogModel; }
            protected set { m_LogModel = value; }
        }

        protected SkyrimModel m_SkryimModel;
        public SkyrimModel Skyrim
        {
            get { return m_SkryimModel; }
            protected set { m_SkryimModel = value; }
        }

        protected System.Windows.WindowState m_State = System.Windows.WindowState.Normal;
        public System.Windows.WindowState State
        {
            get { return m_State; }
            set
            {
                if (m_State.Equals(value))
                {
                    return;
                }

                m_State = value;
                OnPropertyChanged("State");
            }
        }
        
        #endregion

        #region Methods

        /// <summary>
        ///     <para>Shows the user a dialog prompting them to select a folder. Returns the selected folder to the command.</para>
        /// </summary>
        /// <returns></returns>
        public string Browse(string defaultPath = "")
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = defaultPath;

                return (dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : defaultPath);
            }
        }

        protected void ExtractDefaultSettings()
        {
            Assembly asm = Assembly.GetAssembly(typeof(SkyrimModel));

            if (!asm.SaveResource("Settings.xml", m_ConfigPath))
            {
                throw new FileNotFoundException("Unable to write default Settings.xml out to disk.", Path.GetFileName(m_ConfigPath));
            }
        }

        public void Initialise()
        {
            Assembly asm = Assembly.GetAssembly(typeof(MasterViewModel));
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(asm.Location);

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appData = String.Format(Path.Combine(appData, @"{0}\{1}"), info.CompanyName, info.ProductName);

            if (!Directory.Exists(appData))
            {
                Directory.CreateDirectory(appData);
            }

            string configPath = Path.Combine(appData, "Settings.xml");
            string logDirectory = Path.Combine(appData, "logs");
            string backupDirectory = Path.Combine(appData, "backups");

            m_ConfigPath = configPath;

            if (!File.Exists(m_ConfigPath))
            {
                ExtractDefaultSettings();
            }

            m_Config = XDocument.Load(m_ConfigPath);

            m_LogModel = new LogModel(this);
            m_LogModel.Directory = logDirectory;

            m_SkryimModel = new SkyrimModel(this);
            m_SkryimModel.Load(ref m_Config);

            m_LogModel.Initialize();
            m_CharacterModel = new CharacterModel(this);
            m_CharacterModel.BackupDirectory = backupDirectory;
            m_CharacterModel.Load(ref m_Config);

            m_Config.Save(m_ConfigPath);
        }

        /// <summary>
        ///     <para>Minimizes the application to the system tray.</para>
        /// </summary>
        public void Minimize()
        {
            if (!State.Equals(System.Windows.WindowState.Minimized) && KeepOpen)
            {
                m_TrayIcon.Visible = true;

                State = System.Windows.WindowState.Minimized;
            }
        }

        /// <summary>
        ///     <para>Restores the application to the foreground.</para>
        /// </summary>
        public void Restore()
        {
            if (!State.Equals(System.Windows.WindowState.Normal))
            {
                m_TrayIcon.Visible = false;
                State = System.Windows.WindowState.Normal;
            }
        }

        public void Save()
        {
            m_CharacterModel.Save(ref m_Config);
            m_SkryimModel.Save(ref m_Config);

            m_Config.Save(m_ConfigPath);
        }

        public void Shutdown()
        {
            Save();

            m_TrayIcon.Visible = false;
            m_TrayIcon.Dispose();

            m_LogModel.Dispose();
            m_App.Shutdown();
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
