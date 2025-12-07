# Lobot

**Lobot** es una aplicación Android/Unity para interactuar con chatbots locales y remotos utilizando Ollama.

## Características

- 🤖 **Integración con Ollama**: Soporte para modelos locales y remotos
- 📱 **Multiplataforma**: Desarrollado en Unity para Android
- 🎯 **Selección de Modelos**: Interfaz para elegir y cambiar entre modelos
- 🎨 **UI Amigable**: Interfaz intuitiva y responsive
- 🔌 **Módulos Nativos**: Integración con funcionalidades nativas de Android
- 🤖 **ROS2**: Soporte para robótica con Ros2ForUnity
- 🎤 **Voz e Imagen**: Preparado para futuras integraciones
- ⚙️ **Fine-tuning**: Herramientas para ajuste fino de modelos

## Estructura del Proyecto

```
Lobot/
├── Assets/                      # Recursos de Unity
│   ├── Scripts/                 # Scripts C#
│   │   ├── Core/               # Sistema core
│   │   ├── UI/                 # Interfaz de usuario
│   │   ├── API/                # Integración con APIs
│   │   ├── Models/             # Modelos de datos
│   │   └── ROS2/               # Integración ROS2
│   ├── Scenes/                 # Escenas de Unity
│   ├── Prefabs/                # Prefabs reutilizables
│   └── Resources/              # Recursos cargables
├── Plugins/                    # Plugins nativos
│   ├── Android/                # Módulos nativos Android
│   └── iOS/                    # Módulos nativos iOS (futuro)
├── Docs/                       # Documentación
└── README.md                   # Este archivo
```

## Requisitos

- Unity 2021.3 LTS o superior
- Android SDK API Level 24 o superior
- .NET Standard 2.1
- Ollama instalado (para servidor local)

## Instalación

### 1. Clonar el Repositorio

```bash
git clone https://github.com/Jcrex999/Lobot.git
cd Lobot
```

### 2. Abrir en Unity

1. Abre Unity Hub
2. Haz clic en "Add" y selecciona la carpeta del proyecto
3. Abre el proyecto con Unity 2021.3 LTS o superior

### 3. Configurar Android Build

1. Ve a `File > Build Settings`
2. Selecciona `Android` y haz clic en `Switch Platform`
3. En `Player Settings`, configura:
   - Company Name
   - Product Name
   - Package Name (com.tucompania.lobot)
   - Minimum API Level: 24

## Uso

### Configuración de Ollama

#### Servidor Local
```bash
# Instalar Ollama
curl -fsSL https://ollama.ai/install.sh | sh

# Ejecutar servidor
ollama serve

# Descargar un modelo
ollama pull llama2
```

#### Servidor Remoto
Configura la URL del servidor remoto en la aplicación:
```
Settings > Server Configuration > Remote URL
```

### Ejecutar la Aplicación

1. Conecta tu dispositivo Android o inicia un emulador
2. En Unity, ve a `File > Build Settings`
3. Haz clic en `Build And Run`

## Arquitectura

### Core Systems

#### OllamaClient
Maneja las comunicaciones con el servidor Ollama (local o remoto).

```csharp
// Ejemplo de uso
var client = new OllamaClient("http://localhost:11434");
var response = await client.GenerateAsync("Hola, ¿cómo estás?", "llama2");
```

#### ModelManager
Gestiona la selección y configuración de modelos.

```csharp
// Ejemplo de uso
var manager = ModelManager.Instance;
var models = await manager.GetAvailableModelsAsync();
manager.SetActiveModel("llama2");
```

#### ChatController
Controla la lógica de conversación y historial.

```csharp
// Ejemplo de uso
var controller = new ChatController();
await controller.SendMessageAsync("¿Qué es Unity?");
var history = controller.GetChatHistory();
```

### UI Components

- **ChatView**: Vista principal del chat
- **ModelSelector**: Selector de modelos
- **SettingsPanel**: Panel de configuración
- **MessageBubble**: Burbuja de mensaje individual

### ROS2 Integration

Lobot soporta integración con ROS2 para aplicaciones de robótica:

```csharp
// Ejemplo de publicación a ROS2
var publisher = new ROS2Publisher<String>("/chat_response");
publisher.Publish(response.Text);
```

## API de Ollama

### Endpoints Principales

- `POST /api/generate` - Generar respuesta
- `POST /api/chat` - Chat con historial
- `GET /api/tags` - Listar modelos disponibles
- `POST /api/pull` - Descargar modelo
- `DELETE /api/delete` - Eliminar modelo

### Ejemplo de Request

```json
{
  "model": "llama2",
  "prompt": "¿Qué es la inteligencia artificial?",
  "stream": false
}
```

## Optimización de Rendimiento

### Mejores Prácticas

1. **Streaming**: Usa respuestas en streaming para mejor UX
2. **Pooling**: Reutiliza objetos de UI para reducir garbage collection
3. **Async/Await**: Todas las llamadas de red son asíncronas
4. **Caché**: Cachea respuestas comunes y modelos disponibles
5. **Compresión**: Usa compresión para reducir tamaño de datos

### Configuración Android

```xml
<!-- AndroidManifest.xml -->
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
```

## Futuras Características

### Voz
- [ ] Speech-to-Text con Google Speech API
- [ ] Text-to-Speech con Unity TextToSpeech
- [ ] Soporte para comandos de voz

### Imagen
- [ ] Modelos de visión (LLaVA)
- [ ] Generación de imágenes (Stable Diffusion)
- [ ] Análisis de imágenes

### Fine-tuning
- [ ] Interface para datasets
- [ ] Configuración de hiperparámetros
- [ ] Monitoreo de entrenamiento
- [ ] Export de modelos ajustados

## Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## Guía de Estilo

- **C#**: Seguir convenciones de Unity y Microsoft
- **Comentarios**: Documentar funciones públicas
- **Naming**: PascalCase para clases y métodos, camelCase para variables
- **Async**: Sufijo `Async` para métodos asíncronos

## Licencia

Este proyecto está bajo la licencia MIT. Ver el archivo `LICENSE` para más detalles.

## Contacto

- Desarrollador: [Jcrex999](https://github.com/Jcrex999)
- Proyecto: [Lobot](https://github.com/Jcrex999/Lobot)

## Agradecimientos

- [Ollama](https://ollama.ai/) - Framework de LLM local
- [Unity](https://unity.com/) - Motor de desarrollo
- [Ros2ForUnity](https://github.com/RobotecAI/ros2-for-unity) - Integración ROS2
- Comunidad Open Source

---

**¡Hecho con ❤️ para la comunidad de desarrollo de chatbots!**