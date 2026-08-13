# Exploration: `ea-governance-queries-exercise-2`

> Pre-proposal exploration for Exercise 2 of the Proagile 2026 challenge:
> SQL queries over the Sparx Enterprise Architect (EA) repository that answer
> governance questions about applications (Categoria, Vigencia) and impact
> analysis (Base de Datos 28). Read-only diagnostic verification of every
> preliminary finding in the prompt. Read-only diagnostic SQL was executed in
> Explore, but no final delivery SQL queries, spec, or design are produced
> here — only evidence and the approved decisions Proposal must apply.

---

## TL;DR

All eight preliminary findings are **confirmed** against the actual QEA file
at `C:\Proagile\Repositorio Pasantias.qea` (SQLite format 3, 1 728 512 B).
The repository contains exactly 30 `ArchiMate_ApplicationComponent` Class
elements (IDs 44–73), exactly 30 `ArchiMate_DataObject` Class elements (IDs
75–104, of which Base de Datos 28 = Object_ID 102), and 142 `Dependency`
connectors — every one of them with `Direction = 'Source -> Destination'` as
a **string** literal. Five applications depend on
Base de Datos 28 (`Aplicación 5, 12, 22, 25, 27`); three additional
non-application dependencies come from `ArchiMate_Node` elements
(`Servidor 2, 5, 19`). Categoria and Vigencia are stored exclusively as
tagged values in `t_objectproperties`; no application has NULL, empty, or
duplicate tags for either property. The only material risk for the proposal
is ensuring final queries are validated in EA SQL Search and include the
`CLASSGUID` / `CLASSTYPE` navigation aliases where an individual element is
returned. The physical `t_connector.Direction` value is an observed optional
validation, not a required impact-query filter. All other preliminaries are
stable and the dataset is benign.

---

## 1. Scope and inputs verified

| Input | Verified value | Evidence |
| --- | --- | --- |
| Workspace root | `C:\Proagile\Addin` (git repo, branch `main`, clean before Explore) | `git status --short` empty before artifact creation; expected after creation: `?? openspec/changes/ea-governance-queries-exercise-2/` |
| Exclusive Exercise 2 area | `C:\Proagile\Addin\Exercise_2_queries\` (scaffolding exists; `docs/challenge/` contains the challenge brief) | directory listing |
| Challenge brief | `Exercise_2_queries/docs/challenge/Desafio_Tecnico_Practica_EA_Prolaborate_v2 (1).md` §4 | file read |
| QEA database | `C:\Proagile\Repositorio Pasantias.qea` (NOT under `C:\Proagile\Data\…` — that path does not exist; the file is in `C:\Proagile\`) | `Get-Item` header bytes `53 51 4C 69 74 65 20 66 6F 72 6D 61 74 20 33` |
| DB engine | SQLite 3 (runtime client 3.50.4 from `python -c "import sqlite3; print(sqlite3.sqlite_version)"`) | Python runtime |
| DB access mode | Read-only via URI `file:…?mode=ro` (any write would raise) | `sqlite3.connect(uri, uri=True)` |
| Local InsideEA reference | `C:\Proagile\Documentacion\InsideEA.pdf` (105 pp) — corroborates physical table structure, Start/End orientation, and the EA SQL Search rendering contract | `pypdf` extract |
| Local Object Model reference | `C:\Proagile\Documentacion\enterprise-architect-object-model.pdf` (1 817 196 B) — documents Automation API properties, including String `Connector.Direction` | `pypdf` extract |
| Historical Exercise 1 area (`Addino/`) | present, must NOT be modified | orchestrator constraint; out of scope |
| OpenSpec state for this change | change folder exists and contains only `exploration.md`; proposal, spec, design, and tasks do not yet exist | directory listing |
| Archived Exercise 1 artifacts | `openspec/changes/archive/2026-08-12-ea-metadata-review-exercise-1/` — read-only reference, must NOT be modified | directory listing |

> The orchestrator prompt cites the DB as `C:\Proagile\Data\Repositorio Pasantias.qea`,
> but that directory does not exist. The actual file is at
> `C:\Proagile\Repositorio Pasantias.qea`. The path discrepancy does not block exploration because the actual QEA was located and inspected at the confirmed path.

---

## 2. Preliminary findings — confirmation matrix

The matrix maps each preliminary claim in the prompt to the diagnostic that
verified (or refuted) it, the exact SQLite shape used, and the verdict.
All SQLite queries were executed in read-only mode against the live QEA
file; their output is captured in
`%TEMP%\opencode\qea_diag.out.txt` and `qea_diag2.out.txt` (outside the
workspace, so they do not dirty the repo).

### 2.1 — Tables and column shape

| # | Preliminary claim | Diagnostic SQL (abridged) | Observed result | Verdict |
| - | --- | --- | --- | --- |
| 1 | QEA SQLite tables include `t_object`, `t_objectproperties`, `t_connector`, `t_package`, `t_xref`, `t_stereotypes` | `SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name` | All six tables present alongside 73 others (t_attribute, t_diagram, t_document, t_genopt, etc.). Schema columns match the canonical EA shape: `t_object(Object_ID, Object_Type, Name, Stereotype, Package_ID, Status, …, NType, Tagged)`; `t_objectproperties(PropertyID, Object_ID, Property, Value, Notes, ea_guid)`; `t_connector(Connector_ID, Start_Object_ID, End_Object_ID, Connector_Type, Direction, …)`. | **CONFIRMED** |

### 2.2 — Application criterion (`Class` + `ArchiMate_ApplicationComponent`)

| # | Preliminary claim | Diagnostic SQL (abridged) | Observed result | Verdict |
| - | --- | --- | --- | --- |
| 2a | 30 applications identified by `Object_Type='Class' AND Stereotype='ArchiMate_ApplicationComponent'` | `SELECT COUNT(*), COUNT(DISTINCT Object_ID) FROM t_object WHERE Object_Type='Class' AND Stereotype='ArchiMate_ApplicationComponent'` | `30, 30`. Object_IDs `44–73`, Name `Aplicación 1–30` (UTF-8: `41706C6963616369C3B36E…`), Package_ID `3` (`Aplicaciones`). | **CONFIRMED** |
| 2b | Is `Stereotype='ArchiMate_ApplicationComponent'` essential, or is `Object_Type='Class'` alone enough? | `SELECT Stereotype, COUNT(*) FROM t_object WHERE Object_Type='Class' GROUP BY Stereotype` | Class alone yields **85** rows: 30 `ArchiMate_ApplicationComponent` + 30 `ArchiMate_DataObject` + 25 `ArchiMate_BusinessActor`, so it is insufficient. Filtering by `Stereotype` only currently returns the same 30 applications (all are Class). | **CONFIRMED: defensive criterion selected.** `Object_Type='Class' AND Stereotype='ArchiMate_ApplicationComponent'` is an explicit defensive decision consistent with the observed structure; the current dataset does not establish that both predicates are mathematically necessary. |

### 2.3 — `Categoria` is a tagged value on `t_objectproperties`

| # | Preliminary claim | Diagnostic SQL (abridged) | Observed result | Verdict |
| - | --- | --- | --- | --- |
| 3a | `Categoria` lives in `t_objectproperties`, joined by `Object_ID` | `SELECT Property, COUNT(*) FROM t_objectproperties WHERE Property LIKE '%ategor%' GROUP BY Property` | Only one matching value: `'Categoria'`, 60 rows total (30 on apps + 30 on DataObjects, see 3b). | **CONFIRMED** |
| 3b | Applications have no duplicate `Categoria` rows | `SELECT Object_ID, COUNT(*) FROM t_objectproperties WHERE Property='Categoria' GROUP BY Object_ID HAVING COUNT(*) > 1` | Empty. | **CONFIRMED** |
| 3c | Every application has a `Categoria` tag | `SELECT o.Object_ID FROM t_object o WHERE o.Object_Type='Class' AND o.Stereotype='ArchiMate_ApplicationComponent' AND NOT EXISTS (SELECT 1 FROM t_objectproperties p WHERE p.Object_ID = o.Object_ID AND p.Property='Categoria')` | Empty. | **CONFIRMED** |

### 2.4 — Category distribution for applications

| # | Preliminary claim | Diagnostic SQL (abridged) | Observed result | Verdict |
| - | --- | --- | --- | --- |
| 4a | ORO 8 / PLATA 8 / BRONCE 4 / N/A 10 | `SELECT [Value], COUNT(*) FROM t_objectproperties p JOIN t_object o ON o.Object_ID=p.Object_ID WHERE o.Object_Type='Class' AND o.Stereotype='ArchiMate_ApplicationComponent' AND p.Property='Categoria' GROUP BY [Value] ORDER BY c DESC` | Exact counts: `('N/A', 10), ('PLATA', 8), ('ORO', 8), ('BRONCE', 4)`. | **CONFIRMED** |
| 4b | ORO names: Aplicación 1, 2, 3, 4, 6, 8, 20, 29 | `SELECT p.Object_ID, o.Name, p.[Value] … ORDER BY p.Object_ID` | ORO apps are Object_IDs 44, 45, 46, 47, 49, 51, 63, 72 → Aplicación 1, 2, 3, 4, 6, 8, 20, 29. | **CONFIRMED** |
| 4c | NULL/empty/N/A/duplicate risk | `SELECT categoria_norm = CASE WHEN [Value] IS NULL THEN 'NULL' WHEN LTRIM(RTRIM([Value])) = '' THEN 'EMPTY' WHEN UPPER(LTRIM(RTRIM([Value]))) LIKE 'N/A' THEN 'N/A-LIKE' ELSE LTRIM(RTRIM([Value])) END, COUNT(*) …` | All 30 apps normalize to one of `N/A-LIKE`, `ORO`, `PLATA`, `BRONCE`. No NULL, no empty, no duplicate tags, no other stray values. | **CONFIRMED: no quality risk for the Categoria answer.** "N/A" is the literal value, not a placeholder for missing data. |

### 2.5 — `Vigencia` is a tagged value (not `t_object.Status`)

| # | Preliminary claim | Diagnostic SQL (abridged) | Observed result | Verdict |
| - | --- | --- | --- | --- |
| 5a | Vigente 16 / Deprecado 14 / N/A 0 among apps | `SELECT [Value], COUNT(*) FROM t_objectproperties p JOIN t_object o ON o.Object_ID=p.Object_ID WHERE o.Object_Type='Class' AND o.Stereotype='ArchiMate_ApplicationComponent' AND p.Property='Vigencia' GROUP BY [Value]` | `('Vigente', 16), ('Deprecado', 14)`. Zero NULL/empty/N/A rows for apps. | **CONFIRMED** |
| 5b | `Vigencia` is a tag, NOT `t_object.Status` | `SELECT Status, COUNT(*) FROM t_object WHERE Object_Type='Class' AND Stereotype='ArchiMate_ApplicationComponent' GROUP BY Status` | Every application has `Status = 'Proposed'` (30/30). | **CONFIRMED: `Status` is unrelated to vigencia.** The proposal must use `t_objectproperties.Property='Vigencia'`. |
| 5c | No duplicate `Vigencia` tags on any application | `SELECT Object_ID, COUNT(*) FROM t_objectproperties WHERE Property='Vigencia' GROUP BY Object_ID HAVING COUNT(*) > 1` | Empty. | **CONFIRMED** |
| 5d | Every application has a `Vigencia` tag | NOT EXISTS subquery on `Vigencia` for apps | Empty. | **CONFIRMED** |
| 5e | Non-app elements carry `Vigencia` too — keep that out of the app query | `SELECT COUNT(*) FROM t_objectproperties p JOIN t_object o ON o.Object_ID=p.Object_ID WHERE p.Property='Vigencia' AND NOT (o.Object_Type='Class' AND o.Stereotype='ArchiMate_ApplicationComponent')` | 30 rows (the 30 `ArchiMate_DataObject` rows — Vigente 13 / Deprecado 8 / N/A 9). `ArchiMate_BusinessActor` and `ArchiMate_Node` carry no `Vigencia`. | **CONFIRMED with a non-trivial side effect: scoping by app filter is REQUIRED**; the proposal must JOIN/filter by application stereotype so the per-app histogram is not polluted by data objects. |

### 2.6 — `Base de Datos 28` identity

| # | Preliminary claim | Diagnostic SQL (abridged) | Observed result | Verdict |
| - | --- | --- | --- | --- |
| 6a | Object_ID `102` | `SELECT Object_ID, Name, Object_Type, Stereotype, Package_ID FROM t_object WHERE Name='Base de Datos 28'` (returns 1 row) and `WHERE Object_ID=102` (returns 1 row) | `Object_ID = 102`, Name `Base de Datos 28`, `Object_Type='Class'`, `Stereotype='ArchiMate_DataObject'`, `Package_ID=5` (`Bases de Datos`), `Status='Proposed'`. | **CONFIRMED** |
| 6b | "28" is the suffix in the Name, not the Object_ID | `SELECT Object_ID, Name, Object_Type, Stereotype, Package_ID FROM t_object WHERE Object_ID=28` | Empty (Object_ID 28 has no row; the model uses Object_IDs 1–172 sparsely and 28 is not assigned). | **CONFIRMED** — the orchestrator prompt's wording "Object_ID 28" is the natural-language name, not the PK. Proposal must clarify this in the deliverable. |

### 2.7 — `t_connector` schema, orientation, and EA-specific shape

| # | Preliminary claim | Diagnostic SQL (abridged) | Observed result | Verdict |
| - | --- | --- | --- | --- |
| 7a | `Start_Object_ID` is the source, `End_Object_ID` is the target | `SELECT Connector_ID, Start_Object_ID, End_Object_ID, Connector_Type, Direction FROM t_connector LIMIT 10` | Rows like `(1, 44, 93, 'Dependency', 'Source -> Destination')` — source = Aplicación 1 (44), target = Base de Datos 19 (93). | **CONFIRMED** |
| 7b | `Connector_Type='Dependency'`, `Direction='Source -> Destination'` | `SELECT Connector_Type, COUNT(*) FROM t_connector GROUP BY Connector_Type` and `SELECT Direction, COUNT(*) FROM t_connector GROUP BY Direction` | 142/142 connectors are `Dependency`; 142/142 connectors carry the literal string `Source -> Destination` (LENGTH 21). | **CONFIRMED — see §3 for its role as optional diagnostic evidence rather than a mandatory filter.** |
| 7c | InsideEA corroborates the physical schema | `InsideEA.pdf`, section `t_connector` (page 20) — verbatim: *"Direction — String equivalent of the Direction property"*. | The local `InsideEA.pdf` documents physical `t_connector.Direction` as String and confirms `Start_Object_ID` is Source and `End_Object_ID` is Target. | **CONFIRMED: local docs corroborate.** |

### 2.8 — Incoming dependencies to Base de Datos 28

| # | Preliminary claim | Diagnostic SQL (abridged) | Observed result | Verdict |
| - | --- | --- | --- | --- |
| 8a | 8 total incoming dependencies to Base de Datos 28 (Object_ID 102) | `SELECT COUNT(*) FROM t_connector WHERE End_Object_ID = 102 AND Connector_Type = 'Dependency'` | 8 rows, Connector_IDs `11, 32, 38, 40, 51, 79, 82, 91`. All `Direction = 'Source -> Destination'`. | **CONFIRMED** |
| 8b | 5 are sourced by Aplicación 5, 12, 22, 25, 27 | `SELECT c.Start_Object_ID, o.Name FROM t_connector c JOIN t_object o ON o.Object_ID = c.Start_Object_ID WHERE c.End_Object_ID=102 AND c.Connector_Type='Dependency' AND o.Object_Type='Class' AND o.Stereotype='ArchiMate_ApplicationComponent'` | Application origins: Start_Object_IDs `48 (Aplicación 5), 55 (Aplicación 12), 65 (Aplicación 22), 68 (Aplicación 25), 70 (Aplicación 27)`. | **CONFIRMED** |
| 8c | The remaining origins are non-app and need separate classification | Same query as 8b with `NOT (o.Object_Type='Class' AND o.Stereotype='ArchiMate_ApplicationComponent')` and LEFT JOIN to detect orphans | Non-app origins: Start_Object_IDs `128 (Servidor 2), 131 (Servidor 5), 145 (Servidor 19)` — all `Object_Type='Node'`, `Stereotype='ArchiMate_Node'`, Package_ID 6. No orphan/dangling connectors. | **CONFIRMED** — the impact question must distinguish application impact from infrastructure (Node) impact. The challenge text asks specifically for "applications that would be affected", so the deliverable should report only the 5 apps by default and surface the 3 nodes as a footnote. |

---

## 3. Direction semantics and final-delivery surface

Enterprise Architect Object Model 17.1 and the historical SDK define
`Connector.Direction` as a read/write String supporting `Unspecified`,
`Bi-Directional`, `Source -> Destination`, and `Destination -> Source`.
InsideEA documents physical `t_connector.Direction` as String and confirms
`Start_Object_ID` = Source and `End_Object_ID` = Target. Repository and API
therefore use the same String representation; the final-delivery surface is
EA SQL Search.

### 3.1 What the QEA file actually stores

`SELECT DISTINCT Direction FROM t_connector` returns exactly one value:
`'Source -> Destination'` (string, length 21). All 142 connectors share the
same value.

This matches local `InsideEA.pdf`, section `t_connector` (page 20), which
describes `t_connector.Direction` as the *"String equivalent of the Direction
property"*, and Object Model 17.1, section `Connector.Direction`, which
defines the Automation API property as read/write String. The observed
physical value may be used as optional diagnostic validation, but it is not a
mandatory impact-query predicate.

### 3.2 Approved query orientation and validation

The impact query MUST orient the relationship with `Start_Object_ID` as the
source and `End_Object_ID` as the target. It MUST filter the source to
applications, identify the unambiguous target `Base de Datos 28`, and require
`Connector_Type = 'Dependency'`. It MUST return only the five affected
applications; the three Node origins are a footnote, not result rows.

The observed physical value `Direction = 'Source -> Destination'` may be
recorded as an optional diagnostic validation.

### 3.3 Approved execution surface

EA SQL Search is the primary final-delivery and evidence surface. Proposal
MUST validate final SQL in EA before the work is complete. SQLite, connected
to the QEA file with `mode=ro`, remains an exploration and independent
diagnostic-validation surface only; it is not the primary deliverable.

### 3.4 Secondary dialect concern: SQL Search rendering aliases

`InsideEA.pdf`, section `SQL Searches` (page 90), documents a non-obvious
requirement of EA SQL Search: result rendering requires two synthetic columns —
- `CLASSGUID` containing the `ea_guid` of the row's primary table
  (`t_object`, `t_package`, `t_connector`, `t_diagram`, `t_operation`,
  `t_attribute`), and
- `CLASSTYPE` containing either the source's type alias
  (`Object_Type AS CLASSTYPE` for elements, `'Package' AS CLASSTYPE` for
  packages, `'Operation' AS CLASSTYPE` for operations, etc.).

Without these columns, EA SQL Search still runs and returns rows, but icons
are blank and double-click navigation is disabled. SQLite has no such
constraint. Individual-element result queries SHOULD include
`o.ea_guid AS CLASSGUID` and `o.Object_Type AS CLASSTYPE` for EA navigation.
Aggregate queries may omit these aliases and MUST NOT invent a representative
object.

---

## 4. Other cross-cutting risks and observations

| Risk / observation | Severity | Source of evidence |
| --- | --- | --- |
| `Aplicación` names display as `Aplicaci¾n` in Windows console output because PowerShell uses CP1252; the bytes are correct UTF-8 (`C3 B3`). | Low (cosmetic) | `HEX(Name)` = `'41706C6963616369C3B36E2031'` |
| 142 connectors is the entire connector graph; only `Dependency` exists. There is no need to disambiguate multiple connector types in the impact query. | Low (information) | `SELECT Connector_Type, COUNT(*) FROM t_connector GROUP BY Connector_Type` |
| The non-app origins of Base de Datos 28 are `ArchiMate_Node` elements in Package_ID 6 (a sibling package to Aplicaciones and Bases de Datos). The challenge asks for *applications* affected, so the impact query must filter sources by the application stereotype. | Medium (semantic) | `t_object` rows 128/131/145; Package layout |
| Categoria and Vigencia tags also appear on `ArchiMate_DataObject` (DataObjects) but NOT on `ArchiMate_BusinessActor` or `ArchiMate_Node`. The challenge only asks about applications, but the queries must still scope by app stereotype to avoid accidentally including DataObjects. | Medium (semantic) | `t_objectproperties` distribution queries |
| Outgoing connector count per app is between 1 and 3; the impact question is symmetrically well-defined for Base de Datos 28 because it has 8 incoming — but other databases have fewer (Base de Datos 15 / 23 have 0). The proposal must keep the answer scoped to the specific target. | Low (information) | in-degree / out-degree distribution queries |
| The repository contains exactly 30 + 30 + 25 = 85 Class objects across three ArchiMate stereotypes, plus 20 `ArchiMate_Node` objects (`Object_Type='Node'`) and a small package hierarchy (`Model > Aplicaciones/Bases de Datos/Actores/Servidores`). There is no diagram data relevant to the queries (`t_diagram`, `t_diagramobjects` not used). | Low (information) | package / Object_Type distribution queries |
| Local `InsideEA.pdf` (105 pp) and `enterprise-architect-object-model.pdf` (1.8 MB) are present and corroborate the String `Direction` representation. InsideEA covers physical table structure, Start/End orientation, and SQL Search; Object Model 17.1 covers Automation API properties including `Connector.Direction`. | Low (information) | `pypdf` extracts |
| EA SQL Search is the primary final-delivery and evidence surface. Read-only SQLite is retained only for exploration and independent diagnostic validation. Proposal must validate final SQL in EA before completion. | Medium (delivery) | approved direction |
| Prolaborate V5 dashboard is optional per the challenge. It is deferred until the three mandatory queries are implemented and validated in EA; it is out of scope for Explore only. | Low (information) | challenge brief + prompt |
| The challenge text and the orchestrator prompt disagree on the absolute path to the QEA file (`C:\Proagile\Data\…` vs the actual `C:\Proagile\Repositorio Pasantias.qea`). The latter is the local working path for SDD, but final solution and queries must not depend on a developer-machine absolute path. | Low (information, needs user confirmation) | `Test-Path "C:\Proagile\Data"` = `False`; `Test-Path "C:\Proagile\Repositorio Pasantias.qea"` = `True` |
| The archived Exercise 1 artifacts (`openspec/changes/archive/2026-08-12-ea-metadata-review-exercise-1/`) and `openspec/specs/ea-metadata-review/spec.md` are explicitly out of scope. No read access was needed for this exploration. | Low (information) | orchestrator constraint |

---

## 5. Approved decisions Proposal must apply

These approved directions must be carried into Proposal explicitly, in
writing, before any spec or apply work.

1. **Execution surface.** EA SQL Search is primary; SQLite `mode=ro` is
   secondary diagnostic validation. Final SQL must be validated in EA before
   completion.
2. **Impact query semantics.** Return only the five applications. Use
   `Start_Object_ID` as source, join the target `t_object`, and identify the
   unambiguous `Base de Datos 28` target with `Name='Base de Datos 28'`,
   `Object_Type='Class'`, and `Stereotype='ArchiMate_DataObject'`; use the
   application source filter and `Connector_Type = 'Dependency'`. Object_ID
   `102` is observed validation evidence only and final impact queries SHOULD
   NOT hardcode it as their primary identifier; list the three Nodes only in a
   footnote.
3. **Categoria handling.** Categoria is mandatory. Deliver the ORO count and
   list; other observed values are diagnostic only.
4. **Vigencia handling.** Deliver only `Vigente` and `Deprecado`; document
   the observed zero NULL, empty, and `N/A` values without creating artificial
   result buckets.
5. **Evidence capture.** EA SQL Search screenshots/results are primary;
   SQLite evidence is secondary.
6. **QEA path documentation.** Use
   `C:\Proagile\Repositorio Pasantias.qea` as the local working path for SDD,
   but keep final solution and queries independent of a developer-machine
   absolute path.

---

## 6. Artifacts this phase will produce

- `openspec/changes/ea-governance-queries-exercise-2/exploration.md` (this
  file). Nothing else under `openspec/changes/` is created at this phase —
  no proposal, no spec, no design, no tasks.
- Diagnostic SQL transcripts (outside the workspace):
  `%TEMP%\opencode\qea_diag.out.txt`, `%TEMP%\opencode\qea_diag2.out.txt`,
  and the Python runners that produced them
   (`%TEMP%\opencode\qea_diag.py`, `qea_diag2.py`). These are kept for
   reproducibility; they are intentionally outside the repo so the diff
   stays clean.
- No final delivery SQL queries were produced, and no final SQL file exists in
  `Exercise_2_queries/queries/`.

---

## 7. Readiness assessment

| Check | Result |
| --- | --- |
| All 8 preliminary findings verified independently | Yes |
| Git state | Clean before Explore; after artifact creation, expected `?? openspec/changes/ea-governance-queries-exercise-2/` |
| QEA file untouched (read-only connection used; file mtime / sha not checked because not required, but no write was performed) | Yes |
| Local docs consulted and corroborate findings | Yes (`InsideEA.pdf` sections `t_connector` (page 20) and `SQL Searches` (page 90); Object Model 17.1 section `Connector.Direction`) |
| No final delivery SQL / spec / design / task / archive artifact created | Confirmed — only `exploration.md`; read-only diagnostic SQL ran in Explore and no final SQL file exists in `Exercise_2_queries/queries/` |
| Hybrid persistence contract satisfied (filesystem + Engram) | Yes (filesystem write + `mem_save` with `capture_prompt: false`) |
| Approved directions recorded for Proposal | Yes (six items in §5) |

**Ready for Proposal:** Yes. The exploration leaves no architectural
ambiguity; Proposal must implement the approved directions in §5.
