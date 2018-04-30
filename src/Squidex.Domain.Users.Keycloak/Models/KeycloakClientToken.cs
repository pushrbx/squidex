// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.Domain.Users.Keycloak.Models
{
    internal class KeycloakClientToken
    {
        public string Token { get; set; }
        
        public DateTime ExpiresAt { get; set; }
        
        public string RefreshToken { get; set; }
        
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}