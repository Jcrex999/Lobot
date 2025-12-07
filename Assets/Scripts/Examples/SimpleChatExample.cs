using UnityEngine;
using Lobot.Core;
using Lobot.Models;

namespace Lobot.Examples
{
    /// <summary>
    /// Ejemplo simple de uso del ChatController.
    /// </summary>
    public class SimpleChatExample : MonoBehaviour
    {
        [SerializeField] private string testMessage = "Explica qué es Unity en pocas palabras";
        
        private ChatController chatController;

        private void Start()
        {
            // Obtener o crear ChatController
            chatController = FindObjectOfType<ChatController>();
            if (chatController == null)
            {
                chatController = gameObject.AddComponent<ChatController>();
            }

            // Suscribirse a eventos
            chatController.OnMessageAdded += OnMessageReceived;
            chatController.OnError += OnError;
            chatController.OnGeneratingChanged += OnGeneratingChanged;
        }

        private void Update()
        {
            // Presionar Space para enviar mensaje de prueba
            if (Input.GetKeyDown(KeyCode.Space) && !chatController.IsGenerating)
            {
                SendTestMessage();
            }
        }

        private async void SendTestMessage()
        {
            Debug.Log($"Enviando mensaje: {testMessage}");
            await chatController.SendMessageAsync(testMessage);
        }

        private void OnMessageReceived(Message message)
        {
            Debug.Log($"[{message.Role}]: {message.Content}");
        }

        private void OnError(string error)
        {
            Debug.LogError($"Error en chat: {error}");
        }

        private void OnGeneratingChanged(bool isGenerating)
        {
            Debug.Log($"Generando: {isGenerating}");
        }

        private void OnDestroy()
        {
            if (chatController != null)
            {
                chatController.OnMessageAdded -= OnMessageReceived;
                chatController.OnError -= OnError;
                chatController.OnGeneratingChanged -= OnGeneratingChanged;
            }
        }
    }
}
