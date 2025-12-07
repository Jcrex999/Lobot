using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lobot.Core;

namespace Lobot.UI
{
    /// <summary>
    /// Panel de configuración de la aplicación.
    /// </summary>
    public class SettingsPanel : MonoBehaviour
    {
        [Header("Server Configuration")]
        [SerializeField] private TMP_InputField serverUrlInput;
        [SerializeField] private Toggle useStreamingToggle;
        [SerializeField] private Button testConnectionButton;
        [SerializeField] private TMP_Text connectionStatusText;

        [Header("Generate Options")]
        [SerializeField] private Slider temperatureSlider;
        [SerializeField] private TMP_Text temperatureValueText;
        [SerializeField] private Slider topPSlider;
        [SerializeField] private TMP_Text topPValueText;
        [SerializeField] private TMP_InputField maxTokensInput;

        [Header("UI Settings")]
        [SerializeField] private Toggle keepScreenOnToggle;
        [SerializeField] private Toggle allowVibrationToggle;

        [Header("Buttons")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button resetButton;

        private ChatController chatController;
        private string originalServerUrl;

        private void Start()
        {
            chatController = FindObjectOfType<ChatController>();
            SetupUI();
            LoadSettings();
        }

        private void SetupUI()
        {
            if (saveButton != null)
                saveButton.onClick.AddListener(OnSaveSettings);

            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancel);

            if (resetButton != null)
                resetButton.onClick.AddListener(OnReset);

            if (testConnectionButton != null)
                testConnectionButton.onClick.AddListener(OnTestConnection);

            if (temperatureSlider != null)
            {
                temperatureSlider.onValueChanged.AddListener((value) =>
                {
                    if (temperatureValueText != null)
                        temperatureValueText.text = value.ToString("F2");
                });
            }

            if (topPSlider != null)
            {
                topPSlider.onValueChanged.AddListener((value) =>
                {
                    if (topPValueText != null)
                        topPValueText.text = value.ToString("F2");
                });
            }
        }

        private void LoadSettings()
        {
            // Cargar configuración desde PlayerPrefs
            if (serverUrlInput != null)
            {
                originalServerUrl = PlayerPrefs.GetString("ServerUrl", "http://localhost:11434");
                serverUrlInput.text = originalServerUrl;
            }

            if (useStreamingToggle != null)
                useStreamingToggle.isOn = PlayerPrefs.GetInt("UseStreaming", 1) == 1;

            if (temperatureSlider != null)
            {
                temperatureSlider.value = PlayerPrefs.GetFloat("Temperature", 0.8f);
                if (temperatureValueText != null)
                    temperatureValueText.text = temperatureSlider.value.ToString("F2");
            }

            if (topPSlider != null)
            {
                topPSlider.value = PlayerPrefs.GetFloat("TopP", 0.9f);
                if (topPValueText != null)
                    topPValueText.text = topPSlider.value.ToString("F2");
            }

            if (maxTokensInput != null)
                maxTokensInput.text = PlayerPrefs.GetInt("MaxTokens", 2048).ToString();

            if (keepScreenOnToggle != null)
                keepScreenOnToggle.isOn = PlayerPrefs.GetInt("KeepScreenOn", 0) == 1;

            if (allowVibrationToggle != null)
                allowVibrationToggle.isOn = PlayerPrefs.GetInt("AllowVibration", 1) == 1;
        }

        private void OnSaveSettings()
        {
            // Guardar configuración en PlayerPrefs
            if (serverUrlInput != null)
            {
                PlayerPrefs.SetString("ServerUrl", serverUrlInput.text);
                
                // Actualizar URL del servidor si cambió
                if (chatController != null && serverUrlInput.text != originalServerUrl)
                {
                    chatController.SetServerUrl(serverUrlInput.text);
                }
            }

            if (useStreamingToggle != null)
                PlayerPrefs.SetInt("UseStreaming", useStreamingToggle.isOn ? 1 : 0);

            if (temperatureSlider != null)
                PlayerPrefs.SetFloat("Temperature", temperatureSlider.value);

            if (topPSlider != null)
                PlayerPrefs.SetFloat("TopP", topPSlider.value);

            if (maxTokensInput != null)
            {
                if (int.TryParse(maxTokensInput.text, out int maxTokens))
                {
                    PlayerPrefs.SetInt("MaxTokens", maxTokens);
                }
            }

            if (keepScreenOnToggle != null)
            {
                PlayerPrefs.SetInt("KeepScreenOn", keepScreenOnToggle.isOn ? 1 : 0);
                Screen.sleepTimeout = keepScreenOnToggle.isOn ? 
                    SleepTimeout.NeverSleep : 
                    SleepTimeout.SystemSetting;
            }

            if (allowVibrationToggle != null)
                PlayerPrefs.SetInt("AllowVibration", allowVibrationToggle.isOn ? 1 : 0);

            PlayerPrefs.Save();

            Debug.Log("Settings saved");
            gameObject.SetActive(false);
        }

        private void OnCancel()
        {
            LoadSettings(); // Recargar configuración original
            gameObject.SetActive(false);
        }

        private void OnReset()
        {
            // Restablecer valores por defecto
            if (serverUrlInput != null)
                serverUrlInput.text = "http://localhost:11434";

            if (useStreamingToggle != null)
                useStreamingToggle.isOn = true;

            if (temperatureSlider != null)
            {
                temperatureSlider.value = 0.8f;
                if (temperatureValueText != null)
                    temperatureValueText.text = "0.80";
            }

            if (topPSlider != null)
            {
                topPSlider.value = 0.9f;
                if (topPValueText != null)
                    topPValueText.text = "0.90";
            }

            if (maxTokensInput != null)
                maxTokensInput.text = "2048";

            if (keepScreenOnToggle != null)
                keepScreenOnToggle.isOn = false;

            if (allowVibrationToggle != null)
                allowVibrationToggle.isOn = true;
        }

        private async void OnTestConnection()
        {
            if (chatController == null)
            {
                if (connectionStatusText != null)
                {
                    connectionStatusText.text = "✗ ChatController no encontrado";
                    connectionStatusText.color = Color.red;
                }
                return;
            }

            if (connectionStatusText == null) return;

            if (testConnectionButton != null)
                testConnectionButton.interactable = false;

            connectionStatusText.text = "Probando conexión...";
            connectionStatusText.color = Color.yellow;

            bool isConnected = await chatController.CheckServerConnectionAsync();

            if (isConnected)
            {
                connectionStatusText.text = "✓ Conexión exitosa";
                connectionStatusText.color = Color.green;
            }
            else
            {
                connectionStatusText.text = "✗ Error de conexión";
                connectionStatusText.color = Color.red;
            }

            if (testConnectionButton != null)
                testConnectionButton.interactable = true;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            LoadSettings();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
