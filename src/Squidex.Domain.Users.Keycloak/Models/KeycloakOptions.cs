// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.Domain.Users.Keycloak.Models
{
    public class KeycloakOptions
    {
        public bool Enabled { get; set; }
        
        // should contain realm name
        public string BaseUrl { get; set; }
        
        public string AdminUserName { get; set; }
        
        public string AdminPassword { get; set; }
        
        public string ClientNameForAdminFrontend { get; set; }
    }
}