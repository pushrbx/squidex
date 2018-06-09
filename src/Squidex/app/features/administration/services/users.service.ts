/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import {
    ApiUrlConfig,
    HTTP,
    Model,
    pretifyError
} from '@app/shared';

export class UsersDto extends Model {
    constructor(
        public readonly total: number,
        public readonly items: UserDto[]
    ) {
        super();
    }

    public with(value: Partial<UsersDto>): UsersDto {
        return this.clone(value);
    }
}

export class UserDto extends Model {
    constructor(
        public readonly id: string,
        public readonly email: string,
        public readonly displayName: string,
        public readonly isLocked: boolean
    ) {
        super();
    }

    public with(value: Partial<UserDto>): UserDto {
        return this.clone(value);
    }
}

export class CreateUserDto {
    constructor(
        public readonly email: string,
        public readonly displayName: string,
        public readonly password: string
    ) {
    }
}

export class UpdateUserDto {
    constructor(
        public readonly email: string,
        public readonly displayName: string,
        public readonly password?: string
    ) {
    }
}

@Injectable()
export class UsersService {
    constructor(
        private readonly http: HttpClient,
        private readonly apiUrl: ApiUrlConfig
    ) {
    }

    public getUsers(take: number, skip: number, query?: string): Observable<UsersDto> {
        const url = this.apiUrl.buildUrl(`api/user-management?take=${take}&skip=${skip}&query=${query || ''}`);

        return HTTP.getVersioned<any>(this.http, url).pipe(
                map(response => {
                    const body = response.payload.body;

                    const items: any[] = body.items;

                    const users = items.map(item => {
                        return new UserDto(
                            item.id,
                            item.email,
                            item.displayName,
                            item.isLocked);
                    });

                    return new UsersDto(body.total, users);
                }),
                pretifyError('Failed to load users. Please reload.'));
    }

    public getUser(id: string): Observable<UserDto> {
        const url = this.apiUrl.buildUrl(`api/user-management/${id}`);

        return HTTP.getVersioned<any>(this.http, url).pipe(
                map(response => {
                    const body = response.payload.body;

                    return new UserDto(
                        body.id,
                        body.email,
                        body.displayName,
                        body.isLocked);
                }),
                pretifyError('Failed to load user. Please reload.'));
    }

    public postUser(dto: CreateUserDto): Observable<UserDto> {
        const url = this.apiUrl.buildUrl('api/user-management');

        return HTTP.postVersioned<any>(this.http, url, dto).pipe(
                map(response => {
                    const body = response.payload.body;

                    return new UserDto(
                        body.id,
                        dto.email,
                        dto.displayName,
                        false);
                }),
                pretifyError('Failed to create user. Please reload.'));
    }

    public putUser(id: string, dto: UpdateUserDto): Observable<any> {
        const url = this.apiUrl.buildUrl(`api/user-management/${id}`);

        return HTTP.putVersioned(this.http, url, dto).pipe(
                pretifyError('Failed to update user. Please reload.'));
    }

    public lockUser(id: string): Observable<any> {
        const url = this.apiUrl.buildUrl(`api/user-management/${id}/lock`);

        return HTTP.putVersioned(this.http, url, {}).pipe(
                pretifyError('Failed to load users. Please retry.'));
    }

    public unlockUser(id: string): Observable<any> {
        const url = this.apiUrl.buildUrl(`api/user-management/${id}/unlock`);

        return HTTP.putVersioned(this.http, url, {}).pipe(
                pretifyError('Failed to load users. Please retry.'));
    }
}