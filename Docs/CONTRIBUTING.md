# Contributing to Lobot

¡Gracias por tu interés en contribuir a Lobot! Este documento proporciona guías y mejores prácticas para contribuir al proyecto.

## Código de Conducta

### Nuestro Compromiso

Nos comprometemos a hacer de la participación en este proyecto una experiencia libre de acoso para todos, independientemente de:
- Edad
- Tamaño corporal
- Discapacidad
- Etnia
- Identidad y expresión de género
- Nivel de experiencia
- Nacionalidad
- Apariencia personal
- Raza
- Religión
- Identidad y orientación sexual

## Cómo Contribuir

### Reportar Bugs

Si encuentras un bug, por favor crea un issue con:

1. **Título descriptivo**: Resume el problema claramente
2. **Descripción detallada**: Explica qué esperabas y qué ocurrió
3. **Pasos para reproducir**: Lista los pasos exactos
4. **Entorno**: Versión de Unity, versión de Android, dispositivo
5. **Logs**: Incluye logs relevantes si es posible
6. **Screenshots**: Si aplica, agrega capturas de pantalla

### Sugerir Mejoras

Para sugerir nuevas características:

1. Verifica que no exista un issue similar
2. Crea un nuevo issue con el tag "enhancement"
3. Describe la funcionalidad propuesta
4. Explica por qué sería útil
5. Proporciona ejemplos de uso si es posible

### Pull Requests

#### Proceso

1. **Fork** el repositorio
2. **Clona** tu fork localmente
3. **Crea una rama** para tu feature: `git checkout -b feature/mi-feature`
4. **Haz cambios** siguiendo las guías de estilo
5. **Prueba** tus cambios exhaustivamente
6. **Commit** con mensajes descriptivos
7. **Push** a tu fork: `git push origin feature/mi-feature`
8. **Abre un Pull Request** en el repositorio principal

#### Criterios de Aceptación

- [ ] El código sigue las guías de estilo del proyecto
- [ ] Se agregaron tests si aplica
- [ ] La documentación está actualizada
- [ ] No hay conflictos con la rama main
- [ ] Los tests existentes pasan
- [ ] El código compila sin warnings

## Guías de Estilo

### C# (Unity)

#### Convenciones de Nombres

```csharp
// Clases y Structs: PascalCase
public class ChatController { }

// Interfaces: I + PascalCase
public interface IChatService { }

// Métodos: PascalCase
public void SendMessage() { }

// Variables privadas: camelCase
private string modelName;

// Variables públicas y propiedades: PascalCase
public string ModelName { get; set; }

// Constantes: PascalCase
private const int MaxRetries = 3;

// Campos serializados: camelCase con [SerializeField]
[SerializeField] private Button sendButton;

// Métodos async: Sufijo Async
public async Task SendMessageAsync() { }
```

#### Organización del Código

```csharp
using System;                          // System first
using System.Collections.Generic;      // Then System.*
using UnityEngine;                     // Then Unity
using Lobot.Models;                    // Then proyecto

namespace Lobot.Core                   // Namespace
{
    /// <summary>
    /// Documentación de la clase
    /// </summary>
    public class Example : MonoBehaviour
    {
        // 1. Constantes
        private const int MaxSize = 100;
        
        // 2. Campos serializados
        [SerializeField] private string configValue;
        
        // 3. Campos privados
        private bool isInitialized;
        
        // 4. Propiedades
        public string Value { get; set; }
        
        // 5. Unity callbacks
        private void Awake() { }
        private void Start() { }
        private void Update() { }
        
        // 6. Métodos públicos
        public void DoSomething() { }
        
        // 7. Métodos privados
        private void HelperMethod() { }
    }
}
```

#### Comentarios

```csharp
// Comentarios XML para métodos públicos
/// <summary>
/// Envía un mensaje al chatbot.
/// </summary>
/// <param name="content">Contenido del mensaje</param>
/// <returns>True si se envió correctamente</returns>
public async Task<bool> SendMessageAsync(string content)
{
    // Comentarios inline para lógica compleja
    if (string.IsNullOrWhiteSpace(content))
    {
        return false; // Early return si el contenido está vacío
    }
    
    // Evitar comentarios obvios
    // BAD: Incrementar contador
    counter++;
    
    // GOOD: Límite de 3 reintentos para evitar loops infinitos
    if (retryCount >= MaxRetries)
    {
        return false;
    }
}
```

### Markdown (Documentación)

```markdown
# Título Principal (H1) - Uno por documento

## Sección (H2)

### Subsección (H3)

- Usar listas para items
- Mantener líneas cortas (máx 100 chars)
- Usar bloques de código con el lenguaje especificado

\`\`\`csharp
// Código aquí
\`\`\`

**Negrita** para énfasis importante
*Cursiva* para énfasis leve
`código` para referencias inline
```

## Estructura de Commits

### Formato

```
<tipo>(<scope>): <mensaje corto>

<descripción detallada opcional>

<footer opcional>
```

### Tipos

- `feat`: Nueva funcionalidad
- `fix`: Corrección de bug
- `docs`: Cambios en documentación
- `style`: Formato, espacios, etc (no cambia funcionalidad)
- `refactor`: Refactorización de código
- `perf`: Mejoras de rendimiento
- `test`: Agregar o modificar tests
- `chore`: Mantenimiento, build, etc

### Ejemplos

```bash
feat(chat): agregar soporte para streaming

Implementa streaming de respuestas usando IAsyncEnumerable
para mejorar la experiencia de usuario con respuestas largas.

Closes #123

---

fix(api): corregir timeout en respuestas largas

Aumenta timeout de 60s a 300s para modelos grandes

---

docs(readme): actualizar instrucciones de instalación
```

## Testing

### Escribir Tests

```csharp
using NUnit.Framework;

[TestFixture]
public class ChatControllerTests
{
    [Test]
    public void SendMessage_WithEmptyContent_ReturnsFalse()
    {
        // Arrange
        var controller = new ChatController();
        
        // Act
        var result = controller.SendMessage("");
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public async Task SendMessageAsync_WithValidContent_SendsMessage()
    {
        // Arrange
        var controller = new ChatController();
        var message = "Test message";
        
        // Act
        await controller.SendMessageAsync(message);
        var history = controller.GetChatHistory();
        
        // Assert
        Assert.IsTrue(history.Count > 0);
        Assert.AreEqual(message, history[0].Content);
    }
}
```

### Ejecutar Tests

```bash
# En Unity
Window > General > Test Runner
```

## Documentación

### Actualizar Documentación

Al agregar nuevas funcionalidades, actualiza:

1. **README.md**: Si cambia la instalación o uso básico
2. **API.md**: Si agregas nuevos endpoints o métodos
3. **ARCHITECTURE.md**: Si cambias la arquitectura
4. **Comentarios de código**: Siempre documenta métodos públicos

### Ejemplo de Documentación de API

```csharp
/// <summary>
/// Envía un mensaje al chatbot y recibe una respuesta.
/// </summary>
/// <param name="content">Contenido del mensaje a enviar</param>
/// <param name="model">Modelo opcional a usar (null = usar activo)</param>
/// <returns>
/// Tarea que completa con true si el mensaje se envió correctamente
/// </returns>
/// <exception cref="OllamaConnectionException">
/// Si no se puede conectar al servidor
/// </exception>
/// <example>
/// <code>
/// var controller = new ChatController();
/// await controller.SendMessageAsync("Hola");
/// </code>
/// </example>
public async Task<bool> SendMessageAsync(string content, string model = null)
{
    // Implementation
}
```

## Proceso de Revisión

### Para Revisores

Al revisar Pull Requests, verificar:

- [ ] El código sigue las guías de estilo
- [ ] Los tests pasan
- [ ] No hay código comentado sin explicación
- [ ] No hay console.log o Debug.Log innecesarios
- [ ] La documentación está actualizada
- [ ] No hay conflictos de merge
- [ ] El código es legible y mantenible
- [ ] Las funciones son pequeñas y hacen una sola cosa
- [ ] Se manejan correctamente los errores

### Para Autores

Antes de solicitar revisión:

- [ ] Ejecuta todos los tests
- [ ] Revisa tu propio código
- [ ] Actualiza la documentación
- [ ] Escribe descripción clara del PR
- [ ] Referencia issues relacionados
- [ ] Agrega screenshots si hay cambios visuales

## Contacto

Si tienes preguntas sobre cómo contribuir:

- Abre un issue con el tag "question"
- Contacta a [@Jcrex999](https://github.com/Jcrex999)

## Agradecimientos

¡Gracias por contribuir a Lobot! Cada contribución, grande o pequeña, hace que el proyecto sea mejor.

---

**¡Happy Coding! 🚀**
