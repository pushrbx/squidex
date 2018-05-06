﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Rules;
using Squidex.Domain.Apps.Entities.Schemas;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Caching;
using Squidex.Infrastructure.Log;
using Squidex.Infrastructure.Orleans;

namespace Squidex.Domain.Apps.Entities
{
    public sealed class AppProvider : IAppProvider
    {
        private readonly IGrainFactory grainFactory;
        private readonly IRequestCache requestCache;

        public AppProvider(IGrainFactory grainFactory, IRequestCache requestCache)
        {
            Guard.NotNull(grainFactory, nameof(grainFactory));
            Guard.NotNull(requestCache, nameof(requestCache));

            this.grainFactory = grainFactory;
            this.requestCache = requestCache;
        }

        public Task<(IAppEntity, ISchemaEntity)> GetAppWithSchemaAsync(Guid appId, Guid id)
        {
            return requestCache.GetOrCreateAsync($"GetAppWithSchemaAsync({appId}, {id})", async () =>
            {
                using (Profile.Method<AppProvider>())
                {
                    var app = await grainFactory.GetGrain<IAppGrain>(appId).GetStateAsync();

                    if (!IsExisting(app))
                    {
                        return (null, null);
                    }

                    var schema = await grainFactory.GetGrain<ISchemaGrain>(id).GetStateAsync();

                    if (!IsExisting(schema, false))
                    {
                        return (null, null);
                    }

                    return (app.Value, schema.Value);
                }
            });
        }

        public Task<IAppEntity> GetAppAsync(string appName)
        {
            return requestCache.GetOrCreateAsync($"GetAppAsync({appName})", async () =>
            {
                using (Profile.Method<AppProvider>())
                {
                    var appId = await GetAppIdAsync(appName);

                    if (appId == Guid.Empty)
                    {
                        return null;
                    }

                    var app = await grainFactory.GetGrain<IAppGrain>(appId).GetStateAsync();

                    if (!IsExisting(app))
                    {
                        return null;
                    }

                    return app.Value;
                }
            });
        }

        public Task<ISchemaEntity> GetSchemaAsync(Guid appId, string name)
        {
            return requestCache.GetOrCreateAsync($"GetSchemaAsync({appId}, {name})", async () =>
            {
                using (Profile.Method<AppProvider>())
                {
                    var schemaId = await GetSchemaIdAsync(appId, name);

                    if (schemaId == Guid.Empty)
                    {
                        return null;
                    }

                    return await GetSchemaAsync(appId, schemaId, false);
                }
            });
        }

        public Task<ISchemaEntity> GetSchemaAsync(Guid appId, Guid id, bool allowDeleted = false)
        {
            return requestCache.GetOrCreateAsync($"GetSchemaAsync({appId}, {id}, {allowDeleted})", async () =>
            {
                using (Profile.Method<AppProvider>())
                {
                    var schema = await grainFactory.GetGrain<ISchemaGrain>(id).GetStateAsync();

                    if (!IsExisting(schema, allowDeleted) || schema.Value.AppId.Id != appId)
                    {
                        return null;
                    }

                    return schema.Value;
                }
            });
        }

        public Task<List<ISchemaEntity>> GetSchemasAsync(Guid appId)
        {
            return requestCache.GetOrCreateAsync($"GetSchemasAsync({appId})", async () =>
            {
                using (Profile.Method<AppProvider>())
                {
                    var ids = await grainFactory.GetGrain<ISchemasByAppIndex>(appId).GetSchemaIdsAsync();

                    var schemas =
                        await Task.WhenAll(
                            ids.Select(id => grainFactory.GetGrain<ISchemaGrain>(id).GetStateAsync()));

                    return schemas.Where(s => IsFound(s.Value)).Select(s => s.Value).ToList();
                }
            });
        }

        public Task<List<IRuleEntity>> GetRulesAsync(Guid appId)
        {
            return requestCache.GetOrCreateAsync($"GetRulesAsync({appId})", async () =>
            {
                using (Profile.Method<AppProvider>())
                {
                    var ids = await grainFactory.GetGrain<IRulesByAppIndex>(appId).GetRuleIdsAsync();

                    var rules =
                        await Task.WhenAll(
                            ids.Select(id => grainFactory.GetGrain<IRuleGrain>(id).GetStateAsync()));

                    return rules.Where(r => IsFound(r.Value)).Select(r => r.Value).ToList();
                }
            });
        }

        public Task<List<IAppEntity>> GetUserApps(string userId)
        {
            return requestCache.GetOrCreateAsync($"GetUserApps({userId})", async () =>
            {
                using (Profile.Method<AppProvider>())
                {
                    var ids = await grainFactory.GetGrain<IAppsByUserIndex>(userId).GetAppIdsAsync();

                    var apps =
                        await Task.WhenAll(
                            ids.Select(id => grainFactory.GetGrain<IAppGrain>(id).GetStateAsync()));

                    return apps.Where(a => IsFound(a.Value)).Select(a => a.Value).ToList();
                }
            });
        }

        private async Task<Guid> GetAppIdAsync(string name)
        {
            using (Profile.Method<AppProvider>())
            {
                return await grainFactory.GetGrain<IAppsByNameIndex>(SingleGrain.Id).GetAppIdAsync(name);
            }
        }

        private async Task<Guid> GetSchemaIdAsync(Guid appId, string name)
        {
            using (Profile.Method<AppProvider>())
            {
                return await grainFactory.GetGrain<ISchemasByAppIndex>(appId).GetSchemaIdAsync(name);
            }
        }

        private static bool IsFound(IEntityWithVersion entity)
        {
            return entity.Version > EtagVersion.Empty;
        }

        private static bool IsExisting(J<IAppEntity> app)
        {
            return IsFound(app.Value) && !app.Value.IsArchived;
        }

        private static bool IsExisting(J<ISchemaEntity> schema, bool allowDeleted)
        {
            return IsFound(schema.Value) && (!schema.Value.IsDeleted || allowDeleted);
        }
    }
}
