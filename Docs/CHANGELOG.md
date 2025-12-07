# Changelog

Todos los cambios notables de este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto adhiere a [Semantic Versioning](https://semver.org/lang/es/).

## [Unreleased]

### Agregado
- Estructura inicial del proyecto Unity/Android
- Integración completa con Ollama API
  - Soporte para generación simple
  - Soporte para chat con contexto
  - Soporte para streaming de respuestas
  - Gestión de modelos (listar, descargar, eliminar)
- Sistema de gestión de modelos (ModelManager)
- Controlador principal de chat (ChatController)
- Componentes de UI
  - ChatView para vista principal de conversación
  - MessageBubble para burbujas de mensaje
  - ModelSelector para selección de modelos
- Integración básica con ROS2 (placeholder para Ros2ForUnity)
- Puente nativo para Android (AndroidBridge)
  - Toast notifications
  - Network checking
  - Battery level monitoring
  - Share functionality
  - Vibration support
- Modelos de datos completos
  - Message
  - OllamaModel
  - GenerateOptions
  - ServerConfig
  - ChatConversation
- Documentación completa
  - README.md con instrucciones de instalación y uso
  - ARCHITECTURE.md con detalles de arquitectura
  - API.md con documentación de API y ejemplos
  - CONTRIBUTING.md con guías de contribución
  - CHANGELOG.md (este archivo)
- Estructura de carpetas organizada
  - Assets/Scripts/{Core, UI, API, Models, ROS2, Utils}
  - Docs/
  - Plugins/Android/

### Por Hacer
- [ ] Implementar UI completa en Unity con prefabs
- [ ] Integrar Ros2ForUnity completamente
- [ ] Agregar soporte para voz (Speech-to-Text y Text-to-Speech)
- [ ] Agregar soporte para imágenes (modelos de visión)
- [ ] Implementar herramientas de fine-tuning
- [ ] Agregar persistencia de conversaciones
- [ ] Implementar sistema de temas/skins
- [ ] Agregar tests unitarios
- [ ] Agregar tests de integración
- [ ] Optimizar para bajo consumo de batería
- [ ] Implementar caché offline
- [ ] Agregar configuración avanzada de servidor
- [ ] Implementar autenticación para servidores remotos
- [ ] Agregar soporte para múltiples conversaciones simultáneas
- [ ] Implementar búsqueda en historial de chat

## Versiones Futuras

### [0.2.0] - Voz e Imagen
- Integración de Speech-to-Text
- Integración de Text-to-Speech
- Soporte para modelos de visión (LLaVA)
- Análisis de imágenes
- Comandos de voz

### [0.3.0] - Fine-tuning
- Interface para datasets
- Configuración de hiperparámetros
- Monitoreo de entrenamiento
- Export de modelos ajustados
- Gestión de datasets

### [0.4.0] - Mejoras de UX
- Temas personalizables
- Animaciones mejoradas
- Modo oscuro/claro
- Widgets personalizables
- Accesibilidad mejorada

### [0.5.0] - Multi-modal
- Soporte para múltiples tipos de entrada
- Generación de imágenes
- Procesamiento de audio
- Integración de video
- Respuestas enriquecidas

### [1.0.0] - Release Estable
- Todas las funcionalidades core implementadas
- Tests completos
- Documentación completa
- Performance optimizado
- Build de producción
- Publicación en Play Store

## Notas de Desarrollo

### Convenciones
- Usamos [Semantic Versioning](https://semver.org/)
- Formato de changelog basado en [Keep a Changelog](https://keepachangelog.com/)
- Commits siguiendo [Conventional Commits](https://www.conventionalcommits.org/)

### Categorías de Cambios
- **Agregado**: Nuevas funcionalidades
- **Cambiado**: Cambios en funcionalidades existentes
- **Obsoleto**: Funcionalidades que serán removidas
- **Removido**: Funcionalidades removidas
- **Corregido**: Corrección de bugs
- **Seguridad**: Vulnerabilidades corregidas
