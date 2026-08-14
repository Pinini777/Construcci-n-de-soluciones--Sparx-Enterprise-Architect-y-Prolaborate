-- =============================================================================
-- Pino Exercise 2 — EA Governance Queries
-- Delivery container for Enterprise Architect SQL Search
-- =============================================================================
-- This file is NOT an EA execution batch. Each SELECT block is copied,
-- created, and run separately in EA SQL Search. Every executable block begins
-- with SELECT so the copied text can be pasted directly into the EA Search UI.
--
-- Primary validation surface: Enterprise Architect SQL Search (human gate).
-- Secondary validation surface: SQLite read-only URI mode=ro against the local
-- QEA file. SQLite results are diagnostic only and never replace EA evidence.
--
-- Repository scope
--   Application: t_object.Object_Type = 'Class'
--                  AND t_object.Stereotype = 'ArchiMate_ApplicationComponent'
--   Tagged value: t_objectproperties joined by Object_ID
-- =============================================================================

-- ---------------------------------------------------------------------------
-- Q1-C — Categoria ORO — Count
-- Business question: How many applications have the tagged value
--                    Categoria = ORO?
-- ---------------------------------------------------------------------------
SELECT
    COUNT(DISTINCT app.Object_ID) AS ORO_Application_Count
FROM t_object app
INNER JOIN t_objectproperties tag
    ON tag.Object_ID = app.Object_ID
WHERE app.Object_Type = 'Class'
  AND app.Stereotype = 'ArchiMate_ApplicationComponent'
  AND tag.Property = 'Categoria'
  AND tag.Value = 'ORO';

-- ---------------------------------------------------------------------------
-- Q1-L — Categoria ORO — List
-- Business question: Which applications have the tagged value Categoria = ORO?
-- EA Search display name: Q1-L — Categoria ORO — List
-- Navigation aliases: CLASSGUID and CLASSTYPE are required by EA SQL Search
-- to enable icons and double-click navigation on individual result rows.
-- ---------------------------------------------------------------------------
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

-- ---------------------------------------------------------------------------
-- Q2-G — Vigencia — Grouped
-- Business question: How many applications are in each Vigencia state
--                    (Vigente / Deprecado)?
-- EA Search display name: Q2-G — Vigencia — Grouped
-- No CLASSGUID/CLASSTYPE: a grouped row does not represent a single element.
-- ---------------------------------------------------------------------------
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

-- ---------------------------------------------------------------------------
-- Q3-C — Base de Datos 28 Impact — Count
-- Business question: How many applications depend on Base de Datos 28?
-- ---------------------------------------------------------------------------
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

-- ---------------------------------------------------------------------------
-- Q3-L — Base de Datos 28 Impact — List
-- Business question: Which applications depend on Base de Datos 28?
-- EA Search display name: Q3-L — DB28 Impact — List
-- Navigation aliases: CLASSGUID and CLASSTYPE are required by EA SQL Search
-- to enable icons and double-click navigation on individual result rows.
-- Target identity is included because it is stable (exactly one DB28 target).
-- No connector ID is selected, so each source application appears once.
-- ---------------------------------------------------------------------------
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
