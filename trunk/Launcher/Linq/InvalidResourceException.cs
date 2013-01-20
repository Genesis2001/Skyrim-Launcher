using System;

namespace Launcher.Linq
{
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

        public InvalidResourceException(string assemblyName, string resourceName, string message,
                                        Exception innerException)
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