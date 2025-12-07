using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lobot.API;
using Lobot.Models;

namespace Lobot.Core
{
    /// <summary>
    /// Controlador principal del chat.
    /// Gestiona la conversación, historial y comunicación con Ollama.
    /// </summary>
    public class ChatController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private string serverUrl = "http://localhost:11434";
        [SerializeField] private int maxHistorySize = 50;
        [SerializeField] private bool useStreaming = true;

        private OllamaClient client;
        private ChatConversation currentConversation;
        private ModelManager modelManager;
        private bool isGenerating = false;

        public event Action<Message> OnMessageAdded;
        public event Action<string> OnStreamChunk;
        public event Action<bool> OnGeneratingChanged;
        public event Action<string> OnError;

        public bool IsGenerating => isGenerating;

        private void Awake()
        {
            InitializeClient();
            modelManager = ModelManager.Instance;
            modelManager.Initialize(client);
            
            StartNewConversation();
        }

        /// <summary>
        /// Inicializa el cliente de Ollama.
        /// </summary>
        private void InitializeClient()
        {
            client = new OllamaClient(serverUrl);
            Debug.Log($"Ollama client initialized with URL: {serverUrl}");
        }

        /// <summary>
        /// Inicia una nueva conversación.
        /// </summary>
        public void StartNewConversation()
        {
            currentConversation = new ChatConversation();
            Debug.Log($"New conversation started: {currentConversation.Id}");
        }

        /// <summary>
        /// Envía un mensaje al chatbot.
        /// </summary>
        public async Task SendMessageAsync(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                Debug.LogWarning("Cannot send empty message");
                return;
            }

            if (isGenerating)
            {
                Debug.LogWarning("Already generating a response");
                return;
            }

            // Crear mensaje del usuario
            var userMessage = new Message("user", content);
            AddMessage(userMessage);

            // Generar respuesta
            await GenerateResponseAsync();
        }

        /// <summary>
        /// Genera una respuesta del asistente.
        /// </summary>
        private async Task GenerateResponseAsync()
        {
            SetGenerating(true);

            try
            {
                var modelName = modelManager.GetActiveModelName();
                
                if (useStreaming)
                {
                    await GenerateStreamingResponseAsync(modelName);
                }
                else
                {
                    await GenerateCompleteResponseAsync(modelName);
                }
            }
            catch (OllamaConnectionException ex)
            {
                HandleError($"Error de conexión: {ex.Message}");
            }
            catch (OllamaTimeoutException ex)
            {
                HandleError($"Tiempo de espera agotado: {ex.Message}");
            }
            catch (OllamaException ex)
            {
                HandleError($"Error de Ollama: {ex.Message}");
            }
            catch (Exception ex)
            {
                HandleError($"Error inesperado: {ex.Message}");
            }
            finally
            {
                SetGenerating(false);
            }
        }

        /// <summary>
        /// Genera respuesta completa (sin streaming).
        /// </summary>
        private async Task GenerateCompleteResponseAsync(string modelName)
        {
            var response = await client.ChatAsync(
                currentConversation.Messages, 
                modelName
            );

            var assistantMessage = new Message("assistant", response.Response);
            AddMessage(assistantMessage);

            Debug.Log($"Response generated in {response.TotalDuration / 1000000000f:F2}s");
        }

        /// <summary>
        /// Genera respuesta en streaming.
        /// </summary>
        private async Task GenerateStreamingResponseAsync(string modelName)
        {
            // Para streaming, convertir historial a un prompt
            var prompt = BuildPromptFromHistory();
            var responseBuilder = new System.Text.StringBuilder();

            await foreach (var chunk in client.StreamGenerateAsync(prompt, modelName))
            {
                responseBuilder.Append(chunk);
                OnStreamChunk?.Invoke(chunk);
            }

            var assistantMessage = new Message("assistant", responseBuilder.ToString());
            AddMessage(assistantMessage);
        }

        /// <summary>
        /// Construye un prompt a partir del historial de mensajes.
        /// </summary>
        private string BuildPromptFromHistory()
        {
            var builder = new System.Text.StringBuilder();
            
            foreach (var msg in currentConversation.Messages)
            {
                if (msg.Role == "user")
                {
                    builder.AppendLine($"Usuario: {msg.Content}");
                }
                else if (msg.Role == "assistant")
                {
                    builder.AppendLine($"Asistente: {msg.Content}");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Agrega un mensaje al historial.
        /// </summary>
        private void AddMessage(Message message)
        {
            currentConversation.AddMessage(message);
            OnMessageAdded?.Invoke(message);

            // Limitar tamaño del historial
            if (currentConversation.Messages.Count > maxHistorySize)
            {
                currentConversation.Messages.RemoveAt(0);
            }
        }

        /// <summary>
        /// Obtiene el historial de mensajes.
        /// </summary>
        public List<Message> GetChatHistory()
        {
            return new List<Message>(currentConversation.Messages);
        }

        /// <summary>
        /// Obtiene la conversación actual.
        /// </summary>
        public ChatConversation GetCurrentConversation()
        {
            return currentConversation;
        }

        /// <summary>
        /// Limpia el historial de chat.
        /// </summary>
        public void ClearHistory()
        {
            StartNewConversation();
            Debug.Log("Chat history cleared");
        }

        /// <summary>
        /// Cambia el modelo activo.
        /// </summary>
        public void SetModel(string modelName)
        {
            modelManager.SetActiveModel(modelName);
        }

        /// <summary>
        /// Cambia la URL del servidor.
        /// </summary>
        public void SetServerUrl(string url)
        {
            serverUrl = url;
            InitializeClient();
            modelManager.Initialize(client);
        }

        /// <summary>
        /// Verifica si el servidor está disponible.
        /// </summary>
        public async Task<bool> CheckServerConnectionAsync()
        {
            try
            {
                return await client.IsServerAvailableAsync();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Establece el estado de generación.
        /// </summary>
        private void SetGenerating(bool generating)
        {
            isGenerating = generating;
            OnGeneratingChanged?.Invoke(isGenerating);
        }

        /// <summary>
        /// Maneja errores.
        /// </summary>
        private void HandleError(string errorMessage)
        {
            Debug.LogError(errorMessage);
            OnError?.Invoke(errorMessage);

            // Agregar mensaje de error al chat
            var errorMsg = new Message("system", $"Error: {errorMessage}");
            AddMessage(errorMsg);
        }

        /// <summary>
        /// Obtiene el cliente de Ollama.
        /// </summary>
        public OllamaClient GetClient()
        {
            return client;
        }

        private void OnDestroy()
        {
            // Limpieza si es necesaria
        }
    }
}
