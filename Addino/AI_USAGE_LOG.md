# Registro de Uso de Inteligencia Artificial

Este documento registra interacciones reales con herramientas y modelos de Inteligencia Artificial utilizadas durante el desarrollo de **Addino**, Add-in desarrollado en C# para Sparx Enterprise Architect como parte del Desafío Técnico de Práctica de Proagile 2026.

El objetivo del registro es mantener trazabilidad entre:

**consulta → herramienta/modelo → decisión → evidencia → resultado**

La versión ampliada y visual de este registro, incluyendo capturas y contexto adicional, se encuentra en:

```text
Pino_Registro_Uso_IA.pdf
```

Este archivo `AI_USAGE_LOG.md` funciona como versión fuente y trazable dentro del repositorio.

---

## Criterio de utilización de IA

Durante el desafío se utilizaron distintos modelos según el tipo de tarea.

La selección buscó equilibrar:

- especialización del modelo según la fase;
- calidad de razonamiento;
- costo de ejecución;
- consumo de tokens/créditos;
- preservación de la ventana de contexto;
- trazabilidad de las decisiones.

Para las tareas de desarrollo se utilizó **Gentle-AI** integrado con **OpenCode**, siguiendo un flujo de desarrollo guiado por especificaciones (SDD).

Durante las fases SDD, **GPT-5.6 Terra** actuó como agente orquestador, delegando cada fase al modelo especializado indicado en este registro.

Las fases utilizadas incluyen:

```text
Init → Explore → Proposal → Spec → Design → Tasks → Apply → Verify → Archive
```

Los artefactos generados por estas fases se mantienen principalmente en:

```text
openspec/changes/ea-metadata-review-exercise-1/
```

Antes del desarrollo también se utilizó **Gemini Notebook** como apoyo para consultar documentación técnica de Enterprise Architect.

---

# Fase inicial — Documentación técnica

## Resumen de interacciones

| ID | Objetivo | Herramienta | Modelo | Estrategia / Prompt | Decisión tomada | Evidencia verificable | Resultado |
|---|---|---|---|---|---|---|---|
| **DOC-001** | Determinar cómo obtener solamente los elementos directos de un paquete. | Gemini Notebook | Gemini 3.5 Flash | Prompt completo documentado en la sección `DOC-001`. | Utilizar la colección correspondiente del modelo de EA y mantener separada la navegación de subpaquetes. | `Pino_Registro_Uso_IA.pdf`, evidencia `EV:DOC-001`. | La consulta permitió fundamentar el tratamiento de elementos directos y mantener la arquitectura del Add-in sobre C#, `Interop.EA`, COM y los callbacks públicos de EA. |
| **DOC-002** | Determinar cómo validar que el usuario seleccionó realmente un paquete. | Gemini Notebook | Gemini 3.5 Flash | Prompt completo documentado en la sección `DOC-002`. | No utilizar directamente `GetTreeSelectedPackage()` como validación estricta. Comprobar primero `Repository.GetTreeSelectedItemType() == EA.ObjectType.otPackage` y luego recuperar `GetTreeSelectedObject()` como `EA.Package`. | `Pino_Registro_Uso_IA.pdf`, evidencia `EV:DOC-002`; posteriormente `AddinoClass.cs`. | Se definió una validación estricta que evita aceptar accidentalmente el paquete contenedor cuando el usuario seleccionó otro tipo de objeto. |
| **DOC-003** | Determinar cómo tratar `Element.Update()` cuando devuelve `false` sin lanzar excepción. | Gemini Notebook | Gemini 3.5 Flash | Prompt completo documentado en la sección `DOC-003`. | Evaluar explícitamente el booleano retornado por `Update()` además del manejo mediante `try/catch`; un `false` se considera fallo individual de esa fila. | `Pino_Registro_Uso_IA.pdf`, evidencia `EV:DOC-003`; posteriormente `MetadataReviewForm.cs`. | El flujo de persistencia diferencia `Update() == false` de una excepción y continúa procesando las demás filas. |

---

## DOC-001 — Prompt completo

**Objetivo:** Cómo obtener solamente los elementos directos del paquete.

**Herramienta:** Gemini Notebook  
**Modelo:** Gemini 3.5 Flash

**Prompt utilizado:**

> “Según el Object Model de EA, ¿qué colección contiene los elementos directamente incluidos en un Package? Confirmá si Package.Elements incluye subpaquetes o si estos se gestionan separadamente.”

**Evidencia:** `EV:DOC-001` en `Pino_Registro_Uso_IA.pdf`.

---

## DOC-002 — Prompt completo

**Objetivo:** Cómo validar que el usuario seleccionó realmente un paquete.

**Herramienta:** Gemini Notebook  
**Modelo:** Gemini 3.5 Flash

**Prompt utilizado:**

> “Revisá en la documentación de EA cuál es la forma correcta de saber qué objeto está seleccionado en el Project Browser. Compará GetTreeSelectedPackage(), GetTreeSelectedItemType() y GetTreeSelectedObject(). Necesitamos aceptar únicamente una selección que sea realmente EA.Package.”

**Decisión tomada:**

Se descartó utilizar directamente `GetTreeSelectedPackage()`, porque puede devolver el paquete contenedor de otro objeto seleccionado.

Se decidió comprobar primero:

```csharp
Repository.GetTreeSelectedItemType() == EA.ObjectType.otPackage
```

y recién entonces recuperar:

```csharp
Repository.GetTreeSelectedObject()
```

y validarlo como `EA.Package`.

**Evidencia:** `EV:DOC-002` en `Pino_Registro_Uso_IA.pdf`.

---

## DOC-003 — Prompt completo

**Objetivo:** Qué hacer si `Element.Update()` devuelve `false` sin lanzar excepción.

**Herramienta:** Gemini Notebook  
**Modelo:** Gemini 3.5 Flash

**Prompt utilizado:**

> “Investigá el contrato de Element.Update(). ¿Puede fallar devolviendo false sin lanzar una excepción? ¿Cómo deberíamos distinguir ese caso de una excepción?”

**Decisión tomada:**

Se decidió evaluar explícitamente el valor booleano retornado por `Update()` además de utilizar `try/catch`.

Un `false` se trata como fallo de esa fila, se informa al usuario y se continúa procesando el resto.

**Evidencia:** `EV:DOC-003` en `Pino_Registro_Uso_IA.pdf`.

---

# Fase de Desarrollo — Gentle-AI / SDD

## Resumen de interacciones

| ID | Objetivo | Herramienta | Modelo de fase | Estrategia / Prompt | Decisión tomada | Evidencia verificable | Resultado |
|---|---|---|---|---|---|---|---|
| **DES-001** | Inicializar formalmente el proyecto Addino dentro del flujo SDD y reconocer su estado técnico antes de planificar cambios. | OpenCode + Gentle-AI | GPT-5.6 Luna | Prompt completo documentado en `DES-001`. | Preservar el baseline funcional, framework, COM, x64 e infraestructura existente y comenzar el análisis sin implementar la funcionalidad definitiva. | Estado inicial del repositorio, `Addino.csproj`, `Properties/AssemblyInfo.cs`, configuración SDD/OpenSpec y commit baseline funcional. | Gentle-AI reconoció el proyecto existente y dejó preparado el cambio para continuar sin modificar la funcionalidad validada. |
| **DES-002** | Investigar el Ejercicio 1, baseline Addino, requisitos, riesgos y decisiones necesarias antes de proponer una solución. | OpenCode + Gentle-AI | MiniMax-M3 | Prompt completo documentado en `DES-002`. | Separar requisitos obligatorios, mejoras y opcionales; identificar validaciones manuales y no implementar todavía. | `exploration.md`, desafío técnico, guía del Add-in, inspección del baseline y decisiones humanas registradas. | La exploración concluyó que no existían blockers técnicos para continuar y estableció una base verificada para Proposal. |
| **DES-003** | Definir formalmente alcance, intención, estrategia y límites del cambio sin entrar todavía en diseño detallado. | OpenCode + Gentle-AI | GPT-5.6 Terra | Prompt completo documentado en `DES-003`. | Limitar el cambio al Ejercicio 1 obligatorio y mantener fuera del alcance los desafíos opcionales y el Ejercicio 2. | `proposal.md`, `exploration.md`, desafío técnico. | Se aprobó un alcance concreto y proporcional, con criterios de éxito y exclusiones claramente definidos. |
| **DES-004** | Convertir Proposal y los requisitos del desafío en una especificación normativa y verificable. | OpenCode + Gentle-AI | Kimi K2.7 Code | Prompt completo documentado en `DES-004`. | Formalizar callbacks, selección, carga, columnas, edición local, Save, errores, plataforma y entregables como requisitos verificables sin diseñar todavía las clases. | `openspec/changes/ea-metadata-review-exercise-1/specs/ea-metadata-review/spec.md`, `proposal.md`, `exploration.md`. | Se obtuvo una Spec implementable y verificable que convirtió la consigna en requisitos claros y escenarios de aceptación. |
| **DES-005** | Diseñar la arquitectura concreta de implementación respetando Proposal y Spec aprobadas. | OpenCode + Gentle-AI | GPT-5.6 Sol | Prompt completo documentado en `DES-005`. | Definir una arquitectura simple, proporcional, COM-safe y sin sobrearquitectura, manteniendo edición local y persistencia explícita. | `design.md`, `spec.md`, `proposal.md`. | El diseño quedó aprobado sin preguntas abiertas y permitió avanzar a Tasks con arquitectura, flujo de datos, manejo de errores y estrategia de validación definidos. |

> En estas fases, GPT-5.6 Terra se utilizó como orquestador de Gentle-AI y delegó el trabajo al modelo de fase correspondiente.

---

# Prompts completos de Desarrollo

## DES-001 — Prompt completo

**Herramienta:** OpenCode + Gentle-AI  
**Modelo registrado:** GPT-5.6 Luna

**Prompt utilizado:**

```text
Quiero comenzar el trabajo formal del desafío técnico de Proagile sobre el proyccto
existente Addino. Para esta primera etapa realiza únicamente la fase de exploración SDD.
No implementes codigo, no modifiques la funcionalidad existente y no avances a propuesta,
especificación diseño ni tareas.

CONTEXTO
- Addino es un Add-in para Sparx Enterprise Architect 17.1
- Está desarrollado sobre C# sobre .NET Framework 4.7.2
- Usa Interop.EA, COM, y plataforma X64
- Existe un commit Git baseline funcional anterior a la implementación del desafío.
- La funcionalidad actual Say Hello/Say Goodbye es unicamente codigo de prueba y sera
reemplazada posteriormente.

FUENTES DE VERDAD
Lee primero:
- @docs/challenge/Desafio_Tecnico_Practica_EA_Prolaborate_v2 (1).md
- @docs/challenge/Guia_Creacion_Addin_Enterprise_Architect.md

Luego inspecciona unicamente los archivos relevantes del proyecto acutal. La consigna de
@docs/challenge/Desafio_Tecnico_Practica_EA_Prolaborate_v2 (1).md tiene prioirdad
sobre CUALQUIER inferencia o recomendación.

Consulta source-material/pdf/ solo si aparece incertidumbre tecnica que necesite validación.
No hagas una revision general de todos los PDF de esta fase.

OBJETIVOS DE LA EXPLORACION

Determina

1. Estado tecnico actual de Addino y que infraestructura del Add-in de prueba puede
reutilizarse.

2. Que partes de la funcionalidad actual deberan reemplazarse o ampliarse.

3. Los requisitos obligatorios, restricciones técnicas y crietrios de aceptación del Ejercicio 1.

4. Los principales riesgos o incertidumbres que deban resolverse antes de rediseñar la
solucion, especialmente los relacionados son:
- Selección del paquete en EA;
- carga de elementos directos;
- edicion en memoria;
- cancelacion sin persistencia;
- persistencia mediante Element.Update();
- manejo de errores;

5. Separacion explicita entre:
- Requisitos obligatorios;
- Mejoras de calidad posibles;
- Desafios opcionales;

6. Las validaciones que necesariamente deberemos realizar manualmente dentro de
Enterprise Architect

RESTRICCIONES

- No implementes codigo
- No diseñes todavia una arquitectura definitiva.
- No agregues desafios opcionales al alcance base.
- No cambies Framework, plataforma, COM ni tecnologías
- No trabajaes sobre el ejercicio 2 de SQL/Prolaborate, salvo reconocerlo como un
entregable separado (por ahora)
- No conviertas recomendaciones o supuestos en requisitos si la consigna no los exige.
- No tomes decisiones de diseño definitivas, si detectas alternativas, déjalas como
decisiones abiertas para revisión humana.

Manten la exploración proporcional al tamaño actual del proyecto. Proriza los archivos y
requisitos directamente relevantes, y evita auditorías generales del entorno ya validado.

Al finalizar, persiste el artefacto de exploración, presenta un resumen concisso con hechos
verificados, riesgos e incógnitas, y detente para revisión humana antes de cualquier fase
posterior.
```

---

## DES-002 — Prompt completo

**Herramienta:** OpenCode + Gentle-AI  
**Modelo:** MiniMax-M3

**Prompt utilizado:**

```text
Realizá únicamente sdd-explore sobre el proyecto Addino. Leé primero el desafío técnico y
la guía de creación del Add-in. Determiná el estado actual, qué puede reutilizarse, qué debe
reemplazarse, requisitos obligatorios, restricciones, criterios de aceptación, riesgos sobre
selección de Package, elementos directos, edición local, cancelación y Element.Update(), y
qué debe validarse manualmente en EA. Separá obligatorios, mejoras y opcionales. No
implementes código ni diseñes todavía la arquitectura.”
```

**Contexto complementario documentado:**

Como complemento se incorporaron las decisiones sobre WinForms, español, `GetTreeSelectedItemType()`, `Package.Elements`, DTO local, Save explícito, errores por fila, `ShowDialog()`, `.sln` clásica y exclusión de opcionales.

---

## DES-003 — Prompt completo

**Herramienta:** OpenCode + Gentle-AI  
**Modelo:** GPT-5.6 Terra

**Prompt utilizado:**

```text
Ejecutá únicamente sdd-propose para ea-metadata-review-exercise-1 usando la exploración
y decisiones humanas aprobadas. Proponé el alcance mínimo necesario para cumplir el
Ejercicio 1: acción de Extensions, validación estricta de EA.Package, grilla modal en
español con elementos directos, edición mediante DTO local, persistencia explícita con
Update(), .sln, README, evidencias y registro de IA. Mantené fuera del alcance los
desafíos opcionales y el Ejercicio 2. No implementes código ni avances a Spec/Design
```

---

## DES-004 — Prompt completo

**Herramienta:** OpenCode + Gentle-AI  
**Modelo:** Kimi K2.7 Code

**Prompt utilizado:**

```text
Ejecutá únicamente sdd-spec para el cambio aprobado. Convertí Proposal, Exploration y la
consigna en requisitos técnicos verificables usando lenguaje MUST/SHALL y escenarios de
aceptación. Especificá callbacks, validación de Package, carga de hijos directos, columnas
y permisos, modalidad/idioma, ciclo local de edición, Save solo de filas modificadas, manejo
de Update() == false/excepciones, plataforma y entregables. No diseñes clases o detalles
de implementación que correspondan a Design y no incluyas opcionales.
```

---

## DES-005 — Prompt completo

**Herramienta:** OpenCode + Gentle-AI  
**Modelo:** GPT-5.6 Sol

**Prompt utilizado:**

```text
Avanza Formalmente a sdd-ddesing para el cambio ea-metadata-review-1.

La Proposal y la Spec está probada y congeladas. No las modifiques ni reabras decisiones
funcionales ya resueltas. El checkpoint Git previo a Design ya fue realizado.

Usa como fuentes de verdad:
- Los documentos de @docs/challenge\
- @openspec/specs/ea-metadata-review/spec.md
- @openspec/changes/ea-metadata-review-exercise-1/proposal.md
- @openspec/changes/ea-metadata-review-exercise-1/exploration.md
- El codigo actual del proyecto Addino.

OBJETIVO:

Diseña cómo implementar el Ejercicio 1 obligatorio sobre el Add-in existente, de forma
simple, robusta, mantenible y proporcional al tamaño del desafío. El diseño debe respetar
sin cambios lo siguiente:

- C# sobre .NET Framework 4.7.2.
- Windows Forms.
- EA 17.1 x64 como target validado.
- Interop.EA y registro COM.
- Los cinco callbacks EA requeridos.
- Acción en Extensions.
- validación estricta de EA.Package;
- carga exclusiva de elementos directos;
- Name/Alias editables, Notes multilínea, Type/Stereotype read-only;
- edición local hasta Save;
- Cancel/Esc/X sin persistencia;
- Element.Update() solo para elementos modificados;
- procesamiento independiente y manejo de false, excepciones y elementos bloqueados;
- UI modal y visible en español.

Decisiones que debe resolver Design

Define y justifica la estructura concreta de implementación, incluyendo:

- componentes/clases necesarias y responsabilidad de cada una;
- flujo desde EA_MenuClick hasta la apertura del formulario;
- forma de representar localmente una fila y conservar una identidad - - estable del elemento
EA sin depender de Name/Alias;
- estrategia de BindingList/binding y detección interna de modificaciones;
- carga de datos desde EA.Package.Elements;
- configuración de DataGridView, incluyendo Notes multilínea;
- cómo asegurar que una celda todavía activa quede confirmada antes de Save;
- cómo recuperar de forma segura el EA.Element correspondiente durante la persistencia;
- flujo detallado de Save normal, Save sin cambios y Save parcial;
cómo limpiar el estado pendiente de los éxitos y conservar pendiente el de los fallos;
- estrategia de mensajes/resumen de guardado en español;
- comportamiento de Cancel, Esc y X;
- manejo de excepciones COM y liberación/uso razonable de objetos COM;
- modificaciones previstas sobre AddinoClass.cs, nuevos archivos WinForms y
Addino.csproj;
- eliminación completa de Say Hello y Say Goodbye;
- generación posterior de la solución clásica .sln;
- tratamiento de Interop.EA para no romper la configuración actualmente funcional y
permitir únicamente ajustes mínimos de portabilidad si fueran necesarios.

Para cada decisión donde existan alternativas razonables, selecciona la opción más simple
que satisfaga la Spec y explica brevemente por qué.

Evita sobrearquitectura: no introduzcas interfaces, repositories, dependency injection, capas
genéricas, patrones o abstracciones si no aportan una necesidad concreta a este Add-in
pequeño. Tampoco acoples innecesariamente la UI directamente a objetos COM si existe
una alternativa local sencilla y segura.

El Repositorio Pasantías permanece externo al workspace Git y se utiliza únicamente para
validación manual en Enterprise Architect.

Mantén fuera del diseño: recursividad, creación de elementos, reload, dirty-row highlighting,
validación obligatoria de Name vacío y todo el Ejercicio 2/Prolaborate.

Considera los entregables únicamente en aquello que afecte al diseño o a la capacidad de
ejecutar/verificar la solución; no generes ahora capturas ni video.

RESULTADO ESPERADO

Genera y persiste únicamente el artefacto correspondiente a sdd-design.

El documento debe dejar suficientemente claro:

- arquitectura/componentes;
- responsabilidades;
- flujo de datos;
- flujo de apertura;
- flujo de Save;
- manejo de errores;
- ciclo de vida del estado local;
- interacción con EA/COM;
- archivos previstos;
- decisiones técnicas y trade-offs;
- riesgos técnicos restantes y mitigaciones;
- trazabilidad con la Spec.

No implementes código, no generes tasks y no avances a sdd-tasks ni sdd-apply.

Al finalizar, resume las decisiones principales, señala únicamente blockers o riesgos reales
que necesiten decisión humana y detente para revisión.
```

---

# Evidencia relacionada

Además de los artefactos técnicos del repositorio, la entrega incluye documentación visual de las interacciones y decisiones:

```text
Pino_Registro_Uso_IA.pdf
```

El PDF contiene:

- descripción del criterio de selección de modelos;
- capturas del entorno Gemini Notebook;
- evidencia `EV:DOC-001`;
- evidencia `EV:DOC-002`;
- evidencia `EV:DOC-003`;
- explicación del ecosistema Gentle-AI / OpenCode / SDD;
- captura de la asignación de modelos a cada fase;
- tablas de las interacciones `DES-001` a `DES-005`;
- prompts completos documentados;
- capturas del entorno de desarrollo.

Los artefactos SDD relacionados se encuentran en:

```text
openspec/changes/ea-metadata-review-exercise-1/
```

Entre ellos:

```text
exploration.md
proposal.md
design.md
tasks.md
specs/ea-metadata-review/spec.md
```

---

# Notas de trazabilidad

- Los IDs utilizados en este documento se conservan tal como fueron definidos en `Pino_Registro_Uso_IA.pdf`.
- No se asignaron IDs nuevos a interacciones que todavía no fueron incorporadas al registro formal.
- Los prompts reproducidos en este archivo corresponden a los prompts documentados en el registro existente y se mantienen con su redacción original.
- Las nuevas fases o interacciones —por ejemplo Tasks, Apply, remediaciones, Verify, Archive u opcionales— deberán incorporarse únicamente cuando se les asigne formalmente un ID en el registro principal.
- No se fabricaron capturas, resultados de Enterprise Architect ni evidencias inexistentes.
- Las pruebas funcionales reales del Add-in se documentan por separado en:

```text
Pino_Evidencias_Pruebas_Funcionales_Addino.pdf
```

- La guía de ejecución se documenta en:

```text
Pino_Guia_Ejecucion_Addino.pdf
```

- La utilización de IA no sustituyó las verificaciones reales: los comportamientos principales implementados en Addino fueron contrastados mediante compilación, control de versiones y pruebas manuales dentro de Enterprise Architect.