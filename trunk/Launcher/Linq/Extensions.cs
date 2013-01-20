using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Launcher.Linq
{
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
            string resource =
                resources.SingleOrDefault(r => r.EndsWith(resourceName, StringComparison.CurrentCultureIgnoreCase));

            using (Stream s = source.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidResourceException(resource, "The specified resource could not be loaded.");
                }

                var buffer = new byte[s.Length];
                s.Read(buffer, 0, buffer.Length);

                using (var writer = new BinaryWriter(File.Open(targetFile, FileMode.Create)))
                {
                    writer.Write(buffer);
                }
            }

            return (File.Exists(targetFile) && (new FileInfo(targetFile).Length > 0));
        }

        public static Stream GetResource(this Assembly source, string resourceName)
        {
            string[] resources = source.GetManifestResourceNames();
            string resource =
                resources.SingleOrDefault(r => r.EndsWith(resourceName, StringComparison.CurrentCultureIgnoreCase));

            return source.GetManifestResourceStream(resource);
        }
    }
}