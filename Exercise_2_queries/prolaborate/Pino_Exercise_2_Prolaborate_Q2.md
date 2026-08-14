# Pino Exercise 2 — Prolaborate Q2 Dashboard

## Dashboard

**Dashboard name:** Gobierno de Aplicaciones - Pasantías - Pino

## Business question

How many applications are in each `Vigencia` state (`Vigente` / `Deprecado`)?

## EA authority

**Q2-G — Vigencia — Grouped** is the functional EA authority for the chart semantics and results:

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

- **Vigente:** 16 applications
- **Deprecado:** 14 applications

Q2-G executed directly in Prolaborate correctly with **Deprecado=14** and **Vigente=16**. Its aggregate result shape did not bind the widget. The Q2-G SQL remains the functional EA authority; any generated presentation query is not one of the official five SELECT statements.

## Chart configuration

- **Widget type:** Chart Widget
- **Path:** Create Chart Widget → SQL Queries → Skip to Query → Query Configuration
- **Designer configuration:** Class / ApplicationComponent
- **Tag / values:** Vigencia values
- **Series shape:** Generate Series per application
- **Chart types used:** Donut and Bar

## Execution notes

- Designer Configuration created a per-application presentation query with `Series=Vigencia`, preserving exactly the Q2-G semantics and results.
- During widget preview, Prolaborate displayed a generic **Open/Closed 75/25 preview binding**. That aggregate shape did not bind the widget.
- The final widgets use the Designer shape. No `seriesproperty` was introduced; color palette configuration did not require it.

## Rendered result

- **Donut chart:** Deprecado 46.67% / Vigente 53.33%
- **Bar chart:** 14 / 16

## Semantics

The final widgets preserve exactly the Q2-G semantics and results:

| Vigencia | Count | Percentage |
|---|---|---|
| Vigente | 16 | 53.33% |
| Deprecado | 14 | 46.67% |

## Evidence

Real Prolaborate evidence is included in the operator-provided PDF:

- `Exercise_2_queries/Pino_Evidencias_Funcionales_Queries.pdf`

## Procedural note

This optional Prolaborate follow-on was executed after all three mandatory EA gates (Q1, Q2, Q3) had already passed, and before the final Phase 6 documentation consolidation. It is non-blocking and does not affect mandatory acceptance. Task 7.7 (record absence if Prolaborate is unavailable) is marked **N/A** because Prolaborate access was available and this follow-on was completed.
