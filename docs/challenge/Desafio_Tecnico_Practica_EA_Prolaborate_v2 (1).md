**Proagile** · IT Intelligent Solutions 

***DESAFÍO TÉCNICO DE PRÁCTICA*** 

*Construcción de soluciones sobre Sparx Enterprise Architect y Prolaborate* Pasantías Proagile 2026 

**1\. Contexto general del caso** 

El equipo de arquitectura utiliza Sparx Enterprise Architect (EA) como repositorio central para documentar aplicaciones, relaciones, estados, categorías y otros metadatos relevantes del ecosistema organizacional. Sobre ese repositorio se realizan tareas de gobierno, análisis de impacto y generación de información para la toma de decisiones. 

En este contexto, se plantean dos ejercicios complementarios. El primero está orientado al desarrollo de un Addin que permita revisar y corregir metadatos de elementos desde la interfaz de EA. El segundo está orientado a la construcción de consultas SQL sobre la base del repositorio para obtener estadísticas e información de impacto, con posibilidad de llevar los resultados a un dashboard en Prolaborate. 

Ambos ejercicios buscan que se comprenda el modelo de automatización de Enterprise Architect, la estructura de datos del repositorio, la relación entre elementos y conectores, y la forma de convertir información de arquitectura en herramientas prácticas para gobierno y gestión. 

**2\. Objetivo General** 

Desarrollar capacidades prácticas para extender y explotar la información almacenada en Enterprise Architect mediante dos enfoques: automatización funcional con un Addin en C\# y análisis de información mediante queries para estadísticas e impacto, con visualización opcional en Prolaborate.  
**Acceso al entorno de trabajo y entrega** 

**Acceso a Enterprise Architect:** 

para resolver los ejercicios necesitás Enterprise Architect instalado. Si no contás con una licencia propia, podés usar la versión de prueba gratuita de 30 días de Sparx Systems (sparxsystems.com): es totalmente funcional para este desafío e incluye el registro COM necesario para el Addin. 

Te compartimos el trial necesario en el siguiente link 

Si tenés algún problema de acceso o de entorno, escribinos antes de la fecha de entrega, no queremos que la barrera técnica te deje afuera. 

**Acceso a Prolaborate:** 

https://servidor.proagile.com.ar:4445/ 

Te enviaremos por mail el usuario y contraseña. 

**Material de trabajo:** 

El repositorio Repositorio Pasantías.dea y el material de referencia están en la carpeta de Drive que te compartimos en el siguiente link Buscá siempre la versión más reciente antes de empezar. 

**Entrega:** 

Subí todos los entregables (solución .sln, guía de ejecución, evidencias, archivo de queries y el Registro de Uso de IA) a la carpeta que te compartiremos por email 

Nombrá los archivos con tu apellido. 

Fecha límite: martes 18/08 a las 08:00 am. 

**3\. Ejercicio 1: Addin para Enterprise Architect** 

**3.1 Contexto Específico** 

Antes de compartir o publicar información del repositorio EA, el equipo de arquitectura necesita revisar que los elementos de un paquete tengan metadatos mínimos completos y consistentes. En particular, se requiere verificar nombres, alias y notas descriptivas, ya que estos campos son utilizados para reportes, búsquedas, análisis de trazabilidad y comunicación con otros equipos. 

Actualmente esta revisión se realiza abriendo elemento por elemento dentro de Enterprise Architect. El objetivo del Addin es reducir ese trabajo manual mediante una pantalla centralizada que permita visualizar y editar los metadatos principales de los elementos contenidos en un paquete seleccionado. 

**3.2 Objetivo del Ejercicio** 

Diseñar y desarrollar un Addin en C\# (.NET Framework) integrado con Enterprise Architect. El Addin debe tomar como contexto el paquete seleccionado por el usuario en el Project Browser, mostrar una grilla con los elementos contenidos directamente en ese paquete y permitir actualizar los campos editables de forma controlada. 

**3.3 Alcance Funcional y Reglas de Negocio** 

**Interacción con Enterprise Architect** 

● El Addin debe registrar una nueva opción en el menú contextual o menú principal de extensiones de EA. Ejemplo: Proagile Addins \> Revisión de Metadatos de Elementos. 

● Al ejecutar la acción, el Addin debe identificar el paquete seleccionado en el Project Browser. ● Si el usuario no tiene seleccionado un paquete EA.Package, debe mostrar un mensaje claro y detener el flujo de manera segura. 

**Comportamiento de la interfaz visual** 

● La grilla debe cargar automáticamente los elementos contenidos directamente en el paquete seleccionado.  
● Las modificaciones realizadas por el usuario deben permanecer en memoria hasta que se presione Guardar Cambios o Aplicar Actualizaciones. 

● Si el usuario cierra la ventana con Cancelar, Escape o la cruz de cierre, no se debe modificar el repositorio. ● Al finalizar el guardado, debe mostrarse un mensaje de éxito o un detalle de errores encontrados. 

| Propiedad del Elemento  | Tipo de Control  | Permiso / Comportamiento |
| :---- | :---- | :---- |
| Nombre (Name)  | Texto editable  | Permite corregir la  nomenclatura principal del elemento. |
| Alias (Alias)  | Texto editable  | Permite añadir o modificar códigos o nombres alternativos. |
| Notas (Notes)  | Texto editable / multilínea  | Espacio para complementar la descripción conceptual  obligatoria. |
| Tipo (Type)  | Texto solo lectura  | Informativo, por ejemplo Class, Component o Requirement. |
| Estereotipo (Stereotype)  | Texto solo lectura  | Informativo del perfil del  modelo aplicado. |

**3.4 Requisitos Técnicos** 

● Lenguaje y framework: C\# sobre .NET Framework 4.8 o 4.7.2. 

● Interfaz de usuario: Windows Forms o WPF. 

● Integración: Interop.EA y registro COM. 

● Métodos mínimos esperados: EA\_Connect, EA\_GetMenuItems, EA\_GetMenuState, EA\_MenuClick y EA\_Disconnect. 

● Persistencia: uso del método Update() de los elementos modificados mediante la API de EA. 

**3.5 Entregables Requeridos** 

Deberás entregar: 

1\. Solución Visual Studio (.sln): código fuente ordenado, estructurado de forma limpia y tipado de manera estricta. 2\. Guía de Ejecución del Addin: un archivo README.md o documento breve que describa cómo utilizar el Addin dentro de Enterprise Architect. Debe incluir: 

● Requisitos previos para ejecutar el Addin. 

● Cómo abrir Enterprise Architect y seleccionar el paquete de trabajo. 

● Ruta de menú desde la cual se ejecuta la funcionalidad. 

● Descripción paso a paso del flujo: selección del paquete, apertura de la grilla, edición de campos, guardado de cambios y cancelación. 

● Explicación de los mensajes de validación esperados cuando no hay paquete seleccionado o cuando ocurre un error. 

● Evidencia esperada del resultado: cómo verificar que los cambios se aplicaron en los elementos del modelo. 

3\. Evidencia de Funcionamiento: capturas de pantalla o video corto demostrando el flujo completo de selección, edición en la grilla, guardado e impacto reflejado en Enterprise Architect. 

4\. Registro de Uso de Inteligencia Artificial: entregar una tabla con muestras representativas del uso de herramientas de IA durante el desarrollo. La tabla deberá documentar como mínimo cinco interacciones significativas e incluir: 

● Identificador de la interacción. 

● Objetivo de la consulta. 

● Herramienta y modelo utilizados. 

● Estrategia o prompt empleado. 

● Decisión tomada a partir de la respuesta. 

● Evidencia relacionada, como archivo, prueba, captura o commit. 

● Resultado obtenido.

| ID  | Objetivo | Herramien ta | Modelo  | Estrategia/Prompt  | Evidencia  | Resultado |
| ----- | ----- | ----- | ----- | ----- | ----- | ----- |
| ID-000 | Manejar  errores de  guardado | OpenCode  | GPT-5.5 | Considerar que  Update() puede  devolver false sin lanzar excepción. | Captura chat/ plan o resultado de la ejecución | Los elementos  bloqueados se  reportaron  correctamente. |
| ID-001 |  |  |  |  |  |  |
| ID-002 |  |  |  |  |  |  |
| ID-003 |  |  |  |  |  |  |
| ID-004 |  |  |  |  |  |  |

**3.6 Criterios de Evaluación y Aceptación** 

● Correctitud técnica: el Addin se registra e inicia sin alertas ni bloqueos en EA. 

● Robustez: el sistema controla errores comunes, como ausencia de selección, paquete inválido o elemento bloqueado. 

● Fidelidad funcional: las columnas Tipo y Estereotipo permanecen bloqueadas (solo lectura). ● Persistencia efectiva: al presionar guardar, los cambios se reflejan en el modelo mediante la API de EA. ● Claridad de ejecución: la guía entregada permite que otro usuario ejecute el Addin sin asistencia del desarrollador. 

**3.7 Desafíos Opcionales** 

● Recursividad completa: permitir listar elementos de subpaquetes. 

● Indicador visual de modificaciones: resaltar filas modificadas antes de guardar. 

● Validación: impedir guardar si el campo Nombre queda vacío. 

● Botón de recarga: volver a leer el estado actual del paquete desde EA. 

● Botón para agregar elementos que permita seleccionar el tipo y el estereotipo.  
**4\. Ejercicio 2: Query Prolaborate y Estadísticas sobre EA** 

**4.1 Contexto Específico** 

Además de revisar metadatos mediante herramientas visuales, el equipo de arquitectura necesita obtener indicadores del repositorio EA para responder preguntas de gobierno. Estas respuestas suelen requerir consultas SQL sobre la base del repositorio, identificando elementos, tagged values, estereotipos, conectores y relaciones de impacto. 

El ejercicio propone construir queries que permitan analizar aplicaciones según categoría, estado de vigencia e impacto por dependencia. Primero se deben resolver las consultas en EA. Como punto extra, los resultados pueden llevarse a un dashboard de Prolaborate para facilitar su visualización por usuarios no técnicos. 

**4.2 Objetivo del Ejercicio** 

Crear queries sobre el repositorio de Enterprise Architect que respondan interrogantes de gobierno de aplicaciones y análisis de impacto. Las consultas deben ser comprensibles, reutilizables y verificables contra los elementos modelados en EA. 

**4.3 Interrogantes a Resolver** 

Las queries deben responder, como mínimo, las siguientes preguntas: 

1\. ¿Cuántos elementos de tipo aplicación tienen el tagged value Categoria con valor ORO? ¿Cuáles son? 2\. ¿Cuántas aplicaciones tenemos en cada estado de vigencia: Vigente y Deprecado? 

3\. ¿Qué aplicaciones se verían afectadas si se da de baja la Base de Datos 28? 

**4.4 Alcance Funcional y Reglas de Trabajo** 

**Trabajo en Enterprise Architect** 

● Identificar las tablas principales del repositorio EA necesarias para consultar elementos, relaciones y tagged values. 

● Construir las consultas en el motor de búsqueda SQL de EA o herramienta equivalente disponible. ● Validar los resultados contra el contenido visible del modelo. 

**Trabajo opcional en Prolaborate** 

● Tomar una o más queries validadas y convertirlas en insumo para un gráfico o dashboard. ● Se recomienda iniciar con un gráfico tipo donut o de barras. 

● Documentar cómo se configuró la visualización y qué pregunta de negocio responde.

| Consulta  | Resultado Esperado  | Observaciones |
| :---- | :---- | :---- |
| Aplicaciones categoría Oro  | Cantidad total y listado de aplicaciones | Debe indicar el  campo/tagged value  utilizado para detectar la categoría. |
| Aplicaciones por estado de vigencia | Cantidad agrupada por Vigente y Deprecado | Debe contemplar valores vacíos o no informados si corresponde. |
| Impacto por baja de Base de Datos 28 | Cantidad total y listado de aplicaciones dependientes | Debe identificar  dependencias cuyo origen sea una aplicación y cuyo destino sea Base de Datos 28\. |

**4.5 Entregables Requeridos** 

El pasante deberá entregar: 

1\. Archivo con las queries SQL: cada consulta debe estar identificada con el interrogante que responde. 2\. Breve explicación técnica: tablas utilizadas, joins principales, filtros aplicados y supuestos considerados. 3\. Evidencia de ejecución en EA: capturas de pantalla de los resultados obtenidos o exportación de resultados. 4\. Punto extra: dashboard o gráfico en Prolaborate usando al menos una de las queries generadas, acompañado de evidencia visual. 

5\. Registro de Uso de Inteligencia Artificial: entregar una tabla con muestras representativas del uso de herramientas de IA durante el desarrollo. La tabla deberá documentar como mínimo cinco interacciones significativas e incluir: 

● Identificador de la interacción. 

● Objetivo de la consulta. 

● Herramienta y modelo utilizados. 

● Estrategia o prompt empleado. 

● Decisión tomada a partir de la respuesta. 

● Evidencia relacionada, como archivo, prueba, captura o commit. 

● Resultado obtenido. 

| ID  | Objetivo  | Herramienta Modelo  | Estrategia/Prompt | Evidenc  ia | Resultado |
| ----- | ----- | ----- | ----- | ----- | ----- |
| ID-000 | Obtener  propiedad  Estado de los  elementos | Codex GPT-5.6 | Consulta SQL para obtener la  propiedad  “Estado” de los  elementos del  paquete X | Captura  chat/  plan o  resultad o de la  ejecució n | La consulta lista los elementos con su respectivo  Estado |
| ID-001 |  |  |  |  |  |
| ID-002 |  |  |  |  |  |
| ID-003 |  |  |  |  |  |
| ID-004 |  |  |  |  |  |

**4.6 Criterios de Evaluación y Aceptación** 

● Correctitud de resultados: las queries responden exactamente a las preguntas planteadas. ● Claridad técnica: se entiende qué tablas, campos y relaciones fueron utilizados. 

● Reutilización: las consultas están ordenadas, comentadas si corresponde y pueden ejecutarse nuevamente. ● Trazabilidad: cada resultado puede relacionarse con elementos existentes en EA. 

● Visualización opcional: el dashboard de Prolaborate representa correctamente la información de la query.  
**5\. Fuentes de Información Recomendadas** 

Las siguientes fuentes pueden utilizarse como material de consulta para resolver los ejercicios. En los documentos alojados en Drive se recomienda buscar siempre la versión más reciente disponible antes de iniciar el desarrollo o la construcción de queries.

| Fuente  | Uso sugerido  | Enlace |
| ----- | :---- | :---- |
| BD EA  | Consultar estructura de base EA, enfocada a queries para estadísticas. Buscar la versión más reciente.  (Centrarse en las tablas t\_object y t\_objectproperties) | https://drive.google.com/file/d/1Khpe5pgFEHzW \-A5Q5kjrLUrjmfTWuTPm/view?usp=drive\_link |
| Enterprise  Architect  SDK | Referencia para automatización, API y desarrollo de Addins. | https://drive.google.com/file/d/16NvuhqiO9zPf MZ83tj0Yq7Fhh\_t0l6X1/view?usp=drive\_link |
| Object  Model | Modelo de objetos oficial de Enterprise Architect para automatización. | https://sparxsystems.com/resources/user-guides /17.1/automation/enterprise-architect-object-m odel.pdf |
| Query  Gráfico en  Prolaborate | Guía para construir gráficos tipo donut y visualizaciones con queries. | https://prolaborate.sparxsystems.com/resources /v5-documentation/build-donut-charts |
| Buenas  Prácticas C\# | Referencia de estilo, organización y buenas prácticas de desarrollo en C\#. | https://drive.google.com/file/d/1Km8JVSj89GtY4 WIIZFzIVN\_82gVC0lwB/view?usp=drive\_link |
| Crear un  Add-in en  Enterprise  Architect | Guia para la creación de un nuevo proyecto/addin desde 0 en Enterprise Architect. | https://drive.google.com/file/d/1CNljzafXP6iJgtgf i7lcMsPFqdMdugvH/view?usp=sharing |

