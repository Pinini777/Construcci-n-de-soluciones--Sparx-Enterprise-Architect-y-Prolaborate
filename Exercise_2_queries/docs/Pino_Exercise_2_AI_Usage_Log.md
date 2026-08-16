# Registro de Uso de Inteligencia Artificial

**Estado:** FINAL. Este registro contiene interacciones reales completadas que se sustentan en prompts pegados por el operador y/o artefactos e historial recuperados. No es evidencia de ejecución en Enterprise Architect.

El objetivo es mantener la trazabilidad:

**consulta -> herramienta/modelo -> decisión -> evidencia -> resultado**

## Resumen de interacciones

| Identifier | Objective | Tool | Model | Strategy / Prompt | Decision taken from response | Related evidence | Outcome |
|---|---|---|---|---|---|---|---|
| **E2-SDD-001** | Explorar el QEA read-only y establecer los hechos necesarios antes de Proposal. | sdd-explore executor | MiniMax M3 | Se creó el cambio y se ejecutó solo Explore sobre el QEA en modo read-only para confirmar aplicación, Categoria, Vigencia, DB28, conectores, duplicados y compatibilidad EA SQL Search. Se debía detener antes de Proposal. Ver P-EXPLORE | Use EA SQL Search as future primary surface and SQLite `mode=ro` only as independent diagnostic; determine impact from `Start_Object_ID`, `End_Object_ID`, and `Connector_Type='Dependency'`, without requiring `Direction`. | Operator-pasted `EXPLORE` prompt in source log; `openspec/changes/ea-governance-queries-exercise-2/exploration.md`. | Explore completed and became approved basis for Proposal. |
| **E2-SDD-002** | Define the mandatory Exercise 2 scope and validation baselines without implementation. | sdd-propose executor | GPT-5.6 Terra | Se solicitó definir alcance, enfoque, baselines de validación, límites read-only, evidencia EA primaria y Prolaborate opcional, preservando QEA y E1. Ver P-PROPOSAL | Deliver a SELECT-only EA SQL Search query set; retain SQLite `mode=ro` as secondary; keep Prolaborate optional after mandatory EA validation; preserve QEA and E1. | `openspec/changes/ea-governance-queries-exercise-2/proposal.md`; Git history commit `c0ebf79` (`docs: define exercise 2 SDD plan`). | Proposal completed and approved. |
| **E2-SDD-003** | Convert approved exploration and proposal into verifiable requirements and scenarios. | sdd-spec executor | Kimi K2.7 Code | Se ejecutó solo Spec a partir de Explore, Proposal y la consigna, con requisitos y escenarios reconocibles por parser; no debía diseñar SQL ni avanzar a Design. Ver P-SPEC | Specify the application predicate, tagged-value sources, semantic DB28 target, read-only boundaries, EA-primary evidence, and eight-field real-interaction AI log requirement. | Operator-pasted `SPEC` prompt in source log; `openspec/changes/ea-governance-queries-exercise-2/specs/`. | Three capability specifications completed and approved before Design. |
| **E2-SDD-004** | Define the implementation design, final statement decomposition, evidence flow, and human gates. | sdd-design executor | GPT-5.6 Sol | Se ejecutó solo Design para decidir la descomposición SELECT, las estrategias SQL, la validación EA/SQLite y gates humanos incrementales; no debía crear entregables finales ni avanzar a Tasks. Ver P-DESIGN | Use five SELECT statements: Q1-C, Q1-L, Q2-G, Q3-C, and Q3-L; apply distinct application identity; require sequential EA gates; make Q2-G the optional Prolaborate candidate. | Operator-pasted `DESIGN` prompt in source log; `openspec/changes/ea-governance-queries-exercise-2/design.md`. | Design completed and approved before Tasks. |
| **E2-SDD-005** | Produce an incremental, gate-controlled implementation plan. | sdd-tasks executor | Kimi K2.7 Code | Se solicitó solo Tasks, preservando el diseño de cinco sentencias y separando trabajo automatizable de gates humanos bloqueantes; no debía implementar, ejecutar SQL ni avanzar a Apply. El operador solicitó Qwen 3.7 Plus, pero el runtime real fue Kimi K2.7 Code porque Qwen no estaba disponible. Ver P-TASKS | Organize the work as WU1 scaffold, WU2 Q1 → Gate Q1, WU3 Q2 → Gate Q2, WU4 Q3 → Gate Q3, WU5 documentation, WU6 optional Prolaborate, and WU7 pre-verify. | Operator-pasted `TASK` prompt in source log; `openspec/changes/ea-governance-queries-exercise-2/tasks.md`; Git history commit `d530af4` (`docs: add exercise 2 implementation tasks`). | Tasks completed; Q1 was the first permitted implementation increment. |
| **E2-SDD-006** | Implement the initial Q1 increment and independently diagnose Q1-C/Q1-L without running the human EA gate. | sdd-apply executor | Kimi K2.7 Code | Se implementaron WU1 y las tareas WU2 hasta 2.6, con diagnóstico Q1 read-only; se debía detener antes de HUMAN GATE Q1 y no implementar Q2/Q3. Ver P-APPLY-Q1 | Limit the first apply increment to WU1 scaffolding plus WU2 Q1 tasks through 2.6; stop before HUMAN GATE Q1 and do not implement Q2/Q3. | `openspec/changes/ea-governance-queries-exercise-2/tasks.md` tasks 1.1-1.7 and 2.1-2.6; `docs/Pino_Exercise_2_SQLite_Diagnostics.md`; `docs/Pino_Exercise_2_Evidence_Index.md`. | Q1 SQLite diagnostics matched the supplied-QEA oracle. EA execution and evidence remained pending at HUMAN GATE Q1. |
| **E2-SDD-007** | Document confirmed HUMAN GATE Q1, implement Q2-G, run read-only Q2 diagnostics, and stop before HUMAN GATE Q2. | sdd-apply executor | Kimi K2.7 Code | Se incorporó la evidencia real de Q1, se implementó Q2-G y se ejecutaron diagnósticos SQLite `mode=ro`; Q2 EA debía quedar pendiente y no había staging, commit ni PR. Ver P-APPLY-Q2 | Mark task 2.7 complete from real operator evidence; implement Q2-G as a third SELECT block; verify Vigente=16/Deprecado=14 with zero missing/conflicting values; update evidence index, SQLite diagnostics, and technical explainer; leave HUMAN GATE Q2 unchecked. | `openspec/changes/ea-governance-queries-exercise-2/tasks.md` tasks 2.7 and 3.1-3.5; `queries/Pino_Exercise_2_EA_Governance_Queries.sql`; `docs/Pino_Exercise_2_SQLite_Diagnostics.md`; `docs/Pino_Exercise_2_Evidence_Index.md`; `docs/Pino_Exercise_2_Technical_Explainer.md`. | Tasks 2.7 and 3.1-3.5 complete. Q1 recorded as EA_PASS from operator report; Q2-G SQLite diagnostic matched oracle. Q2 EA execution remained pending. |
| **E2-SDD-008** | Document confirmed HUMAN GATE Q2 and implement Q3-C/Q3-L with read-only diagnostics, stopping before HUMAN GATE Q3. | sdd-apply executor | Kimi K2.7 Code | Se incorporó la evidencia real de Q2, se agregaron Q3-C/Q3-L tras validar un único target semántico y se ejecutaron diagnósticos read-only; debía detenerse antes de 4.9. Ver P-APPLY-Q3 | Mark task 3.6 complete from real operator evidence; verify DB28 semantic target cardinality = 1 (Object_ID 102 diagnostic only); add Q3-C and Q3-L as fourth and fifth SELECT blocks; verify Q3-C=5 and Q3-L exact five-application set; document Node negative control; update evidence index, SQLite diagnostics, technical explainer, and AI log; leave HUMAN GATE Q3 unchecked. | `openspec/changes/ea-governance-queries-exercise-2/tasks.md` task 3.6 and tasks 4.1-4.8; `queries/Pino_Exercise_2_EA_Governance_Queries.sql`; `docs/Pino_Exercise_2_SQLite_Diagnostics.md`; `docs/Pino_Exercise_2_Evidence_Index.md`; `docs/Pino_Exercise_2_Technical_Explainer.md`. | Tasks 3.6 and 4.1-4.8 complete. Q2 recorded as EA_PASS from operator report; Q3 implemented and SQLite diagnostics matched oracles. Q3 EA execution remained pending. |
| **E2-SDD-009** | Complete Gate Q3 documentation, post-gate audit, final mandatory documentation, optional Prolaborate consolidation, and pre-verify repository guard. | sdd-apply executor | Kimi K2.7 Code | Se ejecutaron las tareas restantes con evidencia PDF real del operador, documentación final, Prolaborate opcional y auditoría pre-verify; se debía detener listo para Verify, sin ejecutarlo. Ver P-APPLY-FINAL | Promote Q3-C and Q3-L to EA_PASS from operator confirmation; record primary-form acceptance and no fallback for all five statements; finalize evidence index, technical explainer, and AI log; create Prolaborate Q2 documentation; run pre-verify checks. | `openspec/changes/ea-governance-queries-exercise-2/tasks.md` tasks 4.9, 5.1-5.5, 6.1-6.5, 7.1-7.7, 8.1-8.8; `queries/Pino_Exercise_2_EA_Governance_Queries.sql`; `docs/Pino_Exercise_2_Evidence_Index.md`; `docs/Pino_Exercise_2_SQLite_Diagnostics.md`; `docs/Pino_Exercise_2_Technical_Explainer.md`; `docs/Pino_Exercise_2_Prolaborate_Q2.md`; operator PDFs `Pino_Ejercicio_2_Informe_Principal_Queries_EA.pdf` and `docs/Pino_Evidencias_Funcionales_Queries.pdf`. | All remaining tasks completed. Q3 EA_PASS recorded; all five statements traceable to real PDF evidence; Prolaborate documented; pre-verify checks passed. Ready for sdd-verify. |

## Source and fidelity notes

- The original source was the operator-pasted prompt record that previously occupied this file. Its completed-phase instructions are represented above as faithful summaries; historical instructions have not been rewritten as newly given prompts.
- Where a full prompt, tool identity, or date cannot be recovered faithfully, the respective field is explicitly marked unavailable. In this final version, all recovered interactions have identifiable tools and models.
- The model values are limited to recovered actual phase records: MiniMax M3; GPT-5.6 Terra; Kimi K2.7 Code; GPT-5.6 Sol; and Kimi K2.7 Code for Tasks and all Apply interactions.
- The Tasks phase originally requested Qwen 3.7 Plus, but the actual executed model was Kimi K2.7 Code; the log records Kimi, not Qwen, as required.
- No EA PASS, screenshot, date, tool, model, decision, result, or action has been invented. Real EA evidence is provided by the operator PDFs referenced in the Evidence Index.

## Final current state

- Gates Q1, Q2 and Q3: PASS.
- Five official statements: EA_PASS.
- Optional Prolaborate: COMPLETE.
- SDD Verify: PASS.
- SDD Archive: COMPLETE.
- The change is archived at `openspec/changes/archive/ea-governance-queries-exercise-2/`.

## Related artifacts

```text
openspec/changes/archive/ea-governance-queries-exercise-2/
Exercise_2_queries/docs/Pino_Exercise_2_Evidence_Index.md
Exercise_2_queries/docs/Pino_Exercise_2_SQLite_Diagnostics.md
Exercise_2_queries/docs/Pino_Exercise_2_Prolaborate_Q2.md
Exercise_2_queries/Pino_Ejercicio_2_Informe_Principal_Queries_EA.pdf
Exercise_2_queries/docs/Pino_Evidencias_Funcionales_Queries.pdf
```

## Prompts used / references

The following blocks preserve authentic operator-pasted prompts where they are physically present in the source log. Entries without a pasted literal state that limitation rather than reconstructing one.

### P-INIT

#### Resumen documental

Inicio formal del Exercise 2 y delimitación del workspace, repositorio QEA y alcance read-only.
Solicita INIT/ONBOARD/EXPLORE y prohíbe avanzar a fases posteriores.

#### Prompt original

Vamos a iniciar formalmente el Exercise 2 del desafío técnico Proagile 2026 en este workspace.

ROOT DEL WORKSPACE: C:\Proagile\Addin
CAMBIO SDD NUEVO: ea-governance-queries-exercise-2
ÁREA EXCLUSIVA DE IMPLEMENTACIÓN/ENTREGA DEL EXERCISE 2: C:\Proagile\Addin\Exercise_2_queries
REPOSITORIO EA DE TRABAJO: C:\Proagile\Data\Repositorio Pasantias.qea
IMPORTANTE SOBRE EL WORKSPACE EXISTENTE:
- `Addino/` contiene el Exercise 1 ya implementado, verificado, archivado y funcional en Enterprise Architect.
- NO modificar código, configuración, documentación ni binarios de `Addino/`.
- NO reabrir el cambio archivado del Exercise 1.
- El historial del Exercise 1 está preservado bajo `openspec/changes/archive/`.
- `openspec/specs/ea-metadata-review/` es una spec canónica histórica de E1 y no pertenece al nuevo alcance.
- El store OpenSpec de este workspace está en `C:\Proagile\Addin\openspec`.
OBJETIVO DEL EXERCISE 2:
Construir queries SQL sobre el repositorio de Enterprise Architect para responder, como mínimo:
1. Cuántos elementos de tipo aplicación tienen el tagged value `Categoria` con valor `ORO` y cuáles son.
2. Cuántas aplicaciones existen en cada estado de vigencia `Vigente` y `Deprecado`, contemplando valores vacíos/no informados si corresponde.
3. Qué aplicaciones se verían afectadas si se da de baja `Base de Datos 28`, identificando dependencias cuyo origen sea una aplicación y cuyo destino sea `Base de Datos 28`.
REQUISITOS DE LA CONSIGNA:
- Identificar explícitamente las tablas principales del repositorio EA utilizadas para elementos, relaciones y tagged values.
- Las queries finales deberán ser comprensibles, reutilizables y verificables.
- Deberán ejecutarse en el motor SQL/Search de Enterprise Architect o mecanismo equivalente permitido.
- Los resultados deberán validarse manualmente contra elementos visibles del modelo.
- La query de Categoria debe documentar qué campo/tagged value determina la categoría.
- La query de vigencia debe contemplar valores vacíos/no informados si realmente existen.
- La query de impacto debe respetar estrictamente la dirección origen aplicación -> destino Base de Datos 28.
- Los entregables posteriores incluirán archivo SQL, explicación técnica de tablas/joins/filtros/supuestos, evidencia EA y Registro de Uso de IA.
- Existe un punto extra posterior para llevar una query validada a un gráfico/dashboard Prolaborate. NO implementar todavía Prolaborate.
FUENTES DISPONIBLES/RELEVANTES:
- Consigna oficial Proagile.
- `Repositorio Pasantias.qea`.
- `InsideEA.pdf`, especialmente la documentación de t_object, t_objectproperties y t_connector.
- Enterprise Architect Object Model 17.1.
- Enterprise Architect SDK.
- Documentación Prolaborate V5 `Build Donut Charts with Live Information in Prolaborate`, solamente para la fase opcional posterior.
REGLAS PARA ESTA SESIÓN:
1. Recupera mediante ENGRAM cualquier contexto útil del proyecto anterior, especialmente organización del workspace, política de evidencia y proceso SDD, pero NO reutilices decisiones funcionales de Exercise 1 para inventar decisiones de Exercise 2.
2. Consulta el estado SDD nativo antes de crear o modificar artefactos.
3. Inicializa/onboardea el nuevo cambio según el mecanismo nativo disponible.
4. Ejecuta únicamente las fases necesarias de INIT/ONBOARD y EXPLORE.
5. Usa `sdd-init` con GPT-5.6 Luna, `sdd-onboard` con MiniMax M3 cuando corresponda y `sdd-explore` con MiniMax M3.
6. NO ejecutes todavía sdd-propose, sdd-spec, sdd-design, sdd-tasks, sdd-apply, sdd-verify ni sdd-archive.
7. NO escribas todavía las tres queries finales.
8. NO modifiques el repositorio QEA. Toda inspección de `Repositorio Pasantias.qea` debe ser estrictamente READ-ONLY.
9. Puedes inspeccionar el esquema y datos del QEA mediante SQLite/read-only tooling si está disponible.
10. No asumir que `Aplicación` equivale simplemente a un Object_Type concreto: demuestra cómo se identifica realmente en este repositorio mediante tipo, stereotype, paquete u otros metadatos.
11. No asumir que `Vigente/Deprecado` corresponde a `t_object.Status`: inspecciona los datos reales y demuestra dónde está modelada la vigencia.
12. Para `Categoria`, verifica nombre exacto del tagged value, tabla, columna, relación con el elemento y valores existentes.
13. Para `Base de Datos 28`, identifica de forma inequívoca el elemento, su tipo/stereotype y cómo se materializan las dependencias.
14. Para conectores, verifica cuál columna representa origen/source y cuál destino/target en ESTE repositorio y contrástalo con la documentación.
15. Identifica posibles duplicados de tagged values, NULL, valores vacíos o cualquier condición que pueda alterar COUNTs o producir filas duplicadas.
16. No utilices UPDATE, INSERT, DELETE ni cualquier SQL mutante.
17. Mantén separado el optional de Prolaborate del alcance obligatorio.
18. No alteres RDD ni review mode durante esta exploración salvo que el runtime nativo lo exija explícitamente; informa cualquier bloqueo antes de tomar decisiones destructivas.
19. No hagas commits Git automáticamente.
ENTREGABLE DE ESTA SESIÓN:
Produce/actualiza únicamente los artefactos de INIT/ONBOARD/EXPLORE previstos por el SDD y devuelve un reporte de descubrimiento con:
- estado inicial del nuevo change;
- estructura relevante del workspace;
- motor/formato real del `.qea`;
- tablas candidatas y columnas relevantes;
- criterio demostrado para identificar una Aplicación;
- ubicación real de Categoria;
- ubicación real del estado de vigencia;
- identidad de Base de Datos 28;
- estructura/dirección de las dependencias;
- riesgos de duplicación o datos faltantes;
- compatibilidad probable entre SQL que funciona sobre QEA/SQLite y el SQL Search de EA;
- cuestiones abiertas que deban resolverse antes de Proposal;
- archivos creados/modificados;
- `git diff --check`;
- `git status --short`.
Detente al finalizar Explore y espera aprobación humana antes de Proposal.

### P-EXPLORE

#### Resumen documental

Creación nativa del change y ejecución exclusiva de Explore con MiniMax M3.
Incluye hallazgos preliminares que debían validarse read-only y exige detenerse antes de Proposal.

#### Prompt original

El bootstrap de Exercise 2 ya fue revisado y aprobado por el operador. `openspec/config.yaml` representa correctamente el workspace multi-exercise y el área activa SQL/read-only.
Continúa ahora con:
1. creación nativa del cambio SDD `ea-governance-queries-exercise-2` mediante `sdd-new`;
2. únicamente la fase `sdd-explore` usando MiniMax M3.
NO continúes a Proposal, Spec, Design, Tasks, Apply, Verify ni Archive.
ROOT:C:\Proagile\Addin
ÁREA ACTIVA: C:\Proagile\Addin\Exercise_2_queries
REPOSITORIO EA: C:\Proagile\Data\Repositorio Pasantias.qea
El repositorio QEA debe inspeccionarse exclusivamente en modo read-only.
HALLAZGOS PRELIMINARES DE UNA INSPECCIÓN READ-ONLY INDEPENDIENTE:
Estos datos son evidencia inicial para acelerar Explore. NO deben aceptarse a ciegas: debes verificarlos directamente contra el QEA, documentar cómo los reprodujiste y señalar cualquier discrepancia.
1. El archivo QEA parece ser SQLite y contiene al menos:
   - t_object
   - t_objectproperties
   - t_connector
   - t_package
   - t_xref
   - t_stereotypes
2. La inspección encontró 30 elementos que parecen representar aplicaciones mediante:
   - Object_Type = 'Class'
   - Stereotype = 'ArchiMate_ApplicationComponent'
   La pertenencia al universo "Aplicación" parece depender principalmente del stereotype, ya que existen otros elementos Object_Type='Class'. Verifícalo.
3. Categoria parece ser un tagged value:
   - t_objectproperties.Property = 'Categoria'
   - t_objectproperties.Value contiene el valor.
   - join preliminar: t_object.Object_ID = t_objectproperties.Object_ID.
4. Para las aplicaciones identificadas preliminarmente, Categoria produjo:
   - ORO: 8
   - PLATA: 8
   - BRONCE: 4
   - N/A: 10
   Verifica todos los conteos y detecta NULL/vacíos/duplicados.
5. Las aplicaciones ORO encontradas preliminarmente fueron:
   - Aplicación 1
   - Aplicación 2
   - Aplicación 3
   - Aplicación 4
   - Aplicación 6
   - Aplicación 8
   - Aplicación 20
   - Aplicación 29
   No uses esta lista como resultado final sin reproducirla mediante consulta.
6. El estado de vigencia NO parece corresponder a t_object.Status.
   Se encontró como tagged value:
   - t_objectproperties.Property = 'Vigencia'
   Para las aplicaciones:
   - Vigente: 16
   - Deprecado: 14
   - vacíos/N/A: aparentemente 0 dentro del universo de aplicaciones.
   Existen valores N/A de Vigencia en otros tipos de elementos, por lo que debes verificar el universo antes de agrupar.
7. `Base de Datos 28` fue identificada preliminarmente como:
   - Object_ID = 102
   - Object_Type = 'Class'
   - Stereotype = 'ArchiMate_DataObject'
8. En t_connector:
   - Start_Object_ID parece representar Source/origen;
   - End_Object_ID parece representar Target/destino;
   - los conectores relevantes parecen usar Connector_Type = 'Dependency';
   - Direction observada: 'Source -> Destination'.
9. Se encontraron preliminarmente 8 dependencias cuyo destino es Base de Datos 28, pero solo 5 tienen como origen una aplicación.
   Las otras 3 parten de servidores/nodos y NO satisfacen el interrogante de la consigna.
10. Las cinco aplicaciones dependientes encontradas preliminamente son:
   - Aplicación 5
   - Aplicación 12
   - Aplicación 22
   - Aplicación 25
   - Aplicación 27
   Verifícalas de forma independiente.
OBJETIVOS DE EXPLORE:
- confirmar o refutar cada hallazgo preliminar;
- documentar estructura del QEA y tablas relevantes;
- demostrar el criterio exacto para "Aplicación";
- demostrar almacenamiento de Categoria;
- demostrar almacenamiento de Vigencia;
- revisar NULL, vacío, N/A y tagged values duplicados;
- demostrar identidad inequívoca de Base de Datos 28;
- demostrar la dirección source -> target de t_connector;
- identificar Connector_Type y cualquier otro filtro necesario;
- distinguir explícitamente dependencias entrantes totales de dependencias cuyo origen es aplicación;
- evaluar compatibilidad del SQL SQLite con el SQL Search de Enterprise Architect;
- consultar la documentación InsideEA/Object Model cuando corresponda;
- considerar desde diseño futuro que las queries deberán ser reutilizables en EA y potencialmente adaptables a Prolaborate.
IMPORTANTE:
- NO escribas todavía las queries finales de entrega.
- Queries diagnósticas read-only durante Explore sí están permitidas.
- No UPDATE, INSERT, DELETE, ALTER, DROP, PRAGMA mutante ni cambios en el QEA.
- No modifiques Addino/.
- No modifiques artefactos archivados de E1.
- No hagas commit.
- No avances después de Explore.
Detente para aprobación humana.

### P-PROPOSAL

Estado del prompt original: no adjuntado actualmente al registro.

Resumen documental: se definió el alcance obligatorio, el enfoque SELECT/read-only, los baselines de validación, EA como evidencia primaria y Prolaborate como seguimiento opcional, preservando QEA y E1. El operador puede adjuntar posteriormente el literal auténtico sin reconstruirlo.

### P-SPEC

#### Resumen documental

Ejecución exclusiva de Spec con Kimi K2.7 Code desde Explore, Proposal y la consigna aprobados.
Exige requisitos y escenarios reconocibles por parser, sin SQL final ni avance a Design.

#### Prompt original

Explore y Proposal de `ea-governance-queries-exercise-2` han sido revisados y APROBADOS por el operador. Continúa ÚNICAMENTE con la fase `sdd-spec` usando Kimi K2.7 Code.
NO ejecutes Design, Tasks, Apply, Verify ni Archive.
CHANGE: ea-governance-queries-exercise-2
ROOT: C:\Proagile\Addin
ÁREA DE ENTREGA: C:\Proagile\Addin\Exercise_2_queries
QEA LOCAL: C:\Proagile\Repositorio Pasantias.qea
Toma como fuentes normativas de esta fase:
- `openspec/changes/ea-governance-queries-exercise-2/exploration.md`
- `openspec/changes/ea-governance-queries-exercise-2/proposal.md`
- la consigna disponible bajo `Exercise_2_queries/docs/challenge/`
Explore y Proposal ya fueron aprobados. NO redescubras el repositorio ni reabras decisiones congeladas salvo que detectes una contradicción objetiva; si la detectas, repórtala y detente antes de cambiar el alcance.
IMPORTANTE: NO resumas, acortes ni comprimas artificialmente el contenido de la Spec para ajustarlo a un límite de palabras. Conserva todo el detalle funcional, técnico y de aceptación necesario.
FORMATO NATIVO OBLIGATORIO:
La Spec MUST usar encabezados reconocibles por el parser nativo:
`### Requirement:` o `### REQ-n:`
Cada escenario MUST usar:
`#### Scenario:`
Los escenarios deben expresarse con Given / When / Then y usar RFC 2119 (MUST, SHOULD, MAY, MUST NOT) donde corresponda.
OBJETIVO:
Traducir Proposal en requisitos verificables, sin diseñar todavía la SQL final ni decidir prematuramente el número exacto de sentencias.
La Spec debe cubrir como mínimo:
1. UNIVERSO DE APLICACIÓN
El sistema MUST delimitar aplicaciones mediante el criterio defensivo:
`Object_Type='Class' AND Stereotype='ArchiMate_ApplicationComponent'`.
El QEA suministrado contiene 30 aplicaciones como baseline de validación, no como constante hardcodeable.
2. Q1 — CATEGORIA ORO
La solución MUST:
- usar el tagged value `Categoria`;
- obtenerlo desde `t_objectproperties` relacionado con el elemento por `Object_ID`;
- restringir el universo a aplicaciones;
- responder cantidad total y listado de aplicaciones cuyo valor sea `ORO`;
- identificar/documentar explícitamente el tagged value utilizado;
- derivar los resultados desde el repositorio, nunca hardcodearlos.
Baseline del QEA suministrado:
Total = 8
Aplicación 1, Aplicación 2, Aplicación 3, Aplicación 4, Aplicación 6, Aplicación 8, Aplicación 20, Aplicación 29
3. Q2 — VIGENCIA
La solución MUST:
- usar el tagged value `Vigencia` desde `t_objectproperties`;
- restringir el universo a aplicaciones;
- MUST NOT utilizar `t_object.Status` para representar vigencia;
- producir cantidades agrupadas para `Vigente` y `Deprecado`;
- contemplar valores faltantes/no informados mediante validación/documentación sin crear buckets artificiales inexistentes.
Baseline del QEA suministrado:
Vigente = 16
Deprecado = 14
NULL = 0
empty = 0
N/A = 0
Los ceros de missing/unreported son evidencia de validación y documentación, NO filas artificiales obligatorias del resultado agrupado.
4. Q3 — IMPACTO BASE DE DATOS 28
La solución MUST:
- considerar `Dependency`;
- tratar `Start_Object_ID` como source/origen;
- tratar `End_Object_ID` como target/destino;
- exigir que el source cumpla el criterio de aplicación;
- identificar semánticamente el target mediante:
  Name='Base de Datos 28'
  Object_Type='Class'
  Stereotype='ArchiMate_DataObject'
- MUST NOT depender de Object_ID=102 como identificador funcional principal;
- MAY usar Object_ID=102 únicamente como baseline/evidencia diagnóstica;
- MUST NOT requerir `Direction` como filtro si Start/End ya expresan la orientación;
- devolver cantidad total y listado de aplicaciones afectadas;
- excluir Nodes/servidores del resultado obligatorio.
Baseline:
Total = 5
Aplicación 5
Aplicación 12
Aplicación 22
Aplicación 25
Aplicación 27
Servidor 2, Servidor 5 y Servidor 19 MUST quedar fuera del resultado obligatorio y SHOULD utilizarse como negative-control evidence del filtro de aplicación.
5. QUERY SET
La solución MUST consistir exclusivamente en SELECT/read-only SQL.
La Spec MUST NOT fijar todavía una cantidad exacta de sentencias.
Una o más sentencias MAY resolver cada interrogante cuando Design determine que mejora claridad, compatibilidad, reutilización o trazabilidad.
Cada sentencia final MUST quedar identificada con el interrogante que responde.
6. EA SQL SEARCH
EA SQL Search MUST ser la superficie primaria de ejecución y validación.
Las queries finales MUST validarse realmente en EA antes de considerarse aceptadas.
SQLite `mode=ro` MAY utilizarse como validación diagnóstica secundaria pero MUST NOT sustituir evidencia EA.
7. TRAZABILIDAD
Resultados que representan elementos individuales SHOULD devolver `CLASSGUID` y `CLASSTYPE` cuando corresponda para navegación/trazabilidad en EA.
Queries puramente agregadas MUST NOT inventar un elemento representativo solo para proporcionar esos aliases.
8. EVIDENCIA
Debe existir evidencia real de ejecución EA mediante screenshots o exportación de resultados.
NO inventar evidencia.
Debe poder relacionarse cada interrogante con:
pregunta -> SQL -> resultado -> elemento/modelo EA cuando corresponda.
9. DOCUMENTACIÓN
La solución MUST documentar:
- tablas utilizadas;
- campos relevantes;
- joins;
- filtros;
- supuestos;
- criterio de aplicación;
- tagged value utilizado;
- interpretación de los resultados;
- validaciones relevantes.
Las queries MUST ser reutilizables y re-ejecutables y SHOULD incluir comentarios donde aporten claridad.
10. READ-ONLY / SAFETY
El QEA MUST permanecer read-only.
MUST NOT ejecutarse UPDATE, INSERT, DELETE, ALTER, DROP ni operaciones mutantes.
Addino/E1 MUST permanecer sin modificaciones.
13. PROLABORATE
Prolaborate es OPTIONAL FOLLOW-ON.
Solo después de validar los tres interrogantes obligatorios en EA MAY:
- reutilizarse al menos una query validada;
- crearse donut/bar/dashboard;
- documentarse configuración y pregunta de negocio;
- capturarse evidencia visual.
La imposibilidad de completar este punto extra MUST NOT invalidar la aceptación obligatoria.
14. CRITERIOS DE CALIDAD
Los Requirements/Scenarios deben hacer verificables:
- correctitud;
- claridad técnica;
- reutilización;
- trazabilidad;
- read-only;
- evidencia real;
- cumplimiento de entregables.
NO escribas SQL final.
NO decidas todavía nombres finales concretos.
NO modifiques `Exercise_2_queries/`.
NO modifiques el QEA.
NO modifiques Addino/E1.
NO hagas commit.
NO avances a Design.
Al finalizar:
- indica el/los archivo(s) de Spec creado(s);
- enumera los Requirements y cantidad de Scenarios;
- confirma que el parser nativo debería reconocer los headings `### Requirement:` / `### REQ-n:` y `#### Scenario:`;
- ejecuta `git diff --check`;
- muestra `git status --short`;
- detente para revisión humana.

### P-DESIGN

#### Resumen documental

Ejecución exclusiva de Design con GPT-5.6 Sol para decidir implementación, cinco SELECTs, evidencia y gates.
Prohíbe crear archivos finales o avanzar a Tasks, Apply, Verify o Archive.

#### Prompt original

Explore, Proposal y las tres Specs de `ea-governance-queries-exercise-2` han sido revisados y APROBADOS por el operador.
Continúa ÚNICAMENTE con la fase `sdd-design` usando GPT-5.6 Sol.
NO ejecutes Tasks, Apply, Verify ni Archive.
CHANGE: ea-governance-queries-exercise-2
ROOT: C:\Proagile\Addin
ÁREA DE ENTREGA: C:\Proagile\Addin\Exercise_2_queries
QEA LOCAL DE TRABAJO: C:\Proagile\Repositorio Pasantias.qea
FUENTES NORMATIVAS:
- `openspec/changes/ea-governance-queries-exercise-2/exploration.md`
- `openspec/changes/ea-governance-queries-exercise-2/proposal.md`
- las tres Specs aprobadas bajo `openspec/changes/ea-governance-queries-exercise-2/specs/`
- la consigna bajo `Exercise_2_queries/docs/challenge/`
- documentación local de InsideEA / Object Model cuando sea necesaria para decisiones de compatibilidad EA
- guía de Prolaborate únicamente para el optional follow-on.
IMPORTANTE: NO resumas, acortes ni comprimas artificialmente `design.md` para ajustarlo a un límite de palabras. Documenta con el nivel de detalle técnico necesario para que Tasks y Apply puedan implementarlo sin reinterpretar decisiones.
NOTA DE CONTEO:
El resumen anterior del agente distribuyó incorrectamente Requirements/Scenarios por archivo. NO modifiques las Specs por este motivo.
El conteo real de headings es:
- Category: 9 Requirements / 14 Scenarios
- Lifecycle: 7 Requirements / 11 Scenarios
- Impact: 9 Requirements / 13 Scenarios
TOTAL: 25 Requirements / 38 Scenarios.

Las Specs están aprobadas. Si detectas una contradicción funcional nueva y objetiva, repórtala y detente; no reabras decisiones arbitrariamente.
OBJETIVO DE DESIGN:
Definir CÓMO se implementará Exercise 2, incluyendo decisiones SQL, estructura de entregables, compatibilidad EA, validación y evidencia, sin crear todavía los archivos finales de implementación. A diferencia de Spec, Design ahora DEBE tomar una decisión explícita y razonada sobre la descomposición final del query set.
EVALÚA Y DECIDE:
1. QUERY SET / SENTENCIAS
Decide cuántas sentencias SELECT finales habrá y cómo se distribuyen entre Q1, Q2 y Q3.
La decisión debe priorizar:
- claridad para el evaluador;
- compatibilidad con EA SQL Search;
- trazabilidad;
- reutilización;
- facilidad de capturar evidencia;
- cumplimiento de cantidad + listado donde corresponda.
Puede elegirse una o más sentencias por interrogante. Documenta la decisión y su rationale.
NO implementes todavía el SQL final.
2. Q1 — CATEGORIA ORO
Diseña la estrategia SQL para derivar:
- total de aplicaciones ORO;
- listado de aplicaciones ORO;
- aplicaciones lógicamente únicas.
Debe usar:
- t_object;
- t_objectproperties;
- join por Object_ID;
- Property='Categoria';
- Value='ORO';
- criterio defensivo de aplicación.
Baseline supplied-QEA:
8 aplicaciones:
1, 2, 3, 4, 6, 8, 20, 29.
Decide cómo asegurar unicidad sin hardcodear resultados.
Para resultados individuales, toma una decisión explícita sobre implementar el SHOULD de CLASSGUID/CLASSTYPE para navegación EA y documenta rationale.
3. Q2 — VIGENCIA
Diseña una consulta agrupada que derive:
- Vigente=16;
- Deprecado=14;
sobre aplicaciones únicas.
Debe usar tagged value Vigencia y MUST NOT usar t_object.Status.
Debe documentarse:
NULL=0
empty=0
N/A=0
en el supplied QEA, sin generar buckets artificiales.
No inventes CLASSGUID/CLASSTYPE para filas agregadas.
No agregues un listado individual de Q2 salvo que exista una razón clara y justificada; no es obligatorio.
4. Q3 — IMPACTO DB28
Diseña la estrategia SQL para derivar:
- total de aplicaciones afectadas;
- listado de aplicaciones afectadas;
- aplicaciones lógicamente únicas.
Semántica obligatoria:
source t_object application
→ t_connector Dependency
→ target t_object Base de Datos 28.
Source:
Object_Type='Class'
Stereotype='ArchiMate_ApplicationComponent'
Target:
Name='Base de Datos 28'
Object_Type='Class'
Stereotype='ArchiMate_DataObject'
Object_ID=102 es solamente validation oracle.
NO hardcodearlo como mecanismo principal.
Start_Object_ID = Source.
End_Object_ID = Target.
NO añadir Direction como filtro obligatorio.
Baseline:
5 aplicaciones:
5, 12, 22, 25, 27.
Servidor 2, 5 y 19 quedan fuera del resultado obligatorio. Decide cómo utilizar, si resulta útil, ese negative control en validación/documentación.
5. UNICIDAD
Design DEBE elegir una estrategia SQL concreta para garantizar aplicaciones únicas en Q1/Q2/Q3.  Evalúa la alternativa más compatible y legible para EA SQL Search, por ejemplo DISTINCT, COUNT(DISTINCT ...), agrupación u otra forma apropiada.
Justifica la decisión por compatibilidad, legibilidad y semántica.
No uses técnicas complejas si una solución SQL simple y portable satisface los requisitos.
6. COMPATIBILIDAD EA SQL SEARCH
Diseña pensando primero en Enterprise Architect SQL Search, no solo SQLite.
Prioriza SQL simple:
- SELECT;
- JOIN;
- WHERE;
- GROUP BY;
- ORDER BY;
- DISTINCT/agregación cuando se justifique.
Evita CTEs, window functions, extensiones específicas o construcciones innecesarias salvo evidencia de necesidad/compatibilidad.
Documenta qué partes necesitarán validación real dentro de EA.
Las queries que devuelvan elementos individuales deberían ser navegables mediante CLASSGUID/CLASSTYPE si Design adopta ese SHOULD.
Las agregadas no deben inventar esos aliases.
7. ESTRUCTURA DE ENTREGABLES
Ahora Design PUEDE elegir los nombres finales concretos.
Todos los archivos submission-facing MUST incluir `Pino`.
Diseña la estructura final dentro de `Exercise_2_queries/`, cubriendo como mínimo:
- archivo SQL;
- explicación técnica;
- evidencia EA;
- AI Usage Log;
- documentación/área Prolaborate opcional;
- README/índice si aporta claridad.
NO modifiques ni elimines `docs/challenge/`.
Define el propósito de cada archivo y cómo se relacionan.
8. EVIDENCIA Y HUMAN GATES
Diseña un flujo de validación incremental.
La ejecución real en EA requiere intervención humana y no puede ser simulada.
Define cómo Apply/Tasks deberían trabajar de manera incremental, preferentemente:
Q1 implementación → SQLite secundaria → EA real → evidencia
Q2 implementación → SQLite secundaria → EA real → evidencia
Q3 implementación → SQLite secundaria → EA real → evidencia
documentación consolidada
Prolaborate opcional
Verify.
Define qué evidencia se debe capturar por interrogante y cómo demostrar:
pregunta → SQL → resultado → modelo/elemento EA.
NO inventes screenshots ni resultados EA.
9. SQLITE SECUNDARIO
Diseña SQLite `mode=ro` solo como oráculo/validación independiente.
Debe permitir comparar contra:
Q1: 8 + lista
Q2: 16/14 + missing 0
Q3: 5 + lista, con Nodes como control si se utiliza.
Nunca sustituye la validación EA.
11. PROLABORATE OPTIONAL FOLLOW-ON
Diseña solamente la estrategia opcional posterior a la validación EA.
Consulta la guía adjunta/local.
La guía permite construir un Donut mediante SQL Queries entrando en Query Configuration, introduciendo y ejecutando la query antes de configurar el chart.
Evalúa cuál de las queries obligatorias sería el candidato más claro para visualización y documenta la recomendación con rationale, pero NO la implementes todavía.
No introduzcas aliases específicos como `seriesproperty` salvo que la funcionalidad elegida realmente los necesite; la guía lo relaciona específicamente con colores individuales definidos mediante Color Palette Configuration.
Prolaborate sigue sin bloquear la entrega obligatoria.
12. READ-ONLY Y LÍMITES

QEA estrictamente read-only.
No UPDATE/INSERT/DELETE/ALTER/DROP/CREATE.
No modificar Addino/E1.
No modificar RDD/review mode.
No introducir C#.
No mover ni renombrar el QEA.
13. TRAZABILIDAD DE DISEÑO
Incluye una matriz o sección que muestre cómo las decisiones de Design satisfacen las tres capability Specs y los requisitos transversales.
Distingue claramente:
- requirement;
- design decision;
- validation/evidence mechanism.
14. RIESGOS / DECISIONES
Documenta architecture/design decisions con rationale, incluyendo al menos:
- statement decomposition;
- uniqueness strategy;
- EA alias strategy;
- target identification strategy;
- EA vs SQLite validation;
- evidence organization;
- optional Prolaborate candidate;
- human validation gates.
NO escribas los archivos SQL finales.
NO modifiques `Exercise_2_queries/`.
NO ejecutes queries de implementación como producto final.
Las consultas diagnósticas read-only solo están permitidas si son necesarias para resolver una decisión de Design.
NO modifiques Specs/Proposal/Explore.
NO hagas commit.
NO ejecutes Tasks.
Al finalizar:
- indica `design.md` creado/modificado;
- resume las decisiones de arquitectura tomadas;
- informa el número final de sentencias SELECT elegido y su distribución Q1/Q2/Q3;
- informa la estrategia de unicidad;
- informa los archivos finales propuestos;
- informa la estrategia de evidencia y human gates;
- confirma que no implementaste nada;
- ejecuta `git diff --check`;
- muestra `git status --short`;
- detente para revisión humana.

### P-TASKS

#### Resumen documental

Ejecución exclusiva de Tasks sobre los artefactos aprobados y el diseño de cinco SELECTs.
El literal solicita Qwen 3.7 Plus; el runtime real registrado fue Kimi K2.7 Code por indisponibilidad de Qwen.

#### Prompt original

Explore, Proposal, Specs y Design de `ea-governance-queries-exercise-2` han sido revisados y APROBADOS por el operador.
Continúa ÚNICAMENTE con la fase `sdd-tasks` usando Qwen 3.7 Plus.
NO ejecutes Apply, Verify ni Archive.
CHANGE: ea-governance-queries-exercise-2
ROOT: C:\Proagile\Addin
ÁREA DE ENTREGA: C:\Proagile\Addin\Exercise_2_queries
QEA LOCAL DE TRABAJO: C:\Proagile\Repositorio Pasantias.qea
FUENTES NORMATIVAS:
- exploration.md aprobado
- proposal.md aprobado
- las tres Specs aprobadas
- design.md aprobado
- consigna bajo `Exercise_2_queries/docs/challenge/`
Design es ahora la autoridad sobre decisiones de implementación.
IMPORTANTE: NO resumas, acortes ni comprimas artificialmente `tasks.md` para ajustarlo a una cantidad de palabras o tareas. El objetivo es producir un plan ejecutable, incremental y verificable, no minimizar el número de tasks.
NO reabras decisiones de Design salvo contradicción objetiva. Si detectas una contradicción objetiva que impide generar Tasks, repórtala y detente.
DECISIONES CERRADAS:
- exactamente cinco SELECTs finales: Q1-C, Q1-L, Q2-G, Q3-C,Q3-L
- distribución:  Q1 = 2,  Q2 = 1, Q3 = 2
- un único archivo `.sql` de entrega, NO ejecutado como batch;
- cada SELECT se copia/crea/ejecuta individualmente en EA SQL Search;
- texto ejecutable EA comienza en SELECT;
- Q1-C = COUNT(DISTINCT app.Object_ID);
- Q1-L = SELECT DISTINCT con columnas estables por aplicación;
- Q2-G = grouped Vigencia + COUNT(DISTINCT app.Object_ID);
- Q3-C = COUNT(DISTINCT src.Object_ID);
- Q3-L = SELECT DISTINCT con columnas estables por aplicación source;
- Q1-L/Q3-L incluyen CLASSGUID/CLASSTYPE;
- agregados no incluyen aliases representativos;
- no PropertyID ni Connector_ID en listados finales cuando rompan deduplicación;
- Q3 identifica target semánticamente, nunca por Object_ID=102 como filtro;
- no Direction;
- EA SQL Search = autoridad primaria;
- SQLite mode=ro = diagnóstico secundario;
- Prolaborate = opcional, solo después de Q1/Q2/Q3 EA;
- Q2-G es candidato Prolaborate;
- toda evidencia submission-facing lleva Pino;
- Addino/E1 y QEA protegidos.
ESTRUCTURA OBLIGATORIA DE TASKS:
Agrupa las tareas por fases/work units y mantén cada task realizable en una sesión.
El plan DEBE separar claramente trabajo automatizable del agente y HUMAN GATES que requieren al operador.
No marques una tarea humana como realizable por el agente.
FASE 1 — SCAFFOLDING Y TRAZABILIDAD
Planifica la creación, durante Apply, de la estructura aprobada:
`Exercise_2_queries/queries/Pino_Exercise_2_EA_Governance_Queries.sql`
`Exercise_2_queries/docs/Pino_Exercise_2_Technical_Explainer.md`
`Exercise_2_queries/docs/Pino_Exercise_2_AI_Usage_Log.md`
`Exercise_2_queries/evidence/Pino_Exercise_2_Evidence_Index.md`
carpetas:
`evidence/ea/`
`evidence/sqlite/`
`evidence/prolaborate/`
`prolaborate/`
Preservar `docs/challenge/`.
El Evidence Index puede contener estados PENDING antes de ejecución real, pero MUST NOT presentar placeholders como evidencia ejecutada.
No crear PNG ficticios ni archivos vacíos que pretendan ser screenshots.
FASE 2 — Q1 CATEGORIA ORO
Planifica tasks para:
1. Implementar Q1-C.
2. Implementar Q1-L.
3. Revisar estáticamente filtros, aliases y unicidad.
4. Ejecutar diagnóstico SQLite estrictamente mode=ro.
5. Verificar oracle:
   count = 8
   list = Aplicación 1,2,3,4,6,8,20,29.
6. Actualizar documentación secundaria con resultados reales del diagnóstico.
7. DETENERSE EN HUMAN GATE Q1.
HUMAN GATE Q1:
El operador ejecutará Q1-C y Q1-L individualmente en EA SQL Search, idealmente con nombres:
`Q1-C — Categoria ORO — Count`
`Q1-L — Categoria ORO — List`
Debe comprobar:
- count 8;
- lista exacta;
- Categoria visible;
- navegación desde al menos una fila de Q1-L;
- evidencia real screenshot/export.
Apply MUST detenerse antes de continuar a Q2 si todavía no existe confirmación humana de este gate.
Planifica después una task para incorporar/indexar la evidencia REAL proporcionada por el operador.
FASE 3 — Q2 VIGENCIA
Solo después de HUMAN GATE Q1 aprobado.
Planifica:
1. Implementar Q2-G.
2. Verificar que usa tag Vigencia y nunca t_object.Status.
3. Verificar aplicación scope.
4. Verificar COUNT(DISTINCT app.Object_ID).
5. Diagnóstico SQLite mode=ro.
6. Confirmar:
   Vigente=16
   Deprecado=14
   NULL=0
   empty=0
   N/A=0.
7. Comprobar que no existen Vigencias conflictivas por aplicación.
8. Si aparece conflicto, registrar anomalía y detener Q2 sin inventar precedencia.
9. DETENERSE EN HUMAN GATE Q2.
HUMAN GATE Q2:
El operador ejecuta Q2-G individualmente en EA y comprueba:
- 16/14;
- comparación con tagged values visibles;
- ausencia de buckets artificiales;
- evidencia real.
Después, task para incorporar/indexar evidencia real.
FASE 4 — Q3 IMPACTO DB28
Solo después de HUMAN GATE Q2 aprobado.
Planifica:
1. Diagnóstico read-only de cardinalidad del target semántico.
2. Confirmar exactamente un target:
   Name='Base de Datos 28'
   Object_Type='Class'
   Stereotype='ArchiMate_DataObject'.
3. Object_ID=102 solo se registra como oracle, nunca filtro.
4. Si cardinalidad != 1, registrar anomalía y detener Q3.
5. Implementar Q3-C.
6. Implementar Q3-L.
7. Verificar Start_Object_ID=source y End_Object_ID=target.
8. Verificar Connector_Type='Dependency'.
9. Verificar ausencia de Direction predicate.
10. Verificar source application scope.
11. Diagnóstico SQLite mode=ro.
12. Confirmar:
    count = 5
    Aplicación 5,12,22,25,27.
13. Servidor 2,5,19 pueden documentarse como negative-control evidence pero MUST NOT aparecer en resultado obligatorio.
14. DETENERSE EN HUMAN GATE Q3.
HUMAN GATE Q3:
El operador ejecuta Q3-C/Q3-L individualmente en EA.
Debe comprobar:
- count 5;
- lista exacta;
- Nodes excluidos;
- source/target Dependency visible en el modelo;
- navegación desde al menos una fila Q3-L;
- evidencia real.
Después, task para incorporar/indexar evidencia real.
FASE 5 — COMPATIBILIDAD EA
Incluye tasks explícitas para manejar fallos reales de dialecto.
En particular:
- probar COUNT(DISTINCT app.Object_ID);
- probar COUNT(DISTINCT src.Object_ID);
- probar aliases CLASSGUID/CLASSTYPE;
- probar acentos;
- probar ORDER BY;
- comprobar que cada bloque ejecutado comienza en SELECT.
Si EA rechaza COUNT(DISTINCT ...):
- NO cambiar semántica;
- documentar evidencia del fallo;
- implementar únicamente un fallback SQL básico read-only compatible;
- volver a pasar SQLite diagnostic + HUMAN EA gate correspondiente.
No anticipar fallback si EA acepta la forma primaria.
FASE 6 — DOCUMENTACIÓN FINAL OBLIGATORIA
Solo después de Q1/Q2/Q3 human gates aprobados.
Planifica completar
Technical Explainer:
- pregunta de negocio;
- statement ID;
- tablas;
- campos;
- joins;
- filtros;
- criterio de aplicación;
- tagged values;
- estrategia de unicidad;
- target DB28;
- Direction decision;
- EA vs SQLite;
- oracles;
- supuestos/anomalías;
- instrucciones reproducibles de ejecución individual en EA.
Evidence Index:
- question;
- Statement ID;
- SQL location;
- EA Search display name;
- expected oracle;
- actual EA result;
- model comparison;
- evidence filename;
- operator/date;
- pass/fail.
AI Usage Log:
>=5 interacciones significativas REALES.
Cada entrada MUST tener ocho campos separados:
1 ID
2 objetivo
3 herramienta
4 modelo
5 estrategia/prompt
6 decisión tomada
7 evidencia relacionada
8 resultado
Puede documentar retrospectivamente interacciones genuinas ya realizadas si todos los campos se recuperan fielmente.
No fabricar prompts, modelos, decisiones ni evidencia.
FASE 7 — PROLABORATE OPTIONAL FOLLOW-ON
Marcar todas estas tasks como opcionales y solo habilitadas después de Q1/Q2/Q3 aprobados en EA.
Plan:
- partir de Q2-G EA-validada;
- Create Chart Widget;
- SQL Queries;
- Skip to Query;
- Query Configuration;
- View Sample cuando sea útil;
- Execute;
- si funciona directamente, reutilizar Q2-G;
- si Prolaborate exige shape/aliases de presentación, permitir adaptación mínima documentada que preserve exactamente semántica/resultados;
- EA Q2-G sigue siendo autoridad;
- documentar business question y configuración;
- capturar evidencia visual real;
- toda evidencia lleva Pino.
No incluir `seriesproperty` salvo necesidad real demostrada por Color Palette Configuration.
La ausencia de Prolaborate MUST NOT impedir completar el alcance obligatorio.
FASE 8 — PRE-VERIFY
Planifica una auditoría final antes de sdd-verify:
- exactamente 5 SELECTs finales;
- todos SELECT/read-only;
- QEA sin modificación;
- Addino/E1 sin modificación;
- naming Pino;
- query → result → model → evidence trazable;
- oracles correctos;
- evidencia real por cada statement;
- AI log completo;
- links/rutas relativas válidas;
- ningún placeholder presentado como prueba;
- git diff --check;
- git status --short.
IMPORTANTE PARA APPLY:
Tasks debe dejar explícitos los STOP POINTS.
No debe diseñar un futuro `sdd-apply` que implemente Q1/Q2/Q3 de una sola vez ignorando validaciones humanas.
El flujo debe poder ejecutarse incrementalmente:
Apply Q1
→ HUMAN GATE Q1
→ Apply Q2
→ HUMAN GATE Q2
→ Apply Q3
→ HUMAN GATE Q3
→ documentación
→ Prolaborate opcional
→ Verify.
Cada human gate MUST ser una dependencia bloqueante para el siguiente work unit obligatorio.
NO implementes archivos finales during Tasks.
NO ejecutes SQL.
NO modifiques Exercise_2_queries/.
NO modifiques QEA.
NO modifiques Addino/E1.
NO modifiques Specs/Proposal/Explore/Design.
NO hagas commit.
NO ejecutes Apply.
Al finalizar:
- indica `tasks.md` creado;
- informa número de fases y tasks;
- identifica explícitamente los tres human gates obligatorios;
- muestra el orden de dependencias;
- confirma que Apply podrá ejecutarse incrementalmente;
- ejecuta `git diff --check`;
- muestra `git status --short`;
- detente para revisión humana.

### P-APPLY-Q1

Estado del prompt original: no adjuntado actualmente al registro.

Resumen documental: implementó WU1 y WU2 hasta 2.6, con diagnóstico SQLite read-only de Q1-C/Q1-L, y se detuvo antes del HUMAN GATE Q1. El operador puede adjuntar posteriormente el literal auténtico sin reconstruirlo.

### P-APPLY-Q2

Estado del prompt original: no adjuntado actualmente al registro.

Resumen documental: incorporó evidencia real de Q1, implementó Q2-G y su diagnóstico SQLite `mode=ro`, dejando HUMAN GATE Q2 pendiente. El operador puede adjuntar posteriormente el literal auténtico sin reconstruirlo.

### P-APPLY-Q3

Estado del prompt original: no adjuntado actualmente al registro.

Resumen documental: incorporó evidencia real de Q2, implementó Q3-C/Q3-L y verificó read-only el target semántico y controles negativos, deteniéndose antes de HUMAN GATE Q3. El operador puede adjuntar posteriormente el literal auténtico sin reconstruirlo.

### P-APPLY-FINAL

#### Resumen documental

Ejecución exclusiva de tareas restantes con evidencia PDF real, consolidación documental, Prolaborate opcional y auditoría pre-verify.
Prohíbe Verify, Archive, ejecución EA/SQLite, staging, commit y PR; el estado de detención es listo para Verify.

#### Prompt original

Act as sdd-apply executor. Execute ONLY remaining Tasks: 4.9, 5.1–5.5, 6.1–6.5, 7.1–7.7 (explicit N/A where applicable), 8.1–8.8. STOP ready for verify; do not run verify/archive/EA/SQLite/browser automation. Native attempt token `sha256:5b7759f1cbff88275f8cf91133c7e53cdae5ae56e28fc55878d596ca2ff13cd6`; strict_tdd false. Read governing artifacts/tasks/current delivery docs. Never modify SQL, QEA, PDFs, Addino/E1/challenge/OpenSpec except tasks checkboxes; no staging/commit/PR.

FIRST locate real operator PDFs in workspace, exact paths/extensions: names beginning `Pino_Exercise_2_Informe_Principal_Queries_EA` and `Pino_Evidencias_Funcionales_Queries`. Treat only physically found PDFs as read-only real evidence. If either missing, STOP before completing 6.2/6.4(if dependent)/6.5/7.6/8.5, report exact missing path/name, do not invent. Do not invent filenames/pages/screenshots; reference filename+faithful section only if known.

Human fact: Gate Q3 PASS (Q3-C=5; exact Q3-L 5/12/22/25/27; target DB28; servers excluded; navigation incl App22/related links dependency verified; no fallback). Mark 4.9 on this real confirmation and PDF evidence; all Q1/Q2/Q3 statement state EA_PASS.

Phase5 documentary audit: every primary form accepted no fallback: COUNT DISTINCT app/src, aliases, order by, accents, individual blocks start SELECT. no fallback. Update index/explainer exact phrase EA primary form accepted — no fallback required. no re-runs.

Phase6 final docs: technical explainer complete required implementation/design details/results/EApdf references; index every statement exact required fields, PDF paths, no mandatory Pending. Normalize AI log—preserve authentic huge pasted prompts, table eight fields, short faithful prompt summary+P IDs, section Prompts used/references; normalize tools/models Explore MiniMax, Proposal Terra, Spec Kimi, Design Sol, Tasks Kimi NOT Qwen, Applies Kimi; no fabricated prompts/EA interactions; add final apply only if truthful. Add final state note gates pass/prolaborate complete.

Phase7 Prolaborate actually complete: create `Exercise_2_queries/prolaborate/Pino_Exercise_2_Prolaborate_Q2.md` (brief only) documenting dashboard Gobierno de Aplicaciones - Pasantías - Pino; business question; EA Q2-G source 16/14; direct SQL Q2-G executed correctly but generic Open/Closed 75/25 preview binding; Designer Configuration Class/ApplicationComponent/tag Vigencia values/generate Series shape; Donut Deprecado46.67/Vigente53.33; bars 14/16; no seriesproperty; semantics equal EA; PDF evidence ref. Q2-G remains EA authority, generated presentation query not in official 5. 7.7 mark N/A explicitly: Prolaborate available. It happened after gates but before phase6 formal consolidation: record nonblocking procedural sequence, no gate omitted.

Phase8 audit exact 5 official SELECT IDs/no diagnostics; semantic read-only no mutation; protected areas; Pino; complete trace to real PDF; baselines not hardcoded; relative valid Markdown links/no invented paths; status/diff and manual all untracked submission docs (diff check caveat). Mark remaining tasks only genuinely complete/N-A. Evidence PENDING permitted optional only (not mandatory). Save/merge progress. Return all 28 requested result categories incl exact PDFs, tool/model table, prompt references/pending manual, tasks status, blockers. Do NOT settle attempt.
