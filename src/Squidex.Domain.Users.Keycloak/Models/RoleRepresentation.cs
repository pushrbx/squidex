// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;

namespace Squidex.Domain.Users.Keycloak.Models
{
    public class RoleRepresentation
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("composite")]
        public bool Composite { get; set; }
        
        [JsonProperty("clientRole")]
        public bool ClientRole { get; set; }
        
        [JsonProperty("containerId")]
        public string ContainerId { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("scopeParamRequired")]
        public bool ScopeParamRequired { get; set; }
    }
}