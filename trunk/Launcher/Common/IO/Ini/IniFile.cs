namespace Launcher.Common.IO.Ini
{
    using System;
    using System.IO;
    using System.Text;
    using Launcher.Collections;

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
        }

        #endregion

        #region Fields
        
        protected Encoding m_FileEncoding;
        protected DictionaryCollection<IniSection> m_Sections;

        #endregion

        #region Indexers
        #endregion

        #region Properties

        protected string m_FilePath;
        public string Path
        {
            get { return m_FilePath; }
            set { m_FilePath = value; }
        }

        #endregion

        #region Methods

        public string Get(string sectionName, string keyName)
        {
            throw new NotImplementedException();
        }

        public bool Load()
        {
            FileStream fsIn = File.OpenRead(Path);
            StreamReader reader = new StreamReader(fsIn, m_FileEncoding);

            try
            {
                IniSection currentSection = null;
                string line = null;
                string[] kvp = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || line.StartsWith(";")) continue;     // Ignore comments.
                    else if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        if (currentSection != null)
                        {
                            // Since the current section isn't null, we've already began collecting data.
                            // So, we'll add it to the collection before new()'ing it up again.
                            m_Sections.Add(currentSection);
                        }

                        currentSection = new IniSection(line.Substring(1, line.IndexOf(']') - 1));
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
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }

                if (fsIn != null)
                {
                    fsIn.Close();
                    fsIn.Dispose();
                }
            }

            return true;
        }

        public void Save()
        {
            StringBuilder builder = new StringBuilder();

            foreach (IniSection item in m_Sections)
            {
                builder.AppendFormat("[{0}]", item.Name);

                foreach (string key in item.Keys)
                {
                    builder.AppendFormat("{0} = {1}", key, item.Get(key));
                }
                builder.AppendLine();
            }

            // TODO: Save items to file.
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
            throw new NotImplementedException();
        }

        #endregion
    }
}
