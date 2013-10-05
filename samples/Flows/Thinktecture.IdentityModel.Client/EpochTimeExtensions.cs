/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using System;

namespace Thinktecture.IdentityModel.Client
{
    public static class EpochTimeExtensions
    {
        /// <summary>
        /// Converts the given date value to epoch time.
        /// </summary>
        public static long ToEpochTime(this DateTime dateTime)
        {
            var date = dateTime.ToUniversalTime();
            var ts = date - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// Converts the given date value to epoch time.
        /// </summary>
        public static long ToEpochTime(this DateTimeOffset dateTime)
        {
            var date = dateTime.ToUniversalTime();
            var ts = date - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// Converts the given epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
        /// </summary>
        public static DateTime ToDateTimeFromEpoch(this long intDate)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(intDate);
        }

        /// <summary>
        /// Converts the given epoch time to a UTC <see cref="DateTimeOffset"/>.
        /// </summary>
        public static DateTimeOffset ToDateTimeOffsetFromEpoch(this long intDate)
        {
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddSeconds(intDate);
        }
    }
}
