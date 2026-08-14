# Evidence Index — Pino Exercise 2

Evidence manifest and question → statement → result → model traceability for the EA Governance Queries exercise.

## Legend

| Status | Meaning |
|---|---|
| PENDING | Statement not yet implemented or not yet validated. Permitted only for optional non-blocking work. |
| DIAGNOSTIC_OK | Read-only SQLite diagnostic matches the supplied-QEA oracle; EA SQL Search execution is still pending. |
| EA_PASS | Human EA SQL Search execution matched the oracle and visible model; real EA evidence captured. |
| FAIL | Recorded failure or anomaly; blocked pending resolution. |

## Statement inventory

| ID | Business question | SQL file | EA Search display name | Expected oracle | Actual EA result | Oracle confirmation | Visible model check | Real evidence filename | Operator / date | Index status |
|---|---|---|---|---|---|---|---|---|---|---|
| Q1-C | How many applications have Categoria = ORO? | `../queries/Pino_Exercise_2_EA_Governance_Queries.sql` | Q1-C — Categoria ORO — Count | 8 | 8 | Yes | Categoria ORO tags compared; no non-application elements | `Exercise_2_queries/Pino_Exercise_2_Informe_Principal_Queries_EA.pdf` (real EA PDF evidence, Q1-C section) | Operator / 2026-08-13 | EA_PASS |
| Q1-L | Which applications have Categoria = ORO? | `../queries/Pino_Exercise_2_EA_Governance_Queries.sql` | Q1-L — Categoria ORO — List | Aplicación 1, 2, 3, 4, 6, 8, 20, 29 | Aplicación 1, 2, 3, 4, 6, 8, 20, 29 | Yes | All rows are applications; `CLASSGUID`/`CLASSTYPE` navigation verified | `Exercise_2_queries/Pino_Exercise_2_Informe_Principal_Queries_EA.pdf` (real EA PDF evidence, Q1-L section) | Operator / 2026-08-13 | EA_PASS |
| Q2-G | How many applications are Vigente / Deprecado? | `../queries/Pino_Exercise_2_EA_Governance_Queries.sql` | Q2-G — Vigencia — Grouped | Vigente=16, Deprecado=14 | Vigente=16, Deprecado=14 | Yes | Visible `Vigencia` tags sampled; no synthetic buckets or aliases | `Exercise_2_queries/Pino_Exercise_2_Informe_Principal_Queries_EA.pdf` (real EA PDF evidence, Q2-G section) | Operator / 2026-08-13 | EA_PASS |
| Q3-C | How many applications depend on Base de Datos 28? | `../queries/Pino_Exercise_2_EA_Governance_Queries.sql` | Q3-C — DB28 Impact — Count | 5 | 5 | Yes | Dependency Start→End verified; Nodes absent | `Exercise_2_queries/Pino_Evidencias_Funcionales_Queries.pdf` (real EA PDF evidence, Q3-C section) | Operator / 2026-08-14 | EA_PASS |
| Q3-L | Which applications depend on Base de Datos 28? | `../queries/Pino_Exercise_2_EA_Governance_Queries.sql` | Q3-L — DB28 Impact — List | Aplicación 5, 12, 22, 25, 27 | Aplicación 5, 12, 22, 25, 27 | Yes | Source applications only; navigation from list row (Aplicación 22 and related links) verified | `Exercise_2_queries/Pino_Evidencias_Funcionales_Queries.pdf` (real EA PDF evidence, Q3-L section) | Operator / 2026-08-14 | EA_PASS |

## Q1 — Categoria ORO

### Q1-C human gate evidence

- **Date:** 2026-08-13 (operator-reported)
- **Surface:** Enterprise Architect SQL Search, primary execution
- **Result:** `COUNT(DISTINCT app.Object_ID)` = **8**; oracle match confirmed by operator
- **Oracle:** 8
- **Match:** Yes
- **Visible model check:** Categoria ORO tags compared; no non-application elements included
- **Navigation check:** Not applicable for aggregate row
- **Real EA evidence:** `Exercise_2_queries/Pino_Exercise_2_Informe_Principal_Queries_EA.pdf` — real operator-provided EA PDF evidence; Q1-C result is captured in this document.
- **EA accepted form:** EA primary form accepted — no fallback required.
- **Fallback used:** None — primary EA execution passed
- **Status:** EA_PASS

### Q1-L human gate evidence

- **Date:** 2026-08-13 (operator-reported)
- **Surface:** Enterprise Architect SQL Search, primary execution
- **Result:** exact eight applications: **Aplicación 1, Aplicación 2, Aplicación 3, Aplicación 4, Aplicación 6, Aplicación 8, Aplicación 20, Aplicación 29**
- **Oracle:** Aplicación 1, 2, 3, 4, 6, 8, 20, 29
- **Match:** Yes (order-independent)
- **Scope check:** All rows are `Object_Type='Class'` / `Stereotype='ArchiMate_ApplicationComponent'`; no non-application rows
- **Navigation check:** At least one list row opened the model; `CLASSGUID`/`CLASSTYPE` aliases confirmed functional
- **Real EA evidence:** `Exercise_2_queries/Pino_Exercise_2_Informe_Principal_Queries_EA.pdf` — real operator-provided EA PDF evidence; Q1-L result and navigation are captured in this document.
- **EA accepted form:** EA primary form accepted — no fallback required.
- **Fallback used:** None — primary EA execution passed
- **Status:** EA_PASS

### Q1-C diagnostic evidence

- **Date:** 2026-08-13
- **Surface:** SQLite read-only URI `mode=ro` against `C:\Proagile\Repositorio Pasantias.qea`
- **Result:** `ORO_Application_Count = 8`
- **Oracle:** 8
- **Match:** Yes
- **Status:** DIAGNOSTIC_OK (secondary; EA evidence above is primary)

### Q1-L diagnostic evidence

- **Date:** 2026-08-13
- **Surface:** SQLite read-only URI `mode=ro` against `C:\Proagile\Repositorio Pasantias.qea`
- **Result:** 8 rows; names match the oracle exactly
- **Oracle:** Aplicación 1, 2, 3, 4, 6, 8, 20, 29
- **Match:** Yes (order-independent)
- **Uniqueness:** 8 distinct `Object_ID`s
- **Scope check:** All rows satisfy `Object_Type='Class'` and `Stereotype='ArchiMate_ApplicationComponent'`
- **Status:** DIAGNOSTIC_OK (secondary; EA evidence above is primary)

## Q2 — Vigencia distribution

### Q2-G human gate evidence

- **Date:** 2026-08-13 (operator-reported)
- **Surface:** Enterprise Architect SQL Search, primary execution
- **Result:**
  - `Vigente` = **16**
  - `Deprecado` = **14**
- **Oracle:** Vigente=16, Deprecado=14
- **Match:** Yes
- **Visible model check:** Operator compared visible `Vigencia` tags on sampled applications; tags matched the grouped result
- **No synthetic buckets or aliases check:** Query reports only observed `Vigente` and `Deprecado` values; no NULL, empty, `N/A`, or `Unknown` bucket invented; no `CLASSGUID` or `CLASSTYPE` alias in the grouped result
- **Real EA evidence:** `Exercise_2_queries/Pino_Exercise_2_Informe_Principal_Queries_EA.pdf` — real operator-provided EA PDF evidence; Q2-G grouped result is captured in this document.
- **EA accepted form:** EA primary form accepted — no fallback required.
- **Fallback used:** None — primary EA execution passed
- **Status:** EA_PASS

### Q2-G diagnostic evidence

- **Date:** 2026-08-13
- **Surface:** SQLite read-only URI `mode=ro` against `C:\Proagile\Repositorio Pasantias.qea`
- **Result:**
  - `Deprecado` = **14**
  - `Vigente` = **16**
- **Oracle:** Vigente=16, Deprecado=14
- **Match:** Yes
- **Missing / empty / N/A check:** 0 applications with NULL, empty, or `N/A` `Vigencia` value
- **Conflicting-value check:** 0 applications with more than one distinct `Vigencia` value
- **Unrestricted value check:** only `Vigente` and `Deprecado` observed for applications
- **Status:** DIAGNOSTIC_OK (secondary; EA evidence above is primary)

## Q3 — Base de Datos 28 impact

### Q3 target cardinality diagnostic

- **Date:** 2026-08-13
- **Surface:** SQLite read-only URI `mode=ro` against `C:\Proagile\Repositorio Pasantias.qea`
- **Predicate:** `Name='Base de Datos 28' AND Object_Type='Class' AND Stereotype='ArchiMate_DataObject'`
- **Result:** exactly **1** row
- **Observed Object_ID:** **102** (documented as supplied-QEA oracle evidence only; not used as a query filter)
- **Expected cardinality:** 1
- **Match:** Yes
- **Status:** OK

### Q3-C human gate evidence

- **Date:** 2026-08-14 (operator-reported)
- **Surface:** Enterprise Architect SQL Search, primary execution
- **Result:** affected application count = **5**
- **Oracle:** 5
- **Match:** Yes
- **Orientation:** `rel.Start_Object_ID = src.Object_ID`, `rel.End_Object_ID = tgt.Object_ID` (Start→End source→target)
- **Source scope:** `Object_Type='Class'` / `Stereotype='ArchiMate_ApplicationComponent'`
- **Target scope:** `Name='Base de Datos 28'` / `Object_Type='Class'` / `Stereotype='ArchiMate_DataObject'`
- **Direction predicate:** None
- **Object_ID=102 filter:** None
- **Real EA evidence:** `Exercise_2_queries/Pino_Evidencias_Funcionales_Queries.pdf` — real operator-provided EA PDF evidence; Q3-C result is captured in this document.
- **EA accepted form:** EA primary form accepted — no fallback required.
- **Fallback used:** None — primary EA execution passed
- **Status:** EA_PASS

### Q3-L human gate evidence

- **Date:** 2026-08-14 (operator-reported)
- **Surface:** Enterprise Architect SQL Search, primary execution
- **Result:** 5 rows — **Aplicación 5, Aplicación 12, Aplicación 22, Aplicación 25, Aplicación 27**
- **Oracle:** Aplicación 5, 12, 22, 25, 27
- **Match:** Yes (order-independent)
- **Uniqueness:** 5 distinct `src.Object_ID`s; no `Connector_ID` selected
- **Scope check:** all rows are application sources; target is `Base de Datos 28`; Node sources absent
- **Navigation aliases:** `CLASSGUID` and `CLASSTYPE` present on source application; navigation from list row (Aplicación 22 and related links) verified
- **Real EA evidence:** `Exercise_2_queries/Pino_Evidencias_Funcionales_Queries.pdf` — real operator-provided EA PDF evidence; Q3-L result and navigation are captured in this document.
- **EA accepted form:** EA primary form accepted — no fallback required.
- **Fallback used:** None — primary EA execution passed
- **Status:** EA_PASS

### Q3-C diagnostic evidence

- **Date:** 2026-08-13
- **Surface:** SQLite read-only URI `mode=ro` against `C:\Proagile\Repositorio Pasantias.qea`
- **Result:** affected application count = **5**
- **Oracle:** 5
- **Match:** Yes
- **Status:** DIAGNOSTIC_OK (secondary; EA evidence above is primary)

### Q3-L diagnostic evidence

- **Date:** 2026-08-13
- **Surface:** SQLite read-only URI `mode=ro` against `C:\Proagile\Repositorio Pasantias.qea`
- **Result:** 5 rows — **Aplicación 5, Aplicación 12, Aplicación 22, Aplicación 25, Aplicación 27**
- **Oracle:** Aplicación 5, 12, 22, 25, 27
- **Match:** Yes (order-independent)
- **Uniqueness:** 5 distinct `src.Object_ID`s; no `Connector_ID` selected
- **Scope check:** all rows are application sources; target is `Base de Datos 28`
- **Navigation aliases:** `CLASSGUID` and `CLASSTYPE` present on source application
- **Status:** DIAGNOSTIC_OK (secondary; EA evidence above is primary)

### Q3 negative-control diagnostic

- **Date:** 2026-08-13
- **Surface:** SQLite read-only URI `mode=ro` against `C:\Proagile\Repositorio Pasantias.qea`
- **Result:** 3 non-application sources depend on Base de Datos 28
  - `Servidor 2` (`Node` / `ArchiMate_Node`)
  - `Servidor 5` (`Node` / `ArchiMate_Node`)
  - `Servidor 19` (`Node` / `ArchiMate_Node`)
- **Expected Node sources:** Servidor 2, Servidor 5, Servidor 19
- **Match:** Yes
- **Note:** Negative-control evidence only; Node sources are excluded from mandatory Q3-C/Q3-L by the application source predicate.

## Optional Prolaborate

- **Status:** COMPLETE (non-blocking; executed after all mandatory EA gates and before final documentation consolidation)
- **Procedural note:** Prolaborate was available and the Q2-G chart was produced. Task 7.7 (record absence if unavailable) is marked **N/A** because Prolaborate access was available and the optional follow-on was completed.
- **Dashboard:** Gobierno de Aplicaciones - Pasantías - Pino
- **Business question:** How many applications are in each Vigencia state (Vigente / Deprecado)?
- **EA authority:** Q2-G is the functional EA authority: Vigente=16 / Deprecado=14.
- **Direct execution:** Q2-G executed directly in Prolaborate correctly with Deprecado=14 / Vigente=16; its aggregate shape did not bind the widget.
- **Chart configuration:** Donut and bar widgets; Designer Configuration Class/ApplicationComponent; tag Vigencia values; per-application presentation query with `Series=Vigencia`
- **Rendered result:** Donut Deprecado 46.67% / Vigente 53.33%; bars 14/16
- **Adaptation:** Designer Configuration created the per-application presentation query, preserving exactly the Q2-G semantics and results; the final widgets use this Designer shape. The generic Open/Closed 75/25 preview binding was not used. The generated presentation query is not one of the official five SELECT statements. No `seriesproperty` used.
- **Real Prolaborate evidence:** `Exercise_2_queries/Pino_Evidencias_Funcionales_Queries.pdf` — real operator-provided EA PDF evidence; Prolaborate chart/dashboard is captured in this document.
- **Documentation:** `../prolaborate/Pino_Exercise_2_Prolaborate_Q2.md`

## Evidence files

| Path | Statement | Description |
|---|---|---|
| `sqlite/Pino_Exercise_2_SQLite_Diagnostics.md` | Q1-C, Q1-L, Q2-G, Q3-C, Q3-L | Read-only SQLite diagnostic transcript and proof. |
| `Pino_Exercise_2_Informe_Principal_Queries_EA.pdf` | Q1-C, Q1-L, Q2-G | Real operator-provided EA PDF evidence (physically found in `Exercise_2_queries/`). |
| `Pino_Evidencias_Funcionales_Queries.pdf` | Q3-C, Q3-L, Prolaborate | Real operator-provided EA PDF evidence (physically found in `Exercise_2_queries/`). |

## Post-gate compatibility audit

| Statement | EA accepted form | Dialect rejection recorded | Fallback used | Fallback rationale | Revalidated |
|---|---|---|---|---|---|
| Q1-C | Primary SELECT with `COUNT(DISTINCT app.Object_ID)` | None | No | N/A | N/A |
| Q1-L | Primary SELECT DISTINCT with `CLASSGUID`/`CLASSTYPE` | None | No | N/A | N/A |
| Q2-G | Primary SELECT with `COUNT(DISTINCT app.Object_ID)` GROUP BY `tag.Value` | None | No | N/A | N/A |
| Q3-C | Primary SELECT with `COUNT(DISTINCT src.Object_ID)` | None | No | N/A | N/A |
| Q3-L | Primary SELECT DISTINCT with `CLASSGUID`/`CLASSTYPE` | None | No | N/A | N/A |

**Audit conclusion:** Every mandatory statement was accepted in its primary EA form. No dialect rejection, no compatibility fallback, and no preemptive fallback remain. All final gate evidence remains valid; no statements were rerun during this audit.

## Phase 8 — Pre-verify and repository guard

| Check | Method | Result |
|---|---|---|
| 8.1 Exactly five SELECT statements in SQL file | Regex count of `SELECT` at block start in `../queries/Pino_Exercise_2_EA_Governance_Queries.sql` | **PASS — 5 SELECT statements** |
| 8.2 No mutating operations | Scan for `UPDATE`, `INSERT`, `DELETE`, `ALTER`, `DROP`, `CREATE`, `TRUNCATE`, `MERGE`, `GRANT`, `REVOKE` | **PASS — none found** |
| 8.3 Protected areas untouched | `Addino/` exists; `Exercise_2_queries/docs/challenge/` preserved; Exercise 1 archive exists; QEA path unchanged; no edits to RDD/config | **PASS** |
| 8.4 Submission-facing filenames contain `Pino` | Visual inspection of all files under `Exercise_2_queries/` except `docs/challenge/` | **PASS — all delivery files contain `Pino`** |
| 8.5 Traceability: statement → SQL → index → EA result → oracle → PDF evidence → PASS | Review statement inventory and evidence sections above | **PASS — all five mandatory statements are EA_PASS with real PDF evidence** |
| 8.6 Oracles documented as validation evidence, not query logic | Review SQL file and technical explainer | **PASS — baselines appear only in comments/docs, never as filters** |
| 8.7 Relative links valid; no invented paths | Review docs for relative paths; PDF paths match physically found files | **PASS — links are relative; PDF paths are real and physically found** |
| 8.8 `git status --short` and `git diff --check` | Run commands; inspect output | **PASS — `git diff --check` clean; only expected untracked Exercise 2 files and the OpenSpec tasks.md checkbox update** |

### Pre-verify conclusion

All Phase 8 checks passed. The repository guard confirms:

- Five official SELECT IDs: Q1-C, Q1-L, Q2-G, Q3-C, Q3-L.
- All SELECT blocks are semantically read-only.
- No QEA mutation, no Addino/E1/challenge/OpenSpec modification except tasks.md checkboxes.
- Complete traceability from each mandatory statement to real operator PDF evidence.
- No mandatory statement remains PENDING.
- Ready for `sdd-verify`.
