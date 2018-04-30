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
    /// 
    /// </summary>
    /// <remarks>
    /// See: https://www.keycloak.org/docs-api/3.4/rest-api/index.html#_resourceserverrepresentation
    /// </remarks>
    public class ResourceServerRepresentation
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("clientId")]
        public string ClientId { get; set; }
        
        [JsonProperty("allowRemoteResourceManagement")]
        public bool AllowRemoteResourceManagement { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("policies")]
        public List<JObject> Policies { get; set; }
        
        [JsonProperty("resources")]
        public List<JObject> Resources { get; set; }
        
        [JsonProperty("scopes")]
        public List<JObject> Scopes { get; set; }
    }
}