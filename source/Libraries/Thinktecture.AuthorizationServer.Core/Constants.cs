/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

namespace Thinktecture.AuthorizationServer
{
    public static class Constants
    {
        public const string InternalIssuer = "InternalIssuer";

        public static class Roles
        {
            public const string Administrators = "Administrators";
        }

        public static class ClaimTypes
        {
            public const string Subject = "sub";
        }

        public static class Actions
        {
            public const string Configure = "Configure";
        }

        public static class Resources
        {
            public const string General = "General";
            public const string Applications = "Application";
            public const string Global = "Global";
            public const string Keys = "Keys";
            public const string Tokens = "Tokens";
            public const string Clients = "Clients";
        }
    }

}
