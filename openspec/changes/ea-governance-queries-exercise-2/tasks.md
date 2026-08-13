# Tasks: EA Governance Queries — Exercise 2

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | 600–900 authored lines (SQL container + technical explainer + evidence index + AI usage log + optional Prolaborate doc; binary evidence files are excluded from authored-line count) |
| 400-line budget risk | High |
| 400-line forecast interpretation | Workload information only; it is not an Apply blocker |
| Suggested split | Incremental work units (see below); no commit or PR is required |
| Delivery strategy | ask-on-risk |
| Git authorization | Agents MUST NOT create commits or PRs unless the operator explicitly authorizes them |

Decision needed before apply: No
Commit/PR authorization: Required from operator; absent authorization, do not create either
400-line budget risk: High

### Suggested Work Units

| Unit | Goal | Focused test command | Runtime harness | Rollback boundary |
|------|------|----------------------|-----------------|-------------------|
| WU1 | Scaffolding | `git status --short` plus explicit inspection of each newly created file | None | Remove the scaffolding files and directories introduced in this unit |
| WU2 | Q1 implementation and diagnostics → Gate Q1 | `python -c "import sqlite3; conn=sqlite3.connect('file:C:/Proagile/Repositorio Pasantias.qea?mode=ro', uri=True); ..."` for Q1 | Human EA SQL Search execution of Q1-C and Q1-L against `Repositorio Pasantias.qea` | Remove Q1 SQL and evidence changes introduced in this unit |
| WU3 | Q2 implementation and diagnostics → Gate Q2 | Read-only SQLite diagnostic for Q2-G | Human EA SQL Search execution of Q2-G | Remove Q2 SQL and evidence changes introduced in this unit |
| WU4 | Q3 implementation and diagnostics → Gate Q3 | Read-only SQLite diagnostic for Q3-C and Q3-L | Human EA SQL Search execution of Q3-C and Q3-L | Remove Q3 SQL and evidence changes introduced in this unit |
| WU5 | Final mandatory documentation | Static markdown link check against relative paths in `docs/` and `evidence/` | Manual review of evidence entries and AI-log fields | Remove the technical explainer, AI usage log, and their index updates |
| WU6 | Optional Prolaborate | Prolaborate V5 chart widget execution if available | Prolaborate V5 chart widget execution | Remove optional Prolaborate documentation and evidence |
| WU7 | Pre-verify | `git diff --check` for tracked/visible diffs plus manual Phase 8 checklist | Final evidence and repository guard review | Revert only approved-scope delivery changes if verification fails |

## Phase 1: Scaffolding and Traceability

- [ ] 1.1 Create directory tree under `Exercise_2_queries/`: `queries/`, `docs/` (preserve existing `docs/challenge/`), `evidence/ea/`, `evidence/sqlite/`, `evidence/prolaborate/`, `prolaborate/`.
- [ ] 1.2 Create delivery container `Exercise_2_queries/queries/Pino_Exercise_2_EA_Governance_Queries.sql`.
- [ ] 1.3 Create `Exercise_2_queries/docs/Pino_Exercise_2_Technical_Explainer.md`.
- [ ] 1.4 Create `Exercise_2_queries/docs/Pino_Exercise_2_AI_Usage_Log.md`.
- [ ] 1.5 Create evidence manifest `Exercise_2_queries/evidence/Pino_Exercise_2_Evidence_Index.md` with PENDING entries for Q1-C, Q1-L, Q2-G, Q3-C, Q3-L; no fake PNG or executed claims.
- [ ] 1.6 Confirm `Exercise_2_queries/docs/challenge/Desafio_Tecnico_Practica_EA_Prolaborate_v2 (1).md` is preserved and untouched.
- [ ] 1.7 Run `git status --short` to identify untracked content and explicitly inspect every newly created file; run `git diff --check` for tracked/visible diffs only, because it does not check untracked content. Do not add, stage, commit, or create a PR. `git diff --cached --check` MAY be used only if the operator stages changes later.

## Phase 2: Q1 — Categoria ORO (Apply + Gate)

- [ ] 2.1 Implement Q1-C in `Pino_Exercise_2_EA_Governance_Queries.sql`: SELECT-only aggregate counting distinct `app.Object_ID` for applications with `tag.Property='Categoria'` and `tag.Value='ORO'`.
- [ ] 2.2 Implement Q1-L in the same SQL file: SELECT DISTINCT list with `app.ea_guid AS CLASSGUID`, `app.Object_Type AS CLASSTYPE`, `app.Name`, and Categoria value; no tag-row IDs or internal join demonstration columns.
- [ ] 2.3 Static review Q1: every block begins with `SELECT`; `Object_ID` is always qualified; aliases are present only in Q1-L; application scope uses `Object_Type='Class' AND Stereotype='ArchiMate_ApplicationComponent'`.
- [ ] 2.4 Execute read-only SQLite diagnostic for Q1-C against `C:\Proagile\Repositorio Pasantias.qea` using URI `mode=ro`; verify count = 8.
- [ ] 2.5 Execute read-only SQLite diagnostic for Q1-L; verify exact set {Aplicación 1, 2, 3, 4, 6, 8, 20, 29}.
- [ ] 2.6 Record Q1 SQLite diagnostics in `evidence/sqlite/Pino_Exercise_2_SQLite_Diagnostics.md` and update evidence index status to DIAGNOSTIC_OK.
- [ ] 2.7 **STOP — HUMAN GATE Q1**: operator copies Q1-C and Q1-L separately into EA SQL Search, runs each, validates count=8 and exact list, opens at least one list row to confirm navigation, compares visible Categoria tags, and captures real EA screenshot/export named with Pino and mapped to Q1-C/Q1-L in the evidence index. Primary EA execution plus oracle pass is PASS. If EA rejects a statement's dialect, record the real failure first; only then implement a basic read-only semantically identical fallback, rerun its SQLite `mode=ro` diagnostic, and rerun this same gate. Do not use a preemptive fallback or advance to WU3 until final EA PASS evidence exists for Q1-C and Q1-L.

## Phase 3: Q2 — Vigencia Distribution (Apply + Gate)

- [ ] 3.1 After Q1 gate approval, implement Q2-G in the SQL file: SELECT-only grouped count of distinct `app.Object_ID` by `tag.Value` where `tag.Property='Vigencia'` and value is `Vigente` or `Deprecado`.
- [ ] 3.2 Static review Q2: no reference to `app.Status`; no artificial NULL/empty/N/A/Unknown bucket; application scope enforced; grouped result has no `CLASSGUID`/`CLASSTYPE`.
- [ ] 3.3 Execute read-only SQLite diagnostic for Q2-G; verify Vigente=16, Deprecado=14, and zero NULL/empty/N/A application values.
- [ ] 3.4 Run conflicting-Vigencia anomaly query against applications; if any application has more than one distinct Vigencia value, document it and STOP Q2 acceptance.
- [ ] 3.5 Update SQLite diagnostics and evidence index with Q2 results.
- [ ] 3.6 **STOP — HUMAN GATE Q2**: operator runs Q2-G in EA SQL Search, validates Vigente=16 and Deprecado=14, compares visible Vigencia tags on sampled applications, confirms no synthetic buckets or invented aliases, and captures real EA evidence named with Pino. Primary EA execution plus oracle pass is PASS. If EA rejects the dialect, record the real failure first; only then implement a basic read-only semantically identical fallback, rerun its SQLite `mode=ro` diagnostic, and rerun this same gate. Do not use a preemptive fallback or advance to WU4 until final EA PASS evidence exists for Q2-G.

## Phase 4: Q3 — Base de Datos 28 Impact (Apply + Gate)

- [ ] 4.1 After Q2 gate approval, run read-only diagnostic to confirm semantic target predicate `Name='Base de Datos 28' AND Object_Type='Class' AND Stereotype='ArchiMate_DataObject'` resolves exactly one row; if zero or more than one, document anomaly and STOP Q3.
- [ ] 4.2 Implement Q3-C in the SQL file: SELECT-only aggregate counting distinct `src.Object_ID` where `rel.Connector_Type='Dependency'`, source is an application, and target matches the semantic predicate.
- [ ] 4.3 Implement Q3-L in the same SQL file: SELECT DISTINCT list with `src.ea_guid AS CLASSGUID`, `src.Object_Type AS CLASSTYPE`, `src.Name`, and useful target identity; no connector IDs or internal join demonstration columns.
- [ ] 4.4 Static review Q3: `Start_Object_ID` = source, `End_Object_ID` = target; no `Direction` predicate; source scope is application only; target identified by name+type+stereotype, never by `Object_ID=102`.
- [ ] 4.5 Execute read-only SQLite diagnostic for Q3-C; verify count = 5.
- [ ] 4.6 Execute read-only SQLite diagnostic for Q3-L; verify exact set {Aplicación 5, 12, 22, 25, 27}.
- [ ] 4.7 Optional diagnostic: list non-application sources (Node control) for Base de Datos 28; confirm Servidor 2/5/19; record as negative-control evidence only, not as a final SELECT.
- [ ] 4.8 Update SQLite diagnostics and evidence index with Q3 results.
- [ ] 4.9 **STOP — HUMAN GATE Q3**: operator runs Q3-C and Q3-L in EA SQL Search, validates count=5 and exact list, confirms dependency direction (Start→End), verifies Nodes are absent from results, tests navigation from a list row, and captures real EA evidence named with Pino. Primary EA execution plus oracle pass is PASS. If EA rejects a statement's dialect, record the real failure first; only then implement a basic read-only semantically identical fallback, rerun its SQLite `mode=ro` diagnostic, and rerun this same gate. Do not use a preemptive fallback or advance to Phase 5 until final EA PASS evidence exists for Q3-C and Q3-L.

## Phase 5: Post-Gate Compatibility Audit

- [ ] 5.1 After all three gates pass, audit and record the EA-accepted form for each of Q1-C, Q1-L, Q2-G, Q3-C, and Q3-L, including whether the primary or a gate-local fallback passed.
- [ ] 5.2 Record each documented dialect rejection, fallback rationale, and final accepted fallback form in the evidence index and technical explainer; record that no fallback was used where primary execution passed.
- [ ] 5.3 Confirm every fallback was introduced only after its real EA rejection and was revalidated by read-only SQLite diagnostics and its corresponding human gate.
- [ ] 5.4 Do not rerun any of the five statements during this audit when final gate evidence remains valid; rerun only the affected statement's SQLite diagnostic and same gate if a later change invalidates its final evidence.
- [ ] 5.5 Confirm no preemptive compatibility fallback remains and that the accepted-form record maps to final real EA evidence.

## Phase 6: Final Mandatory Documentation

- [ ] 6.1 Write `Exercise_2_queries/docs/Pino_Exercise_2_Technical_Explainer.md` covering: tables used, joins, filters, application criteria, tag interpretation, uniqueness and DISTINCT rationale, semantic target identification, Node negative-control evidence, EA vs SQLite roles, oracles, execution steps, and any compatibility fallback.
- [ ] 6.2 Finalize `Exercise_2_queries/evidence/Pino_Exercise_2_Evidence_Index.md` as a manifest mapping each mandatory statement (Q1-C, Q1-L, Q2-G, Q3-C, Q3-L) to SQL file path, EA Search display name, expected oracle, actual EA result, oracle confirmation, visible model check, real evidence filename, operator/date, updated index status, and PASS-equivalent outcome. After the three gates, PENDING is invalid for every mandatory statement.
- [ ] 6.3 Write `Exercise_2_queries/docs/Pino_Exercise_2_AI_Usage_Log.md` with at least five real interactions; each interaction must contain: ID, objective, tool, model, strategy/prompt, decision from response, related evidence, outcome. Record the Tasks interaction actually used as Kimi K2.7 Code because Qwen was unavailable; do not report Qwen as used. Retrospective entries are allowed only when all eight fields are faithfully recoverable.
- [ ] 6.4 Verify every submission-facing supplemental evidence artifact filename contains `Pino` and is indexed with its statement ID.
- [ ] 6.5 Verify no placeholder is described as executed evidence or artificial proof; PENDING is permitted only for optional non-blocking work such as Prolaborate, never for a mandatory statement after its gate.

## Phase 7: Optional Prolaborate

- [ ] 7.1 After Phase 6 and only if Prolaborate V5 access is available, begin with EA-validated Q2-G.
- [ ] 7.2 In Prolaborate: Create Chart Widget → SQL Queries → Skip to Query → Query Configuration; use View Sample when useful; execute the query before configuring chart settings.
- [ ] 7.3 Reuse Q2-G unchanged if it executes and renders correctly. If presentation aliases or shape are needed, create a minimally adapted derived SQL query that preserves exact Q2-G semantics and results; document the adaptation.
- [ ] 7.4 Do not introduce `seriesproperty` unless tagged-value color-palette requirements are evidenced.
- [ ] 7.5 Create `Exercise_2_queries/prolaborate/Pino_Exercise_2_Prolaborate_Q2.md` documenting chart configuration, business question, and result mapping.
- [ ] 7.6 Capture real Prolaborate chart/dashboard evidence to `Exercise_2_queries/evidence/prolaborate/Pino_Exercise_2_Prolaborate_Q2.png` (or export) and index it.
- [ ] 7.7 If Prolaborate is unavailable, record absence as non-blocking in the evidence index and technical explainer.

## Phase 8: Pre-Verify and Repository Guard

- [ ] 8.1 Verify `Pino_Exercise_2_EA_Governance_Queries.sql` contains exactly five SELECT statements and no mutating operations.
- [ ] 8.2 Verify every executable SELECT block is semantically read-only.
- [ ] 8.3 Verify protected areas are untouched: `Exercise_2_queries/docs/challenge/`, `Addino/`, Exercise 1 artifacts, `C:\Proagile\Repositorio Pasantias.qea`, RDD/config artifacts, and other OpenSpec files not belonging to this change.
- [ ] 8.4 Verify all final delivery filenames contain `Pino` (OpenSpec artifacts exempt).
- [ ] 8.5 Verify traceability: each mandatory statement → SQL block → evidence index → actual EA result → oracle confirmation → real EA evidence → PASS-equivalent outcome. Block and fail pre-verify if any mandatory statement is PENDING or lacks actual EA evidence; PENDING is permitted only for optional non-blocking work.
- [ ] 8.6 Verify oracles are documented as validation evidence, not hardcoded query logic.
- [ ] 8.7 Verify relative links in docs are valid and no placeholder paths are presented as real evidence.
- [ ] 8.8 Run `git status --short` to identify untracked content and explicitly inspect every newly created file; run `git diff --check` for tracked/visible diffs only, because it excludes untracked content. Do not add or stage changes to test whitespace; `git diff --cached --check` MAY be used only if the operator stages changes later. Confirm no unexpected changes outside approved scope and do not commit or create a PR without operator authorization.
