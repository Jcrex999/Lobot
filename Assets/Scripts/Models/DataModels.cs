using System;
using Newtonsoft.Json;

namespace Lobot.Models
{
    /// <summary>
    /// Modelo de mensaje en una conversación.
    /// </summary>
    [Serializable]
    public class Message
    {
        /// <summary>
        /// Rol del mensaje: "user", "assistant", o "system"
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; set; }

        /// <summary>
        /// Contenido del mensaje
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Timestamp Unix del mensaje
        /// </summary>
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Message()
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Constructor con parámetros
        /// </summary>
        public Message(string role, string content)
        {
            Role = role;
            Content = content;
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }

    /// <summary>
    /// Información detallada de un modelo Ollama.
    /// </summary>
    [Serializable]
    public class OllamaModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("modified_at")]
        public string ModifiedAt { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }

        [JsonProperty("details")]
        public ModelDetails Details { get; set; }

        /// <summary>
        /// Formatea el tamaño del modelo en formato legible
        /// </summary>
        public string GetFormattedSize()
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = Size;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
    }

    /// <summary>
    /// Detalles adicionales de un modelo.
    /// </summary>
    [Serializable]
    public class ModelDetails
    {
        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("family")]
        public string Family { get; set; }

        [JsonProperty("families")]
        public string[] Families { get; set; }

        [JsonProperty("parameter_size")]
        public string ParameterSize { get; set; }

        [JsonProperty("quantization_level")]
        public string QuantizationLevel { get; set; }
    }

    /// <summary>
    /// Opciones de generación para el modelo.
    /// </summary>
    [Serializable]
    public class GenerateOptions
    {
        /// <summary>
        /// Temperatura (0.0 - 2.0): Controla la aleatoriedad de las respuestas.
        /// Valores más bajos = más determinista, valores más altos = más creativo.
        /// </summary>
        [JsonProperty("temperature")]
        public float? Temperature { get; set; }

        /// <summary>
        /// Top P (0.0 - 1.0): Nucleus sampling.
        /// Considera solo los tokens con probabilidad acumulada hasta P.
        /// </summary>
        [JsonProperty("top_p")]
        public float? TopP { get; set; }

        /// <summary>
        /// Top K: Número de tokens con mayor probabilidad a considerar.
        /// </summary>
        [JsonProperty("top_k")]
        public int? TopK { get; set; }

        /// <summary>
        /// Número máximo de tokens a generar.
        /// </summary>
        [JsonProperty("num_predict")]
        public int? MaxTokens { get; set; }

        /// <summary>
        /// Penalización por repetición (0.0 - 2.0).
        /// Reduce la probabilidad de repetir tokens.
        /// </summary>
        [JsonProperty("repeat_penalty")]
        public float? RepeatPenalty { get; set; }

        /// <summary>
        /// Semilla para reproducibilidad de resultados.
        /// </summary>
        [JsonProperty("seed")]
        public int? Seed { get; set; }

        /// <summary>
        /// Secuencias donde detener la generación.
        /// </summary>
        [JsonProperty("stop")]
        public string[] Stop { get; set; }

        /// <summary>
        /// Contexto del sistema (instrucciones al modelo).
        /// </summary>
        [JsonProperty("system")]
        public string System { get; set; }

        /// <summary>
        /// Número de contexto (tokens de contexto a mantener).
        /// </summary>
        [JsonProperty("num_ctx")]
        public int? NumContext { get; set; }

        /// <summary>
        /// Constructor con valores por defecto recomendados.
        /// </summary>
        public static GenerateOptions Default()
        {
            return new GenerateOptions
            {
                Temperature = 0.8f,
                TopP = 0.9f,
                TopK = 40,
                MaxTokens = 2048,
                RepeatPenalty = 1.1f,
                NumContext = 2048
            };
        }

        /// <summary>
        /// Configuración para respuestas más creativas.
        /// </summary>
        public static GenerateOptions Creative()
        {
            return new GenerateOptions
            {
                Temperature = 1.2f,
                TopP = 0.95f,
                TopK = 50,
                MaxTokens = 2048,
                RepeatPenalty = 1.1f
            };
        }

        /// <summary>
        /// Configuración para respuestas más precisas y deterministas.
        /// </summary>
        public static GenerateOptions Precise()
        {
            return new GenerateOptions
            {
                Temperature = 0.3f,
                TopP = 0.85f,
                TopK = 30,
                MaxTokens = 2048,
                RepeatPenalty = 1.2f
            };
        }
    }

    /// <summary>
    /// Configuración del servidor Ollama.
    /// </summary>
    [Serializable]
    public class ServerConfig
    {
        /// <summary>
        /// URL del servidor
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Indica si es un servidor local o remoto
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// Timeout en segundos
        /// </summary>
        public int TimeoutSeconds { get; set; }

        /// <summary>
        /// Token de autenticación (si se requiere)
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// Constructor con valores por defecto
        /// </summary>
        public ServerConfig()
        {
            Url = "http://localhost:11434";
            IsLocal = true;
            TimeoutSeconds = 300;
        }

        /// <summary>
        /// Crea una configuración para servidor local
        /// </summary>
        public static ServerConfig Local(int port = 11434)
        {
            return new ServerConfig
            {
                Url = $"http://localhost:{port}",
                IsLocal = true,
                TimeoutSeconds = 300
            };
        }

        /// <summary>
        /// Crea una configuración para servidor remoto
        /// </summary>
        public static ServerConfig Remote(string url, string authToken = null)
        {
            return new ServerConfig
            {
                Url = url,
                IsLocal = false,
                TimeoutSeconds = 300,
                AuthToken = authToken
            };
        }
    }

    /// <summary>
    /// Modelo de conversación.
    /// </summary>
    [Serializable]
    public class ChatConversation
    {
        /// <summary>
        /// ID único de la conversación
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Título de la conversación
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Modelo usado en la conversación
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Historial de mensajes
        /// </summary>
        public System.Collections.Generic.List<Message> Messages { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ChatConversation()
        {
            Id = Guid.NewGuid().ToString();
            Messages = new System.Collections.Generic.List<Message>();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Agrega un mensaje a la conversación
        /// </summary>
        public void AddMessage(Message message)
        {
            Messages.Add(message);
            UpdatedAt = DateTime.UtcNow;

            // Actualizar título si es el primer mensaje del usuario
            if (string.IsNullOrEmpty(Title) && message.Role == "user")
            {
                Title = message.Content.Length > 50 
                    ? message.Content.Substring(0, 50) + "..." 
                    : message.Content;
            }
        }
    }
}
