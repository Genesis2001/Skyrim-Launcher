namespace Launcher.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Input;
    using System.Xml.Linq;
    using Launcher.Commands;
    using Launcher.Common;
    using Launcher.Common.IO.Ini;
    using Launcher.Linq;
    using Launcher.ViewModels;

    public class CharacterModel : INotifyPropertyChanged
    {
        #region Constructor(s)

        public CharacterModel(MasterViewModel viewModel)
        {
            m_ViewModel = viewModel;
            m_Characters = new ObservableCollection<string>();

            string skyrimIni = Path.Combine(m_ViewModel.Skyrim.DataPath, "Skyrim.ini");
            if (!File.Exists(skyrimIni))
            {
                // TODO: Extract (?) skyrim.ini
            }

            m_SkyrimIni = IniFile.Load(skyrimIni);

            AddCharacterCommand = new RelayCommand((o) => { Characters.Add((string)o); }, (o) => true);
            BackupCharacterCommand = new RelayCommand((o) => { Backup((string)o); }, (o) => true);
        }

        #endregion

        #region Fields

        protected IniFile m_SkyrimIni;
        protected MasterViewModel m_ViewModel;

        #endregion

        #region Commands

        public ICommand AddCharacterCommand { get; protected set; }
        public ICommand BackupCharacterCommand { get; protected set; }

        #endregion

        #region Properties

        protected string m_BackupDirectory = String.Empty;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.String" /> value representing the directory where backups are to be placed.</para>
        /// </summary>
        public string BackupDirectory
        {
            get { return m_BackupDirectory; }
            set
            {
                if (m_BackupDirectory.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                m_BackupDirectory = value;
                OnPropertyChanged("BackupDirectory");
            }
        }

        protected string m_Current = String.Empty;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.String" /> value representing the currently selected character.</para>
        /// </summary>
        public string Current
        {
            get { return m_Current; }
            set
            {
                if (m_Current.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                m_Current = value;
                OnPropertyChanged("Current");
                OnCharacterChange(m_Current);
            }
        }

        protected ObservableCollection<string> m_Characters;
        /// <summary>
        ///     <para>Represents a collection of characters.</para>
        /// </summary>
        public ObservableCollection<string> Characters
        {
            get { return m_Characters; }
            protected set { m_Characters = value; }
        }

        protected Progress m_Progress;
        /// <summary>
        ///     <para>Gets or sets a value indicating progress being done.</para>
        /// </summary>
        public Progress Progess
        {
            get { return m_Progress; }
            set
            {
                if (m_Progress.Equals(value))
                {
                    return;
                }

                m_Progress = value;
                OnPropertyChanged("Progress");
            }
        }

        protected string m_SavesDirectory = String.Empty;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.String" /> value representing the location of the saved games.</para>
        /// </summary>
        public string SavesDirectory
        {
            get { return m_SavesDirectory; }
            set
            {
                if (m_SavesDirectory.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                m_SavesDirectory = value;
                OnPropertyChanged("SavesDirectory");
            }
        }

        #endregion

        #region Methods

        public bool Backup(string character)
        {
            string archiveName = String.Format(Path.Combine(BackupDirectory, @"character-{0}.{1}.zip"), character, DateTime.Now.ToUnixTimestamp());

            string cDir = Path.Combine(SavesDirectory, character);
            if (!Directory.Exists(cDir))
            {
                return false;
            }

            ZipArchive archive = new ZipArchive(cDir, "*.ess", String.Empty, false);

            return archive.Save(archiveName);
        }

        public bool BackupAll()
        {
            string fileName = String.Format(Path.Combine(BackupDirectory, @"fullbackup.{0}.zip"), DateTime.Now.ToUnixTimestamp());

            ZipArchive archive = new ZipArchive(SavesDirectory, "*", String.Empty, true);
            return archive.Save(fileName);
        }

        /// <summary>
        ///     <para>Loads characters into the collection of characters.</para>
        /// </summary>
        public void Load(ref XDocument config)
        {
            if (!Directory.Exists(BackupDirectory))
            {
                Directory.CreateDirectory(BackupDirectory);
            }

            SavesDirectory = Path.Combine(m_ViewModel.Skyrim.DataPath, @"Saves");

            m_ViewModel.Log.Write("Loading characters");

            var characters = config.Element("Settings").Element("Characters");
            if (characters == null)
            {
                characters = new XElement("Characters", new XAttribute("LastSelected", String.Empty));
                config.Element("Settings").Add(characters);
            }

            if (!characters.HasElements)
            {
                LoadCharactersFromDirectory();
            }
            else
            {
                LoadCharactersFromXml(ref config);
            }

            m_ViewModel.Log.Write("Loaded {0} characters.", Characters.Count);
        }

        protected void LoadCharactersFromDirectory()
        {
            string[] dirs = Directory.GetDirectories(SavesDirectory, "*", SearchOption.TopDirectoryOnly);
            if (dirs.Length > 0)
            {
                foreach (string item in dirs)
                {
                    Characters.Add(item);
                }
            }

            string[] games = Directory.GetFiles(SavesDirectory, "*.ess", SearchOption.TopDirectoryOnly);
            if (games.Length > 0)
            {
                // We need to sort the saves directory.
                BackupAll();
            }


        }

        protected void LoadCharactersFromXml(ref XDocument x)
        {
            var cs = x.Element("Settings").Element("Characters");
            foreach (XElement item in cs.Descendants("Character"))
            {
                var name = item.Attribute("Name");
                if (name == null)
                {
                    m_ViewModel.Log.Write("Unable to load a character: {0}", item.ToString());
                    continue;
                }

                Characters.Add(name.Value);
            }

            var last = cs.Attribute("LastSelected");
            if (last == null)
            {
                last = new XAttribute("LastSelected", Characters[0]);
                cs.Add(last);
            }

            Current = last.Value;
        }

        protected void OnCharacterChange(string character)
        {
            if (m_SkyrimIni != null)
            {
                string path = Path.Combine(SavesDirectory, character);



                m_SkyrimIni.Set("General", "SLocalSavePath", path);

                if (m_ViewModel.AutoSave)
                {
                    m_SkyrimIni.Save();
                }
            }
        }

        public void Save(ref XDocument config)
        {
            m_ViewModel.Log.Write("Saving characters to XML Cache.");

            OnCharacterChange(Current);
            m_SkyrimIni.Save();

            var cs = config.Element("Settings").Element("Characters");
            if (cs == null)
            {
                cs = new XElement("Characters", new XAttribute("LastSelected", String.Empty));
                config.Element("Settings").Add(cs);
            }
            else { cs.RemoveAll(); }

            if (Characters.Contains(Current))
            {
                var last = cs.Attribute("LastSelected");
                if (last == null)
                {
                    last = new XAttribute("LastSelected", Current);
                    cs.Add(last);
                }
                else
                {
                    cs.Attribute("LastSelected").Value = Current;
                }
            }

            foreach (string item in Characters)
            {
                XElement c = new XElement("Character", new XAttribute("Name", item));
                cs.Add(c);
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
