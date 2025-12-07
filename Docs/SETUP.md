# Guía de Configuración de Lobot

Esta guía te ayudará a configurar y ejecutar Lobot en tu entorno de desarrollo.

## Requisitos Previos

### Software Necesario

1. **Unity Hub** (última versión)
   - Descarga: https://unity.com/download

2. **Unity Editor 2021.3 LTS o superior**
   - Se recomienda 2021.3 LTS para estabilidad
   - Módulos necesarios:
     - Android Build Support
     - Android SDK & NDK Tools
     - OpenJDK

3. **Ollama**
   - Descarga: https://ollama.ai/download
   - Requerido para ejecutar modelos localmente

4. **Android Studio** (opcional pero recomendado)
   - Para emuladores Android y SDK management
   - Descarga: https://developer.android.com/studio

5. **Git**
   - Para control de versiones
   - Descarga: https://git-scm.com/

## Paso 1: Clonar el Repositorio

```bash
git clone https://github.com/Jcrex999/Lobot.git
cd Lobot
```

## Paso 2: Abrir el Proyecto en Unity

1. Abre **Unity Hub**
2. Haz clic en **Add** (o **Agregar**)
3. Navega a la carpeta del proyecto `Lobot`
4. Selecciona la carpeta y haz clic en **Abrir**
5. El proyecto aparecerá en Unity Hub
6. Haz clic en el proyecto para abrirlo en Unity Editor

### Primera Vez Abriendo el Proyecto

La primera vez que abras el proyecto, Unity necesitará:
- Importar todos los assets
- Compilar scripts
- Generar archivos de proyecto (.sln, .csproj)

Este proceso puede tomar varios minutos.

## Paso 3: Instalar Dependencias

### Newtonsoft.Json (JSON .NET)

El proyecto requiere Newtonsoft.Json para serialización JSON:

1. En Unity, ve a **Window > Package Manager**
2. Haz clic en el botón **+** en la esquina superior izquierda
3. Selecciona **Add package by name...**
4. Ingresa: `com.unity.nuget.newtonsoft-json`
5. Haz clic en **Add**

**Alternativa manual:**

1. Descarga Newtonsoft.Json desde: https://www.newtonsoft.com/json
2. Copia el DLL a `Assets/Plugins/`

### TextMeshPro

TextMeshPro debería venir incluido con Unity, pero si no:

1. Ve a **Window > Package Manager**
2. Busca **TextMeshPro**
3. Haz clic en **Install**

## Paso 4: Configurar Android Build

### Configurar SDK y NDK

1. En Unity, ve a **Edit > Preferences** (Windows) o **Unity > Preferences** (Mac)
2. Selecciona **External Tools**
3. Configura las rutas:
   - **Android SDK**: Ruta a tu Android SDK
   - **Android NDK**: Ruta a tu Android NDK
   - **JDK**: Ruta a OpenJDK

Si instalaste Android Build Support con Unity Hub, estas rutas deberían estar configuradas automáticamente.

### Configurar Build Settings

1. Ve a **File > Build Settings**
2. Selecciona **Android** en la lista de plataformas
3. Haz clic en **Switch Platform** (si no está ya seleccionado)
4. Espera a que Unity complete el cambio de plataforma

### Configurar Player Settings

1. En Build Settings, haz clic en **Player Settings**
2. Configura las siguientes opciones:

**Identification:**
- Company Name: `Tu Nombre/Empresa`
- Product Name: `Lobot`
- Package Name: `com.tucompania.lobot` (debe ser único)
- Version: `0.1.0`

**Other Settings:**
- Scripting Backend: `IL2CPP`
- API Compatibility Level: `.NET Standard 2.1`
- Target API Level: `Automatic (highest installed)`
- Minimum API Level: `Android 7.0 'Nougat' (API level 24)`

**Publishing Settings:**
- Create keystore si aún no tienes uno (necesario para publicar)

## Paso 5: Configurar Ollama

### Instalación Local

#### Windows
```bash
# Descargar e instalar desde https://ollama.ai/download
# O usar winget
winget install Ollama.Ollama
```

#### macOS
```bash
# Descargar desde https://ollama.ai/download
# O usar Homebrew
brew install ollama
```

#### Linux
```bash
curl -fsSL https://ollama.ai/install.sh | sh
```

### Ejecutar Servidor Ollama

```bash
# Iniciar servidor
ollama serve
```

El servidor estará disponible en `http://localhost:11434`

### Descargar Modelos

```bash
# Modelo recomendado para comenzar
ollama pull llama2

# Otros modelos útiles
ollama pull mistral
ollama pull codellama
ollama pull llama2:13b
```

### Verificar Instalación

```bash
# Listar modelos instalados
ollama list

# Probar modelo
ollama run llama2 "Hola, ¿cómo estás?"
```

## Paso 6: Configurar el Proyecto

### Archivo de Configuración

Edita `Assets/Resources/config.json`:

```json
{
  "serverUrl": "http://localhost:11434",
  "defaultModel": "llama2",
  "useStreaming": true,
  "maxHistorySize": 50
}
```

**Para servidor remoto:**
```json
{
  "serverUrl": "https://tu-servidor-ollama.com",
  "defaultModel": "llama2",
  "useStreaming": true
}
```

## Paso 7: Crear Escena Principal

### Crear Escena Básica

1. En Unity, crea una nueva escena: **File > New Scene**
2. Guarda como: `Assets/Scenes/MainScene.unity`

### Agregar UI Canvas

1. **GameObject > UI > Canvas**
2. En Canvas, configura:
   - Render Mode: `Screen Space - Overlay`
   - Canvas Scaler:
     - UI Scale Mode: `Scale With Screen Size`
     - Reference Resolution: `1080 x 1920`
     - Match: `0.5`

### Agregar ChatController

1. Crea un GameObject vacío: **GameObject > Create Empty**
2. Nómbralo `ChatManager`
3. Agrega el componente `ChatController`:
   - Inspector > **Add Component** > busca `ChatController`
4. Configura en el Inspector:
   - Server URL: `http://localhost:11434`
   - Max History Size: `50`
   - Use Streaming: ✓

## Paso 8: Probar en Editor

1. Haz clic en el botón **Play** en Unity
2. Verifica los logs en la **Console**:
   ```
   Ollama client initialized with URL: http://localhost:11434
   ```

Si hay errores, verifica que:
- Ollama esté ejecutándose
- La URL sea correcta
- Tengas al menos un modelo descargado

## Paso 9: Build para Android

### Conectar Dispositivo o Emulador

**Dispositivo físico:**
1. Habilita **Opciones de Desarrollador** en tu Android
2. Habilita **Depuración USB**
3. Conecta el dispositivo por USB
4. Acepta la autorización en el dispositivo

**Emulador:**
1. Abre Android Studio
2. Ve a **Tools > AVD Manager**
3. Crea o inicia un emulador

### Build and Run

1. Ve a **File > Build Settings**
2. Verifica que **Android** esté seleccionado
3. Haz clic en **Build And Run**
4. Elige una ubicación para guardar el APK
5. Espera a que Unity compile y despliegue

El proceso tomará varios minutos la primera vez.

## Paso 10: Configurar en Dispositivo

### Primera Ejecución

1. La app pedirá permisos de internet (ya incluidos en manifest)
2. En Settings, configura la URL del servidor:
   - Para dispositivo físico con servidor local:
     - Usa la IP de tu computadora en la red local
     - Ejemplo: `http://192.168.1.100:11434`
   - Para emulador:
     - Usa `http://10.0.2.2:11434` (IP especial que apunta al host)

### Encontrar IP de tu Computadora

**Windows:**
```bash
ipconfig
# Busca "IPv4 Address" en tu adaptador de red activo
```

**macOS/Linux:**
```bash
ifconfig
# O
ip addr show
```

## Solución de Problemas Comunes

### Error: "Unable to connect to server"

- Verifica que Ollama esté ejecutándose: `ollama serve`
- Verifica la URL del servidor en Settings
- En Android, asegúrate de usar la IP correcta
- Verifica que el firewall permita conexiones en el puerto 11434

### Error: "Model not found"

```bash
# Listar modelos disponibles
ollama list

# Si no hay modelos, descarga uno
ollama pull llama2
```

### Error de Build en Android

- Verifica que Android SDK/NDK estén instalados correctamente
- Ve a **Edit > Preferences > External Tools** y verifica las rutas
- Intenta **File > Build Settings > Android > Switch Platform** de nuevo
- Limpia el cache: **Edit > Preferences > Cache Server > Clean Cache**

### Problemas de Rendimiento

- Usa modelos más pequeños (llama2:7b en lugar de llama2:13b)
- Reduce `maxHistorySize` en configuración
- Desactiva streaming si la conexión es lenta
- Cierra otras aplicaciones en el dispositivo

### Errores de Compilación

Si hay errores de compilación relacionados con Newtonsoft.Json:

1. Verifica que el package esté instalado: **Window > Package Manager**
2. Si no, reinstala: **Package Manager > + > Add package by name**
3. Ingresa: `com.unity.nuget.newtonsoft-json`

## Recursos Adicionales

### Documentación
- [README.md](../README.md) - Información general
- [ARCHITECTURE.md](ARCHITECTURE.md) - Arquitectura del proyecto
- [API.md](API.md) - Documentación de la API
- [CONTRIBUTING.md](CONTRIBUTING.md) - Guía de contribución

### Enlaces Útiles
- [Ollama Documentation](https://github.com/ollama/ollama/tree/main/docs)
- [Unity Android Documentation](https://docs.unity3d.com/Manual/android.html)
- [Unity Manual](https://docs.unity3d.com/Manual/index.html)

### Comunidad
- [Issues](https://github.com/Jcrex999/Lobot/issues)
- [Discussions](https://github.com/Jcrex999/Lobot/discussions)

## Próximos Pasos

Ahora que tienes Lobot configurado:

1. **Explora el código**: Revisa los scripts en `Assets/Scripts/`
2. **Personaliza la UI**: Modifica colores, fuentes, layouts
3. **Agrega funcionalidades**: Consulta [CONTRIBUTING.md](CONTRIBUTING.md)
4. **Integra ROS2**: Si trabajas con robótica
5. **Experimenta con modelos**: Prueba diferentes modelos de Ollama

## Contacto y Soporte

Si tienes problemas:
1. Revisa los [Issues](https://github.com/Jcrex999/Lobot/issues) existentes
2. Crea un nuevo issue si tu problema es único
3. Contacta a [@Jcrex999](https://github.com/Jcrex999)

---

**¡Bienvenido a Lobot! 🤖**
