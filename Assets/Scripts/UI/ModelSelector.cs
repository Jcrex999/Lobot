using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lobot.Core;
using Lobot.Models;

namespace Lobot.UI
{
    /// <summary>
    /// Selector de modelos de Ollama.
    /// Permite al usuario elegir entre modelos disponibles.
    /// </summary>
    public class ModelSelector : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Dropdown modelDropdown;
        [SerializeField] private Button refreshButton;
        [SerializeField] private TMP_Text currentModelText;
        [SerializeField] private TMP_Text modelInfoText;

        private ModelManager modelManager;
        private List<OllamaModel> availableModels = new List<OllamaModel>();

        private void Start()
        {
            modelManager = ModelManager.Instance;
            
            SetupUI();
            SubscribeToEvents();
            RefreshModels();
        }

        private void SetupUI()
        {
            if (modelDropdown != null)
            {
                modelDropdown.onValueChanged.AddListener(OnModelSelected);
            }

            if (refreshButton != null)
            {
                refreshButton.onClick.AddListener(RefreshModels);
            }
        }

        private void SubscribeToEvents()
        {
            modelManager.OnModelChanged += OnModelChanged;
            modelManager.OnModelsUpdated += OnModelsUpdated;
        }

        private async void RefreshModels()
        {
            if (refreshButton != null)
            {
                refreshButton.interactable = false;
            }

            try
            {
                availableModels = await modelManager.GetAvailableModelsAsync(forceRefresh: true);
                UpdateDropdown();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error refreshing models: {ex.Message}");
            }
            finally
            {
                if (refreshButton != null)
                {
                    refreshButton.interactable = true;
                }
            }
        }

        private void UpdateDropdown()
        {
            if (modelDropdown == null) return;

            modelDropdown.ClearOptions();

            var options = new List<string>();
            foreach (var model in availableModels)
            {
                options.Add($"{model.Name} ({model.GetFormattedSize()})");
            }

            modelDropdown.AddOptions(options);

            // Seleccionar modelo activo actual
            var activeModel = modelManager.GetActiveModel();
            if (activeModel != null)
            {
                int index = availableModels.FindIndex(m => m.Name == activeModel.Name);
                if (index >= 0)
                {
                    modelDropdown.SetValueWithoutNotify(index);
                }
            }
        }

        private void OnModelSelected(int index)
        {
            if (index < 0 || index >= availableModels.Count) return;

            var selectedModel = availableModels[index];
            modelManager.SetActiveModel(selectedModel);
        }

        private void OnModelChanged(OllamaModel model)
        {
            UpdateCurrentModelDisplay(model);
            UpdateModelInfo(model);
        }

        private void OnModelsUpdated(List<OllamaModel> models)
        {
            availableModels = models;
            UpdateDropdown();
        }

        private void UpdateCurrentModelDisplay(OllamaModel model)
        {
            if (currentModelText != null && model != null)
            {
                currentModelText.text = $"Modelo actual: {model.Name}";
            }
        }

        private void UpdateModelInfo(OllamaModel model)
        {
            if (modelInfoText == null || model == null) return;

            var info = new System.Text.StringBuilder();
            info.AppendLine($"Nombre: {model.Name}");
            info.AppendLine($"Tamaño: {model.GetFormattedSize()}");
            
            if (model.Details != null)
            {
                if (!string.IsNullOrEmpty(model.Details.Family))
                {
                    info.AppendLine($"Familia: {model.Details.Family}");
                }
                if (!string.IsNullOrEmpty(model.Details.ParameterSize))
                {
                    info.AppendLine($"Parámetros: {model.Details.ParameterSize}");
                }
            }

            modelInfoText.text = info.ToString();
        }

        private void OnDestroy()
        {
            if (modelManager != null)
            {
                modelManager.OnModelChanged -= OnModelChanged;
                modelManager.OnModelsUpdated -= OnModelsUpdated;
            }
        }
    }
}
