# Application Lifecycle Governance Specification

## Purpose

Define the SELECT-only EA SQL Search query set, evidence, and documentation that answer the governance question: *How many applications are in each `Vigencia` state: `Vigente` and `Deprecado`?* The spec covers tag-only lifecycle resolution, result reporting, validation in EA, and reuse constraints.

## Requirements

### Requirement: Vigencia derived exclusively from t_objectproperties

The system MUST read the `Vigencia` value from `t_objectproperties` joined to `t_object` by `Object_ID`. The system MUST NOT use `t_object.Status` or any other column as a proxy for lifecycle state.

#### Scenario: Vigente and Deprecado counts come from the tag

- GIVEN every application has a `Vigencia` row in `t_objectproperties`
- WHEN the query evaluates application `Vigencia` tags where `Property='Vigencia'`
- THEN the result reports `Vigente` and `Deprecado` counts derived from the tag value

#### Scenario: t_object.Status is ignored

- GIVEN all applications share the same `t_object.Status` value
- WHEN the query is executed
- THEN the query does not reference `t_object.Status` and the counts differ from a `Status`-based count

### Requirement: Results report Vigente and Deprecado only

The system MUST report only the observed lifecycle values `Vigente` and `Deprecado`. The system MUST NOT create artificial buckets such as `N/A`, `NULL`, `Empty`, or `Unknown` unless the repository actually contains such values, and any genuinely missing values MUST be documented rather than coerced into a bucket.

#### Scenario: Supplied-QEA validation oracle distribution is reproduced

- GIVEN the supplied-QEA validation oracle is 16 `Vigente` and 14 `Deprecado` applications
- WHEN the aggregate query executes
- THEN it returns exactly those counts
- AND the query logic derives the counts from repository data without hardcoding the oracle

#### Scenario: Missing values are documented, not bucketed

- GIVEN the supplied-QEA validation oracle contains zero NULL, empty, or `N/A` `Vigencia` values for applications
- WHEN the query and technical explanation are reviewed
- THEN the explanation states the absence of missing values and the query does not synthesize an artificial bucket

### Requirement: Application scope excludes non-application Vigencia tags

The system MUST scope `Vigencia` aggregation to the application universe (`Object_Type='Class' AND Stereotype='ArchiMate_ApplicationComponent'`). The system MUST NOT include `Vigencia` tags from `ArchiMate_DataObject` or any other element type.

#### Scenario: DataObject Vigencia tags do not pollute application counts

- GIVEN `ArchiMate_DataObject` elements also carry `Vigencia` tags
- WHEN the query applies the application scope predicate
- THEN the application `Vigente`/`Deprecado` counts exclude data object values

### Requirement: Grouped Vigencia results with conditional EA navigation aliases

The system MUST produce grouped counts per `Vigencia` value. Individual-element results, if provided, SHOULD expose `ea_guid AS CLASSGUID` and `Object_Type AS CLASSTYPE` where applicable. Grouped results MUST NOT invent representative-object `CLASSGUID` or `CLASSTYPE` aliases.

#### Scenario: Aggregate reports counts without invented object aliases

- GIVEN the query groups applications by `Vigencia` value
- WHEN the aggregate query executes
- THEN the output MUST include the `Vigencia` value and application count and MUST NOT invent representative `CLASSGUID` or `CLASSTYPE`

#### Scenario: Vigencia counts are logically unique by application

- GIVEN matching joins include more than one row associated with one application in a `Vigencia` state
- WHEN the Q2 grouped counts are obtained
- THEN each application contributes once to the count for its `Vigencia` value

### Requirement: Query identifiers and technical documentation

Each final statement MUST map unambiguously to Q2 and its business purpose (`Applications by Vigencia`). Comments MAY be used as a Design technique. The system MUST document, in SQL comments or an accompanying technical explanation, the tables, fields, joins, filters, assumptions, application criteria, tag interpretation, and validation steps used.

#### Scenario: Query file maps statement to business question

- GIVEN the delivered SQL file contains one or more SELECT statements for Q2
- WHEN a reader inspects the file
- THEN each statement unambiguously maps to Q2 and its purpose

### Requirement: Read-only repository access

The executable query set MUST be exclusively read-only against the QEA repository. It MUST NOT contain or execute `UPDATE`, `INSERT`, `DELETE`, `ALTER`, `DROP`, `CREATE`, or other repository-mutating operations. Documentary or comment mentions of those terms do not violate this requirement.

#### Scenario: Executable query set is semantically read-only

- GIVEN the executable query set and any diagnostic scripts are reviewed
- WHEN their executable behavior is assessed
- THEN they do not contain or execute repository-mutating operations
- AND documentary or comment references are not treated as executable operations

### Requirement: EA Search is the primary validation surface

The system MUST validate Q2 in Enterprise Architect SQL Search and capture real EA screenshot or export evidence. SQLite read-only (`mode=ro`) diagnostic execution is permitted as secondary validation but MUST NOT substitute for mandatory EA evidence.

#### Scenario: EA Search returns the Vigencia validation oracle

- GIVEN the Q2 query is executed in EA SQL Search
- WHEN the result is compared with the visible model
- THEN the counts match the supplied-QEA validation oracle and validate against visible/modelled application elements and their `Vigencia` tags in EA

#### Scenario: Optional individual drilldown offers traceability when applicable

- GIVEN a later individual-element or drilldown result is provided for Q2
- WHEN it is reviewed in EA
- THEN it SHOULD offer element traceability where applicable
