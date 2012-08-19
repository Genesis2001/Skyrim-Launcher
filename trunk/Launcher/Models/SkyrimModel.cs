namespace Launcher.Models
{
    using Launcher.Commands;
    using Launcher.Linq;
    using Launcher.ViewModels;

    using Microsoft.Win32;

    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Windows.Input;
    using System.Xml.Linq;

    public class SkyrimModel : INotifyPropertyChanged
    {
        #region Constructor(s)

        public SkyrimModel(MasterViewModel viewModel)
        {
            m_ViewModel = viewModel;

            BrowseDataPath = new RelayCommand((o) => DataPath = m_ViewModel.Browse(DataPath), (o) => true);
            BrowseInstallPath = new RelayCommand((o) => InstallPath = m_ViewModel.Browse(InstallPath), (o) => true);
            LaunchGame = new RelayCommand((o) =>
            {
                Process p = new Process();
                p.StartInfo.WorkingDirectory = Path.GetPathRoot(GameExe);
                p.StartInfo.FileName = GameExe;
                p.StartInfo.Arguments = (String.IsNullOrEmpty(CommandLine) ? String.Empty : CommandLine);

                p.Exited += new EventHandler(OnGameExit);
                bool started = p.Start();

                if (started)
                {
                    OnGameStart(p);
                }
            },
            (o) =>
            {
                return !String.IsNullOrEmpty(m_ViewModel.Character.Current) && !String.IsNullOrEmpty(GameExe);
            });
        }

        #endregion

        #region Fields

        protected MasterViewModel m_ViewModel;

        #endregion

        #region Commands

        public ICommand BrowseDataPath { get; protected set; }
        public ICommand BrowseInstallPath { get; protected set; }
        public ICommand LaunchGame { get; protected set; }

        #endregion

        #region Properties

        protected string m_CommandLine = String.Empty;
        public string CommandLine
        {
            get { return m_CommandLine; }
            set
            {
                if (m_CommandLine.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                m_CommandLine = value;
                OnPropertyChanged("CommandLine");
            }
        }

        protected string m_DataPath = String.Empty;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.String" /> value representing the data path of Skyrim.</para>
        /// </summary>
        public string DataPath
        {
            get { return m_DataPath; }
            set
            {
                if (m_DataPath.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                m_DataPath = value;
                OnPropertyChanged("DataPath");
            }
        }

        protected string m_Program = String.Empty;
        public string GameExe
        {
            get { return m_Program; }
            set
            {
                if (m_Program.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                m_Program = value;
                OnPropertyChanged("GameExe");
            }
        }

        protected string m_InstallPath = String.Empty;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.String" /> value representing the installation path of Skyrim.</para>
        /// </summary>
        public string InstallPath
        {
            get { return m_InstallPath; }
            set
            {
                if (m_InstallPath.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                m_InstallPath = value;
                OnPropertyChanged("InstallPath");
            }
        }

        #endregion

        #region Methods

        public void Load(ref XDocument config)
        {
            var paths = config.Element("Settings").Element("Paths");
            if (paths == null || paths.IsEmpty)
            {
                ExtractDefaultPathsXml(ref config);
            }

            var dataPath = paths.Element("DataPath");
            var instPath = paths.Element("InstallPath");

            if (String.IsNullOrEmpty(dataPath.Value) || String.IsNullOrEmpty(instPath.Value))
            {
                LoadPathsFromRegistry();
            }
            else
            {
                LoadPathsFromXml(ref config);
            }

            var launchPath = config.Element("Settings").Element("Launch");
            if (launchPath == null)
            {
                string path = Path.Combine(InstallPath, @"tesv.exe");

                launchPath = new XElement("Launch", new XAttribute("Application", path), new XAttribute("CommandLine", String.Empty));
                config.Element("Settings").Add(launchPath);
            }

            var startupApp = launchPath.Attribute("Application");
            if (startupApp == null)
            {
                string path = Path.Combine(InstallPath, @"tesv.exe");

                startupApp = new XAttribute("Application", path);
                launchPath.Add(startupApp);
            }

            GameExe = startupApp.Value;
        }

        protected void ExtractDefaultPathsXml(ref XDocument x)
        {
            var paths = x.Element("Settings").Element("Paths");
            if (paths == null || paths.IsEmpty)
            {
                if (paths.IsEmpty)
                {
                    paths.Remove();
                    paths = null;
                }

                Assembly asm = Assembly.GetAssembly(typeof(SkyrimModel));
                using (Stream s = asm.GetResource("Settings.xml"))
                {
                    if (s != null)
                    {
                        XDocument tmp = XDocument.Load(s);
                        paths = tmp.Element("Settings").Element("Paths");

                        x.Element("Settings").Add(paths);
                    }
                }
            }
        }

        protected void LoadPathsFromRegistry()
        {
            string docLibrary = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DataPath = Path.Combine(docLibrary, @"My Games\Skyrim");

            RegistryKey steam = null;
            try
            {
                steam = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
                if (steam == null)
                {
                    // TODO: Report bug to error log.
                }
                else
                {
                    string steamPath = steam.GetValue("SteamPath").ToString().Replace('/', '\\');
                    InstallPath = Path.Combine(steamPath, @"steamapps\common\skyrim");
                }
            }
            catch (System.Security.SecurityException)
            {
                // TODO: Report bug to error log.
            }
            finally
            {
                if (steam != null)
                {
                    steam.Close();
                }
            }
        }

        protected void LoadPathsFromXml(ref XDocument x)
        {
            var paths = x.Element("Settings").Element("Paths");
            if (paths == null || paths.IsEmpty)
            {
                Assembly asm = Assembly.GetAssembly(typeof(SkyrimModel));
                using (Stream s = asm.GetResource("Settings.xml"))
                {
                    if (s != null)
                    {
                        XDocument tmp = XDocument.Load(s);
                        paths = tmp.Element("Settings").Element("Paths");

                        x.Element("Settings").Add(paths);
                    }
                }
            }

            var dataPath = paths.Element("DataPath");
            var instPath = paths.Element("InstallPath");

            if ((dataPath == null || instPath == null) || (dataPath.IsEmpty || instPath.IsEmpty))
            {
                // TODO: Report bug to error log.
                LoadPathsFromRegistry();
                return;
            }

            DataPath = dataPath.Value;
            InstallPath = instPath.Value;
        }

        protected void OnGameExit(object sender, EventArgs e)
        {
            if (!(sender is Process))
            {
                return;
            }

            m_ViewModel.Restore();

            Process p = (Process)sender;
            p.Dispose();
        }

        protected void OnGameStart(object sender)
        {
            m_ViewModel.Minimize();
        }

        public void Save(ref XDocument config)
        {
            var paths = config.Element("Settings").Element("Paths");
            if (paths == null || paths.IsEmpty)
            {
                Assembly asm = Assembly.GetAssembly(typeof(SkyrimModel));
                using (Stream s = asm.GetResource("Settings.xml"))
                {
                    if (s != null)
                    {
                        XDocument tmp = XDocument.Load(s);
                        paths = tmp.Element("Settings").Element("Paths");

                        config.Element("Settings").Add(paths);
                    }
                }
            }

            var dataPath = paths.Element("DataPath");
            var instPath = paths.Element("InstallPath");

            dataPath.Value = DataPath;
            instPath.Value = InstallPath;

            var launchInfo = config.Element("Settings").Element("Launch");
            if (launchInfo == null)
            {
                launchInfo = new XElement("Launch", new XAttribute("Application", GameExe), new XAttribute("CommandLine", CommandLine));
                config.Element("Settings").Add(launchInfo);
            }
            else
            {
                var startupPath = launchInfo.Attribute("Application");
                if (startupPath == null)
                {
                    startupPath = new XAttribute("Application", GameExe);
                    launchInfo.Add(startupPath);
                }
                else
                {
                    startupPath.Value = GameExe;
                }
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
