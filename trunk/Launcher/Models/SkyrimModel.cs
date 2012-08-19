namespace Launcher.Models
{
    using Launcher.Commands;
    using Launcher.Linq;
    using Launcher.ViewModels;

    using Microsoft.Win32;

    using System;
    using System.ComponentModel;
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

            LaunchGame = new LaunchGameCommand(viewModel);
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
        }

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
