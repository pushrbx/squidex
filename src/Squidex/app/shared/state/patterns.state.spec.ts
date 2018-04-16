/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { Observable } from 'rxjs';
import { IMock, Mock } from 'typemoq';

import {
    AppsState,
    AppPatternDto,
    AppPatternsDto,
    AppPatternsService,
    PatternsState,
    DialogService,
    EditAppPatternDto,
    Version,
    Versioned
 } from '@app/shared';

describe('PatternsState', () => {
    const app = 'my-app';
    const version = new Version('1');
    const newVersion = new Version('2');

    const oldPatterns = [
        new AppPatternDto('id1', 'name1', 'pattern1', ''),
        new AppPatternDto('id2', 'name2', 'pattern2', '')
    ];

    let dialogs: IMock<DialogService>;
    let appsState: IMock<AppsState>;
    let patternsService: IMock<AppPatternsService>;
    let patternsState: PatternsState;

    beforeEach(() => {
        dialogs = Mock.ofType<DialogService>();

        appsState = Mock.ofType<AppsState>();

        appsState.setup(x => x.appName)
            .returns(() => app);

        patternsService = Mock.ofType<AppPatternsService>();

        patternsService.setup(x => x.getPatterns(app))
            .returns(() => Observable.of(new AppPatternsDto(oldPatterns, version)));

        patternsState = new PatternsState(patternsService.object, appsState.object, dialogs.object);
        patternsState.load().subscribe();
    });

    it('should load patterns', () => {
        expect(patternsState.snapshot.patterns.values).toEqual(oldPatterns);
        expect(patternsState.snapshot.version).toEqual(version);
    });

    it('should add pattern to snapshot when created', () => {
        const newPattern = new AppPatternDto('id3', 'name3', 'pattern3', '');

        const request = new EditAppPatternDto('name3', 'pattern3', '');

        patternsService.setup(x => x.postPattern(app, request, version))
            .returns(() => Observable.of(new Versioned<AppPatternDto>(newVersion, newPattern)));

        patternsState.create(request).subscribe();

        expect(patternsState.snapshot.patterns.values).toEqual([...oldPatterns, newPattern]);
        expect(patternsState.snapshot.version).toEqual(newVersion);
    });

    it('should update properties when updated', () => {
        const request = new EditAppPatternDto('a_name2', 'a_pattern2', 'a_message2');

        patternsService.setup(x => x.putPattern(app, oldPatterns[1].id, request, version))
            .returns(() => Observable.of(new Versioned<any>(newVersion, {})));

        patternsState.update(oldPatterns[1], request).subscribe();

        const pattern_1 = patternsState.snapshot.patterns.at(0);

        expect(pattern_1.name).toBe('a_name2');
        expect(pattern_1.pattern).toBe('a_pattern2');
        expect(pattern_1.message).toBe('a_message2');
        expect(patternsState.snapshot.version).toEqual(newVersion);
    });

    it('should remove pattern from snapshot when deleted', () => {
        patternsService.setup(x => x.deletePattern(app, oldPatterns[0].id, version))
            .returns(() => Observable.of(new Versioned<any>(newVersion, {})));

        patternsState.delete(oldPatterns[0]).subscribe();

        expect(patternsState.snapshot.patterns.values).toEqual([oldPatterns[1]]);
        expect(patternsState.snapshot.version).toEqual(newVersion);
    });
});