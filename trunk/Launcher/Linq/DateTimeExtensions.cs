﻿using System;

namespace Launcher.Linq
{
    public static partial class Extensions
    {
        /// <summary>
        ///     <para>
        ///         Converts a <see cref="System.Int64" /> into a <see cref="System.DateTime" />
        ///     </para>
        /// </summary>
        /// <param name="source"></param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns></returns>
        public static DateTime FromUnixTimestamp(this long source)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0);

            throw new NotImplementedException();
        }

        /// <summary>
        ///     <para>Converts a <see cref="System.DateTime" /> into it's corresponding UNIX Timestamp.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static long ToUnixTimestamp(this DateTime source)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0);

            return Convert.ToInt64(Math.Floor((source.ToUniversalTime() - origin).TotalSeconds));
        }
    }
}