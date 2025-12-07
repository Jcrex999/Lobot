using UnityEngine;
using Lobot.API;
using System.Threading.Tasks;

namespace Lobot.Examples
{
    /// <summary>
    /// Ejemplo de uso de streaming con Ollama.
    /// </summary>
    public class StreamingExample : MonoBehaviour
    {
        [SerializeField] private string serverUrl = "http://localhost:11434";
        [SerializeField] private string modelName = "llama2";
        [SerializeField] private string prompt = "Escribe un poema corto sobre la programación";

        private OllamaClient client;

        private void Start()
        {
            client = new OllamaClient(serverUrl);
        }

        private void Update()
        {
            // Presionar S para iniciar streaming
            if (Input.GetKeyDown(KeyCode.S))
            {
                StartStreaming();
            }
        }

        private async void StartStreaming()
        {
            Debug.Log("Iniciando streaming...");
            var fullResponse = "";

            try
            {
                await foreach (var chunk in client.StreamGenerateAsync(prompt, modelName))
                {
                    fullResponse += chunk;
                    Debug.Log($"Chunk: {chunk}");
                }

                Debug.Log($"\nRespuesta completa:\n{fullResponse}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error: {ex.Message}");
            }
        }
    }
}
