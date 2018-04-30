// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Squidex.Shared.Users;

namespace Squidex.Domain.Users.Keycloak
{
    public class KeycloakUserFactory : IUserFactory
    {      
        public IUser Create(string email)
        {
            throw new NotSupportedException();
        }
    }
}