// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Squidex.Domain.Users.Keycloak.Models;
using Squidex.Infrastructure;
using Squidex.Shared.Users;

namespace Squidex.Domain.Users.Keycloak
{
    public class KeycloakUser : IUser
    {
        private readonly UserRepresentation _userRepresentation;
        private readonly List<Claim> _claims;

        public KeycloakUser(UserRepresentation userRepresentation)
        {
            Guard.NotNull(userRepresentation, nameof(userRepresentation));
            this._userRepresentation = userRepresentation;
            this._claims = new List<Claim>();
        }

        public bool IsLocked => !this._userRepresentation.Enabled;
        
        public string Id => this._userRepresentation.Id;
        
        public string Email => this._userRepresentation.Email;
        
        public string NormalizedEmail => this._userRepresentation.Email.ToUpperInvariant();

        public IReadOnlyList<Claim> Claims => this._claims;
        
        public IReadOnlyList<ExternalLogin> Logins => new List<ExternalLogin>();

        public void SetEmail(string email)
        {
            throw new NotSupportedException();
        }

        public void SetClaim(string type, string value)
        {
            throw new NotSupportedException();
        }

        public void AddClaim(Claim claim)
        {
            if (_claims.Contains(claim))
            {
                return;
            }

            _claims.Add(claim);
        }
    }
}