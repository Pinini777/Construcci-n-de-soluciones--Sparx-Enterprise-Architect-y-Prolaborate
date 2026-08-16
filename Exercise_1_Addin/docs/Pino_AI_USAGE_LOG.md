# AI_USAGE_LOG — Ejercicio 1: Addino

## Introducción

Este archivo complementa el documento `Pino_Registro_Uso_IA_Ejercicio 1.pdf` y resume las interacciones de Inteligencia Artificial más relevantes utilizadas durante el desarrollo del Add-in para Sparx Enterprise Architect.

El uso de IA se realizó principalmente mediante **Gemini Notebook** para consultas sobre documentación técnica y mediante **OpenCode + Gentle-AI** para organizar el desarrollo bajo metodología SDD.

Las respuestas generadas por IA fueron utilizadas como apoyo para análisis, diseño, implementación y verificación. Las decisiones finales permanecieron bajo revisión humana y las funcionalidades que dependían del comportamiento real de Enterprise Architect fueron validadas manualmente mediante pruebas funcionales y Human Gates.

Este registro no pretende reproducir todas las conversaciones realizadas durante el proyecto. Se incluyen interacciones representativas que tuvieron impacto directo sobre decisiones técnicas, artefactos SDD, implementación o verificación.

## Artefactos SDD

Durante el Ejercicio 1 se utilizaron dos changes principales de OpenSpec:

- `ea-metadata-review-exercise-1`: implementación y validación del alcance obligatorio.
- `ea-metadata-review-optionals-ui`: incorporación de los requisitos opcionales y actualización visual.

Mientras se encontraban en desarrollo, estos changes permanecieron activos bajo `openspec/changes/`. Una vez completadas la implementación, las validaciones y la fase Verify, fueron cerrados mediante **Archive** y trasladados a `openspec/changes/archive/`.

Rutas finales principales:

- `openspec/changes/archive/ea-metadata-review-exercise-1/`
- `openspec/changes/archive/2026-08-15-ea-metadata-review-optionals-ui/`

Las carpetas archivadas conservan Proposal, Spec, Design, Tasks, Verify y demás artefactos necesarios para reconstruir la trazabilidad de las decisiones tomadas durante el desarrollo.

---

## 1. Consultas sobre documentación técnica

| ID | Objetivo | Herramienta | Modelo | Estrategia / Prompt | Decisión tomada | Evidencia | Resultado |
|---|---|---|---|---|---|---|---|
| **DOC-001** | Determinar cómo obtener únicamente los elementos directos de un paquete. | Gemini Notebook | Gemini 3.5 Flash | Consultar el Object Model de EA para identificar la diferencia entre `Package.Elements` y `Package.Packages`. | Utilizar `Package.Elements` para los elementos directos y tratar `Package.Packages` como una colección independiente. | `EV:DOC-001` en el PDF de Registro de Uso de IA. | Se confirmó la separación entre elementos directos y subpaquetes, estableciendo la base para la carga inicial y la posterior extensión recursiva. |
| **DOC-002** | Determinar cómo validar que el usuario seleccionó realmente un paquete en el Project Browser. | Gemini Notebook | Gemini 3.5 Flash | Comparar `GetTreeSelectedPackage()`, `GetTreeSelectedItemType()` y `GetTreeSelectedObject()`. | Comprobar primero `Repository.GetTreeSelectedItemType() == EA.ObjectType.otPackage` y recuperar posteriormente el objeto mediante `GetTreeSelectedObject()`. | `EV:DOC-002` en el PDF de Registro de Uso de IA. | Se evitó aceptar accidentalmente el paquete contenedor cuando el objeto seleccionado no era realmente un `EA.Package`. |
| **DOC-003** | Analizar el comportamiento de `Element.Update()` ante errores. | Gemini Notebook | Gemini 3.5 Flash | Revisar si `Element.Update()` puede devolver `false` sin lanzar una excepción y cómo distinguir ambos casos. | Evaluar explícitamente el valor booleano retornado por `Update()` además de utilizar manejo de excepciones. | `EV:DOC-003` en el PDF de Registro de Uso de IA. | El guardado pudo diferenciar éxito, `Update() == false` y excepciones, procesando cada fila independientemente. |

---

## 2. Desarrollo SDD — Alcance obligatorio

| ID | Fase | Objetivo | Herramienta | Modelo | Estrategia / Prompt | Decisión tomada | Evidencia | Resultado |
|---|---|---|---|---|---|---|---|---|
| **DES-001** | Init | Inicializar formalmente Addino dentro del flujo SDD y reconocer el estado técnico existente. | OpenCode / Gentle-AI | GPT-5.6 Luna | Inspeccionar el baseline, `Addino.csproj`, `AssemblyInfo.cs`, configuración COM y estructura SDD/OpenSpec antes de modificar funcionalidad. | Mantener la infraestructura existente del Add-in como baseline y desarrollar el ejercicio sobre el mismo proyecto sin alterar inicialmente COM, framework ni plataforma. | Estado inicial del repositorio, proyecto, configuración SDD/OpenSpec y baseline Git. | El proyecto existente fue reconocido correctamente y quedó preparado para iniciar Explore. |
| **DES-002** | Explore | Analizar el desafío, el Add-in existente, restricciones, riesgos y decisiones pendientes. | OpenCode / Gentle-AI | MiniMax M3 | Revisar consigna, guía de creación de Add-ins, código baseline y puntos críticos de selección, carga, edición, cancelación y persistencia. | Reutilizar C#/.NET Framework, WinForms, Interop.EA y COM; validar estrictamente `EA.Package`; mantener inicialmente los opcionales fuera del alcance base. | `exploration.md`, consigna, guía técnica e inspección del baseline. | No se detectaron blockers técnicos y se estableció una base verificada para Proposal. |
| **DES-003** | Proposal | Definir formalmente alcance, estrategia, restricciones y criterios de éxito. | OpenCode / Gentle-AI | GPT-5.6 Terra | Convertir los hallazgos de Explore en una propuesta proporcional al desafío sin entrar aún en detalles de diseño. | Implementar primero únicamente el alcance obligatorio: selección de paquete, elementos directos, edición local y guardado explícito. | `proposal.md` y `exploration.md`. | Se aprobó un alcance concreto, con criterios de éxito y exclusiones claramente definidos. |
| **DES-004** | Spec | Transformar la consigna y Proposal en requisitos verificables. | OpenCode / Gentle-AI | Kimi K2.7 Code | Formalizar requisitos mediante lenguaje normativo y escenarios de aceptación antes de diseñar la arquitectura. | Utilizar la Spec como contrato funcional y separar requisitos de decisiones de implementación. | `specs/ea-metadata-review/spec.md`, `proposal.md`, `exploration.md`. | Se obtuvo una especificación implementable y verificable para el alcance obligatorio. |
| **DES-005** | Design | Diseñar la arquitectura concreta del Add-in respetando Proposal y Spec. | OpenCode / Gentle-AI | GPT-5.6 Sol | Definir componentes, modelo de datos local, binding, flujo de apertura, Save, manejo de errores y relación con objetos COM. | Utilizar `AddinoClass`, `MetadataElementRow` y `MetadataReviewForm`; evitar referencias COM persistentes en la UI y recuperar cada elemento mediante `ElementId` al guardar. | `design.md`, Spec y Proposal. | El diseño quedó aprobado con arquitectura, flujo de datos, persistencia y manejo de errores definidos. |
| **DES-TASKS** | Tasks | Convertir el Design aprobado en un plan incremental y verificable. | OpenCode / Gentle-AI | Qwen 3.7 Plus | Dividir la implementación en tareas pequeñas, mantener las restricciones del baseline y separar trabajo automatizable de validaciones manuales. | Ejecutar la implementación de forma incremental y detener el avance cuando una comprobación necesitara validación real en Enterprise Architect. | `tasks.md`. | Se obtuvo un plan de implementación verificable que permitió avanzar de forma controlada hacia Apply. |
| **DES-APPLY** | Apply | Implementar el alcance obligatorio definido por Spec y Design. | OpenCode / Gentle-AI | Kimi K2.7 Code | Modificar únicamente los componentes definidos por el Design, preservar COM/x64 y comprobar build después de los cambios. | Mantener la edición en memoria hasta Guardar, persistir únicamente filas modificadas y preservar Cancelar/Esc/X sin escrituras. | Código fuente de `Exercise_1_Addin`, `tasks.md` y builds. | El alcance obligatorio quedó implementado y preparado para validación manual. |
| **DES-006** | Verify | Auditar el alcance obligatorio contra la consigna y los artefactos SDD. | OpenCode / Gentle-AI | GPT-5.6 Sol | Revisar Spec, Proposal, Design, Tasks, implementación, README, evidencias y build sin asumir cumplimiento por el estado de las tareas. | Considerar el alcance base terminado únicamente después de una revisión independiente de requisitos, implementación y evidencia. | `verify-report.md`. | El alcance obligatorio fue verificado con `18/18` tasks completadas y quedó preparado para su cierre SDD. |
| **DES-ARCHIVE** | Archive | Cerrar formalmente el change correspondiente al alcance obligatorio. | OpenCode / Gentle-AI | MiMo V2.5 | Consolidar los artefactos aprobados y retirar el change de la lista de cambios activos. | Preservar el historial SDD completo bajo `openspec/changes/archive/` en lugar de eliminar los artefactos del desarrollo. | `openspec/changes/archive/ea-metadata-review-exercise-1/`. | El alcance obligatorio quedó archivado y disponible como baseline estable para extensiones posteriores. |

---

## 3. Desarrollo SDD — Requisitos opcionales

Después de completar el alcance obligatorio se inició un segundo change independiente, `ea-metadata-review-optionals-ui`, para incorporar las mejoras opcionales sin modificar las garantías funcionales ya validadas.

| ID | Fase / Objetivo | Herramienta | Modelo | Estrategia / Prompt | Decisión tomada | Evidencia | Resultado |
|---|---|---|---|---|---|---|---|
| **OPT-001** | Incorporar los requisitos opcionales sobre el baseline estable. | OpenCode / Gentle-AI | Agentes SDD según fase | Crear un change independiente y preservar las funcionalidades ya aprobadas del alcance obligatorio. | Extender Addino sin modificar identidad COM, framework, plataforma x64 ni reglas previas de persistencia. | `proposal.md`, Spec y Design de `ea-metadata-review-optionals-ui`. | Los opcionales quedaron aislados del cambio base y definidos mediante requisitos verificables. |
| **OPT-002** | Implementar validación estricta de `Name`. | OpenCode / Gentle-AI | Kimi K2.7 Code | Validar globalmente todas las filas dirty antes de ejecutar cualquier `Element.Update()`. | Bloquear completamente el guardado si alguna fila modificada contiene `Name` vacío o compuesto únicamente por espacios. | Spec, `tasks.md`, código y Human Gate 1. | La validación preventiva quedó implementada sin permitir persistencia parcial provocada por un Nombre inválido. |
| **OPT-003** | Incorporar carga recursiva y columna `Paquete`. | OpenCode / Gentle-AI | Kimi K2.7 Code | Recorrer el paquete seleccionado y todos sus descendientes utilizando identidad estable para evitar repeticiones. | Utilizar un loader recursivo común, registrar `PackagePath` y proteger el recorrido frente a paquetes o elementos repetidos. | Código, `tasks.md` y Human Gate 2. | La grilla pasó a representar elementos del paquete raíz y de toda su jerarquía, mostrando su ubicación mediante `Paquete`. |
| **OPT-004** | Incorporar seguimiento visual de modificaciones pendientes. | OpenCode / Gentle-AI | Kimi K2.7 Code | Utilizar los valores originales de cada fila como línea base y derivar el estado dirty mediante comparación. | Adoptar `IsDirty` como autoridad para determinar si una fila contiene modificaciones pendientes. | `MetadataElementRow.cs`, formulario, `tasks.md` y Human Gate 3. | Las filas modificadas pudieron resaltarse y volver automáticamente a estado limpio al restaurar los valores originales. |
| **OPT-005** | Implementar Recargar de forma segura. | OpenCode / Gentle-AI | Kimi K2.7 Code | Reutilizar el mismo loader de apertura, diferenciar estado clean/dirty y evitar cualquier llamada implícita a Save. | Recargar inmediatamente si no existen cambios; ante cambios pendientes solicitar confirmación Sí/No, donde No conserva el estado y Sí descarta y vuelve a consultar EA. | Código, Spec, `tasks.md` y Human Gate 4. | Recargar quedó implementado sin auto-save ni persistencia accidental de modificaciones locales. |
| **OPT-006** | Mejorar la presentación visual de la ventana. | OpenCode / Gentle-AI | Kimi K2.7 Code | Actualizar jerarquía visual, encabezados, botones y comportamiento de resize sin implementar custom chrome. | Mantener la caption y controles nativos de Windows y utilizar `#557DA5` como color principal de la interfaz. | Design, código y primera ejecución de Human Gate 5. | La primera propuesta visual funcionó técnicamente, pero fue rechazada manualmente por problemas de superposición del header interno. |
| **OPT-007** | Corregir Phase 5 después del rechazo manual de HG5. | OpenCode / Gentle-AI | Kimi K2.7 Code | Eliminar completamente el header interno y aplicar color sobre la caption nativa mediante DWM de forma best-effort. | Reemplazar la decisión visual anterior por una única caption nativa; autorizar únicamente `DwmSetWindowAttribute` para color de caption/texto y conservar fallback nativo seguro. | `MetadataReviewForm.cs`, `MetadataReviewForm.Designer.cs`, Spec, Design, Tasks y re-Human Gate 5. | La interfaz corregida fue validada nuevamente en Enterprise Architect y Human Gate 5 quedó aprobado. |
| **OPT-008** | Cerrar Tasks después de aprobar todos los Human Gates. | OpenCode / Gentle-AI | Qwen 3.7 Plus | Registrar re-HG5 como PASS, completar la planificación de regresión y reconciliar Tasks con Spec/Design. | Considerar completas las tareas únicamente después de los cinco Human Gates aprobados por el operador. | `tasks.md`. | Phase 5 y Phase 6 quedaron cerradas con **33/33 tasks completadas**. |
| **OPT-009** | Ejecutar Verify sobre la versión final del Add-in. | OpenCode / Gentle-AI | GPT-5.6 Sol | Auditar código real contra Proposal, Delta Spec, Design, Tasks, Human Gates y README; compilar nuevamente Debug/x64. | Utilizar las pruebas manuales del operador como evidencia runtime autoritativa y mantener como no ejecutado el checklist formal R-01..R-13 cuando correspondiera. | `verify-report.md`, Human Gates, README y build `bin\x64\Debug\Addino.dll`. | Verify finalizó con **PASS: 6/6 requirements, 32/32 scenarios, 33/33 tasks, 0 errores y 0 warnings**. |
| **OPT-010** | Cerrar formalmente el change opcional. | OpenCode / Gentle-AI | Agente de Archive configurado | Reconciliar la documentación final, consolidar el Delta Spec y ejecutar Archive sin modificar el código funcional. | Actualizar la especificación canónica con los opcionales aprobados y preservar el change completo como historial cerrado. | `openspec/changes/archive/2026-08-15-ea-metadata-review-optionals-ui/`. | Archive finalizó correctamente, el change dejó de estar activo y la versión final de Addino quedó cerrada dentro del flujo SDD. |

---

## 4. Validación humana y uso responsable de IA

Las respuestas de los modelos no fueron utilizadas como evidencia suficiente de funcionamiento por sí mismas. Las funcionalidades dependientes de Enterprise Architect fueron comprobadas manualmente sobre la aplicación real.

Durante el desarrollo de los opcionales se utilizaron **Human Gates** para detener el avance hasta recibir aprobación del operador. Este mecanismo permitió, entre otras cosas, rechazar una primera implementación visual de Phase 5 y corregirla antes de continuar.

El resultado final del Ejercicio 1 quedó validado mediante:

- Human Gates 1 a 5 aprobados.
- Pruebas funcionales manuales en Enterprise Architect.
- Build `Debug|x64` exitoso.
- `0` errores de compilación.
- `0` warnings.
- `6/6` requirements conformes.
- `32/32` scenarios conformes.
- `33/33` tasks completadas.
- Verify final: **PASS**.
- Archive final: **COMPLETE**.

## Resultado final

El uso de Inteligencia Artificial permitió apoyar tareas de investigación documental, estructuración SDD, análisis técnico, implementación, revisión y documentación. Las decisiones funcionales y la aceptación final permanecieron bajo control humano, utilizando Enterprise Architect y los artefactos del repositorio como fuentes de evidencia.

La trazabilidad completa del desarrollo se conserva en los artefactos archivados de OpenSpec y en el documento principal `Pino_Registro_Uso_IA_Ejercicio 1.pdf`.