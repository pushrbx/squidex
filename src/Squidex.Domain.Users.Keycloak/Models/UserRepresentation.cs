// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Squidex.Domain.Users.Keycloak.Models
{
    /// <summary>
    /// Representation of a user in keycloak rest api
    /// </summary>
    /// <remarks>
    /// https://www.keycloak.org/docs-api/3.4/rest-api/index.html#_userrepresentation
    /// </remarks>
    public class UserRepresentation
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("attributes")]
        public Dictionary<string, JObject> Attributes { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }
        
        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; }
        
        [JsonProperty("realmRoles")]
        public List<string> RealmRoles { get; set; }
        
        [JsonProperty("clientRoles")]
        public Dictionary<string, List<string>> ClientRoles { get; set; }
        
        [JsonProperty("access")]
        public Dictionary<string, bool> Access { get; set; }
        
        [JsonProperty("createdTimestamp")]
        public long CreatedTimestamp { get; set; }
    }
}