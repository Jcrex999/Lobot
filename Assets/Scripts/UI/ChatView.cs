using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lobot.Core;
using Lobot.Models;

namespace Lobot.UI
{
    /// <summary>
    /// Vista principal del chat.
    /// Muestra mensajes y gestiona la entrada del usuario.
    /// </summary>
    public class ChatView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform messageContainer;
        [SerializeField] private GameObject messageBubblePrefab;
        [SerializeField] private GameObject loadingIndicator;

        [Header("Colors")]
        [SerializeField] private Color userMessageColor = new Color(0.3f, 0.6f, 1f);
        [SerializeField] private Color assistantMessageColor = new Color(0.8f, 0.8f, 0.8f);
        [SerializeField] private Color systemMessageColor = new Color(0.9f, 0.7f, 0.3f);

        private ChatController chatController;

        private void Awake()
        {
            chatController = GetComponent<ChatController>();
            if (chatController == null)
            {
                chatController = gameObject.AddComponent<ChatController>();
            }

            SetupUI();
            SubscribeToEvents();
        }

        private void SetupUI()
        {
            // Configurar botón de envío
            sendButton.onClick.AddListener(OnSendButtonClicked);

            // Configurar campo de entrada
            inputField.onSubmit.AddListener((text) => OnSendButtonClicked());

            // Ocultar indicador de carga
            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(false);
            }
        }

        private void SubscribeToEvents()
        {
            chatController.OnMessageAdded += OnMessageAdded;
            chatController.OnStreamChunk += OnStreamChunk;
            chatController.OnGeneratingChanged += OnGeneratingChanged;
            chatController.OnError += OnError;
        }

        private void OnSendButtonClicked()
        {
            string message = inputField.text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            inputField.text = "";
            inputField.ActivateInputField();

            // Enviar mensaje (async)
            _ = chatController.SendMessageAsync(message);
        }

        private void OnMessageAdded(Message message)
        {
            CreateMessageBubble(message);
            ScrollToBottom();
        }

        private GameObject currentStreamingBubble = null;
        private TMP_Text currentStreamingText = null;

        private void OnStreamChunk(string chunk)
        {
            if (currentStreamingBubble == null)
            {
                // Crear burbuja para streaming
                var message = new Message("assistant", "");
                currentStreamingBubble = CreateMessageBubbleObject(message);
                currentStreamingText = currentStreamingBubble.GetComponentInChildren<TMP_Text>();
            }

            // Agregar chunk al texto
            if (currentStreamingText != null)
            {
                currentStreamingText.text += chunk;
            }

            ScrollToBottom();
        }

        private void OnGeneratingChanged(bool isGenerating)
        {
            sendButton.interactable = !isGenerating;
            inputField.interactable = !isGenerating;

            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(isGenerating);
            }

            // Resetear burbuja de streaming cuando termina
            if (!isGenerating)
            {
                currentStreamingBubble = null;
                currentStreamingText = null;
            }
        }

        private void OnError(string errorMessage)
        {
            // El error ya se agregó como mensaje del sistema
            Debug.LogError($"Chat error: {errorMessage}");
        }

        private void CreateMessageBubble(Message message)
        {
            // Si ya existe una burbuja de streaming, no crear otra
            if (message.Role == "assistant" && currentStreamingBubble != null)
            {
                return;
            }

            CreateMessageBubbleObject(message);
        }

        private GameObject CreateMessageBubbleObject(Message message)
        {
            if (messageBubblePrefab == null)
            {
                Debug.LogError("Message bubble prefab not assigned!");
                return null;
            }

            var bubbleObj = Instantiate(messageBubblePrefab, messageContainer);
            var bubble = bubbleObj.GetComponent<MessageBubble>();

            if (bubble != null)
            {
                bubble.SetMessage(message);
                bubble.SetColor(GetColorForRole(message.Role));
            }
            else
            {
                // Fallback si no hay componente MessageBubble
                var text = bubbleObj.GetComponentInChildren<TMP_Text>();
                if (text != null)
                {
                    text.text = message.Content;
                }

                var image = bubbleObj.GetComponent<Image>();
                if (image != null)
                {
                    image.color = GetColorForRole(message.Role);
                }
            }

            return bubbleObj;
        }

        private Color GetColorForRole(string role)
        {
            switch (role)
            {
                case "user":
                    return userMessageColor;
                case "assistant":
                    return assistantMessageColor;
                case "system":
                    return systemMessageColor;
                default:
                    return Color.white;
            }
        }

        private void ScrollToBottom()
        {
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        public void ClearChat()
        {
            chatController.ClearHistory();

            // Eliminar todas las burbujas de mensajes
            foreach (Transform child in messageContainer)
            {
                Destroy(child.gameObject);
            }

            currentStreamingBubble = null;
            currentStreamingText = null;
        }

        private void OnDestroy()
        {
            // Desuscribirse de eventos
            if (chatController != null)
            {
                chatController.OnMessageAdded -= OnMessageAdded;
                chatController.OnStreamChunk -= OnStreamChunk;
                chatController.OnGeneratingChanged -= OnGeneratingChanged;
                chatController.OnError -= OnError;
            }
        }
    }
}
