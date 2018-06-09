/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

import {
    AnalyticsService,
    ApiUrlConfig,
    HTTP,
    Model,
    pretifyError,
    Version,
    Versioned
} from '@app/framework';

export class AppClientsDto extends Model {
    constructor(
        public readonly clients: AppClientDto[],
        public readonly version: Version
    ) {
        super();
    }

    public with(value: Partial<AppClientsDto>): AppClientsDto {
        return this.clone(value);
    }
}

export class AppClientDto extends Model {
    constructor(
        public readonly id: string,
        public readonly name: string,
        public readonly secret: string,
        public readonly permission: string
    ) {
        super();
    }

    public with(value: Partial<AppClientDto>): AppClientDto {
        return this.clone(value);
    }
}

export class CreateAppClientDto {
    constructor(
        public readonly id: string
    ) {
    }
}

export class UpdateAppClientDto {
    constructor(
        public readonly name?: string,
        public readonly permission?: string
    ) {
    }
}

export class AccessTokenDto {
    constructor(
        public readonly accessToken: string,
        public readonly tokenType: string
    ) {
    }
}

@Injectable()
export class AppClientsService {
    constructor(
        private readonly http: HttpClient,
        private readonly apiUrl: ApiUrlConfig,
        private readonly analytics: AnalyticsService
    ) {
    }

    public getClients(appName: string): Observable<AppClientsDto> {
        const url = this.apiUrl.buildUrl(`api/apps/${appName}/clients`);

        return HTTP.getVersioned<any>(this.http, url).pipe(
                map(response => {
                    const body = response.payload.body;

                    const items: any[] = body;

                    const clients = items.map(item => {
                        return new AppClientDto(
                            item.id,
                            item.name || body.id,
                            item.secret,
                            item.permission);
                    });

                    return new AppClientsDto(clients, response.version);
                }),
                pretifyError('Failed to load clients. Please reload.'));
    }

    public postClient(appName: string, dto: CreateAppClientDto, version: Version): Observable<Versioned<AppClientDto>> {
        const url = this.apiUrl.buildUrl(`api/apps/${appName}/clients`);

        return HTTP.postVersioned<any>(this.http, url, dto, version).pipe(
                map(response => {
                    const body = response.payload.body;

                    const client = new AppClientDto(
                        body.id,
                        body.name || body.id,
                        body.secret,
                        body.permission);

                    return new Versioned(response.version, client);
                }),
                tap(() => {
                    this.analytics.trackEvent('Client', 'Created', appName);
                }),
                pretifyError('Failed to add client. Please reload.'));
    }

    public putClient(appName: string, id: string, dto: UpdateAppClientDto, version: Version): Observable<Versioned<any>> {
        const url = this.apiUrl.buildUrl(`api/apps/${appName}/clients/${id}`);

        return HTTP.putVersioned(this.http, url, dto, version).pipe(
                tap(() => {
                    this.analytics.trackEvent('Client', 'Updated', appName);
                }),
                pretifyError('Failed to revoke client. Please reload.'));
    }

    public deleteClient(appName: string, id: string, version: Version): Observable<Versioned<any>> {
        const url = this.apiUrl.buildUrl(`api/apps/${appName}/clients/${id}`);

        return HTTP.deleteVersioned(this.http, url, version).pipe(
                tap(() => {
                    this.analytics.trackEvent('Client', 'Deleted', appName);
                }),
                pretifyError('Failed to revoke client. Please reload.'));
    }

    public createToken(appName: string, client: AppClientDto): Observable<AccessTokenDto> {
        const options = {
            headers: new HttpHeaders({
                'Content-Type': 'application/x-www-form-urlencoded', 'NoAuth': 'true'
            })
        };

        const body = `grant_type=client_credentials&scope=squidex-api&client_id=${appName}:${client.id}&client_secret=${client.secret}`;

        const url = this.apiUrl.buildUrl('identity-server/connect/token');

        return this.http.post(url, body, options).pipe(
                map((response: any) => {
                    return new AccessTokenDto(response.access_token, response.token_type);
                }),
                pretifyError('Failed to create token. Please retry.'));
    }
}