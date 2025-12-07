using UnityEngine;

namespace Lobot.Utils
{
    /// <summary>
    /// Puente para funcionalidades nativas de Android.
    /// </summary>
    public class AndroidBridge : MonoBehaviour
    {
        private static AndroidBridge instance;
        public static AndroidBridge Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("AndroidBridge");
                    instance = go.AddComponent<AndroidBridge>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private AndroidJavaObject currentActivity;
        private bool isAndroid;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAndroid();
        }

        private void InitializeAndroid()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            isAndroid = true;
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }
                Debug.Log("AndroidBridge initialized successfully");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error initializing AndroidBridge: {ex.Message}");
                isAndroid = false;
            }
#else
            isAndroid = false;
            Debug.Log("AndroidBridge: Not running on Android device");
#endif
        }

        /// <summary>
        /// Muestra un Toast nativo de Android.
        /// </summary>
        public void ShowToast(string message, bool longDuration = false)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!isAndroid || currentActivity == null) return;

            try
            {
                currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                    int duration = longDuration ? 1 : 0; // LENGTH_LONG = 1, LENGTH_SHORT = 0
                    
                    AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", 
                        currentActivity, 
                        message, 
                        duration
                    );
                    toast.Call("show");
                }));
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error showing toast: {ex.Message}");
            }
#else
            Debug.Log($"[Toast] {message}");
#endif
        }

        /// <summary>
        /// Verifica si hay conexión a internet.
        /// </summary>
        public bool IsNetworkAvailable()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!isAndroid || currentActivity == null) return false;

            try
            {
                AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                AndroidJavaObject connectivityManager = context.Call<AndroidJavaObject>(
                    "getSystemService", 
                    "connectivity"
                );
                AndroidJavaObject networkInfo = connectivityManager.Call<AndroidJavaObject>("getActiveNetworkInfo");
                
                if (networkInfo != null)
                {
                    return networkInfo.Call<bool>("isConnected");
                }
                return false;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error checking network: {ex.Message}");
                return false;
            }
#else
            return Application.internetReachability != NetworkReachability.NotReachable;
#endif
        }

        /// <summary>
        /// Obtiene el nivel de batería del dispositivo (0-100).
        /// </summary>
        public int GetBatteryLevel()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!isAndroid || currentActivity == null) return -1;

            try
            {
                AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter", "android.intent.action.BATTERY_CHANGED");
                AndroidJavaObject batteryIntent = context.Call<AndroidJavaObject>("registerReceiver", null, intentFilter);
                
                int level = batteryIntent.Call<int>("getIntExtra", "level", -1);
                int scale = batteryIntent.Call<int>("getIntExtra", "scale", -1);
                
                return (int)((level / (float)scale) * 100);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error getting battery level: {ex.Message}");
                return -1;
            }
#else
            return (int)(SystemInfo.batteryLevel * 100);
#endif
        }

        /// <summary>
        /// Abre una URL en el navegador.
        /// </summary>
        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }

        /// <summary>
        /// Hace vibrar el dispositivo.
        /// </summary>
        public void Vibrate(long milliseconds = 100)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!isAndroid || currentActivity == null) return;

            try
            {
                AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                AndroidJavaObject vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator");
                vibrator.Call("vibrate", milliseconds);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error vibrating: {ex.Message}");
            }
#else
            Handheld.Vibrate();
#endif
        }

        /// <summary>
        /// Comparte texto usando el share nativo de Android.
        /// </summary>
        public void ShareText(string text, string subject = "")
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!isAndroid || currentActivity == null) return;

            try
            {
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
                intent.Call<AndroidJavaObject>("setAction", "android.intent.action.SEND");
                intent.Call<AndroidJavaObject>("setType", "text/plain");
                intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.TEXT", text);
                
                if (!string.IsNullOrEmpty(subject))
                {
                    intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.SUBJECT", subject);
                }

                AndroidJavaObject chooser = new AndroidJavaClass("android.content.Intent")
                    .CallStatic<AndroidJavaObject>("createChooser", intent, "Compartir");
                
                currentActivity.Call("startActivity", chooser);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error sharing: {ex.Message}");
            }
#else
            Debug.Log($"[Share] {text}");
#endif
        }

        /// <summary>
        /// Obtiene el ID único del dispositivo.
        /// </summary>
        public string GetDeviceId()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        /// <summary>
        /// Mantiene la pantalla encendida.
        /// </summary>
        public void KeepScreenOn(bool keepOn)
        {
            Screen.sleepTimeout = keepOn ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
        }
    }
}
