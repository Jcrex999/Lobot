# Ejemplos de Uso de Lobot

Esta carpeta contiene ejemplos de código que demuestran cómo usar las diferentes funcionalidades de Lobot.

## Ejemplos Disponibles

### 1. SimpleChatExample.cs

Ejemplo básico de cómo usar el ChatController para enviar mensajes y recibir respuestas.

**Uso:**
1. Agrega el componente a un GameObject en tu escena
2. Presiona **Space** para enviar un mensaje de prueba
3. Observa los logs en la consola

**Características demostradas:**
- Inicialización del ChatController
- Envío de mensajes
- Suscripción a eventos (OnMessageAdded, OnError, OnGeneratingChanged)
- Manejo básico de respuestas

### 2. StreamingExample.cs

Demuestra cómo usar el streaming de respuestas para obtener texto en tiempo real.

**Uso:**
1. Agrega el componente a un GameObject
2. Configura serverUrl y modelName en el Inspector
3. Presiona **S** para iniciar streaming
4. Observa los chunks de texto en la consola

**Características demostradas:**
- Uso de OllamaClient directamente
- Streaming con IAsyncEnumerable
- Procesamiento de chunks en tiempo real
- Construcción de respuesta completa

### 3. ModelManagementExample.cs

Ejemplo de gestión de modelos de Ollama.

**Uso:**
1. Agrega el componente a un GameObject
2. Presiona **M** para refrescar la lista de modelos
3. Presiona **1-9** para cambiar entre modelos disponibles
4. Observa los cambios en la consola

**Características demostradas:**
- Uso del ModelManager singleton
- Obtención de modelos disponibles
- Cambio de modelo activo
- Eventos de modelo (OnModelChanged, OnModelsUpdated)

### 4. ROS2Example.cs

Demuestra la integración con ROS2 para aplicaciones robóticas.

**Uso:**
1. Agrega el componente a un GameObject
2. Presiona **P** para publicar un mensaje de prueba
3. Presiona **C** para publicar un comando
4. Observa los logs en la consola

**Características demostradas:**
- Inicialización de ROS2Bridge
- Publicación de mensajes a topics
- Suscripción a comandos externos
- Procesamiento de comandos

## Cómo Usar los Ejemplos

### Método 1: En el Editor de Unity

1. Abre tu escena en Unity
2. Crea un GameObject vacío (**GameObject > Create Empty**)
3. Nómbralo según el ejemplo (ej: "SimpleChatExample")
4. Agrega el componente del ejemplo:
   - Inspector > **Add Component**
   - Busca el nombre del ejemplo
   - Haz clic para agregarlo
5. Presiona **Play** en Unity
6. Usa las teclas indicadas para probar

### Método 2: En un Script Personalizado

```csharp
using UnityEngine;
using Lobot.Core;
using Lobot.Models;

public class MiScript : MonoBehaviour
{
    private ChatController chatController;

    private void Start()
    {
        chatController = FindObjectOfType<ChatController>();
        chatController.OnMessageAdded += OnMessage;
    }

    private async void SendCustomMessage(string text)
    {
        await chatController.SendMessageAsync(text);
    }

    private void OnMessage(Message msg)
    {
        Debug.Log($"{msg.Role}: {msg.Content}");
    }
}
```

## Escenarios Comunes

### Escenario 1: Chat Simple

```csharp
// Enviar un mensaje y esperar respuesta
var controller = FindObjectOfType<ChatController>();
await controller.SendMessageAsync("Hola, ¿cómo estás?");
```

### Escenario 2: Chat con Historial

```csharp
// El historial se mantiene automáticamente
await controller.SendMessageAsync("Soy desarrollador");
await controller.SendMessageAsync("¿Qué es C#?"); // El contexto se mantiene
```

### Escenario 3: Cambiar Modelo

```csharp
var modelManager = ModelManager.Instance;
modelManager.SetActiveModel("mistral");
```

### Escenario 4: Streaming en UI

```csharp
chatController.OnStreamChunk += (chunk) => 
{
    textComponent.text += chunk;
};
```

### Escenario 5: Publicar a ROS2

```csharp
var ros2 = GetComponent<ROS2Bridge>();
ros2.PublishChatResponse("Respuesta del chatbot");
```

## Personalización

### Modificar Opciones de Generación

```csharp
var options = new GenerateOptions
{
    Temperature = 0.7f,
    MaxTokens = 500,
    TopP = 0.9f
};

// Las opciones se configuran a nivel de servidor
// o se pueden pasar directamente al cliente
```

### Crear Mensajes del Sistema

```csharp
var systemMsg = new Message("system", "Eres un experto en Unity");
// Agregar al historial antes de enviar mensajes de usuario
```

### Manejo de Errores Personalizado

```csharp
chatController.OnError += (error) => 
{
    // Mostrar en UI
    errorText.text = error;
    // O hacer algo más
    Debug.LogError(error);
};
```

## Testing

Para probar los ejemplos sin Unity:

```csharp
// En un test unitario
[Test]
public async Task TestChatController()
{
    var go = new GameObject();
    var controller = go.AddComponent<ChatController>();
    
    await controller.SendMessageAsync("Test");
    var history = controller.GetChatHistory();
    
    Assert.IsTrue(history.Count > 0);
}
```

## Troubleshooting

### "OllamaConnectionException"

- Verifica que Ollama esté ejecutándose: `ollama serve`
- Verifica la URL del servidor
- Asegúrate de tener al menos un modelo descargado

### "Model not found"

```bash
# Listar modelos disponibles
ollama list

# Descargar un modelo
ollama pull llama2
```

### El streaming no funciona

- Verifica que `useStreaming` esté habilitado en ChatController
- Asegúrate de estar suscrito a `OnStreamChunk`
- Verifica que el servidor soporte streaming

## Recursos Adicionales

- [API Documentation](../Docs/API.md)
- [Architecture](../Docs/ARCHITECTURE.md)
- [Setup Guide](../Docs/SETUP.md)

## Contribuir con Ejemplos

Si creas ejemplos útiles, considera contribuirlos:

1. Crea un script bien documentado
2. Sigue las convenciones de código del proyecto
3. Agrega una sección aquí explicando tu ejemplo
4. Haz un Pull Request

---

**¿Preguntas?** Abre un [issue](https://github.com/Jcrex999/Lobot/issues) o consulta la [documentación](../Docs/).
