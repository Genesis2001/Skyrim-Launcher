namespace Launcher.Models
{
    using System;
    using System.IO;
    using ICSharpCode.SharpZipLib.Zip;
    using ICSharpCode.SharpZipLib.Core;

    public class ZipArchive
    {
        #region Constructor(s)

        public ZipArchive(string fileName, string searchPattern, string password)
        {
            Path = fileName;
            Pattern = searchPattern;
            Password = password;

            IsFile = true;
        }

        public ZipArchive(string directoryPath, string searchPattern, string password, bool recurse)
        {
            Path = directoryPath;
            Pattern = searchPattern;
            m_IsRecursive = recurse;
        }

        #endregion

        #region Properties

        protected bool m_IsFile = false;
        /// <summary>
        ///     <para>Gets a <see cref="System.Boolean" /> value representing whether the path is a file.</para>
        /// </summary>
        public bool IsFile
        {
            get { return m_IsFile; }
            protected set { m_IsFile = value; }
        }

        protected bool m_IsRecursive = false;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.Boolean" /> value representing whether to recursively search inside the path for files.</para>
        /// </summary>
        public bool IsRecursive
        {
            get { return m_IsRecursive; }
            set { m_IsRecursive = value; }
        }

        protected string m_Password;
        /// <summary>
        ///     <para>Sets a <see cref="System.String" /> value representing the password to the ZipArchive file.</para>
        /// </summary>
        public string Password
        {
            protected get { return m_Password; }
            set { m_Password = value; }
        }

        protected string m_Path;
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.String" /> value representing the directory to be archived.</para>
        /// </summary>
        public string Path
        {
            get { return m_Path; }
            set { m_Path = value; }
        }


        protected string m_Pattern = "*";
        /// <summary>
        ///     <para>Gets or sets a <see cref="System.String" /> value representing the search pattern for files.</para>
        /// </summary>
        public string Pattern
        {
            get { return m_Pattern; }
            set { m_Pattern = value; }
        }

        #endregion

        #region Methods

        protected virtual void AppendFile(string path, ZipOutputStream stream, int folderOffset)
        {
            FileInfo info = new FileInfo(path);
            string entryName = path.Substring(folderOffset);
            entryName = ZipEntry.CleanName(entryName);

            ZipEntry entry = new ZipEntry(entryName);
            entry.DateTime = info.LastWriteTime;

            if (!String.IsNullOrEmpty(Password))
            {
                entry.AESKeySize = 256; // possible values: 0 (off), 128, 256.
                entry.Size = info.Length;
            }

            stream.PutNextEntry(entry);
            byte[] buffer = new byte[4096];
            using (FileStream fsStream = File.OpenRead(path))
            {
                StreamUtils.Copy(fsStream, stream, buffer);
            }

            stream.CloseEntry();
        }

        protected virtual void AppendFolder(string path, ZipOutputStream stream, int folderOffset)
        {
            string[] files = Directory.GetFiles(path, Pattern);

            foreach (string item in files)
            {
                AppendFile(item, stream, folderOffset);
            }

            if (IsRecursive)
            {
                string[] folders = System.IO.Directory.GetDirectories(path);
                foreach (string item in folders)
                {
                    AppendFolder(item, stream, folderOffset);
                }
            }
        }

        /// <summary>
        ///     <para>Saves the directory to the specified output file.</para>
        /// </summary>
        /// <param name="outputFile"></param>
        public bool Save(string outputFile)
        {
            FileStream fsOut = File.Create(outputFile);
            ZipOutputStream stream = new ZipOutputStream(fsOut);
            stream.SetLevel(5);

            if (!String.IsNullOrEmpty(Password))
            {
                stream.Password = Password;
            }

            try
            {
                int folderOffset = Path.Length + (Path.EndsWith("\\") ? 0 : 1);

                if (!IsFile)
                {
                    AppendFolder(Path, stream, folderOffset);
                }
                else
                {
                    AppendFile(Path, stream, folderOffset);
                }
            }
            finally
            {
                if (stream != null && stream.IsFinished)
                {
                    stream.IsStreamOwner = true;
                    stream.Close();
                }
            }

            return (File.Exists(outputFile) && (new FileInfo(outputFile).Length > 0));
        }

        #endregion
    }
}
