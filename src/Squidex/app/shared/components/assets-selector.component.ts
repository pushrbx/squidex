/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

// tslint:disable:prefer-for-of

import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';

import {
    AssetDto,
    AssetsDialogState,
    fadeAnimation
} from '@app/shared/internal';

@Component({
    selector: 'sqx-assets-selector',
    styleUrls: ['./assets-selector.component.scss'],
    templateUrl: './assets-selector.component.html',
    animations: [
        fadeAnimation
    ]
})
export class AssetsSelectorComponent implements OnInit {
    public selectedAssets: { [id: string]: AssetDto } = {};
    public selectionCount = 0;

    @Output()
    public selected = new EventEmitter<AssetDto[]>();

    public assetsFilter = new FormControl('');

    constructor(
        public readonly state: AssetsDialogState
    ) {
    }

    public ngOnInit() {
        this.state.load(false, true).onErrorResumeNext().subscribe();

        this.assetsFilter.setValue(this.state.snapshot.assetsQuery);
    }

    public search() {
        this.state.search(this.assetsFilter.value).onErrorResumeNext().subscribe();
    }

    public complete() {
        this.selected.emit([]);
    }

    public select() {
        this.selected.emit(Object.values(this.selectedAssets));
    }

    public onAssetSelected(asset: AssetDto) {
        if (this.selectedAssets[asset.id]) {
            delete this.selectedAssets[asset.id];
        } else {
            this.selectedAssets[asset.id] = asset;
        }

        this.selectionCount = Object.keys(this.selectedAssets).length;
    }
}

