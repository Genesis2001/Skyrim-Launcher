namespace Skyrim.Data
{
    using Skyrim.Data.Collections;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.IO;

    public class Character : INotifyPropertyChanged, INamedObject
    {
        #region Fields
        #endregion

        #region Properties

        private string m_Name;
        /// <summary>
        ///     <para>Gets or sets the name of the current character.</para>
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set
            {
                if (m_Name == value)
                    return;

                m_Name = value;
                OnPropertyChanged("Name");
            }
        }

        private string m_directory;
        /// <summary>
        ///     <para>Gets or sets the name of the directory </para>
        /// </summary>
        public string Directory
        {
            get { return m_directory; }
            set
            {
                if (m_directory == value)
                    return;

                m_directory = value;
                OnPropertyChanged("Directory");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     <para></para>
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        public static Character Create(string characterName, string dataPath = "")
        {
            Character c = new Character();
            c.Name = characterName;

            if (!String.IsNullOrWhiteSpace(dataPath))
                c.Directory = String.Format(Path.Combine(dataPath, @"Saves\{0}"), characterName);

            return c;
        }

        /// <summary>
        ///     <para>Backs the current character into a zip archive.</para>
        /// </summary>
        /// <returns></returns>
        public bool Backup()
        {
            return false;
        }

        /// <summary>
        ///     <para>Renames the current character to the specified name.</para>
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        public bool Rename(string characterName)
        {
            return false;
        }

        /// <summary>
        ///     <para>Returns a <see cref="System.String" /> representation of the current <see cref="Skyrim.Data.Character" /> instance.</para>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
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
