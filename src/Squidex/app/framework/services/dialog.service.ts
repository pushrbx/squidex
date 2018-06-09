/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { Injectable } from '@angular/core';
import { Observable, Subject, throwError } from 'rxjs';

import { ErrorDto } from './../utils/error';
import { Types } from './../utils/types';

export const DialogServiceFactory = () => {
    return new DialogService();
};

export class DialogRequest {
    private readonly resultStream$ = new Subject<boolean>();

    public get closed(): Observable<boolean> {
        return this.resultStream$;
    }

    constructor(
        public readonly title: string,
        public readonly text: string
    ) {
    }

    public complete(value: boolean) {
        this.resultStream$.next(value);
        this.resultStream$.complete();
    }
}

export class Notification {
    constructor(
        public readonly message: string,
        public readonly messageType: string,
        public readonly displayTime: number = 5000
    ) {
    }

    public static error(message: string): Notification {
        return new Notification(message, 'danger');
    }

    public static info(message: string): Notification {
        return new Notification(message, 'info');
    }
}

@Injectable()
export class DialogService {
    private readonly requestStream$ = new Subject<DialogRequest>();
    private readonly notificationsStream$ = new Subject<Notification>();

    public get dialogs(): Observable<DialogRequest> {
        return this.requestStream$;
    }

    public get notifications(): Observable<Notification> {
        return this.notificationsStream$;
    }

    public notifyError(error: string | ErrorDto) {
        if (Types.is(error, ErrorDto)) {
            this.notify(Notification.error(error.displayMessage));
        } else {
            this.notify(Notification.error(error));
        }

        return throwError(error);
    }

    public notifyInfo(text: string) {
        this.notificationsStream$.next(Notification.info(text));
    }

    public notify(notification: Notification) {
        this.notificationsStream$.next(notification);
    }

    public confirmUnsavedChanges(): Observable<boolean> {
        return this.confirm('Unsaved changes', 'You have unsaved changes, do you want to close the current content view?');
    }

    public confirm(title: string, text: string): Observable<boolean> {
        const request = new DialogRequest(title, text);

        this.requestStream$.next(request);

        return request.closed;
    }
}