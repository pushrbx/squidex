/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import '@app/framework/utils/rxjs-extensions';

import {
    DialogService,
    ImmutableArray,
    State
} from '@app/shared';

import { EventConsumerDto, EventConsumersService } from './../services/event-consumers.service';

interface Snapshot {
    eventConsumers: ImmutableArray<EventConsumerDto>;
}

@Injectable()
export class EventConsumersState extends State<Snapshot> {
    public eventConsumers =
        this.changes.map(x => x.eventConsumers);

    constructor(
        private readonly dialogs: DialogService,
        private readonly eventConsumersService: EventConsumersService
    ) {
        super({ eventConsumers: ImmutableArray.empty() });
    }

    public load(notifyLoad = false, notifyError = false): Observable<any> {
        return this.eventConsumersService.getEventConsumers()
            .do(dtos => {
                if (notifyLoad) {
                    this.dialogs.notifyInfo('Event Consumers reloaded.');
                }

                this.next(s => {
                    const eventConsumers = ImmutableArray.of(dtos);

                    return { ...s, eventConsumers };
                });
            })
            .catch(error => {
                if (notifyError) {
                    this.dialogs.notifyError(error);
                }

                return Observable.throw(error);
            });
    }

    public start(es: EventConsumerDto): Observable<any> {
        return this.eventConsumersService.putStart(es.name)
            .do(() => {
                this.replaceEventConsumer(start(es));
            })
            .notify(this.dialogs);
    }

    public stop(es: EventConsumerDto): Observable<any> {
        return this.eventConsumersService.putStop(es.name)
            .do(() => {
                this.replaceEventConsumer(stop(es));
            })
            .notify(this.dialogs);
    }

    public reset(es: EventConsumerDto): Observable<any> {
        return this.eventConsumersService.putReset(es.name)
            .do(() => {
                this.replaceEventConsumer(reset(es));
            })
            .notify(this.dialogs);
    }

    private replaceEventConsumer(es: EventConsumerDto) {
        this.next(s => {
            const eventConsumers = s.eventConsumers.replaceBy('name', es);

            return { ...s, eventConsumers };
        });
    }
}

const start = (es: EventConsumerDto) =>
    new EventConsumerDto(es.name, false, false, es.error, es.position);

const stop = (es: EventConsumerDto) =>
    new EventConsumerDto(es.name, true, false, es.error, es.position);

const reset = (es: EventConsumerDto) =>
    new EventConsumerDto(es.name, es.isStopped, true, es.error, es.position);