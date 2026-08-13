# Design: EA Governance Queries — Exercise 2

## Design Outcome

The final mandatory query set will contain **five SELECT statements**: **Q1-C** (count), **Q1-L** (navigable list), **Q2-G** (grouped counts), **Q3-C** (count), and **Q3-L** (navigable list). This decomposition matches the business outputs without using mixed aggregate/detail result shapes, preserves EA Search navigation for individual elements, and keeps each statement simple enough for EA's repository-backed SQL engine.

The approved specifications contain **25 requirements and 38 scenarios**: Category **9R/14S**, Lifecycle **7R/11S**, and Impact **9R/13S**. No contradiction was found.

## Technical Approach

Create one identified, SELECT-only `.sql` delivery container, backed by a technical explainer and real execution evidence. The `.sql` file is **not** an EA execution batch: each of its five statements is copied, created, and run separately in EA SQL Search. Use only basic `SELECT`, `JOIN`, `WHERE`, `GROUP BY`, `ORDER BY`, `DISTINCT`, and aggregate constructs; do not use CTEs, window functions, scripts, C#, mutations, or vendor-specific extensions. EA SQL Search is the primary execution and acceptance surface. SQLite opened with `mode=ro` is a secondary diagnostic oracle only.

Implementation and validation proceed incrementally by question. A statement cannot advance from diagnostic validation to documented completion until a human executes it in EA, compares it with visible model content, and captures genuine evidence.

## Query Decomposition and Contracts

| ID | Output | Shape | Navigation aliases | Supplied-QEA oracle |
|---|---|---|---|---|
| Q1-C | ORO application count | One aggregate row | None | `8` |
| Q1-L | ORO application list | One row per unique application | Source application `ea_guid AS CLASSGUID`, `Object_Type AS CLASSTYPE` | Apps 1, 2, 3, 4, 6, 8, 20, 29 |
| Q2-G | Applications grouped by Vigencia | One row per observed requested state | None | `Vigente=16`, `Deprecado=14`; missing/empty/`N/A` = `0` documented separately |
| Q3-C | Affected application count | One aggregate row | None | `5` |
| Q3-L | Affected application list | One row per unique source application | Source application `ea_guid AS CLASSGUID`, `Object_Type AS CLASSTYPE` | Apps 5, 12, 22, 25, 27 |

Each statement will be visibly identified by its ID and business question. The SQL file may use short comments as separators, but the executable text copied into EA begins with `SELECT`; human headings and comments are not included before that `SELECT` when leading comments affect the EA Search UI. Operators copy, create, and run each statement in separate blocks rather than submitting the file as a batch. Recommended EA Search display names preserve the IDs: `Q1-C — Categoria ORO — Count`, `Q1-L — Categoria ORO — List`, `Q2-G — Vigencia — Grouped`, `Q3-C — DB28 Impact — Count`, and `Q3-L — DB28 Impact — List`. UI names may be adjusted, but the evidence index must map each adjusted name unambiguously to its statement ID. No result or query will depend on a local absolute QEA path.

### Q1 — Categoria ORO

Both Q1 statements alias the application `t_object` as `app` and `t_objectproperties` as `tag`, join `app.Object_ID` to `tag.Object_ID`, select only `tag.Property='Categoria'` with `tag.Value='ORO'`, and enforce `app.Object_Type='Class' AND app.Stereotype='ArchiMate_ApplicationComponent'`. Q1-C uses `COUNT(DISTINCT app.Object_ID)`; no unqualified `Object_ID` is permitted in a multi-table statement. The count and list derive from repository data; `8` and the eight names are validation baselines, never query constants.

Q1-L includes the EA navigation aliases and returns the application name plus useful semantic columns such as category. Its `SELECT DISTINCT` fields must be stable per application and semantic stable result attributes: it MAY include `app.ea_guid AS CLASSGUID`, `app.Object_Type AS CLASSTYPE`, `app.Object_ID` if useful, `app.Name`, and Categoria, but MUST NOT include `tag` row IDs such as `PropertyID` or internal join demonstration columns that would defeat logical deduplication. Its contract is exactly one logical row per unique application. Q1-C has no object aliases because an aggregate row is not an element. Separate statements are chosen over a combined count/list shape because EA Search handles homogeneous rows more clearly and only individual rows can be navigable.

### Q2 — Vigencia distribution

Q2-G aliases the application `t_object` as `app` and `t_objectproperties` as `tag`, joins `app.Object_ID` to `tag.Object_ID`, uses `tag.Property='Vigencia'`, and groups by the tag value for the requested observed states `Vigente` and `Deprecado`. It never references `app.Status`. `COUNT(DISTINCT app.Object_ID)` within each Vigencia group deduplicates equivalent rows for the same application and state; it does not invent precedence when one application has conflicting different Vigencia values. The supplied QEA has no such conflicting values. If one is detected, it is a data anomaly: document it, do not choose an arbitrary state, and do not accept Q2 until the anomaly is understood. No unqualified `Object_ID` is permitted in this multi-table statement. The grouped result contains the lifecycle value and unique application count only; it has no `CLASSGUID` or `CLASSTYPE`, because no individual element represents a group.

No individual Q2 list will be delivered. The challenge asks for grouped counts, and adding a drilldown would increase the final query set and evidence burden without improving the mandatory answer. The explainer records that the supplied QEA has zero NULL, empty, or `N/A` application Vigencia values; the query must not fabricate zero-valued result rows or synthetic buckets.

### Q3 — Base de Datos 28 impact

Both Q3 statements alias the source application `t_object` as `src`, `t_connector` as `rel`, and target `t_object` as `tgt`, join `rel.Start_Object_ID` to `src.Object_ID` and `rel.End_Object_ID` to `tgt.Object_ID`. They require `rel.Connector_Type='Dependency'`, apply the defensive source application predicate, and identify the target semantically with `tgt.Name='Base de Datos 28'`, `tgt.Object_Type='Class'`, and `tgt.Stereotype='ArchiMate_DataObject'`. Q3-C uses `COUNT(DISTINCT src.Object_ID)`; no unqualified `Object_ID` is permitted in this multi-table statement. Before Q3 acceptance, implementation validation must establish that this supplied-QEA predicate resolves exactly one target; observed `Object_ID=102` is diagnostic evidence only and is not a filter. If the predicate resolves zero or more than one target, record the anomaly and block Q3 acceptance pending resolution; do not silently aggregate across targets. The `102` check and target-cardinality check may use diagnostic or implementation validation, but they do not create a sixth final SELECT. No `Direction` predicate is permitted.

Q3-L navigates to each source application, not to the target or connector. Its `SELECT DISTINCT` fields must be stable per source application and semantic stable result attributes: it MAY include source equivalents of `src.ea_guid AS CLASSGUID`, `src.Object_Type AS CLASSTYPE`, `src.Object_ID` if useful, `src.Name`, and semantic target identity if useful, but MUST NOT include connector IDs such as `rel.Connector_ID` or internal join demonstration columns that would defeat logical deduplication. Its contract is exactly one logical row per unique source application. Q3-C has no aliases. The three observed Node sources (`Servidor 2`, `Servidor 5`, `Servidor 19`) will be documented as negative-control evidence in the explainer/evidence index; no additional final SELECT is allocated to them. If a read-only diagnostic is used to demonstrate them, it remains diagnostic and is not part of the five final SELECT statements.

## Architecture Decisions

| Decision | Alternatives considered | Choice and rationale |
|---|---|---|
| Decomposition | Three mixed queries; five focused queries; extra Q2/Node drilldowns | **Five focused SELECTs (2/1/2).** Count/list questions receive separate aggregate/detail statements; Q2 remains one grouped statement. This is the clearest EA Search-compatible mapping. |
| Logical uniqueness | Trust current one-tag/one-connector data; broad `DISTINCT`; nested/advanced SQL | Q1-L/Q3-L use `SELECT DISTINCT` only over stable application identity/detail columns and semantic stable result attributes, never tag row IDs, connector IDs, or internal join demonstration columns. Q1-C/Q2-G/Q3-C use `COUNT(DISTINCT app.Object_ID)`, `COUNT(DISTINCT app.Object_ID)`, and `COUNT(DISTINCT src.Object_ID)` respectively; Q2 also groups by Vigencia. This prevents duplicate tags/connectors from double-counting while staying simple. Real EA acceptance must confirm `COUNT(DISTINCT ...)` support; if EA rejects that exact aggregate form, implementation may use an equally read-only basic derived-table fallback only after documenting the compatibility evidence and preserving identical semantics. |
| EA aliases | Aliases everywhere; nowhere; detail only | Add application `CLASSGUID`/`CLASSTYPE` only to Q1-L and Q3-L. Aggregates receive no fabricated representative aliases. |
| Semantic target | Hardcode `102`; name only; name + type + stereotype | Use **name + Class + ArchiMate_DataObject**. It is portable and disambiguates the target; `102` remains an oracle. |
| Execution authority | SQLite only; EA only; dual surface | EA is primary and mandatory. SQLite `mode=ro` is a secondary diagnostic oracle that cannot satisfy EA evidence. |
| Evidence organization | Screenshots mixed into docs; per-question folders; one flat folder | Use a question-oriented EA evidence folder plus a central evidence index. This keeps question → statement → result → model verification explicit and prevents screenshots from becoming context-free. |
| Optional Prolaborate | Q1 list; Q2 grouped counts; Q3 list | Begin with EA-validated **Q2-G** after all mandatory EA gates. Its two-category aggregate naturally fits a donut or bar chart and requires no element aliases. In Prolaborate use **Create Chart Widget → SQL Queries → Skip to Query → Query Configuration**; use **View Sample** when useful and execute the query. If it works, reuse Q2-G unchanged. If presentation aliases or result shape are needed, a minimally adapted derived SQL query may be used only when it preserves exact Q2-G semantics and results and the adaptation is documented; the EA Q2-G version remains authoritative. No aliases are selected now; `seriesproperty` remains excluded unless tagged-value color-palette requirements demand it. Optional work cannot block acceptance. |
| Human gates | Validate everything at the end; incremental promotion | Gate Q1, then Q2, then Q3, then documentation, optional Prolaborate, and final verification. Failures stop promotion of that question and produce no fictitious evidence. |

## Data and Evidence Flow

```text
Business question -> identified SELECT -> SQLite mode=ro diagnostic
        -> human EA SQL Search execution -> result/model comparison
        -> real screenshot/export -> evidence index -> technical explanation
```

Traceability is recorded per statement: question and statement ID, SQL-file location, EA Search display name, expected oracle, actual EA result, visible EA element/tag/connector comparison, execution-evidence filename, operator/date, and pass/fail outcome. The baseline is one genuine EA execution-evidence item per final statement, not an artificial requirement for exactly five PNG files: an EA screenshot or EA export is valid when it preserves the statement identity and result content. Every submission-facing supplemental evidence artifact added later, not only the five baseline items, MUST contain `Pino` in its filename; the evidence index records every such artifact and its statement ID. Extra evidence is allowed for navigation, tags, relations, and model verification. If an EA export replaces a named capture, it retains Pino naming and the evidence index maps it unambiguously to the statement ID. Q1-L and Q3-L additionally support direct EA navigation through their aliases. Q2 traceability is demonstrated by comparing grouped output with visible application Vigencia tags; no object alias is invented. Empty placeholders may be created during implementation, but they must say **pending** and must never be represented as executed evidence or artificial proof.

## Final Submission-Facing Files

Only `design.md` is created in this phase. A later Apply phase may create the following relative paths under the existing area while preserving `Exercise_2_queries/docs/challenge/`:

| Path | Action | Purpose |
|---|---|---|
| `Exercise_2_queries/queries/Pino_Exercise_2_EA_Governance_Queries.sql` | Create later | Delivery container, not an EA batch: exactly five identified SELECT statements in Q1-C, Q1-L, Q2-G, Q3-C, Q3-L order; each is copied, created, and run separately in EA. |
| `Exercise_2_queries/docs/Pino_Exercise_2_Technical_Explainer.md` | Create later | Tables, joins, predicates, uniqueness, assumptions, oracles, EA/SQLite roles, Node control, and reproducible execution steps. |
| `Exercise_2_queries/docs/Pino_Exercise_2_AI_Usage_Log.md` | Create later | At least five genuine interactions, each with eight explicit fields: ID, objective, tool, model, strategy/prompt, decision from response, related evidence, outcome. |
| `Exercise_2_queries/evidence/Pino_Exercise_2_Evidence_Index.md` | Create later | Evidence manifest and question → SQL → result → model traceability, including pending/failure status. |
| `Exercise_2_queries/evidence/ea/Pino_Q1_Categoria_ORO_Count.png` | Capture or replace with EA export later | Baseline genuine Q1-C EA execution evidence; an export replacement retains Pino naming and is mapped to Q1-C in the evidence index. |
| `Exercise_2_queries/evidence/ea/Pino_Q1_Categoria_ORO_List.png` | Capture or replace with EA export later | Baseline genuine Q1-L EA execution evidence; extra navigation or model evidence is allowed and indexed. |
| `Exercise_2_queries/evidence/ea/Pino_Q2_Vigencia_Grouped.png` | Capture or replace with EA export later | Baseline genuine Q2-G EA execution evidence; an export replacement retains Pino naming and is mapped to Q2-G in the evidence index. |
| `Exercise_2_queries/evidence/ea/Pino_Q3_Database_28_Count.png` | Capture or replace with EA export later | Baseline genuine Q3-C EA execution evidence; an export replacement retains Pino naming and is mapped to Q3-C in the evidence index. |
| `Exercise_2_queries/evidence/ea/Pino_Q3_Database_28_List.png` | Capture or replace with EA export later | Baseline genuine Q3-L EA execution evidence; extra relation, navigation, or model evidence is allowed and indexed. |
| `Exercise_2_queries/evidence/sqlite/Pino_Exercise_2_SQLite_Diagnostics.md` | Create later if diagnostics run | Commands/mode, actual read-only outputs, and comparison; never primary evidence. |
| `Exercise_2_queries/prolaborate/Pino_Exercise_2_Prolaborate_Q2.md` | Optional later | Q2 chart setup, business meaning, result mapping, and limitations. |
| `Exercise_2_queries/evidence/prolaborate/Pino_Exercise_2_Prolaborate_Q2.png` | Optional capture later | Real chart/dashboard evidence only. |

No separate README is required: the technical explainer is the submission index and links the SQL, evidence index, AI log, and optional Prolaborate artifact.

## Interfaces and Result Contracts

| Statement | Required logical columns | Cardinality contract |
|---|---|---|
| Q1-C | ORO application count | Exactly one aggregate row |
| Q1-L | `CLASSGUID`, `CLASSTYPE`, application name, Categoria | One row per unique ORO application |
| Q2-G | Vigencia value, application count | One row per observed requested lifecycle value |
| Q3-C | affected application count | Exactly one aggregate row |
| Q3-L | `CLASSGUID`, `CLASSTYPE`, source application name, target identification as useful | One row per unique affected application |

Column display labels may be human-readable, but identity, grouping, and filtering remain based on repository fields. Ordering is deterministic for lists and lifecycle groups; oracle comparison treats application-name sets as order-independent.

## Incremental Human Evidence Gates

| Gate | Required evidence and pass condition |
|---|---|
| 1 — Q1 SQLite | Optional secondary `mode=ro` execution yields count `8` and the exact eight-name set; failures do not authorize EA evidence claims. |
| 2 — Q1 EA | Human runs Q1-C/Q1-L in EA, obtains `8` and exact list, verifies visible Categoria tags, tests navigation from at least one list row, and captures real evidence. |
| 3 — Q2 | Secondary diagnostic may confirm `16/14` and missing `0`; human EA execution must show grouped `Vigente=16`, `Deprecado=14`, with visible tag comparison and no invented aliases/buckets. Equivalent Vigencia rows for the same application/state are deduplicated; a conflicting different Vigencia value is an anomaly that must be documented and understood before Q2 acceptance, with no arbitrary precedence or additional final SELECT. |
| 4 — Q3 | Secondary diagnostic may confirm `5`, exact list, and optionally the three Nodes as control; implementation validation must first confirm the supplied semantic target predicate resolves exactly one target, with `102` diagnostic only. A zero or multiple-target match is an anomaly that blocks Q3 acceptance and cannot be silently aggregated. Human EA execution must show count/list, validate Start-source/End-target Dependency semantics, and confirm Nodes are absent; target validation does not add a sixth final SELECT. |
| 5 — Documentation | Explainer, evidence index, and AI log cross-reference actual artifacts; all eight AI-log fields and at least five genuine interactions are present. No placeholder is described as proof. |
| 6 — Optional Prolaborate | Only after Gates 2–4: begin with EA-validated Q2-G and use **Create Chart Widget → SQL Queries → Skip to Query → Query Configuration**. Use **View Sample** when useful, execute the query before chart settings, then configure donut/bar and capture real evidence. Reuse Q2-G unchanged if it works. If presentation aliases or shape are needed, MAY minimally adapt derived SQL while preserving exact Q2-G semantics/results and documenting the adaptation; EA Q2-G remains authoritative. No aliases are selected now; `seriesproperty` is excluded unless tagged-value color-palette requirements demand it. The local V5 guide documents this order. |
| 7 — Verify | Review exactly five SELECT statements, separate EA copy/create/run blocks rather than batch execution, read-only semantics, relative links, filenames containing Pino, one genuine EA execution-evidence item per statement with indexed extras where present, evidence authenticity, oracle agreement, and untouched protected areas/QEA. |

## Testing Strategy

| Layer | What to test | Approach |
|---|---|---|
| Static review | Five-statement distribution, SELECT-only behavior, prohibited fields/predicates, aliases, qualified multi-table identity, list DISTINCT stability, semantic target, paths, and individual EA execution blocks | Manual inspection of SQL and documentation; distinguish executable SQL from documentary mentions, verify copied executable text begins with `SELECT`, and verify every submission-facing supplemental evidence artifact filename contains Pino and is indexed with its statement ID. |
| Diagnostic integration | Derived counts/lists, uniqueness behavior, Q2 conflicting-state anomalies, and Q3 target cardinality | SQLite URI `mode=ro` only; compare Q1 `8/list`, Q2 `16/14` plus missing `0`, Q3 `5/list`; confirm Q3 semantic target resolves exactly one row; Nodes optional control. Diagnostic or implementation validation does not add a final SELECT. |
| Primary acceptance | EA dialect, rendering, navigation, and visible-model agreement | Human EA SQL Search execution and one genuine EA screenshot or export per mandatory statement, with extra navigation/tag/relation/model evidence indexed when captured. |
| Documentation acceptance | Reproducibility and evidence lineage | Follow evidence index from each question to statement, actual result, visible model check, and artifact. |

Real EA validation points are: statement parsing; individual copy/create/run behavior with executable text beginning `SELECT`; `COUNT(DISTINCT app.Object_ID)` and `COUNT(DISTINCT src.Object_ID)` support; tag `Value` field handling; Q2 conflicting-state anomaly handling; Q3 semantic target cardinality; aggregate rendering without aliases; list icons and double-click navigation; accented names; deterministic ordering; and screenshot/export readability. Any failure must be recorded and corrected before that gate passes.

## Spec-to-Decision Matrix

| Spec / requirement group | Design decision | Required evidence |
|---|---|---|
| Category: application universe and Categoria source | Class + ApplicationComponent; tag join by Object_ID | SQL review, Q1 EA visible-tag comparison |
| Category: aggregate/list and uniqueness | Q1-C + Q1-L; count distinct identity and distinct detail rows | `8`, exact list, navigation evidence |
| Category: identifiers/docs/read-only/EA | Identified SELECTs, explainer, EA primary | SQL/explainer review and real EA evidence |
| Category: AI log/naming/optional Prolaborate | Dedicated eight-field log; Pino filenames; Q2 optional chart | AI-log audit; path audit; optional real chart |
| Lifecycle: tag not Status and application scope | Vigencia tag join; no Status; Class + ApplicationComponent | SQL review and visible Vigencia tags |
| Lifecycle: grouped requested values/missing behavior | One Q2-G; no synthetic missing bucket or aliases; equivalent same-state rows deduplicated, conflicting states block acceptance pending understanding | `16/14`; explainer records missing `0` and any anomaly |
| Lifecycle: uniqueness/docs/read-only/EA | Count distinct application per group; EA primary | Q2 EA screenshot/export and model comparison |
| Impact: orientation and Dependency | Start source → End target; Dependency required | SQL review and EA connector/model comparison |
| Impact: semantic target and 102 oracle | Name + Class + DataObject; never filter on `102`; supplied predicate must resolve exactly one target before acceptance | SQL/explainer review; target visible in EA; cardinality validation/anomaly record |
| Impact: source scope and Node control | Application source only; Nodes documented separately | Q3 exact list; Node absence; optional control diagnostic |
| Impact: Direction optional | No Direction predicate | Static SQL review |
| Impact: aggregate/list and uniqueness | Q3-C + Q3-L; count distinct identity and distinct detail rows | `5`, exact list, navigation evidence |
| Impact: identifiers/docs/read-only/EA | Identified SELECTs, explainer, EA primary | Q3 EA screenshots/export and traceability index |

## Threat Matrix

N/A — this design introduces no routing, shell command, subprocess, VCS/PR automation, executable-file classification, or process-integration boundary. It defines read-only SQL and human evidence/documentation flows only.

## Migration / Rollout

No data migration is required. Rollout consists solely of adding relative-path deliverables after each gate passes. Rollback removes only newly introduced Exercise 2 deliverables; it never changes the QEA, challenge brief, Addino/Exercise 1, proposal, specs, exploration, configuration, or RDD artifacts.

## Risks and Open Questions

- **Human dependency:** mandatory completion remains blocked until a human with EA access executes all five statements and captures genuine evidence.
- **EA aggregate compatibility:** `COUNT(DISTINCT ...)` must be proven in the actual EA SQL Search environment. The semantic uniqueness contract is fixed even if a basic compatibility rewrite becomes necessary.
- **Evidence format:** one genuine EA execution-evidence item per statement is the baseline, not exactly five PNG files. A real EA screenshot or export is valid only if it clearly preserves statement identity and result content in the evidence index; extra navigation, tag, relation, and model evidence is allowed and indexed. An export that replaces a named capture retains Pino naming and unambiguous ID mapping.
- No design-blocking open question remains. Optional Prolaborate access does not affect mandatory acceptance.
