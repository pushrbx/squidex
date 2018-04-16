/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Sebastian Stehle. All rights r vbeserved
 */

import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import {
    DialogRequest,
    DialogService,
    fadeAnimation,
    ModalView,
    Notification
} from '@app/framework/internal';

@Component({
    selector: 'sqx-dialog-renderer',
    styleUrls: ['./dialog-renderer.component.scss'],
    templateUrl: './dialog-renderer.component.html',
    animations: [
        fadeAnimation
    ]
})
export class DialogRendererComponent implements OnDestroy, OnInit {
    private dialogSubscription: Subscription;
    private dialogsSubscription: Subscription;
    private notificationsSubscription: Subscription;

    public dialogView = new ModalView(false, true);
    public dialogRequest: DialogRequest | null = null;

    public notifications: Notification[] = [];

    @Input()
    public position = 'bottomright';

    constructor(
        private readonly dialogs: DialogService
    ) {
    }

    public ngOnDestroy() {
        this.notificationsSubscription.unsubscribe();
        this.dialogSubscription.unsubscribe();
        this.dialogsSubscription.unsubscribe();
    }

    public ngOnInit() {
        this.dialogSubscription =
            this.dialogView.isOpen.subscribe(isOpen => {
                if (!isOpen) {
                    this.cancel();
                }
            });

        this.notificationsSubscription =
            this.dialogs.notifications.subscribe(notification => {
                this.notifications.push(notification);

                if (notification.displayTime > 0) {
                    setTimeout(() => {
                        this.close(notification);
                    }, notification.displayTime);
                }
            });

        this.dialogsSubscription =
            this.dialogs.dialogs
                .subscribe(request => {
                    this.cancel();

                    this.dialogRequest = request;
                    this.dialogView.show();
                });
    }

    public cancel() {
        if (this.dialogRequest) {
            this.dialogRequest.complete(false);
            this.dialogRequest = null;
            this.dialogView.hide();
        }
    }

    public confirm() {
        if (this.dialogRequest) {
            this.dialogRequest.complete(true);
            this.dialogRequest = null;
            this.dialogView.hide();
        }
    }

    public close(notification: Notification) {
        this.notifications.splice(this.notifications.indexOf(notification), 1);
    }
}