# Pino Exercise 2 — Technical Explainer

This document explains the SELECT-only SQL query set developed for Exercise 2 of the Proagile 2026 challenge. The queries answer three governance questions over the Enterprise Architect repository. All three mandatory human EA gates have passed; real EA evidence is provided by the operator PDFs indexed in the Evidence Index.

## Quick path

1. Open `../queries/Pino_Exercise_2_EA_Governance_Queries.sql`.
2. Copy each `SELECT` block individually into EA SQL Search.
3. Run Q1-C and Q1-L; compare the results with the oracles below and with visible `Categoria` tags in EA.
4. After Q1 passes, run Q2-G; compare the grouped counts with visible `Vigencia` tags.
5. After Q2 passes, run Q3-C and Q3-L.
6. Record real EA evidence in the Evidence Index.

## Repository tables used

| Table | Role |
|---|---|
| `t_object` | Contains model elements (applications, data objects, nodes, etc.). |
| `t_objectproperties` | Contains tagged values such as `Categoria` and `Vigencia`, joined by `Object_ID`. |
| `t_connector` | Contains relationships; used for Q3 impact analysis. |

## Application criteria

Every query that targets applications uses the defensive predicate:

```sql
app.Object_Type = 'Class'
AND app.Stereotype = 'ArchiMate_ApplicationComponent'
```

This excludes `ArchiMate_DataObject`, `ArchiMate_BusinessActor`, `ArchiMate_Node`, and any other element type.

## Q1 — Applications with Categoria ORO

### Question

- Q1-C: How many applications have the tagged value `Categoria = ORO`?
- Q1-L: Which applications have the tagged value `Categoria = ORO`?

### Tables and joins

- `t_object` aliased as `app` (application).
- `t_objectproperties` aliased as `tag` (tagged value).
- Join: `tag.Object_ID = app.Object_ID`.

### Filters

- `app.Object_Type = 'Class'`
- `app.Stereotype = 'ArchiMate_ApplicationComponent'`
- `tag.Property = 'Categoria'`
- `tag.Value = 'ORO'`

### Uniqueness rationale

`Categoria` has exactly one row per application in the supplied QEA, but the query still uses `COUNT(DISTINCT app.Object_ID)` in Q1-C and `SELECT DISTINCT` in Q1-L so it remains correct if duplicate tag rows ever appear.

### Q1-C statement

```sql
SELECT
    COUNT(DISTINCT app.Object_ID) AS ORO_Application_Count
FROM t_object app
INNER JOIN t_objectproperties tag
    ON tag.Object_ID = app.Object_ID
WHERE app.Object_Type = 'Class'
  AND app.Stereotype = 'ArchiMate_ApplicationComponent'
  AND tag.Property = 'Categoria'
  AND tag.Value = 'ORO';
```

### Q1-L statement

```sql
SELECT DISTINCT
    app.ea_guid AS CLASSGUID,
    app.Object_Type AS CLASSTYPE,
    app.Name AS Application_Name,
    tag.Value AS Categoria
FROM t_object app
INNER JOIN t_objectproperties tag
    ON tag.Object_ID = app.Object_ID
WHERE app.Object_Type = 'Class'
  AND app.Stereotype = 'ArchiMate_ApplicationComponent'
  AND tag.Property = 'Categoria'
  AND tag.Value = 'ORO'
ORDER BY app.Name;
```

### Why `CLASSGUID` and `CLASSTYPE` only in Q1-L

EA SQL Search uses `CLASSGUID` (the element `ea_guid`) and `CLASSTYPE` (the `Object_Type`) to render row icons and enable double-click navigation. Aggregate rows (Q1-C) do not represent a single element, so they must not invent representative aliases.

### Oracle

| Statement | Expected | SQLite diagnostic result |
|---|---|---|
| Q1-C | 8 | 8 |
| Q1-L | Aplicación 1, 2, 3, 4, 6, 8, 20, 29 | 8 rows matching exactly |

### EA validation status

**EA_PASS.** The operator executed Q1-C and Q1-L in EA SQL Search and confirmed:

- Q1-C returned **8**, matching the oracle.
- Q1-L returned the exact eight applications: **Aplicación 1, Aplicación 2, Aplicación 3, Aplicación 4, Aplicación 6, Aplicación 8, Aplicación 20, Aplicación 29**.
- All listed rows are applications (`Object_Type='Class'` / `Stereotype='ArchiMate_ApplicationComponent'`); no non-application elements appeared.
- The visible `Categoria` tags matched the ORO filter.
- At least one list row was opened to confirm EA navigation via the `CLASSGUID`/`CLASSTYPE` aliases.
- Real EA evidence: `../Pino_Ejercicio_2_Informe_Principal_Queries_EA.pdf` (operator-provided EA PDF).
- **EA accepted form:** EA primary form accepted — no fallback required.
- No compatibility fallback was required.

## Q2 — Applications by Vigencia

### Question

Q2-G: How many applications are in each `Vigencia` state (`Vigente` / `Deprecado`)?

### Tables and joins

- `t_object` aliased as `app` (application).
- `t_objectproperties` aliased as `tag` (tagged value).
- Join: `tag.Object_ID = app.Object_ID`.

### Filters

- `app.Object_Type = 'Class'`
- `app.Stereotype = 'ArchiMate_ApplicationComponent'`
- `tag.Property = 'Vigencia'`
- `tag.Value IN ('Vigente', 'Deprecado')` — only the requested observed states

### Why no `Status`, no synthetic buckets, and no aliases

The `Vigencia` lifecycle state is stored only in `t_objectproperties`; `t_object.Status` is intentionally not used. The query reports only the observed requested values and does not manufacture rows for `NULL`, empty, or `N/A`. Because the result is grouped, no single element represents a group, so `CLASSGUID` and `CLASSTYPE` are not invented.

### Q2-G statement

```sql
SELECT
    tag.Value AS Vigencia,
    COUNT(DISTINCT app.Object_ID) AS Application_Count
FROM t_object app
INNER JOIN t_objectproperties tag
    ON tag.Object_ID = app.Object_ID
WHERE app.Object_Type = 'Class'
  AND app.Stereotype = 'ArchiMate_ApplicationComponent'
  AND tag.Property = 'Vigencia'
  AND tag.Value IN ('Vigente', 'Deprecado')
GROUP BY tag.Value
ORDER BY tag.Value;
```

### Oracle

| Vigencia | Expected | SQLite diagnostic result |
|---|---|---|
| Vigente | 16 | 16 |
| Deprecado | 14 | 14 |

### Data-quality checks

| Check | Expected | Actual |
|---|---|---|
| NULL/empty/`N/A` application `Vigencia` values | 0 | 0 |
| Applications with conflicting `Vigencia` values | 0 | 0 |

### EA validation status

**EA_PASS.** The operator executed Q2-G in EA SQL Search and confirmed:

- `Vigente` = **16** and `Deprecado` = **14**, matching the oracle.
- Sampled visible `Vigencia` tags on applications matched the grouped result.
- No synthetic `NULL`, empty, `N/A`, or `Unknown` bucket was invented.
- No representative-object `CLASSGUID` or `CLASSTYPE` alias was invented for the grouped rows.
- Real EA evidence: `../Pino_Ejercicio_2_Informe_Principal_Queries_EA.pdf` (operator-provided EA PDF).
- **EA accepted form:** EA primary form accepted — no fallback required.
- No compatibility fallback was required.

## Q3 — Applications impacted by Base de Datos 28

### Question

- Q3-C: How many applications depend on `Base de Datos 28`?
- Q3-L: Which applications depend on `Base de Datos 28`?

### Tables and joins

- `t_object` aliased as `src` (source application).
- `t_connector` aliased as `rel` (dependency relationship).
- `t_object` aliased as `tgt` (target database).
- Joins:
  - `rel.Start_Object_ID = src.Object_ID` — the dependency source is the application.
  - `rel.End_Object_ID = tgt.Object_ID` — the dependency target is the database.

### Filters

- `src.Object_Type = 'Class'`
- `src.Stereotype = 'ArchiMate_ApplicationComponent'`
- `rel.Connector_Type = 'Dependency'`
- `tgt.Name = 'Base de Datos 28'`
- `tgt.Object_Type = 'Class'`
- `tgt.Stereotype = 'ArchiMate_DataObject'`
- No `Direction` predicate.
- No `Object_ID = 102` filter.

### Semantic target identification

The target database is identified by name, type, and stereotype rather than by its primary key. Before Q3 acceptance, a read-only diagnostic confirmed the predicate `Name='Base de Datos 28' AND Object_Type='Class' AND Stereotype='ArchiMate_DataObject'` resolves to exactly one row. The observed `Object_ID` is **102**, recorded only as supplied-QEA oracle evidence.

### Why no `Direction` and no `Object_ID=102`

- `Direction` is not a mandatory filter; the orientation is enforced by `Start_Object_ID` → source and `End_Object_ID` → target.
- `Object_ID=102` is repository-specific; using the semantic predicate makes the query portable and disambiguates the target.

### Q3-C statement

```sql
SELECT
    COUNT(DISTINCT src.Object_ID)
FROM t_object src
INNER JOIN t_connector rel
    ON rel.Start_Object_ID = src.Object_ID
INNER JOIN t_object tgt
    ON tgt.Object_ID = rel.End_Object_ID
WHERE src.Object_Type = 'Class'
  AND src.Stereotype = 'ArchiMate_ApplicationComponent'
  AND rel.Connector_Type = 'Dependency'
  AND tgt.Name = 'Base de Datos 28'
  AND tgt.Object_Type = 'Class'
  AND tgt.Stereotype = 'ArchiMate_DataObject';
```

### Q3-L statement

```sql
SELECT DISTINCT
    src.ea_guid AS CLASSGUID,
    src.Object_Type AS CLASSTYPE,
    src.Name AS Source_Application,
    tgt.Name AS Target_Database
FROM t_object src
INNER JOIN t_connector rel
    ON rel.Start_Object_ID = src.Object_ID
INNER JOIN t_object tgt
    ON tgt.Object_ID = rel.End_Object_ID
WHERE src.Object_Type = 'Class'
  AND src.Stereotype = 'ArchiMate_ApplicationComponent'
  AND rel.Connector_Type = 'Dependency'
  AND tgt.Name = 'Base de Datos 28'
  AND tgt.Object_Type = 'Class'
  AND tgt.Stereotype = 'ArchiMate_DataObject'
ORDER BY src.Name;
```

### Oracle

| Statement | Expected | SQLite diagnostic result |
|---|---|---|
| Q3-C | 5 | 5 |
| Q3-L | Aplicación 5, 12, 22, 25, 27 | 5 rows matching exactly |

### Node negative-control evidence

Non-application sources that also depend on `Base de Datos 28`:

| Source | Object_Type | Stereotype |
|---|---|---|
| Servidor 2 | Node | ArchiMate_Node |
| Servidor 5 | Node | ArchiMate_Node |
| Servidor 19 | Node | ArchiMate_Node |

These Node sources are excluded from Q3-C and Q3-L by the application source predicate. They are documented as negative-control evidence only.

### EA validation status

**EA_PASS.** The operator executed Q3-C and Q3-L in EA SQL Search and confirmed:

- Q3-C returned **5**, matching the oracle.
- Q3-L returned the exact five applications: **Aplicación 5, Aplicación 12, Aplicación 22, Aplicación 25, Aplicación 27**.
- All listed rows are application sources (`Object_Type='Class'` / `Stereotype='ArchiMate_ApplicationComponent'`); Node sources were absent.
- Dependency orientation was verified as `Start_Object_ID` → source application, `End_Object_ID` → target `Base de Datos 28`.
- Navigation from a list row (Aplicación 22 and related links) was verified via the `CLASSGUID`/`CLASSTYPE` aliases.
- Real EA evidence: `Pino_Evidencias_Funcionales_Queries.pdf` (operator-provided EA PDF).
- **EA accepted form:** EA primary form accepted — no fallback required.
- No compatibility fallback was required.

## Execution surfaces

| Surface | Role | Status for Q1 | Status for Q2 | Status for Q3 |
|---|---|---|---|---|
| Enterprise Architect SQL Search | Primary acceptance and evidence | EA_PASS | EA_PASS | EA_PASS |
| SQLite `mode=ro` on local QEA | Secondary diagnostic validation | Completed for Q1-C and Q1-L | Completed for Q2-G; no anomalies | Completed for Q3-C and Q3-L; target cardinality 1; Node control matched |

## Real EA evidence

Real EA evidence was provided by the operator as PDF files physically present in the workspace:

| PDF file | Statements covered | Location |
|---|---|---|
| `Pino_Ejercicio_2_Informe_Principal_Queries_EA.pdf` | Q1-C, Q1-L, Q2-G, Q3-C, Q3-L, Prolaborate | `../Pino_Ejercicio_2_Informe_Principal_Queries_EA.pdf` |
| `Pino_Evidencias_Funcionales_Queries.pdf` | Functional test evidence | `Pino_Evidencias_Funcionales_Queries.pdf` |

These PDFs are treated as read-only real evidence; no screenshot filename, page number, or image content has been invented.

## Post-gate compatibility audit

| Statement | EA accepted form | Dialect rejection | Fallback used | Preemptive fallback remaining |
|---|---|---|---|---|
| Q1-C | Primary `COUNT(DISTINCT app.Object_ID)` | None | No | No |
| Q1-L | Primary `SELECT DISTINCT` with `CLASSGUID`/`CLASSTYPE` | None | No | No |
| Q2-G | Primary grouped `COUNT(DISTINCT app.Object_ID)` | None | No | No |
| Q3-C | Primary `COUNT(DISTINCT src.Object_ID)` | None | No | No |
| Q3-L | Primary `SELECT DISTINCT` with `CLASSGUID`/`CLASSTYPE` | None | No | No |

**Audit conclusion:** All five statements passed in their primary EA form. No fallback was introduced, no statement was rerun during the audit, and no preemptive compatibility workaround remains.

## Optional Prolaborate follow-on

Prolaborate V5 was available and the optional follow-on was completed after all mandatory EA gates and before final documentation consolidation. It is non-blocking and does not affect mandatory acceptance.

- **Dashboard:** Gobierno de Aplicaciones - Pasantías - Pino
- **Business question:** How many applications are in each Vigencia state (Vigente / Deprecado)?
- **EA authority:** Q2-G (Vigente=16, Deprecado=14)
- **Chart path:** Create Chart Widget → SQL Queries → Skip to Query → Query Configuration → View Sample → Execute → configure Donut/Bars
- **Designer configuration:** Class / ApplicationComponent; tag Vigencia values; generate Series shape
- **Rendered result:** Donut Deprecado 46.67% / Vigente 53.33%; bars 14/16
- **Adaptation:** The Prolaborate widget preview showed a generic Open/Closed 75/25 binding; the final chart uses the EA-validated Q2-G semantics (no `seriesproperty`).
- **Documentation:** `Pino_Exercise_2_Prolaborate_Q2.md`
- **Real evidence:** `../Pino_Ejercicio_2_Informe_Principal_Queries_EA.pdf`

## Final state note

- Gate Q1: PASS
- Gate Q2: PASS
- Gate Q3: PASS
- Optional Prolaborate: COMPLETE
- All five mandatory SELECT statements: EA primary form accepted — no fallback required.
- QEA repository remains read-only; no mutations executed.

## Active work-unit mapping

| Work unit | Scope | Gate/dependency |
|---|---|---|
| WU1 | Scaffold | Completed as part of the first apply increment. |
| WU2 | Q1 | Gate Q1 passed. |
| WU3 | Q2 | Gate Q2 passed. |
| WU4 | Q3 | HUMAN GATE Q3 PASS; Q3-C EA_PASS; Q3-L EA_PASS; Phase 4 complete; no fallback. |
| WU5 | Mandatory documentation | After mandatory gates. |
| WU6 | Optional Prolaborate | Optional follow-on after mandatory EA validation. |
| WU7 | Pre-verify | Final repository and evidence guard. |

## Assumptions and constraints

- The QEA file remains read-only; no `UPDATE`, `INSERT`, `DELETE`, `ALTER`, `DROP`, or `CREATE` statements are executed.
- `Categoria` and `Vigencia` are stored exclusively in `t_objectproperties`.
- `t_object.Status` is not used as a lifecycle proxy.
- The impact relationship is oriented `Start_Object_ID` → source, `End_Object_ID` → target.

## References

- `../queries/Pino_Exercise_2_EA_Governance_Queries.sql`
- `Pino_Exercise_2_Evidence_Index.md`
- `Pino_Exercise_2_SQLite_Diagnostics.md`
