# Proposal: EA Governance Queries — Exercise 2

## Intent

Deliver a SELECT-only EA SQL Search query set covering the three mandatory governance questions, with execution evidence, while preserving the QEA repository and Exercise 1 history. One or more SELECT statements may be used per question where needed for clarity, compatibility, reuse, or traceability; do not decide the final statement count now. Every final statement must be clearly identified with its business question.

## Scope

### In Scope
- Identify applications with `Object_Type='Class' AND Stereotype='ArchiMate_ApplicationComponent'` (30 observed); report mandatory ORO count and list (8 expected).
- Report `Vigencia` tags: `Vigente` (16 expected) and `Deprecado` (14 expected); never use `t_object.Status` or artificial buckets.
- Report Dependency sources affecting semantic target `Base de Datos 28`: total `5` and named list `Aplicación 5`, `Aplicación 12`, `Aplicación 22`, `Aplicación 25`, and `Aplicación 27`. Nodes remain negative-filter evidence only.
- Produce an identified SQL file, technical explanation, EA evidence, and an AI log with at least five real, meaningful interactions. Each interaction must include an identifier, objective, tool, model, strategy/prompt, decision from the response, related evidence (file, test, screenshot, or commit when applicable), and outcome. Do not fabricate or reconstruct interactions.

### Out of Scope
- QEA mutations; Addino; Exercise 1 artifacts; RDD/review mode.
- Prolaborate before mandatory EA validation, except for the optional follow-on scope defined below.

## Validation Baselines

All values are validation baselines for the supplied QEA only, not query constants. Final queries MUST derive results from repository tables, relationships, and tagged values and MUST NOT hardcode values as query logic.

- Q1: ORO total `8`; Apps `1`, `2`, `3`, `4`, `6`, `8`, `20`, and `29`.
- Q2: `Vigente` `16`; `Deprecado` `14`.
- Q3: impact total `5`; Apps `5`, `12`, `22`, `25`, and `27`.

## Capabilities

### New Capabilities
- `application-category-governance`: ORO count and list from application Categoria tags.
- `application-lifecycle-governance`: Vigente and Deprecado totals from application Vigencia tags.
- `application-database-impact-analysis`: Applications with Dependency impact on Base de Datos 28.

### Modified Capabilities
None. Historical `ea-metadata-review` remains untouched.

## Approach

Use a SELECT-only query set in EA SQL Search as the primary execution and evidence source. EA SQL Search execution, screenshots, or export is mandatory primary evidence. SQLite `mode=ro` is allowed as secondary diagnostic validation and technical evidence but MUST NOT replace or satisfy mandatory EA evidence. Join tags by `Object_ID`; use tags and never `t_object.Status`. Impact uses Start source, End target, Dependency, a defensive application source predicate, and semantic target `Base de Datos 28` identified by Name + Class + DataObject stereotype; Object_ID `102` validates only. Start/End and Dependency are required; Direction is optional validation only. Use conditional aliases where compatibility requires them. Nodes are negative-filter evidence only. Exclude Addino/E1. Individual results SHOULD expose `ea_guid AS CLASSGUID` and `Object_Type AS CLASSTYPE` when applicable; aggregates omit them. Order, identify, comment where useful or appropriate, and keep reusable queries re-executable. SDD agents MUST NOT create commits automatically unless explicitly authorized by the operator. Final delivery file names must include the surname `Pino`; internal OpenSpec artifacts are exempt. Final artifacts use relative paths; do not choose final names now.

### Optional Follow-On Scope

After EA implementation and validation of the mandatory three questions, optionally reuse at least one validated query in Prolaborate, create a suitable donut chart, bar chart, or dashboard, document its configuration and business question, and capture visual evidence. Inability to complete this optional follow-on does not block mandatory acceptance.

## Affected Areas

| Area | Impact | Description |
|---|---|---|
| `Exercise_2_queries/` | Modified/Additions | This pre-existing area receives added deliverables (SQL, explanation, EA evidence, and AI log) without altering or removing reference or challenge material. |
| `openspec/changes/ea-governance-queries-exercise-2/` | Modified | Planning artifacts for this change. |

## Risks

| Risk | Likelihood | Mitigation |
|---|---|---|
| SQL works diagnostically but not in EA | Med | Execute and capture all final queries in EA SQL Search. |
| Non-app sources pollute impact results | Med | Enforce source application predicate; record Nodes separately. |
| EA evidence unavailable | Med | Obtain human EA execution/screenshots before completion. |

## Rollback Plan

Remove or revert only artifacts introduced by this change; no data rollback is needed because queries are read-only. Preserve `Exercise_2_queries/docs/challenge/`, all other pre-existing material, Addino/E1, and QEA.

## Dependencies

- Human access to EA SQL Search for execution/screenshots; optional later Prolaborate access.

## Success Criteria

- [ ] The SELECT-only query set correctly answers all three mandatory governance questions in EA SQL Search, with real EA SQL Search primary screenshots or export evidence.
- [ ] The impact result reports total `5` and the named applications `Aplicación 5`, `Aplicación 12`, `Aplicación 22`, `Aplicación 25`, and `Aplicación 27`; Nodes are retained only as negative-filter evidence.
- [ ] Technical documentation identifies tables, fields, joins, filters, and assumptions for every final statement.
- [ ] Queries are ordered, identified with their business question, commented where useful or appropriate, reusable, and re-executable; the final statement count is determined only when implementation needs it.
- [ ] Individual traceability results expose EA elements with `CLASSGUID` and `CLASSTYPE` when applicable.
- [ ] Deliverables include the SQL file, technical explanation, and an AI log with at least five real meaningful interactions, each with its identifier, objective, tool, model, strategy/prompt, response decision, related evidence when applicable, and outcome.
- [ ] QEA remains read-only. If performed, Prolaborate reuse is correct and documented with configuration, business question, and visual evidence; it is optional and does not block mandatory acceptance.
- [ ] All final delivery file names include the surname `Pino`; internal OpenSpec artifacts are exempt.
- [ ] No QEA mutation or changes outside the approved Exercise 2/change-artifact scope occur.
- [ ] Future Spec uses `### Requirement:` or `### REQ-n:` and `#### Scenario:` headings.
