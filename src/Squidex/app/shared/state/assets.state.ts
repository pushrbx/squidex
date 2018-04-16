/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { Injectable } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';

import '@app/framework/utils/rxjs-extensions';

import {
    DialogService,
    ImmutableArray,
    Pager,
    Form,
    State
} from '@app/framework';

import { AppsState } from './apps.state';
import { AssetDto, AssetsService} from './../services/assets.service';

export class RenameAssetForm extends Form<FormGroup> {
    constructor(formBuilder: FormBuilder) {
        super(formBuilder.group({
            name: ['',
                [
                    Validators.required
                ]
            ]
        }));
    }
}

interface Snapshot {
    assets: ImmutableArray<AssetDto>;
    assetsPager: Pager;
    assetsQuery?: string;

    loaded: false;
}

@Injectable()
export class AssetsState extends State<Snapshot> {
    public assets =
        this.changes.map(x => x.assets)
            .distinctUntilChanged();

    public assetsPager =
        this.changes.map(x => x.assetsPager)
            .distinctUntilChanged();

    constructor(
        private readonly appsState: AppsState,
        private readonly assetsService: AssetsService,
        private readonly dialogs: DialogService
    ) {
        super({ assets: ImmutableArray.empty(), assetsPager: new Pager(0, 0, 30), loaded: false });
    }

    public load(notifyLoad = false, noReload = false): Observable<any> {
        if (this.snapshot.loaded && noReload) {
            return Observable.of({});
        }

        return this.assetsService.getAssets(this.appName, this.snapshot.assetsPager.pageSize, this.snapshot.assetsPager.skip, this.snapshot.assetsQuery)
            .do(dtos => {
                if (notifyLoad) {
                    this.dialogs.notifyInfo('Assets reloaded.');
                }

                this.next(s => {
                    const assets = ImmutableArray.of(dtos.items);
                    const assetsPager = s.assetsPager.setCount(dtos.total);

                    return { ...s, assets, assetsPager, loaded: true };
                });
            })
            .notify(this.dialogs);
    }

    public add(asset: AssetDto) {
        this.next(s => {
            const assets = s.assets.pushFront(asset);
            const assetsPager = s.assetsPager.incrementCount();

            return { ...s, assets, assetsPager };
        });
    }

    public update(asset: AssetDto) {
        this.next(s => {
            const assets = s.assets.replaceBy('id', asset);

            return { ...s, assets };
        });
    }

    public delete(asset: AssetDto): Observable<any> {
        return this.assetsService.deleteAsset(this.appName, asset.id, asset.version)
            .do(dto => {
                return this.next(s => {
                    const assets = s.assets.filter(x => x.id !== asset.id);
                    const assetsPager = s.assetsPager.decrementCount();

                    return { ...s, assets, assetsPager };
                });
            })
            .notify(this.dialogs);
    }

    public search(query: string): Observable<any> {
        this.next(s => ({ ...s, assetsPager: new Pager(0, 0, 30), assetsQuery: query }));

        return this.load();
    }

    public goNext(): Observable<any> {
        this.next(s => ({ ...s, assetsPager: s.assetsPager.goNext() }));

        return this.load();
    }

    public goPrev(): Observable<any> {
        this.next(s => ({ ...s, assetsPager: s.assetsPager.goPrev() }));

        return this.load();
    }

    private get appName() {
        return this.appsState.appName;
    }
}

@Injectable()
export class AssetsDialogState extends AssetsState { }