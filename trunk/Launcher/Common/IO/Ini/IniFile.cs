namespace Launcher.Common.IO.Ini
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class IniFile
    {
        #region Constructor(s)

        public IniFile()
            : this(String.Empty)
        {
        }

        public IniFile(string fileName)
            : this(fileName, Encoding.UTF8)
        {
        }

        public IniFile(string fileName, Encoding fileEncoding)
        {
            Path = fileName;
            m_FileEncoding = fileEncoding;

            m_Sections = new HashSet<IniSection>();
        }

        #endregion

        #region Fields

        protected Encoding m_FileEncoding = Encoding.UTF8;
        protected HashSet<IniSection> m_Sections;

        #endregion

        #region Indexers
        #endregion

        #region Properties

        /// <summary>
        ///     <para>Gets a <see cref="System.Int32" /> value indicating the number of sections loaded.</para>
        /// </summary>
        public int Count
        {
            get { return m_Sections.Count; }
        }

        protected string m_FilePath;
        /// <summary>
        ///     <para>Gets a <see cref="System.String" /> value representing the path to the file being accessed.</para>
        /// </summary>
        public string Path
        {
            get { return m_FilePath; }
            protected set { m_FilePath = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     <para>Gets a <see cref="System.String" /> value for the specified section and key names.</para>
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string Get(string sectionName, string keyName)
        {
            IniSection section = m_Sections.SingleOrDefault(s => s.SectionName.Equals(sectionName, StringComparison.InvariantCultureIgnoreCase));

            if (section == null)
            {
                return String.Empty;
            }

            if (section.HasKey(keyName))
            {
                return section.Get(keyName);
            }

            return String.Empty;
        }

        /// <summary>
        ///     <para>Loads the Ini file into memory.</para>
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            FileStream fsIn = null;
            StreamReader fsReader = null;

            try
            {
                fsIn = File.OpenRead(Path);
                fsReader = new StreamReader(fsIn, m_FileEncoding);

                IniSection currentSection = null;
                string line = null;
                string[] kvp = null;
                while ((line = fsReader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (line.StartsWith("#") || line.StartsWith(";")) continue;     // Ignore comments.
                    else if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        if (currentSection != null)
                        {
                            // Since the current section isn't null, we've already began collecting data.
                            // So, we'll add it to the collection before new()'ing it up again.
                            m_Sections.Add(currentSection);
                        }

                        currentSection = new IniSection(this, line.Substring(1, line.IndexOf(']') - 1));
                    }
                    else if ((kvp = line.Split('=')).Length > 0)
                    {
                        if (currentSection != null)
                        {
                            if (!currentSection.HasKey(kvp[0]))
                            {
                                currentSection.Add(kvp[0], kvp[1]);
                                kvp = null;     // null the array out.
                            }
                            else continue;      // Ignore duplicate keys under sections.
                        }
                        else continue;
                    }
                    else
                    {
                        // throw exception for unrecognized text.
                        throw new Exception();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            finally
            {
                if (fsReader != null)
                {
                    fsReader.Close();
                    fsReader.Dispose();
                }
            }

            return true;
        }

        /// <summary>
        ///     <para>Saves the INI information out to the path.</para>
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            StringBuilder builder = new StringBuilder();

            foreach (IniSection item in m_Sections)
            {
                builder.AppendFormat("[{0}]\n", item.SectionName);

                foreach (string key in item.Keys)
                {
                    builder.AppendFormat("{0} = {1}\n", key, item.Get(key));
                }

                builder.AppendLine();
            }

            FileStream fsOut = null;
            StreamWriter fsWriter = null;

            try
            {
                fsOut = File.OpenWrite(Path);
                fsWriter = new StreamWriter(fsOut, m_FileEncoding);

                fsWriter.Write(builder.ToString());
                fsWriter.Flush();
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            finally
            {
                if (fsWriter != null)
                {
                    fsWriter.Close();
                    fsWriter.Dispose();
                }
            }

            return (File.Exists(Path) && (new FileInfo(Path).Length > 0));
        }

        public void Set(string sectionName, string key, string value)
        {
            IniSection section = m_Sections.SingleOrDefault(s => s.SectionName.Equals(sectionName, StringComparison.InvariantCultureIgnoreCase));

            if (section == null)
            {
                throw new Exception();
            }

            if (section.HasKey(key))
            {
                section.Set(key, value);
            }
            else
            {
                section.Add(key, value);
            }
        }

        #endregion

        #region IniFile Factories

        public static IniFile Load(string fileName)
        {
            IniFile ini = new IniFile(fileName);

            bool result = ini.Load();
            if (!result)
            {
                throw new Exception(); // TODO: Exception.
            }

            return ini;
        }

        public static IniFile Load(string fileName, Encoding fileEncoding)
        {
            IniFile ini = new IniFile(fileName, fileEncoding);

            bool result = ini.Load();
            if (!result)
            {
                throw new Exception(); // TODO: Exception.
            }

            return ini;
        }

        #endregion
    }
}
