namespace Skyrim.Data.Linq
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class AssemblyExtensions
    {
        public static bool SaveResource(this Assembly source, string resourceName, string targetFile)
        {
            string resource = String.Join(".", source.GetName().Name, resourceName);
            using (Stream s = source.GetManifestResourceStream(resource))
            {
                if (s == null)
                    throw new InvalidResourceException(resource, "The specified resource could not be loaded.");

                byte[] buffer = new byte[s.Length];
                s.Read(buffer, 0, buffer.Length);

                using (BinaryWriter writer = new BinaryWriter(File.Open(targetFile, FileMode.Create)))
                {
                    writer.Write(buffer);
                }
            }

            return (File.Exists(targetFile) && (new FileInfo(targetFile).Length > 0));
        }
    }

    public class InvalidResourceException : Exception
    {
        #region Constructors

        public InvalidResourceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidResourceException(string resourceName, string message)
            : this(resourceName, message, null)
        {
        }

        public InvalidResourceException(string resourceName, string message, Exception innerException)
            : this(message, innerException)
        {
            Resource = resourceName;
        }

        public InvalidResourceException(string assemblyName, string resourceName, string message, Exception innerException)
            : this(resourceName, message, innerException)
        {
            Assembly = assemblyName;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     <para>Gets the assembly name of the invalid resource.</para>
        /// </summary>
        public string Assembly { get; private set; }

        /// <summary>
        ///     <para>Gets the resource name that is invalid.</para>
        /// </summary>
        public string Resource { get; private set; }

        #endregion
    }
}
