# SQLite Diagnostics — Pino Exercise 2

**Date:** 2026-08-13  
**Database:** `C:\Proagile\Repositorio Pasantias.qea`  
**Access mode:** Read-only SQLite URI `file:C:\Proagile\Repositorio Pasantias.qea?mode=ro`  
**Tool:** Python 3 `sqlite3` module (client version 3.50.4)  
**Scope:** This diagnostic transcript covers WU1 scaffolding, WU2 Q1 through Gate Q1 (EA_PASS), WU3 Q2 through Gate Q2 (EA_PASS), and WU4 Q3 through Gate Q3 (EA_PASS). All three mandatory human EA gates have passed; SQLite remains the secondary diagnostic surface only.

> **Important:** SQLite is the secondary diagnostic surface only. These results do NOT satisfy the mandatory Enterprise Architect SQL Search evidence requirement. Real EA evidence is provided by the operator PDFs indexed in `../Pino_Exercise_2_Evidence_Index.md`.

## Read-only mode proof

The connection URI includes `?mode=ro`. An actual attempt to create a table in that connection fails as expected:

```text
Read-only mode confirmed: attempt to write a readonly database
```

## Q1-C — Categoria ORO — Count

### SQL

```sql
SELECT COUNT(DISTINCT app.Object_ID) AS ORO_Application_Count
FROM t_object app
INNER JOIN t_objectproperties tag ON tag.Object_ID = app.Object_ID
WHERE app.Object_Type = 'Class'
  AND app.Stereotype = 'ArchiMate_ApplicationComponent'
  AND tag.Property = 'Categoria'
  AND tag.Value = 'ORO';
```

### Result

```text
Columns: ['ORO_Application_Count']
(8,)
Row count: 1
```

### Oracle comparison

| Expected | Actual | Match |
|---|---|---|
| 8 | 8 | Yes |

## Q1-L — Categoria ORO — List

### SQL

```sql
SELECT DISTINCT
    app.ea_guid AS CLASSGUID,
    app.Object_Type AS CLASSTYPE,
    app.Name AS Application_Name,
    tag.Value AS Categoria
FROM t_object app
INNER JOIN t_objectproperties tag ON tag.Object_ID = app.Object_ID
WHERE app.Object_Type = 'Class'
  AND app.Stereotype = 'ArchiMate_ApplicationComponent'
  AND tag.Property = 'Categoria'
  AND tag.Value = 'ORO'
ORDER BY app.Name;
```

### Result

```text
Columns: ['CLASSGUID', 'CLASSTYPE', 'Application_Name', 'Categoria']
('{16D2CFAB-6B13-4d39-85E6-BECF41775C2B}', 'Class', 'Aplicación 1', 'ORO')
('{F3F1CC31-B691-42b9-8E1A-3F8ED82A9A83}', 'Class', 'Aplicación 2', 'ORO')
('{FEF514B9-63DF-4884-9527-20E5AA2CD4F7}', 'Class', 'Aplicación 20', 'ORO')
('{4CAE20AF-49E6-4f44-AC87-BCAAD3F8ED2E}', 'Class', 'Aplicación 29', 'ORO')
('{9957E36C-98DE-4f55-8453-109BA4C56193}', 'Class', 'Aplicación 3', 'ORO')
('{793E7FE6-C340-4d2f-8324-A31D498214EF}', 'Class', 'Aplicación 4', 'ORO')
('{B0985987-B36D-4d74-8A42-90FF23FEDEDB}', 'Class', 'Aplicación 6', 'ORO')
('{C0BC8085-C989-49ae-A367-17A15A306FC5}', 'Class', 'Aplicación 8', 'ORO')
Row count: 8
```

### UTF-8 byte verification

The accented character is stored as the correct UTF-8 sequence `C3 B3` (`ó`):

| Application | HEX(Name) |
|---|---|
| Aplicación 1 | `41706C6963616369C3B36E2031` |
| Aplicación 2 | `41706C6963616369C3B36E2032` |
| Aplicación 3 | `41706C6963616369C3B36E2033` |
| Aplicación 4 | `41706C6963616369C3B36E2034` |
| Aplicación 6 | `41706C6963616369C3B36E2036` |
| Aplicación 8 | `41706C6963616369C3B36E2038` |
| Aplicación 20 | `41706C6963616369C3B36E203230` |
| Aplicación 29 | `41706C6963616369C3B36E203239` |

### Oracle comparison

| Expected set | Actual set | Match |
|---|---|---|
| Aplicación 1, 2, 3, 4, 6, 8, 20, 29 | Aplicación 1, 2, 3, 4, 6, 8, 20, 29 | Yes (order-independent) |

### Uniqueness and scope checks

- Distinct `Object_ID` count in Q1-L: **8**
- Rows satisfying the application scope predicate: **8** (no non-application rows)
- Each row exposes `CLASSGUID` and `CLASSTYPE` for EA Search navigation compatibility.

## Q2-G — Vigencia — Grouped

### SQL

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

### Result

```text
Columns: ['Vigencia', 'Application_Count']
('Deprecado', 14)
('Vigente', 16)
Row count: 2
```

### Oracle comparison

| Vigencia | Expected | Actual | Match |
|---|---|---|---|
| Vigente | 16 | 16 | Yes |
| Deprecado | 14 | 14 | Yes |

### Missing / empty / N / A diagnostic

SQL to count applications whose `Vigencia` tag is NULL, empty, or `N/A`:

```sql
SELECT COUNT(DISTINCT app.Object_ID) AS Missing_Apps
FROM t_object app
INNER JOIN t_objectproperties tag
    ON tag.Object_ID = app.Object_ID
WHERE app.Object_Type = 'Class'
  AND app.Stereotype = 'ArchiMate_ApplicationComponent'
  AND tag.Property = 'Vigencia'
  AND (tag.Value IS NULL OR tag.Value = '' OR tag.Value = 'N/A');
```

Result:

```text
Columns: ['Missing_Apps']
(0,)
```

### Conflicting-Vigencia anomaly diagnostic

SQL to detect any application carrying more than one distinct `Vigencia` value:

```sql
SELECT app.Object_ID, app.Name, COUNT(DISTINCT tag.Value) AS Distinct_Vigencia_Values
FROM t_object app
INNER JOIN t_objectproperties tag
    ON tag.Object_ID = app.Object_ID
WHERE app.Object_Type = 'Class'
  AND app.Stereotype = 'ArchiMate_ApplicationComponent'
  AND tag.Property = 'Vigencia'
GROUP BY app.Object_ID
HAVING COUNT(DISTINCT tag.Value) > 1;
```

Result:

```text
Row count: 0
```

### Unrestricted value check

All application `Vigencia` values observed in the repository:

```text
Columns: ['Value', 'Count']
('Deprecado', 14)
('Vigente', 16)
```

### Data-quality summary

- NULL/empty/`N/A` application `Vigencia` values: **0**
- Applications with conflicting `Vigencia` values: **0**
- Only requested states observed: **Vigente** and **Deprecado**

## Q3 — Base de Datos 28 impact

### Target cardinality and Object_ID 102 oracle

SQL to verify the semantic target predicate resolves to exactly one row:

```sql
SELECT Object_ID, Name, Object_Type, Stereotype, ea_guid
FROM t_object
WHERE Name = 'Base de Datos 28'
  AND Object_Type = 'Class'
  AND Stereotype = 'ArchiMate_DataObject';
```

Result:

```text
Columns: ['Object_ID', 'Name', 'Object_Type', 'Stereotype', 'ea_guid']
(102, 'Base de Datos 28', 'Class', 'ArchiMate_DataObject', '{7C82132F-1D34-45a2-97BE-1C8D1A3F9665}')
Row count: 1
```

| Check | Expected | Actual | Status |
|---|---|---|---|
| Semantic target cardinality | 1 | 1 | OK |
| Observed Object_ID | Diagnostic only | 102 | Recorded as oracle evidence, not query filter |

### Q3-C — Affected application count

### SQL

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

### Result

```text
Columns: ['COUNT(DISTINCT src.Object_ID)']
(5,)
Row count: 1
```

### Oracle comparison

| Expected | Actual | Match |
|---|---|---|
| 5 | 5 | Yes |

### Q3-L — Affected application list

### SQL

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

### Result

```text
Columns: ['CLASSGUID', 'CLASSTYPE', 'Source_Application', 'Target_Database']
('{18B2499A-2C81-4662-AD21-67B09157F84A}', 'Class', 'Aplicación 5', 'Base de Datos 28')
('{085D0E19-7D79-4770-BF76-0938D2F0911F}', 'Class', 'Aplicación 12', 'Base de Datos 28')
('{26D466BB-764B-42fb-B538-1109DE0BD797}', 'Class', 'Aplicación 22', 'Base de Datos 28')
('{57EA0333-E1C8-4423-8211-3762A37A908D}', 'Class', 'Aplicación 25', 'Base de Datos 28')
('{2E366CA6-5AAF-4ce9-BDC6-B215AA9E0BAD}', 'Class', 'Aplicación 27', 'Base de Datos 28')
Row count: 5
```

### Oracle comparison

| Expected set | Actual set | Match |
|---|---|---|
| Aplicación 5, 12, 22, 25, 27 | Aplicación 5, 12, 22, 25, 27 | Yes (order-independent) |

### Uniqueness and scope checks

- Distinct `src.Object_ID` count in Q3-L: **5**
- Q3-C distinct count: **5**
- Match between Q3-C and Q3-L distinct counts: **Yes**
- Source scope check: all rows are `Object_Type='Class'` / `Stereotype='ArchiMate_ApplicationComponent'`
- Target scope check: all rows target `Base de Datos 28` with `Object_Type='Class'` / `Stereotype='ArchiMate_DataObject'`
- Orientation check: `rel.Start_Object_ID = src.Object_ID` and `rel.End_Object_ID = tgt.Object_ID`
- Reverse orientation count (Start=target, End=source): **0** — confirms the chosen orientation
- `Direction` predicate: **not used** in Q3-C or Q3-L
- `Object_ID=102`: **not used** as a filter; only documented as observed oracle

### Negative control — non-application sources

SQL to list non-application sources that also depend on Base de Datos 28:

```sql
SELECT DISTINCT
    src.Object_Type,
    src.Stereotype,
    src.Name
FROM t_object src
INNER JOIN t_connector rel
    ON rel.Start_Object_ID = src.Object_ID
INNER JOIN t_object tgt
    ON tgt.Object_ID = rel.End_Object_ID
WHERE NOT (src.Object_Type = 'Class' AND src.Stereotype = 'ArchiMate_ApplicationComponent')
  AND rel.Connector_Type = 'Dependency'
  AND tgt.Name = 'Base de Datos 28'
  AND tgt.Object_Type = 'Class'
  AND tgt.Stereotype = 'ArchiMate_DataObject'
ORDER BY src.Name;
```

Result:

```text
Columns: ['Object_Type', 'Stereotype', 'Name']
('Node', 'ArchiMate_Node', 'Servidor 19')
('Node', 'ArchiMate_Node', 'Servidor 2')
('Node', 'ArchiMate_Node', 'Servidor 5')
Row count: 3
```

Negative-control comparison:

| Expected Node source | Observed | Status |
|---|---|---|
| Servidor 2 | Servidor 2 | OK |
| Servidor 5 | Servidor 5 | OK |
| Servidor 19 | Servidor 19 | OK |

These Node sources are documented as negative-control evidence only and are excluded from the mandatory Q3 result by the application source predicate.

### Connector type and Direction diagnostic

All relations ending at Base de Datos 28 in the repository use `Connector_Type='Dependency'`:

```text
Columns: ['Connector_Type', 'Count']
('Dependency', 8)
```

Sampled `Direction` values for these Dependency connectors (diagnostic only, not a query filter):

```text
Columns: ['Connector_ID', 'Direction', 'Connector_Type', 'SourceName', 'TargetName']
(11, 'Source -> Destination', 'Dependency', 'Aplicación 12', 'Base de Datos 28')
(32, 'Source -> Destination', 'Dependency', 'Aplicación 22', 'Base de Datos 28')
(38, 'Source -> Destination', 'Dependency', 'Aplicación 25', 'Base de Datos 28')
(40, 'Source -> Destination', 'Dependency', 'Aplicación 27', 'Base de Datos 28')
(51, 'Source -> Destination', 'Dependency', 'Aplicación 5', 'Base de Datos 28')
```

## Notes

- No `UPDATE`, `INSERT`, `DELETE`, `ALTER`, `DROP`, or `CREATE` statements were executed against the QEA file.
- Q3-C and Q3-L are implemented, diagnosed, and promoted to EA_PASS by the operator-run human gate Q3.
- All five final SELECT statements use the EA primary form; no compatibility fallback was required.
