/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { Component, OnInit } from '@angular/core';
import { onErrorResumeNext } from 'rxjs/operators';

import {
    AppsState,
    ModalView,
    ruleActions,
    RuleDto,
    RulesState,
    ruleTriggers,
    SchemasState
} from '@app/shared';

@Component({
    selector: 'sqx-rules-page',
    styleUrls: ['./rules-page.component.scss'],
    templateUrl: './rules-page.component.html'
})
export class RulesPageComponent implements OnInit {
    public ruleActions = ruleActions;
    public ruleTriggers = ruleTriggers;

    public addRuleDialog = new ModalView();

    public wizardMode = 'Wizard';
    public wizardRule: RuleDto | null;

    constructor(
        public readonly appsState: AppsState,
        public readonly rulesState: RulesState,
        public readonly schemasState: SchemasState
    ) {
    }

    public ngOnInit() {
        this.schemasState.load().pipe(onErrorResumeNext()).subscribe();

        this.rulesState.load().pipe(onErrorResumeNext()).subscribe();
    }

    public reload() {
        this.rulesState.load(true).pipe(onErrorResumeNext()).subscribe();
    }

    public delete(rule: RuleDto) {
        this.rulesState.delete(rule).pipe(onErrorResumeNext()).subscribe();
    }

    public toggle(rule: RuleDto) {
        if (rule.isEnabled) {
            this.rulesState.disable(rule).pipe(onErrorResumeNext()).subscribe();
        } else {
            this.rulesState.enable(rule).pipe(onErrorResumeNext()).subscribe();
        }
    }

    public createNew() {
        this.wizardMode = 'Wizard';
        this.wizardRule = null;

        this.addRuleDialog.show();
    }

    public editTrigger(rule: RuleDto) {
        this.wizardMode = 'EditTrigger';
        this.wizardRule = rule;

        this.addRuleDialog.show();
    }

    public editAction(rule: RuleDto) {
        this.wizardMode = 'EditAction';
        this.wizardRule = rule;

        this.addRuleDialog.show();
    }

    public trackByRule(index: number, rule: RuleDto) {
        return rule.id;
    }
}
