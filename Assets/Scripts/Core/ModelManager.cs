using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lobot.API;
using Lobot.Models;

namespace Lobot.Core
{
    /// <summary>
    /// Gestor singleton para modelos de Ollama.
    /// Maneja la selección, caché y actualización de modelos disponibles.
    /// </summary>
    public class ModelManager : MonoBehaviour
    {
        private static ModelManager instance;
        public static ModelManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("ModelManager");
                    instance = go.AddComponent<ModelManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        [SerializeField] private string defaultModel = "llama2";
        
        private OllamaClient client;
        private List<OllamaModel> availableModels = new List<OllamaModel>();
        private OllamaModel activeModel;
        private DateTime lastRefresh;
        private const int CacheExpirationMinutes = 5;

        public event Action<OllamaModel> OnModelChanged;
        public event Action<List<OllamaModel>> OnModelsUpdated;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Inicializa el gestor con un cliente de Ollama.
        /// </summary>
        public void Initialize(OllamaClient ollamaClient)
        {
            client = ollamaClient;
        }

        /// <summary>
        /// Obtiene la lista de modelos disponibles.
        /// Usa caché si es reciente, sino actualiza.
        /// </summary>
        public async Task<List<OllamaModel>> GetAvailableModelsAsync(bool forceRefresh = false)
        {
            if (client == null)
            {
                Debug.LogError("ModelManager not initialized. Call Initialize() first.");
                return new List<OllamaModel>();
            }

            // Usar caché si es válido
            if (!forceRefresh && availableModels.Count > 0)
            {
                var cacheAge = DateTime.Now - lastRefresh;
                if (cacheAge.TotalMinutes < CacheExpirationMinutes)
                {
                    return availableModels;
                }
            }

            // Actualizar modelos
            await RefreshModelsAsync();
            return availableModels;
        }

        /// <summary>
        /// Actualiza la lista de modelos desde el servidor.
        /// </summary>
        public async Task RefreshModelsAsync()
        {
            if (client == null)
            {
                Debug.LogError("ModelManager not initialized. Call Initialize() first.");
                return;
            }

            try
            {
                availableModels = await client.GetModelsAsync();
                lastRefresh = DateTime.Now;
                OnModelsUpdated?.Invoke(availableModels);

                Debug.Log($"Refreshed {availableModels.Count} models");

                // Establecer modelo activo si no hay uno
                if (activeModel == null && availableModels.Count > 0)
                {
                    var defaultModelObj = availableModels.Find(m => m.Name.Contains(defaultModel));
                    SetActiveModel(defaultModelObj ?? availableModels[0]);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error refreshing models: {ex.Message}");
            }
        }

        /// <summary>
        /// Establece el modelo activo por nombre.
        /// </summary>
        public void SetActiveModel(string modelName)
        {
            var model = availableModels.Find(m => m.Name == modelName);
            if (model != null)
            {
                SetActiveModel(model);
            }
            else
            {
                Debug.LogWarning($"Model '{modelName}' not found in available models");
            }
        }

        /// <summary>
        /// Establece el modelo activo.
        /// </summary>
        public void SetActiveModel(OllamaModel model)
        {
            if (model == null)
            {
                Debug.LogWarning("Attempted to set null model");
                return;
            }

            activeModel = model;
            OnModelChanged?.Invoke(activeModel);
            Debug.Log($"Active model set to: {activeModel.Name}");
        }

        /// <summary>
        /// Obtiene el modelo activo actual.
        /// </summary>
        public OllamaModel GetActiveModel()
        {
            return activeModel;
        }

        /// <summary>
        /// Obtiene el nombre del modelo activo.
        /// </summary>
        public string GetActiveModelName()
        {
            return activeModel?.Name ?? defaultModel;
        }

        /// <summary>
        /// Descarga un modelo nuevo.
        /// </summary>
        public async Task<bool> PullModelAsync(string modelName, Action<float> onProgress = null)
        {
            if (client == null)
            {
                Debug.LogError("ModelManager not initialized. Call Initialize() first.");
                return false;
            }

            try
            {
                await client.PullModelAsync(modelName, (progress) =>
                {
                    onProgress?.Invoke(progress.Percent);
                    Debug.Log($"Downloading {modelName}: {progress.Percent:F1}%");
                });

                // Actualizar lista de modelos
                await RefreshModelsAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error pulling model '{modelName}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un modelo.
        /// </summary>
        public async Task<bool> DeleteModelAsync(string modelName)
        {
            if (client == null)
            {
                Debug.LogError("ModelManager not initialized. Call Initialize() first.");
                return false;
            }

            try
            {
                await client.DeleteModelAsync(modelName);
                
                // Si era el modelo activo, cambiar a otro
                if (activeModel?.Name == modelName)
                {
                    await RefreshModelsAsync();
                    if (availableModels.Count > 0)
                    {
                        SetActiveModel(availableModels[0]);
                    }
                    else
                    {
                        activeModel = null;
                    }
                }
                else
                {
                    await RefreshModelsAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error deleting model '{modelName}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si un modelo está disponible.
        /// </summary>
        public bool IsModelAvailable(string modelName)
        {
            return availableModels.Exists(m => m.Name == modelName);
        }

        /// <summary>
        /// Obtiene información de un modelo por nombre.
        /// </summary>
        public OllamaModel GetModelInfo(string modelName)
        {
            return availableModels.Find(m => m.Name == modelName);
        }
    }
}
