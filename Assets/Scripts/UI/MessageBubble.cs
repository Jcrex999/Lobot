using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lobot.Models;

namespace Lobot.UI
{
    /// <summary>
    /// Componente de burbuja de mensaje individual.
    /// </summary>
    public class MessageBubble : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private TMP_Text roleText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private LayoutElement layoutElement;

        [Header("Settings")]
        [SerializeField] private float maxWidth = 800f;
        [SerializeField] private float padding = 20f;

        private Message message;

        /// <summary>
        /// Establece el mensaje a mostrar.
        /// </summary>
        public void SetMessage(Message msg)
        {
            message = msg;
            UpdateUI();
        }

        /// <summary>
        /// Establece el color de fondo.
        /// </summary>
        public void SetColor(Color color)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = color;
            }
        }

        private void UpdateUI()
        {
            if (message == null) return;

            // Actualizar texto del mensaje
            if (messageText != null)
            {
                messageText.text = message.Content;
            }

            // Actualizar etiqueta de rol
            if (roleText != null)
            {
                roleText.text = GetRoleDisplayName(message.Role);
            }

            // Ajustar layout
            if (layoutElement != null)
            {
                layoutElement.preferredWidth = maxWidth;
            }
        }

        private string GetRoleDisplayName(string role)
        {
            switch (role)
            {
                case "user":
                    return "Tú";
                case "assistant":
                    return "Asistente";
                case "system":
                    return "Sistema";
                default:
                    return role;
            }
        }

        /// <summary>
        /// Obtiene el mensaje.
        /// </summary>
        public Message GetMessage()
        {
            return message;
        }
    }
}
