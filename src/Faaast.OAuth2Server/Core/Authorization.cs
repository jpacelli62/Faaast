﻿using Microsoft.AspNetCore.Authentication;

namespace Faaast.Authentication.OAuth2Server.Core
{
    public class Authorization
    {
        public AuthenticationTicket AuthenticationTicket { get; set; }

        public string AuthorizationCode { get; set; }
    }
}
