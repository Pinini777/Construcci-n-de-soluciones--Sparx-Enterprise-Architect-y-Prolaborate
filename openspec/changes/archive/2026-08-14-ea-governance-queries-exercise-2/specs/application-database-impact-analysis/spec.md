# Application Database Impact Analysis Specification

## Purpose

Define the SELECT-only EA SQL Search query set, evidence, and documentation that answer the impact question: *Which applications would be affected if `Base de Datos 28` is decommissioned?* The spec covers dependency orientation, source and target predicates, negative-control evidence, and validation in EA.

## Requirements

### Requirement: Dependency orientation uses Start as source and End as target

The system MUST interpret `t_connector.Start_Object_ID` as the dependency source and `t_connector.End_Object_ID` as the dependency target. The system MUST filter by `Connector_Type='Dependency'`.

#### Scenario: Impact query follows Start-to-End orientation

- GIVEN `t_connector` rows of type `Dependency`
- WHEN the impact query joins `Start_Object_ID` to the source application and `End_Object_ID` to the target database
- THEN only connectors whose source depends on the target are returned

### Requirement: Target identified semantically as Base de Datos 28

The system MUST identify the target by `Name='Base de Datos 28'`, `Object_Type='Class'`, and `Stereotype='ArchiMate_DataObject'`. The system MUST NOT use `Object_ID=102` as the primary identifier; it MAY be used only as supplied-QEA validation-oracle evidence.

#### Scenario: Target is resolved by name, type, and stereotype

- GIVEN the repository contains `Base de Datos 28` as a `Class` with `ArchiMate_DataObject` stereotype
- WHEN the impact query filters the target by name, type, and stereotype
- THEN the query returns the correct target without relying on a hardcoded primary key

#### Scenario: Object_ID 102 is a supplied-QEA validation oracle only

- GIVEN the supplied-QEA validation oracle shows `Base de Datos 28` has `Object_ID=102`
- WHEN the technical explanation is reviewed
- THEN `Object_ID=102` is documented as observed validation evidence, not as the query's primary filter

### Requirement: Source predicate is limited to applications

The system MUST filter dependency sources to the application universe (`Object_Type='Class' AND Stereotype='ArchiMate_ApplicationComponent'`). The system MUST NOT include `ArchiMate_Node`, `ArchiMate_BusinessActor`, or any other non-application source in the main impact result.

#### Scenario: Only application sources appear in the impact list

- GIVEN dependencies originate from applications and from `ArchiMate_Node` elements
- WHEN the impact query applies the application source predicate
- THEN the result contains only the affected applications identified by the supplied-QEA validation oracle

### Requirement: Non-application sources are retained as negative-control evidence

The supplied-QEA validation oracle identifies `Servidor 2`, `Servidor 5`, and `Servidor 19` as non-application sources. The system SHOULD document or query those sources as negative-control evidence without hardcoding the oracle in query logic. The system MUST NOT present them as affected applications.

#### Scenario: Node sources are excluded from the mandatory result

- GIVEN dependencies from `ArchiMate_Node` elements also end at `Base de Datos 28`
- WHEN the mandatory application impact query executes
- THEN the node sources identified by the supplied-QEA validation oracle are absent from the result set

#### Scenario: Node sources are recorded separately as evidence

- GIVEN negative-control evidence is included
- WHEN it is reviewed
- THEN the supplied-QEA Node sources are documented separately as non-applications

### Requirement: Direction is optional validation only

The system MUST NOT require `t_connector.Direction` as a mandatory impact-query filter. The supplied-QEA validation oracle value `Source -> Destination` MAY be recorded as optional diagnostic validation and MUST NOT be hardcoded as query logic.

#### Scenario: Impact query does not depend on Direction

- GIVEN the supplied-QEA connector has Start=Source, End=Target, and `Direction='Source -> Destination'`
- WHEN the impact query is reviewed
- THEN it applies Start=Source and End=Target and contains no additional `Direction` predicate

### Requirement: Impact total and application-list results

The executable Q3 query set MUST make both the total count of affected applications and the named affected-application list obtainable. Design MAY use one or more SELECT statements for clarity, compatibility, reuse, or traceability; it MUST NOT prescribe a pre-decided aggregate/list statement split. Individual-element results SHOULD expose `ea_guid AS CLASSGUID` and `Object_Type AS CLASSTYPE` where applicable to support EA Search navigation. Aggregate results MUST NOT invent representative-object `CLASSGUID` or `CLASSTYPE` aliases.

#### Scenario: Impact total matches the supplied-QEA validation oracle

- GIVEN the supplied-QEA validation oracle is a total of 5 affected applications
- WHEN the Q3 query set is executed with the application source and semantic target filters
- THEN its obtainable total is 5
- AND the query logic derives that total from repository data without hardcoding the oracle

#### Scenario: Impact application list matches the supplied-QEA validation oracle

- GIVEN the supplied-QEA validation oracle lists applications 5, 12, 22, 25, and 27 as affected
- WHEN the Q3 query set is executed
- THEN the returned names match exactly that set, regardless of row order

#### Scenario: Impact results are logically unique by application

- GIVEN multiple matching dependency relations originate from the same affected application
- WHEN the Q3 total and list are obtained
- THEN that application appears once in the list and contributes once to the total

### Requirement: Query identifiers and technical documentation

Each final statement MUST map unambiguously to Q3 and its business purpose (`Applications impacted by Base de Datos 28`). Comments MAY be used as a Design technique. The system MUST document, in SQL comments or an accompanying technical explanation, the tables, fields, joins, filters, assumptions, application criteria, dependency semantics, and validation steps used.

#### Scenario: Query file maps statement to business question

- GIVEN the delivered SQL file contains one or more SELECT statements for Q3
- WHEN a reader inspects the file
- THEN each statement unambiguously maps to Q3 and its purpose

### Requirement: Read-only repository access

The executable query set MUST be exclusively read-only against the QEA repository. It MUST NOT contain or execute `UPDATE`, `INSERT`, `DELETE`, `ALTER`, `DROP`, `CREATE`, or other repository-mutating operations. Documentary or comment mentions of those terms do not violate this requirement.

#### Scenario: Executable query set is semantically read-only

- GIVEN the executable query set and any diagnostic scripts are reviewed
- WHEN their executable behavior is assessed
- THEN they do not contain or execute repository-mutating operations
- AND documentary or comment references are not treated as executable operations

### Requirement: EA Search is the primary validation surface

The system MUST validate Q3 in Enterprise Architect SQL Search and capture real EA screenshot or export evidence. SQLite read-only (`mode=ro`) diagnostic execution is permitted as secondary validation but MUST NOT substitute for mandatory EA evidence.

#### Scenario: EA Search returns the impact validation oracle

- GIVEN the Q3 query is executed in EA SQL Search
- WHEN the result is compared with the visible model
- THEN the count equals the supplied-QEA validation oracle and every affected application can be traced through its dependency to `Base de Datos 28` in EA
