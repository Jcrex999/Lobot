using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

namespace Lobot.API
{
    /// <summary>
    /// Cliente principal para interactuar con el servidor Ollama.
    /// Soporta operaciones de generación, chat, streaming y gestión de modelos.
    /// </summary>
    public class OllamaClient
    {
        private readonly string baseUrl;
        private readonly HttpClient httpClient;
        private const int DefaultTimeout = 300; // 5 minutos

        /// <summary>
        /// Constructor del cliente Ollama.
        /// </summary>
        /// <param name="baseUrl">URL base del servidor Ollama (ej: http://localhost:11434)</param>
        public OllamaClient(string baseUrl)
        {
            this.baseUrl = baseUrl.TrimEnd('/');
            this.httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(DefaultTimeout)
            };
        }

        /// <summary>
        /// Genera una respuesta a un prompt sin contexto de conversación.
        /// </summary>
        /// <param name="prompt">Texto del prompt</param>
        /// <param name="model">Nombre del modelo a usar</param>
        /// <param name="options">Opciones de generación</param>
        /// <returns>Respuesta del modelo</returns>
        public async Task<OllamaResponse> GenerateAsync(
            string prompt, 
            string model, 
            GenerateOptions options = null)
        {
            try
            {
                var request = new
                {
                    model = model,
                    prompt = prompt,
                    stream = false,
                    options = options
                };

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{baseUrl}/api/generate", content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OllamaResponse>(responseJson);
            }
            catch (HttpRequestException ex)
            {
                throw new OllamaConnectionException($"Error connecting to Ollama server: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new OllamaTimeoutException("Request timed out", ex);
            }
            catch (Exception ex)
            {
                throw new OllamaException($"Unexpected error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Realiza un chat con contexto de conversación.
        /// </summary>
        /// <param name="messages">Historial de mensajes</param>
        /// <param name="model">Nombre del modelo a usar</param>
        /// <param name="options">Opciones de generación</param>
        /// <returns>Respuesta del modelo</returns>
        public async Task<OllamaResponse> ChatAsync(
            List<Message> messages, 
            string model, 
            GenerateOptions options = null)
        {
            try
            {
                var request = new
                {
                    model = model,
                    messages = messages,
                    stream = false,
                    options = options
                };

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{baseUrl}/api/chat", content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var chatResponse = JsonConvert.DeserializeObject<OllamaChatResponse>(responseJson);

                return new OllamaResponse
                {
                    Model = chatResponse.Model,
                    Response = chatResponse.Message.Content,
                    Done = chatResponse.Done,
                    TotalDuration = chatResponse.TotalDuration,
                    LoadDuration = chatResponse.LoadDuration,
                    PromptEvalCount = chatResponse.PromptEvalCount,
                    EvalCount = chatResponse.EvalCount
                };
            }
            catch (HttpRequestException ex)
            {
                throw new OllamaConnectionException($"Error connecting to Ollama server: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new OllamaTimeoutException("Request timed out", ex);
            }
            catch (Exception ex)
            {
                throw new OllamaException($"Unexpected error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Genera una respuesta en streaming (fragmentos en tiempo real).
        /// </summary>
        /// <param name="prompt">Texto del prompt</param>
        /// <param name="model">Nombre del modelo a usar</param>
        /// <param name="options">Opciones de generación</param>
        /// <returns>Enumerador asíncrono de fragmentos de texto</returns>
        public async IAsyncEnumerable<string> StreamGenerateAsync(
            string prompt, 
            string model, 
            GenerateOptions options = null)
        {
            var request = new
            {
                model = model,
                prompt = prompt,
                stream = true,
                options = options
            };

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{baseUrl}/api/generate", content);
            response.EnsureSuccessStatusCode();

            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new System.IO.StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        var chunk = JsonConvert.DeserializeObject<OllamaStreamResponse>(line);
                        if (!string.IsNullOrEmpty(chunk.Response))
                        {
                            yield return chunk.Response;
                        }

                        if (chunk.Done) break;
                    }
                    catch (JsonException)
                    {
                        // Ignorar líneas con JSON inválido
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene la lista de modelos disponibles en el servidor.
        /// </summary>
        /// <returns>Lista de modelos</returns>
        public async Task<List<OllamaModel>> GetModelsAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"{baseUrl}/api/tags");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<OllamaModelsResponse>(json);
                return result.Models;
            }
            catch (HttpRequestException ex)
            {
                throw new OllamaConnectionException($"Error connecting to Ollama server: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new OllamaException($"Error getting models: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Descarga un modelo del registro de Ollama.
        /// </summary>
        /// <param name="modelName">Nombre del modelo a descargar</param>
        /// <param name="onProgress">Callback para reportar progreso</param>
        public async Task PullModelAsync(string modelName, Action<PullProgress> onProgress = null)
        {
            try
            {
                var request = new { name = modelName, stream = true };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{baseUrl}/api/pull", content);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new System.IO.StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        try
                        {
                            var progress = JsonConvert.DeserializeObject<PullProgress>(line);
                            onProgress?.Invoke(progress);

                            if (progress.Status == "success") break;
                        }
                        catch (JsonException)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new OllamaConnectionException($"Error connecting to Ollama server: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new OllamaException($"Error pulling model: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un modelo del sistema local.
        /// </summary>
        /// <param name="modelName">Nombre del modelo a eliminar</param>
        public async Task DeleteModelAsync(string modelName)
        {
            try
            {
                var request = new { name = modelName };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"{baseUrl}/api/delete")
                {
                    Content = content
                };

                var response = await httpClient.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new OllamaConnectionException($"Error connecting to Ollama server: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new OllamaException($"Error deleting model: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica si el servidor está disponible.
        /// </summary>
        /// <returns>True si el servidor responde</returns>
        public async Task<bool> IsServerAvailableAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"{baseUrl}/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    #region Response Models

    [Serializable]
    public class OllamaResponse
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("response")]
        public string Response { get; set; }

        [JsonProperty("done")]
        public bool Done { get; set; }

        [JsonProperty("total_duration")]
        public long TotalDuration { get; set; }

        [JsonProperty("load_duration")]
        public long LoadDuration { get; set; }

        [JsonProperty("prompt_eval_count")]
        public int PromptEvalCount { get; set; }

        [JsonProperty("eval_count")]
        public int EvalCount { get; set; }
    }

    [Serializable]
    public class OllamaChatResponse
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("done")]
        public bool Done { get; set; }

        [JsonProperty("total_duration")]
        public long TotalDuration { get; set; }

        [JsonProperty("load_duration")]
        public long LoadDuration { get; set; }

        [JsonProperty("prompt_eval_count")]
        public int PromptEvalCount { get; set; }

        [JsonProperty("eval_count")]
        public int EvalCount { get; set; }
    }

    [Serializable]
    public class OllamaStreamResponse
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("response")]
        public string Response { get; set; }

        [JsonProperty("done")]
        public bool Done { get; set; }
    }

    [Serializable]
    public class OllamaModelsResponse
    {
        [JsonProperty("models")]
        public List<OllamaModel> Models { get; set; }
    }

    [Serializable]
    public class PullProgress
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("completed")]
        public long Completed { get; set; }

        public float Percent => Total > 0 ? (float)Completed / Total * 100f : 0f;
    }

    #endregion

    #region Exceptions

    public class OllamaException : Exception
    {
        public OllamaException(string message) : base(message) { }
        public OllamaException(string message, Exception inner) : base(message, inner) { }
    }

    public class OllamaConnectionException : OllamaException
    {
        public OllamaConnectionException(string message) : base(message) { }
        public OllamaConnectionException(string message, Exception inner) : base(message, inner) { }
    }

    public class OllamaTimeoutException : OllamaException
    {
        public OllamaTimeoutException(string message) : base(message) { }
        public OllamaTimeoutException(string message, Exception inner) : base(message, inner) { }
    }

    public class OllamaModelNotFoundException : OllamaException
    {
        public string ModelName { get; }

        public OllamaModelNotFoundException(string modelName) 
            : base($"Model '{modelName}' not found")
        {
            ModelName = modelName;
        }
    }

    #endregion
}
