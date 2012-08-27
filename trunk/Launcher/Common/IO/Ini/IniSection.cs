namespace Launcher.Common.IO.Ini
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class IniSection
    {
        #region Constructor(s)

        public IniSection(IniFile ini, string sectionName)
        {
            m_Ini = ini;
            SectionName = sectionName;

            m_SectionData = new Dictionary<string, string>();
        }

        #endregion

        #region Fields

        protected IniFile m_Ini;
        protected Dictionary<string, string> m_SectionData;

        #endregion

        #region Indexers

        /// <summary>
        ///     <para>Gets or sets a <see cref="System.String" /> value from the current section using the specified key.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        #endregion

        #region Properties

        protected string m_SectionName;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.String" /> value representing the section name.</para>
        /// </summary>
        public string SectionName
        {
            get { return m_SectionName; }
            set { m_SectionName = value; }
        }

        /// <summary>
        ///     <para>Retrieves a collection containing a list of keys in the current section.</para>
        /// </summary>
        public ReadOnlyCollection<String> Keys
        {
            get { return m_SectionData.Keys.ToList().AsReadOnly(); }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     <para>Adds the specified key-value pair to the current section if it doesn't already exist.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            if (!HasKey(key))
            {
                m_SectionData.Add(key, value);
            }
        }

        /// <summary>
        ///     <para>Retrieves a <see cref="System.String" /> value indexed by the specified key.</para>
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string Get(string keyName)
        {
            if (!m_SectionData.ContainsKey(keyName))
            {
                throw new KeyNotFoundException("The specified key was not found in the current IniSection.");
            }

            return m_SectionData[keyName];
        }

        /// <summary>
        ///     <para>Returns a <see cref="System.Boolean" /> value indicating whether the section has the specified key.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasKey(string key)
        {
            return m_SectionData.ContainsKey(key);
        }

        public void Set(string key, string value)
        {
            if (m_SectionData.ContainsKey(key))
            {
                m_SectionData[key] = value;
            }
            else
            {
                m_SectionData.Add(key, value);
            }
        }

        #endregion
    }
}
