using UnityEngine;
using Lobot.Core;

namespace Lobot.Examples
{
    /// <summary>
    /// Ejemplo de gestión de modelos.
    /// </summary>
    public class ModelManagementExample : MonoBehaviour
    {
        private ModelManager modelManager;

        private void Start()
        {
            modelManager = ModelManager.Instance;
            
            // Suscribirse a eventos
            modelManager.OnModelChanged += OnModelChanged;
            modelManager.OnModelsUpdated += OnModelsUpdated;

            // Cargar modelos
            LoadModels();
        }

        private async void LoadModels()
        {
            Debug.Log("Cargando modelos disponibles...");
            
            try
            {
                var models = await modelManager.GetAvailableModelsAsync();
                
                Debug.Log($"Modelos disponibles: {models.Count}");
                foreach (var model in models)
                {
                    Debug.Log($"- {model.Name} ({model.GetFormattedSize()})");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error cargando modelos: {ex.Message}");
            }
        }

        private void Update()
        {
            // Presionar M para refrescar modelos
            if (Input.GetKeyDown(KeyCode.M))
            {
                RefreshModels();
            }

            // Presionar 1-9 para cambiar de modelo
            for (int i = 1; i <= 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    SelectModel(i - 1);
                }
            }
        }

        private async void RefreshModels()
        {
            Debug.Log("Refrescando modelos...");
            await modelManager.RefreshModelsAsync();
        }

        private async void SelectModel(int index)
        {
            var models = await modelManager.GetAvailableModelsAsync();
            
            if (index >= 0 && index < models.Count)
            {
                modelManager.SetActiveModel(models[index]);
            }
        }

        private void OnModelChanged(Models.OllamaModel model)
        {
            Debug.Log($"Modelo activo cambiado a: {model.Name}");
        }

        private void OnModelsUpdated(System.Collections.Generic.List<Models.OllamaModel> models)
        {
            Debug.Log($"Lista de modelos actualizada: {models.Count} modelos");
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
