// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Squidex.Domain.Users.Keycloak.Models;
using Squidex.Infrastructure;
using Squidex.Shared.Users;

namespace Squidex.Domain.Users.Keycloak
{
    public class KeycloakUserResolver : IUserResolver
    {
        private readonly IKeycloakClient _keycloakClient;
        private readonly IKeycloakUserClaimMapper _userClaimMapper;
    
        public KeycloakUserResolver(IKeycloakClient keycloakClient, IKeycloakUserClaimMapper userClaimMapper)
        {
            Guard.NotNull(keycloakClient, nameof(keycloakClient));
    
            _keycloakClient = keycloakClient;
            _userClaimMapper = userClaimMapper;
        }
        
        public async Task<IUser> FindByIdOrEmailAsync(string idOrEmail)
        {
            var users = await SearchByEmail(idOrEmail);
            UserRepresentation user;
            if (users == null || users.Count == 0)
            {
                user = await GetById(idOrEmail);
            }
            else
            {
                user = users.FirstOrDefault();
            }

            if (user == null)
                return null;
    
            return await _userClaimMapper.MapClaimsAsync(new KeycloakUser(user));
        }
    
        public async Task<List<IUser>> QueryByEmailAsync(string email)
        {
            var users = await SearchByEmail(email);
            var result = new List<IUser>();
            if (users == null)
                return result;
    
            foreach (var user in users)
            {
                result.Add(await _userClaimMapper.MapClaimsAsync(new KeycloakUser(user)));
            }
    
            return result;
        }
    
        private async Task<List<UserRepresentation>> SearchByEmail(string email)
        {
            var users = await _keycloakClient.GetUsersAsync(new Dictionary<string, string>()
            {
                {"email", email}
            });
    
            return users;
        }
    
        private async Task<UserRepresentation> GetById(string id)
        {
            var user = await _keycloakClient.GetUserAsync(id);
            
            return user;
        }
    }
}