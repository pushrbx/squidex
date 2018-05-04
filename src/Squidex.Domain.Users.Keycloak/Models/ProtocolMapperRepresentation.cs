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
    /// See: https://www.keycloak.org/docs-api/3.4/rest-api/index.html#_protocolmapperrepresentation
    /// </remarks>
    public class ProtocolMapperRepresentation
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("config")]
        public Dictionary<string, string> Config { get; set; }
        
        [JsonProperty("consentRequired")]
        public bool ConsentRequired { get; set; }
        
        [JsonProperty("consentText")]
        public string ConsentText { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
        
        [JsonProperty("protocolMapper")]
        public string ProtocolMapper { get; set; }
    }
}