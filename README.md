# Addino — Revisión de Metadatos para Enterprise Architect

**Addino** es un Add-in desarrollado en C# para Sparx Enterprise Architect como parte del Ejercicio 1 del Desafío Técnico de Práctica de Proagile 2026.

Su objetivo es facilitar la revisión y actualización de los metadatos de los elementos contenidos en un paquete de Enterprise Architect mediante una grilla centralizada.

Desde Addino es posible editar:

- **Nombre**
- **Alias**
- **Notas**

Mientras que los siguientes campos se muestran únicamente como información:

- **Tipo**
- **Estereotipo**

Los cambios permanecen localmente en memoria hasta que el usuario presiona **Guardar**.

---

## Tecnologías

- C#
- .NET Framework 4.7.2
- Windows Forms
- Interop.EA
- COM
- Visual Studio
- Enterprise Architect 17.1 x64
- Git

La solución fue desarrollada y validada específicamente utilizando **Enterprise Architect 17.1 x64**.

La edición Trial de Enterprise Architect no constituye un requisito propio de Addino.

---

## Estructura principal

```text
Addino/
├── docs/
├── openspec/
├── Addino.sln
├── Addino.csproj
├── AddinoClass.cs
├── MetadataElementRow.cs
├── MetadataReviewForm.cs
├── MetadataReviewForm.Designer.cs
├── README.md
├── AI_USAGE_LOG.md
└── Properties/
    └── AssemblyInfo.cs
```

### `AddinoClass.cs`

Contiene la clase principal cargada por Enterprise Architect y los callbacks requeridos:

- `EA_Connect`
- `EA_GetMenuItems`
- `EA_GetMenuState`
- `EA_MenuClick`
- `EA_Disconnect`

También valida que el usuario haya seleccionado un `EA.Package` válido y carga los elementos correspondientes.

### `MetadataElementRow.cs`

Representa localmente cada elemento mostrado en la grilla.

Los datos editables se mantienen en memoria y no se enlazan directamente con objetos `EA.Element`, permitiendo cancelar modificaciones sin alterar accidentalmente el repositorio.

### `MetadataReviewForm.cs`

Implementa la ventana de revisión de metadatos:

- visualización mediante `DataGridView`;
- edición;
- cancelación;
- guardado;
- control de cambios;
- manejo de errores;
- persistencia mediante `Element.Update()`.

---

# Instalación y ejecución

Para una explicación detallada y visual consultar:

**`Pino_Guia_Ejecucion_Addino.pdf`**

## Requisitos previos

- Windows x64.
- Visual Studio con soporte para .NET Framework.
- .NET Framework 4.7.2.
- Sparx Enterprise Architect instalado.
- Acceso a `Interop.EA.dll`.
- Permisos necesarios para compilar y registrar el componente COM.

## 1. Abrir la solución

Abrir:

```text
Addino.sln
```

Se recomienda ejecutar Visual Studio como **Administrador**, debido al registro COM realizado durante la compilación.

## 2. Compilar

Configuración validada:

```text
Debug | x64
```

Desde Visual Studio:

```text
Build → Build Solution
```

O mediante MSBuild:

```cmd
msbuild Addino.csproj /t:Build /p:Configuration=Debug /p:Platform=x64
```

Estado de la última validación:

```text
0 errores
0 advertencias
```

## 3. Registro COM

Enterprise Architect debe poder localizar la clase COM del Add-in.

La configuración utilizada registra Addino bajo:

```text
HKEY_CURRENT_USER\SOFTWARE\Sparx Systems\EAAddins64
```

La entrada de Addino debe apuntar a:

```text
Addino.AddinoClass
```

## 4. Abrir Enterprise Architect

1. Iniciar Enterprise Architect.
2. Abrir el repositorio de trabajo.
3. Localizar el **Project Browser**.
4. Navegar hasta el paquete que se desea revisar.

## 5. Seleccionar un paquete

Addino trabaja tomando como contexto el paquete seleccionado.

Debe seleccionarse un objeto de tipo:

```text
EA.Package
```

Si se selecciona un elemento, diagrama u otro objeto, Addino muestra un mensaje de validación y detiene el flujo de forma segura.

## 6. Ejecutar Addino

Con un paquete seleccionado:

1. Abrir la pestaña **Especializar** de Enterprise Architect.
2. Localizar **Addino**.
3. Ejecutar:

```text
Revisión de Metadatos de Elementos
```

Se abrirá la ventana modal **Revisión de Metadatos**.

---

# Uso

## Elementos mostrados

La grilla carga automáticamente los elementos contenidos **directamente** en el paquete seleccionado.

En la versión base obligatoria no se recorren subpaquetes.

Si el paquete está vacío, Addino abre normalmente mostrando una grilla vacía.

## Columnas

| Columna | Editable | Descripción |
|---|---|---|
| Nombre | Sí | Nombre principal del elemento |
| Alias | Sí | Alias o nombre alternativo |
| Notas | Sí | Descripción del elemento |
| Tipo | No | Tipo del elemento |
| Estereotipo | No | Estereotipo aplicado |

## Editar elementos

Para modificar información:

1. Seleccionar una celda de **Nombre**, **Alias** o **Notas**.
2. Escribir el nuevo valor.
3. Continuar editando otras filas si es necesario.

Mientras no se presione **Guardar**, los cambios permanecen únicamente en memoria y el repositorio de Enterprise Architect no se modifica.

### Notas multilínea

La columna **Notas** admite texto multilínea.

Para insertar un salto de línea dentro de una celda:

```text
Shift + Enter
```

Este comportamiento fue comprobado manualmente dentro de Enterprise Architect.

---

# Guardar cambios

Al presionar **Guardar**, Addino:

1. finaliza la edición activa;
2. identifica únicamente las filas modificadas;
3. recupera el elemento correspondiente mediante su `ElementId`;
4. actualiza Nombre, Alias y Notas;
5. ejecuta `Element.Update()`;
6. comprueba el resultado devuelto por Enterprise Architect;
7. continúa procesando las demás filas aunque alguna falle;
8. muestra un resumen final en español.

Las filas guardadas correctamente dejan de considerarse pendientes.

## Guardar sin cambios

Si no existen modificaciones pendientes, Addino no realiza escrituras innecesarias y muestra:

```text
No hay cambios pendientes para guardar.
```

Este escenario fue validado manualmente.

---

# Cancelar cambios

Una edición pendiente puede descartarse mediante:

- botón **Cancelar**;
- tecla **Escape**;
- botón **X** de la ventana.

En todos los casos:

- el formulario se cierra;
- los cambios todavía no guardados se descartan;
- no se ejecuta `Element.Update()`;
- los valores guardados previamente permanecen intactos.

---

# Verificar persistencia

Después de guardar:

1. cerrar Addino;
2. localizar el elemento modificado en Enterprise Architect;
3. abrir sus propiedades;
4. comprobar Nombre, Alias o Notas;
5. verificar que el nuevo valor esté presente.

También puede volver a abrirse Addino sobre el mismo paquete para confirmar que el valor persistido vuelve a cargarse.

La persistencia fue validada manualmente en Enterprise Architect.

---

# Manejo de errores

El guardado se procesa de manera independiente para cada elemento.

La implementación contempla:

- `Element.Update() == false`;
- errores al recuperar un elemento;
- excepciones de Enterprise Architect / COM;
- elementos bloqueados o no escribibles.

Ante un fallo de una fila, Addino registra el error y continúa procesando las restantes.

Los escenarios de `Update() == false`, excepciones COM y elementos realmente bloqueados cuentan con manejo implementado, aunque no todos pudieron reproducirse manualmente en el repositorio utilizado durante el desarrollo.

---

# Pruebas y evidencia

Las pruebas funcionales realizadas se encuentran documentadas en:

```text
Pino_Evidencias_Pruebas_Funcionales_Addino.pdf
```

Incluyen, entre otras:

- carga del Add-in;
- selección inválida;
- selección de paquete válido;
- paquete vacío;
- carga de elementos;
- permisos de edición;
- Notas multilínea;
- Cancelar / Escape / X;
- guardado exitoso;
- guardado sin cambios;
- persistencia real;
- conservación del último estado guardado.

También se grabó un **video funcional completo** de la ejecución en Enterprise Architect.

> El enlace definitivo al video será incorporado en la documentación final antes de la entrega.

---

# Uso de Inteligencia Artificial

El desarrollo fue asistido mediante herramientas y modelos de Inteligencia Artificial utilizando un flujo de trabajo basado en **Spec Driven Development (SDD)**.

La trazabilidad completa de herramientas, modelos, prompts, decisiones y evidencias se documenta en:

```text
Pino_Registro_Uso_IA.pdf
```

y en la versión fuente:

```text
AI_USAGE_LOG.md
```

Las decisiones importantes fueron contrastadas contra:

- la consigna original;
- documentación de Enterprise Architect;
- código fuente;
- compilaciones reales;
- control de versiones;
- pruebas manuales dentro de Enterprise Architect.

---

# Documentación adicional

La entrega del Ejercicio 1 incluye:

```text
Pino_Guia_Ejecucion_Addino.pdf
Pino_Evidencias_Pruebas_Funcionales_Addino.pdf
Pino_Registro_Uso_IA.pdf
```

Además del código fuente y la solución Visual Studio.

---

# Funcionalidades opcionales

La primera versión de Addino fue desarrollada priorizando el cumplimiento completo del alcance obligatorio.

Los desafíos opcionales definidos por la consigna se gestionarán como extensiones posteriores de la versión base:

- recursividad en subpaquetes;
- indicador visual de filas modificadas;
- validación de Nombre vacío;
- botón de recarga;
- creación de nuevos elementos.

La documentación será actualizada a medida que estas funcionalidades sean incorporadas y verificadas.

---

## Autor

**Ezequiel Pino**  
Desafío Técnico de Práctica — Proagile 2026
