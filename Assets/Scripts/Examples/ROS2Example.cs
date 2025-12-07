using UnityEngine;
using Lobot.ROS2;

namespace Lobot.Examples
{
    /// <summary>
    /// Ejemplo de integración con ROS2.
    /// </summary>
    public class ROS2Example : MonoBehaviour
    {
        private ROS2Bridge ros2Bridge;

        private void Start()
        {
            // Crear y configurar bridge
            ros2Bridge = gameObject.AddComponent<ROS2Bridge>();
            
            // Suscribirse a comandos externos
            ros2Bridge.SubscribeToExternalCommands(OnExternalCommand);

            Debug.Log("ROS2 Bridge configurado. Presiona P para publicar mensaje de prueba.");
        }

        private void Update()
        {
            // Presionar P para publicar mensaje
            if (Input.GetKeyDown(KeyCode.P))
            {
                PublishTestMessage();
            }

            // Presionar C para publicar comando
            if (Input.GetKeyDown(KeyCode.C))
            {
                PublishCommand();
            }
        }

        private void PublishTestMessage()
        {
            string message = "Hola desde Lobot Unity!";
            ros2Bridge.PublishChatResponse(message);
            Debug.Log($"Mensaje publicado a ROS2: {message}");
        }

        private void PublishCommand()
        {
            string command = "move_forward";
            ros2Bridge.PublishCommand(command);
            Debug.Log($"Comando publicado a ROS2: {command}");
        }

        private void OnExternalCommand(string command)
        {
            Debug.Log($"Comando recibido de ROS2: {command}");
            
            // Procesar comando
            switch (command.ToLower())
            {
                case "start_chat":
                    Debug.Log("Iniciando chat...");
                    break;
                case "stop_chat":
                    Debug.Log("Deteniendo chat...");
                    break;
                default:
                    Debug.Log($"Comando desconocido: {command}");
                    break;
            }
        }
    }
}
