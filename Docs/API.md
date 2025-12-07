# API de Lobot

## Integración con Ollama

### Configuración del Cliente

```csharp
using Lobot.API;

// Cliente local
var localClient = new OllamaClient("http://localhost:11434");

// Cliente remoto
var remoteClient = new OllamaClient("https://your-ollama-server.com");
```

## Endpoints

### Generate (Generación Simple)

Genera una respuesta a un prompt sin contexto de conversación.

```csharp
// Request
var response = await client.GenerateAsync(
    prompt: "¿Qué es Unity?",
    model: "llama2",
    options: new GenerateOptions 
    {
        Temperature = 0.7f,
        MaxTokens = 500
    }
);

// Response
public class OllamaResponse
{
    public string Model { get; set; }
    public string Response { get; set; }
    public bool Done { get; set; }
    public long TotalDuration { get; set; }
    public long LoadDuration { get; set; }
    public int PromptEvalCount { get; set; }
    public int EvalCount { get; set; }
}
```

### Chat (Conversación con Contexto)

Mantiene el contexto de la conversación para respuestas más coherentes.

```csharp
// Request
var messages = new List<Message>
{
    new Message { Role = "user", Content = "Hola, soy desarrollador" },
    new Message { Role = "assistant", Content = "¡Hola! Encantado de ayudarte." },
    new Message { Role = "user", Content = "¿Qué es C#?" }
};

var response = await client.ChatAsync(messages, "llama2");

// Message Model
public class Message
{
    public string Role { get; set; } // "user", "assistant", "system"
    public string Content { get; set; }
    public long Timestamp { get; set; }
}
```

### Streaming

Recibe la respuesta en tiempo real a medida que se genera.

```csharp
await foreach (var chunk in client.StreamGenerateAsync("Explica Unity", "llama2"))
{
    Debug.Log(chunk);
    // Actualizar UI con cada fragmento
    chatView.AppendText(chunk);
}
```

### List Models

Obtiene la lista de modelos disponibles en el servidor.

```csharp
var models = await client.GetModelsAsync();

foreach (var model in models)
{
    Debug.Log($"Model: {model.Name}");
    Debug.Log($"Size: {model.Size}");
    Debug.Log($"Modified: {model.ModifiedAt}");
}

// Model Info
public class OllamaModel
{
    public string Name { get; set; }
    public string ModifiedAt { get; set; }
    public long Size { get; set; }
    public string Digest { get; set; }
    public ModelDetails Details { get; set; }
}
```

### Pull Model

Descarga un modelo del registro de Ollama.

```csharp
await client.PullModelAsync(
    modelName: "llama2",
    onProgress: (progress) => 
    {
        Debug.Log($"Downloading: {progress.Percent}%");
        progressBar.SetProgress(progress.Percent);
    }
);
```

### Delete Model

Elimina un modelo del sistema local.

```csharp
await client.DeleteModelAsync("llama2");
```

## Opciones de Generación

### GenerateOptions

```csharp
public class GenerateOptions
{
    // Temperatura (0.0 - 2.0): Creatividad de las respuestas
    public float? Temperature { get; set; } = 0.8f;
    
    // Top P (0.0 - 1.0): Nucleus sampling
    public float? TopP { get; set; } = 0.9f;
    
    // Top K: Número de tokens a considerar
    public int? TopK { get; set; } = 40;
    
    // Número máximo de tokens a generar
    public int? MaxTokens { get; set; } = 2048;
    
    // Penalización por repetición (0.0 - 2.0)
    public float? RepeatPenalty { get; set; } = 1.1f;
    
    // Semilla para reproducibilidad
    public int? Seed { get; set; }
    
    // Detener generación en estos tokens
    public List<string> Stop { get; set; }
    
    // Contexto del sistema
    public string System { get; set; }
}
```

## Manejo de Errores

### Tipos de Errores

```csharp
try
{
    var response = await client.GenerateAsync("Test", "llama2");
}
catch (OllamaConnectionException ex)
{
    // Error de conexión con el servidor
    Debug.LogError($"Connection failed: {ex.Message}");
    ShowErrorMessage("No se pudo conectar al servidor");
}
catch (OllamaModelNotFoundException ex)
{
    // Modelo no encontrado
    Debug.LogError($"Model not found: {ex.ModelName}");
    ShowErrorMessage($"Modelo '{ex.ModelName}' no disponible");
}
catch (OllamaTimeoutException ex)
{
    // Timeout
    Debug.LogError($"Request timeout: {ex.Message}");
    ShowErrorMessage("La petición tardó demasiado");
}
catch (OllamaException ex)
{
    // Error general de Ollama
    Debug.LogError($"Ollama error: {ex.Message}");
    ShowErrorMessage($"Error: {ex.Message}");
}
```

### Códigos de Estado HTTP

| Código | Significado | Acción |
|--------|-------------|--------|
| 200 | OK | Continuar |
| 400 | Bad Request | Validar parámetros |
| 404 | Not Found | Modelo no existe |
| 500 | Server Error | Reintentar o usar fallback |
| 503 | Service Unavailable | Servidor ocupado, esperar |

## Ejemplos de Uso

### Ejemplo 1: Chat Simple

```csharp
using UnityEngine;
using Lobot.API;
using System.Threading.Tasks;

public class SimpleChatExample : MonoBehaviour
{
    private OllamaClient client;
    
    async void Start()
    {
        client = new OllamaClient("http://localhost:11434");
        
        var response = await client.GenerateAsync(
            "Explica qué es Unity en una línea",
            "llama2"
        );
        
        Debug.Log($"Response: {response.Response}");
    }
}
```

### Ejemplo 2: Chat con Historial

```csharp
using UnityEngine;
using Lobot.API;
using Lobot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ChatHistoryExample : MonoBehaviour
{
    private OllamaClient client;
    private List<Message> history = new List<Message>();
    
    async void Start()
    {
        client = new OllamaClient("http://localhost:11434");
        await SendMessage("Hola, soy un desarrollador de Unity");
        await SendMessage("¿Qué es C#?");
    }
    
    async Task SendMessage(string content)
    {
        history.Add(new Message 
        { 
            Role = "user", 
            Content = content 
        });
        
        var response = await client.ChatAsync(history, "llama2");
        
        history.Add(new Message 
        { 
            Role = "assistant", 
            Content = response.Response 
        });
        
        Debug.Log($"User: {content}");
        Debug.Log($"Assistant: {response.Response}");
    }
}
```

### Ejemplo 3: Streaming

```csharp
using UnityEngine;
using Lobot.API;
using System.Text;
using System.Threading.Tasks;

public class StreamingExample : MonoBehaviour
{
    private OllamaClient client;
    private StringBuilder responseBuilder = new StringBuilder();
    
    async void Start()
    {
        client = new OllamaClient("http://localhost:11434");
        
        await foreach (var chunk in client.StreamGenerateAsync(
            "Escribe un poema corto sobre Unity",
            "llama2"
        ))
        {
            responseBuilder.Append(chunk);
            Debug.Log($"Chunk: {chunk}");
            // Actualizar UI en tiempo real
        }
        
        Debug.Log($"Complete response: {responseBuilder}");
    }
}
```

### Ejemplo 4: Selección de Modelos

```csharp
using UnityEngine;
using Lobot.API;
using System.Threading.Tasks;

public class ModelSelectionExample : MonoBehaviour
{
    private OllamaClient client;
    
    async void Start()
    {
        client = new OllamaClient("http://localhost:11434");
        
        var models = await client.GetModelsAsync();
        
        Debug.Log("Available models:");
        foreach (var model in models)
        {
            Debug.Log($"- {model.Name} ({FormatSize(model.Size)})");
        }
    }
    
    string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;
        
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        
        return $"{size:0.##} {sizes[order]}";
    }
}
```

### Ejemplo 5: Configuración Avanzada

```csharp
using UnityEngine;
using Lobot.API;
using System.Threading.Tasks;

public class AdvancedConfigExample : MonoBehaviour
{
    private OllamaClient client;
    
    async void Start()
    {
        client = new OllamaClient("http://localhost:11434");
        
        var options = new GenerateOptions
        {
            Temperature = 0.5f,      // Más determinista
            TopP = 0.95f,
            TopK = 50,
            MaxTokens = 1000,
            RepeatPenalty = 1.2f,
            System = "Eres un experto en Unity y C#"
        };
        
        var response = await client.GenerateAsync(
            "¿Cuáles son las mejores prácticas en Unity?",
            "llama2",
            options
        );
        
        Debug.Log(response.Response);
    }
}
```

## Integración con UI

### ChatController

```csharp
using UnityEngine;
using UnityEngine.UI;
using Lobot.API;
using Lobot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;

public class ChatController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private Transform chatContainer;
    [SerializeField] private GameObject messagePrefab;
    
    private OllamaClient client;
    private List<Message> history = new List<Message>();
    private string currentModel = "llama2";
    
    void Start()
    {
        client = new OllamaClient("http://localhost:11434");
        sendButton.onClick.AddListener(() => OnSendMessage());
        inputField.onSubmit.AddListener((text) => OnSendMessage());
    }
    
    async void OnSendMessage()
    {
        string message = inputField.text.Trim();
        if (string.IsNullOrEmpty(message)) return;
        
        inputField.text = "";
        sendButton.interactable = false;
        
        // Mostrar mensaje del usuario
        AddMessageToUI(message, true);
        
        // Agregar al historial
        history.Add(new Message { Role = "user", Content = message });
        
        try
        {
            // Obtener respuesta
            var response = await client.ChatAsync(history, currentModel);
            
            // Agregar respuesta al historial
            history.Add(new Message 
            { 
                Role = "assistant", 
                Content = response.Response 
            });
            
            // Mostrar respuesta en UI
            AddMessageToUI(response.Response, false);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error: {ex.Message}");
            AddMessageToUI($"Error: {ex.Message}", false);
        }
        finally
        {
            sendButton.interactable = true;
        }
    }
    
    void AddMessageToUI(string content, bool isUser)
    {
        var messageObj = Instantiate(messagePrefab, chatContainer);
        var messageText = messageObj.GetComponentInChildren<TMP_Text>();
        messageText.text = content;
        
        // Aplicar estilo según el remitente
        var background = messageObj.GetComponent<Image>();
        background.color = isUser ? 
            new Color(0.3f, 0.6f, 1f) : 
            new Color(0.8f, 0.8f, 0.8f);
    }
    
    public void SetModel(string modelName)
    {
        currentModel = modelName;
    }
    
    public void ClearHistory()
    {
        history.Clear();
        
        foreach (Transform child in chatContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
```

## Performance Tips

### 1. Usar Streaming para Respuestas Largas

```csharp
// ✓ Mejor UX
await foreach (var chunk in client.StreamGenerateAsync(prompt, model))
{
    chatView.AppendText(chunk);
}

// ✗ Esperar respuesta completa
var response = await client.GenerateAsync(prompt, model);
chatView.SetText(response.Response);
```

### 2. Caché de Modelos

```csharp
private Dictionary<string, List<OllamaModel>> modelsCache = 
    new Dictionary<string, List<OllamaModel>>();

async Task<List<OllamaModel>> GetModelsWithCache()
{
    if (modelsCache.ContainsKey("models"))
    {
        return modelsCache["models"];
    }
    
    var models = await client.GetModelsAsync();
    modelsCache["models"] = models;
    return models;
}
```

### 3. Limitar Historial

```csharp
private const int MAX_HISTORY = 20;

void AddToHistory(Message message)
{
    history.Add(message);
    
    if (history.Count > MAX_HISTORY)
    {
        history.RemoveAt(0);
    }
}
```

### 4. Async/Await Correctamente

```csharp
// ✓ Correcto
async void OnButtonClick()
{
    try
    {
        await SendMessageAsync();
    }
    catch (Exception ex)
    {
        HandleError(ex);
    }
}

// ✗ Incorrecto - Fire and forget
void OnButtonClick()
{
    SendMessageAsync(); // No await, no error handling
}
```

## Recursos Adicionales

- [Ollama API Documentation](https://github.com/ollama/ollama/blob/main/docs/api.md)
- [Unity Async/Await](https://docs.unity3d.com/Manual/JobSystemAsyncAwait.html)
- [REST Best Practices](https://restfulapi.net/)
