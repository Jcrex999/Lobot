using UnityEngine;

namespace Lobot.Utils
{
    /// <summary>
    /// Utilidades de networking y conectividad.
    /// </summary>
    public static class NetworkUtils
    {
        /// <summary>
        /// Verifica si hay conexión a internet.
        /// </summary>
        public static bool IsInternetAvailable()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var bridge = AndroidBridge.Instance;
            if (bridge != null)
            {
                return bridge.IsNetworkAvailable();
            }
            return Application.internetReachability != NetworkReachability.NotReachable;
#else
            return Application.internetReachability != NetworkReachability.NotReachable;
#endif
        }

        /// <summary>
        /// Obtiene el tipo de conexión actual.
        /// </summary>
        public static string GetConnectionType()
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return "Datos móviles";
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return "WiFi";
                case NetworkReachability.NotReachable:
                default:
                    return "Sin conexión";
            }
        }

        /// <summary>
        /// Valida una URL.
        /// </summary>
        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return System.Uri.TryCreate(url, System.UriKind.Absolute, out System.Uri uriResult)
                && (uriResult.Scheme == System.Uri.UriSchemeHttp || uriResult.Scheme == System.Uri.UriSchemeHttps);
        }

        /// <summary>
        /// Normaliza una URL del servidor Ollama.
        /// </summary>
        public static string NormalizeServerUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "http://localhost:11434";

            url = url.Trim();

            // Agregar protocolo si falta
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "http://" + url;
            }

            // Remover trailing slash
            url = url.TrimEnd('/');

            return url;
        }
    }

    /// <summary>
    /// Utilidades para formateo de texto.
    /// </summary>
    public static class TextUtils
    {
        /// <summary>
        /// Trunca un texto a una longitud máxima.
        /// </summary>
        public static string Truncate(string text, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - suffix.Length) + suffix;
        }

        /// <summary>
        /// Cuenta las palabras en un texto.
        /// </summary>
        public static int CountWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            return text.Split(new char[] { ' ', '\t', '\n', '\r' }, 
                System.StringSplitOptions.RemoveEmptyEntries).Length;
        }

        /// <summary>
        /// Estima el tiempo de lectura en minutos.
        /// </summary>
        public static int EstimateReadingTime(string text, int wordsPerMinute = 200)
        {
            int wordCount = CountWords(text);
            return Mathf.Max(1, Mathf.CeilToInt((float)wordCount / wordsPerMinute));
        }

        /// <summary>
        /// Limpia espacios en blanco excesivos.
        /// </summary>
        public static string CleanWhitespace(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
        }
    }

    /// <summary>
    /// Utilidades para formateo de tamaños de archivo.
    /// </summary>
    public static class SizeUtils
    {
        private static readonly string[] SizeUnits = { "B", "KB", "MB", "GB", "TB" };

        /// <summary>
        /// Formatea un tamaño en bytes a formato legible.
        /// </summary>
        public static string FormatBytes(long bytes)
        {
            if (bytes == 0) return "0 B";

            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < SizeUnits.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {SizeUnits[order]}";
        }
    }

    /// <summary>
    /// Utilidades para formateo de tiempo.
    /// </summary>
    public static class TimeUtils
    {
        /// <summary>
        /// Formatea duración en nanosegundos a formato legible.
        /// </summary>
        public static string FormatDuration(long nanoseconds)
        {
            var seconds = nanoseconds / 1_000_000_000.0;

            if (seconds < 1)
                return $"{(nanoseconds / 1_000_000.0):F0} ms";
            else if (seconds < 60)
                return $"{seconds:F1} s";
            else if (seconds < 3600)
                return $"{(seconds / 60):F1} min";
            else
                return $"{(seconds / 3600):F1} h";
        }

        /// <summary>
        /// Obtiene timestamp Unix en segundos.
        /// </summary>
        public static long GetUnixTimestamp()
        {
            return System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Convierte timestamp Unix a DateTime.
        /// </summary>
        public static System.DateTime FromUnixTimestamp(long timestamp)
        {
            return System.DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
        }

        /// <summary>
        /// Formatea tiempo relativo (ej: "hace 5 minutos").
        /// </summary>
        public static string GetRelativeTime(System.DateTime dateTime)
        {
            var span = System.DateTime.UtcNow - dateTime;

            if (span.TotalSeconds < 60)
                return "hace unos segundos";
            else if (span.TotalMinutes < 60)
                return $"hace {(int)span.TotalMinutes} minuto{(span.TotalMinutes >= 2 ? "s" : "")}";
            else if (span.TotalHours < 24)
                return $"hace {(int)span.TotalHours} hora{(span.TotalHours >= 2 ? "s" : "")}";
            else if (span.TotalDays < 30)
                return $"hace {(int)span.TotalDays} día{(span.TotalDays >= 2 ? "s" : "")}";
            else if (span.TotalDays < 365)
                return $"hace {(int)(span.TotalDays / 30)} mes{(span.TotalDays >= 60 ? "es" : "")}";
            else
                return $"hace {(int)(span.TotalDays / 365)} año{(span.TotalDays >= 730 ? "s" : "")}";
        }
    }

    /// <summary>
    /// Pool de objetos para optimizar instanciación.
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        private readonly T prefab;
        private readonly Transform parent;
        private readonly System.Collections.Generic.Queue<T> pool;
        private readonly int initialSize;

        public ObjectPool(T prefab, Transform parent, int initialSize = 10)
        {
            this.prefab = prefab;
            this.parent = parent;
            this.initialSize = initialSize;
            this.pool = new System.Collections.Generic.Queue<T>();

            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialSize; i++)
            {
                var obj = Object.Instantiate(prefab, parent);
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            T obj;
            
            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else
            {
                obj = Object.Instantiate(prefab, parent);
            }

            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }

        public void Clear()
        {
            while (pool.Count > 0)
            {
                var obj = pool.Dequeue();
                if (obj != null)
                    Object.Destroy(obj.gameObject);
            }
        }
    }
}
