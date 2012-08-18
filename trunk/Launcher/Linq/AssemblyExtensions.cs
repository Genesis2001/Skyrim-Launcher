namespace Launcher.Linq
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static partial class Extensions
    {
        /// <summary>
        ///     <para>Extracts the specified resource from the assembly and writes it out to the specified target file.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="resourceName"></param>
        /// <param name="targetFile"></param>
        /// <returns></returns>
        /// <exception cref="Launcher.Linq.InvalidResourceException" />
        public static bool SaveResource(this Assembly source, string resourceName, string targetFile)
        {
            string[] resources = source.GetManifestResourceNames();
            string resource = resources.SingleOrDefault(r => r.EndsWith(resourceName, StringComparison.CurrentCultureIgnoreCase));

            using (Stream s = source.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidResourceException(resource, "The specified resource could not be loaded.");
                }

                byte[] buffer = new byte[s.Length];
                s.Read(buffer, 0, buffer.Length);

                using (BinaryWriter writer = new BinaryWriter(File.Open(targetFile, FileMode.Create)))
                {
                    writer.Write(buffer);
                }
            }

            return (File.Exists(targetFile) && (new FileInfo(targetFile).Length > 0));
        }

        public static Stream GetResource(this Assembly source, string resourceName)
        {
            string[] resources = source.GetManifestResourceNames();
            string resource = resources.SingleOrDefault(r => r.EndsWith(resourceName, StringComparison.CurrentCultureIgnoreCase));

            return source.GetManifestResourceStream(resource);
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
