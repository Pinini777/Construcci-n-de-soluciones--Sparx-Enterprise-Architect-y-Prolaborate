# Application Category Governance Specification

## Purpose

Define the SELECT-only EA SQL Search query set, evidence, and documentation that answer the governance question: *Which applications have the tagged value `Categoria` equal to `ORO`?* The spec covers the application universe, tag resolution, mandatory total and application-list results, and validation in EA.

## Requirements

### Requirement: Application universe defined by Class and ArchiMate_ApplicationComponent

The system MUST identify the application universe using the defensive predicate `Object_Type='Class' AND Stereotype='ArchiMate_ApplicationComponent'`. The supplied-QEA validation oracle of 30 applications MUST be treated as validation evidence only and MUST NOT be hardcoded into query logic.

#### Scenario: Application universe is derived from repository metadata

- GIVEN the QEA repository contains elements with `Object_Type='Class'` and `Stereotype='ArchiMate_ApplicationComponent'`
- WHEN the query enumerates the application universe
- THEN the result set includes only those elements and excludes `ArchiMate_DataObject`, `ArchiMate_BusinessActor`, `ArchiMate_Node`, and any other element type

### Requirement: Categoria sourced from t_objectproperties via Object_ID

The system MUST read the `Categoria` value from `t_objectproperties` joined to `t_object` by `Object_ID`. The system MUST NOT infer category from element name, stereotype, status, package, or any hardcoded mapping.

#### Scenario: ORO applications are resolved through their tagged value

- GIVEN every application has a `Categoria` row in `t_objectproperties`
- WHEN the Q1 query set evaluates application `Categoria` tags where `Property='Categoria'` and `Value='ORO'`
- THEN the required outputs make both the ORO total and named ORO application list obtainable

#### Scenario: Non-application elements carrying Categoria are excluded

- GIVEN non-application elements such as `ArchiMate_DataObject` also carry `Categoria` tags
- WHEN the query applies the application scope predicate before reading the tag
- THEN no data object or non-application element appears in the ORO application result

### Requirement: ORO aggregate and list results

The executable Q1 query set MUST make both the total count of ORO applications and the named ORO application list obtainable. Design MAY use one or more SELECT statements for clarity, compatibility, reuse, or traceability; it MUST NOT prescribe a pre-decided aggregate/list statement split. Individual-element results SHOULD expose `ea_guid AS CLASSGUID` and `Object_Type AS CLASSTYPE` where applicable to support EA Search navigation. Aggregate results MUST NOT invent representative-object `CLASSGUID` or `CLASSTYPE` aliases.

#### Scenario: ORO total matches the supplied-QEA validation oracle

- GIVEN the supplied-QEA validation oracle is a total of 8 ORO applications
- WHEN the Q1 query set is executed with the application scope and `Categoria='ORO'` filter
- THEN its obtainable total is 8
- AND the query logic derives that total from repository data without hardcoding the oracle

#### Scenario: ORO application list matches the supplied-QEA validation oracle

- GIVEN the supplied-QEA validation oracle lists applications 1, 2, 3, 4, 6, 8, 20, and 29 as ORO
- WHEN the Q1 query set is executed
- THEN the returned names match exactly that set, regardless of row order

#### Scenario: ORO results are logically unique by application

- GIVEN matching underlying repository rows include more than one row associated with one ORO application
- WHEN the Q1 total and list are obtained
- THEN each application appears once in the list and contributes once to the total

### Requirement: Query identifiers and technical documentation

Each final statement MUST map unambiguously to Q1 and its business purpose (`Applications with Categoria ORO`). Comments MAY be used as a Design technique. The system MUST document, in SQL comments or an accompanying technical explanation, the tables, fields, joins, filters, assumptions, application criteria, tag interpretation, and validation steps used.

#### Scenario: Query file maps statement to business question

- GIVEN the delivered SQL file contains one or more SELECT statements for Q1
- WHEN a reader inspects the file
- THEN each statement unambiguously maps to Q1 and its purpose

#### Scenario: Technical explanation is reusable

- GIVEN the technical explanation is provided
- WHEN another practitioner reads it
- THEN they can reproduce the query on a different EA repository with the same table structure without reverse-engineering the SQL

### Requirement: Read-only repository access

The executable query set MUST be exclusively read-only against the QEA repository. It MUST NOT contain or execute `UPDATE`, `INSERT`, `DELETE`, `ALTER`, `DROP`, `CREATE`, or other repository-mutating operations. Documentary or comment mentions of those terms do not violate this requirement.

#### Scenario: Executable query set is semantically read-only

- GIVEN the executable query set and any diagnostic scripts are reviewed
- WHEN their executable behavior is assessed
- THEN they do not contain or execute repository-mutating operations
- AND documentary or comment references are not treated as executable operations

### Requirement: EA Search is the primary validation surface

The system MUST validate Q1 in Enterprise Architect SQL Search and capture real EA screenshot or export evidence. SQLite read-only (`mode=ro`) diagnostic execution is permitted as secondary validation but MUST NOT substitute for mandatory EA evidence.

#### Scenario: EA Search returns the ORO validation oracle

- GIVEN the Q1 query is executed in EA SQL Search
- WHEN the result is compared with the visible model
- THEN the count equals the supplied-QEA validation oracle and every named ORO application can be traced to its element in EA

### Requirement: Cross-cutting Exercise 2 AI log

As a cross-cutting Exercise 2 delivery requirement, the system MUST deliver a global Exercise 2 AI log with at least five real significant interactions. Each interaction MUST contain Proposal's eight distinct fields: identifier, objective, tool, model, strategy/prompt, decision taken from the response, related evidence, and outcome. The system MUST NOT fabricate interactions. Retrospective documentation is permitted only when all actual objective, tool, model, strategy/prompt, decision taken from the response, related evidence, and outcome are faithfully recoverable from records.

#### Scenario: Global Exercise 2 AI log is complete and verifiable

- GIVEN the global Exercise 2 AI log is opened
- WHEN its interactions are reviewed
- THEN it contains at least five real significant interactions and each has Proposal's eight distinct fields
- AND retrospective entries, if any, faithfully recover every required field from records and do not fabricate interactions

### Requirement: Cross-cutting Exercise 2 delivery naming

As a cross-cutting Exercise 2 delivery requirement, Pino final-delivery filenames MUST apply outside OpenSpec, and OpenSpec artifacts are exempt. This specification MUST NOT select those filenames.

#### Scenario: Final deliverables use Pino naming outside OpenSpec

- GIVEN the final Exercise 2 deliverables are reviewed
- WHEN a final deliverable is outside OpenSpec
- THEN its filename conforms to Pino naming
- AND OpenSpec artifacts are exempt

### Requirement: Optional Prolaborate follow-on

Only after Q1, Q2, and Q3 have been EA-validated, a Prolaborate follow-on MAY reuse at least one EA-validated query. Its absence MUST NOT block mandatory acceptance. This specification MUST NOT choose a query, chart, or aliases.

#### Scenario: Prolaborate follow-on remains optional

- GIVEN Q1-Q3 EA validation has completed
- WHEN no Prolaborate visualization, documentation, or visual evidence is delivered
- THEN mandatory Exercise 2 acceptance remains unblocked

#### Scenario: Implemented Prolaborate follow-on is evidenced

- GIVEN Q1, Q2, and Q3 have been EA-validated and a Prolaborate follow-on is implemented
- WHEN the follow-on is reviewed
- THEN it reuses at least one EA-validated query, documents its configuration and business question, and captures visual evidence
- AND its visualization correctly represents the reused query
