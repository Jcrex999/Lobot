# Arquitectura de Lobot

## Visión General

Lobot es una aplicación Android/Unity diseñada con una arquitectura modular y escalable que facilita la integración con chatbots locales y remotos usando Ollama.

## Capas de la Arquitectura

### 1. Capa de Presentación (UI)
- **ChatView**: Vista principal de conversación
- **ModelSelector**: Componente de selección de modelos
- **SettingsPanel**: Panel de configuración
- **MessageBubble**: Componente de mensaje individual
- **LoadingIndicator**: Indicador de carga

### 2. Capa de Lógica de Negocio (Core)
- **ChatController**: Controlador principal del chat
- **ConversationManager**: Gestor de conversaciones
- **StateManager**: Gestor de estado de la aplicación
- **EventBus**: Sistema de eventos

### 3. Capa de Integración (API)
- **OllamaClient**: Cliente principal de Ollama
- **ApiClient**: Cliente base HTTP
- **StreamHandler**: Manejador de respuestas en streaming
- **RequestBuilder**: Constructor de peticiones

### 4. Capa de Datos (Models)
- **Message**: Modelo de mensaje
- **ChatModel**: Modelo de chat
- **OllamaModel**: Modelo de configuración de Ollama
- **ServerConfig**: Configuración del servidor

### 5. Capa de Integración Nativa (Plugins)
- **AndroidBridge**: Puente con funcionalidades Android
- **PermissionManager**: Gestor de permisos
- **NetworkMonitor**: Monitor de conectividad

### 6. Capa de Robótica (ROS2)
- **ROS2Bridge**: Puente con ROS2
- **TopicPublisher**: Publicador de topics
- **TopicSubscriber**: Suscriptor de topics

## Flujo de Datos

```
Usuario → UI → ChatController → OllamaClient → Ollama Server
                    ↓                             ↓
              ConversationManager ← Response ← Streaming
                    ↓
              StateManager → UI Update
```

## Patrones de Diseño

### Singleton
- **ModelManager**: Única instancia para gestión de modelos
- **StateManager**: Estado global de la aplicación
- **EventBus**: Sistema de eventos centralizado

### Observer
- **EventBus**: Notificación de cambios
- **StateManager**: Actualización de UI

### Factory
- **MessageFactory**: Creación de mensajes
- **UIFactory**: Creación de componentes UI

### Repository
- **ConversationRepository**: Persistencia de conversaciones
- **ModelRepository**: Caché de modelos

### Adapter
- **OllamaAdapter**: Adaptación de respuestas de Ollama
- **ROS2Adapter**: Adaptación de mensajes ROS2

## Componentes Principales

### OllamaClient

Responsabilidades:
- Comunicación con servidor Ollama
- Manejo de streaming
- Gestión de timeouts
- Retry logic

```csharp
public class OllamaClient
{
    public async Task<OllamaResponse> GenerateAsync(string prompt, string model);
    public async Task<OllamaResponse> ChatAsync(List<Message> history, string model);
    public async Task<List<OllamaModel>> GetModelsAsync();
    public IAsyncEnumerable<string> StreamGenerateAsync(string prompt, string model);
}
```

### ChatController

Responsabilidades:
- Control de flujo de conversación
- Validación de entrada
- Manejo de errores
- Coordinación entre UI y API

```csharp
public class ChatController
{
    public async Task SendMessageAsync(string message);
    public List<Message> GetChatHistory();
    public void ClearHistory();
    public void SetModel(string modelName);
}
```

### ModelManager

Responsabilidades:
- Gestión de modelos disponibles
- Selección de modelo activo
- Caché de información de modelos
- Actualización de lista de modelos

```csharp
public class ModelManager
{
    public static ModelManager Instance { get; }
    public async Task<List<OllamaModel>> GetAvailableModelsAsync();
    public void SetActiveModel(string modelName);
    public OllamaModel GetActiveModel();
    public async Task RefreshModelsAsync();
}
```

## Gestión de Estado

### Estados de la Aplicación
1. **Idle**: Esperando interacción del usuario
2. **Connecting**: Conectando con servidor
3. **Generating**: Generando respuesta
4. **Streaming**: Recibiendo respuesta en streaming
5. **Error**: Error en la operación
6. **Offline**: Sin conexión

### Transiciones de Estado
```
Idle → Connecting → Generating → Streaming → Idle
  ↓         ↓           ↓            ↓         ↓
  → → → → → Error ← ← ← ← ← ← ← ← ← ←
```

## Manejo de Errores

### Categorías de Errores
1. **Network Errors**: Problemas de conexión
2. **API Errors**: Errores del servidor Ollama
3. **Validation Errors**: Errores de validación
4. **System Errors**: Errores del sistema

### Estrategias de Recuperación
- **Retry**: Reintentar operación con backoff exponencial
- **Fallback**: Usar servidor alternativo
- **Cache**: Usar respuesta en caché
- **User Notification**: Notificar al usuario

## Optimizaciones

### Performance
- **Object Pooling**: Pool de objetos UI
- **Lazy Loading**: Carga diferida de recursos
- **Async Operations**: Operaciones asíncronas
- **Caching**: Caché de respuestas y modelos

### Memory
- **Dispose Pattern**: Liberación de recursos
- **Weak References**: Referencias débiles cuando apropiado
- **Limited History**: Límite en historial de chat

### Network
- **Request Batching**: Agrupación de peticiones
- **Compression**: Compresión de datos
- **Connection Pooling**: Pool de conexiones

## Seguridad

### Mejores Prácticas
1. **HTTPS**: Usar siempre HTTPS para servidores remotos
2. **Certificate Validation**: Validar certificados SSL
3. **Input Sanitization**: Sanitizar entrada de usuario
4. **Rate Limiting**: Limitar frecuencia de peticiones
5. **Token Management**: Gestión segura de tokens

### Permisos Android
```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.CAMERA" />
```

## Extensibilidad

### Puntos de Extensión
1. **Custom Models**: Soporte para modelos personalizados
2. **Plugins**: Sistema de plugins para funcionalidades adicionales
3. **UI Themes**: Temas personalizables
4. **Message Handlers**: Handlers personalizados de mensajes

### Futuras Extensiones
- Voice Integration
- Image Processing
- Fine-tuning Interface
- Multi-modal Support
- Custom Training

## Testing

### Estrategia de Testing
- **Unit Tests**: Lógica de negocio y utilidades
- **Integration Tests**: Integración con APIs
- **UI Tests**: Componentes de interfaz
- **Performance Tests**: Benchmarks de rendimiento

### Herramientas
- NUnit para testing unitario
- Unity Test Framework
- Moq para mocking
- BenchmarkDotNet para benchmarks

## Deployment

### Build Pipeline
1. Run tests
2. Static analysis
3. Build APK
4. Sign APK
5. Upload to store

### Configuraciones
- **Debug**: Desarrollo local
- **Staging**: Testing pre-producción
- **Release**: Producción

## Documentación

### Tipos de Documentación
1. **README.md**: Información general
2. **ARCHITECTURE.md**: Este documento
3. **API.md**: Documentación de API
4. **CONTRIBUTING.md**: Guía de contribución
5. **CHANGELOG.md**: Registro de cambios

### Standards
- Markdown para toda la documentación
- Diagramas con Mermaid
- Code examples en bloques de código
- Enlaces a recursos externos

## Referencias

- [Unity Best Practices](https://unity.com/how-to/programming-unity)
- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Ollama API Documentation](https://github.com/ollama/ollama/blob/main/docs/api.md)
- [ROS2 Documentation](https://docs.ros.org/en/rolling/)
