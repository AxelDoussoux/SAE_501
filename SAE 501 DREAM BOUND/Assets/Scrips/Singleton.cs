using UnityEngine;

namespace Sphax
{
    public abstract class Singleton<T> : Singleton where T : MonoBehaviour
    {
        // STATIC

        private static T _instance;

        // PROPERTIES
        public static T instance
        {
            get
            {
                // Quit warning
                if (isQuitting && Application.isPlaying)
                {
                    if (_instance == null)
                        Debug.LogWarning($"{typeof(T)} instance should not be called while quitting");
                }

                // Return instance
                if (_instance != null)
                    return _instance;

                // No instance so Find one
                _instance = FindObjectOfType<T>(true);
                if (_instance != null)
                {
                    // Call OnInit which has not been called before (only at runtime)
                    if (Application.isPlaying)
                    {
                        var singleton = _instance as Singleton<T>;
                        singleton._onInitCalled = true;
                        singleton.OnInit();
                    }
                    return _instance;
                }

                // No Singleton found
                Debug.LogError($"No instance of {typeof(T)} found!");
                return null;
            }
        }

        /// <summary>
        /// Returns true if instance is not null and is not quitting
        /// </summary>
        public static bool isActive => _instance != null && !isQuitting;

        // PRIVATE

        private bool _onInitCalled = false;

        // ABSTRACT METHODS

        protected abstract void OnInit();
        protected abstract void OnDispose();

        // UNITY

        /// <summary>
        /// DO NOT OVERRIDE: Use <see cref="OnInit"/>
        /// </summary>
        protected void Start()
        {
            // Instance already initialized and different from this
            if (_instance != null && _instance.gameObject != gameObject)
            {
                Debug.Log($"Destroying <{name}> as doublon", this);
                Destroy(gameObject);
                return;
            }

            // Make Singleton Persistent
            if (_persistent)
            {
                Debug.Log($"Making <{name}> persistent", this);
                DontDestroyOnLoad(gameObject);
            }

            // Init Singleton Instance in Start (normal behaviour)
            if (_instance == null)
            {
                _onInitCalled = true;
                _instance = this as T;
                OnInit();
            }
        }

        /// <summary>
        /// DO NOT OVERRIDE: Use <see cref="OnDispose"/>
        /// </summary>
        protected void OnDisable()
        {
            if (_onInitCalled)
                OnDispose();
        }

        /// <summary>
        /// DO NOT OVERRIDE: Use <see cref="OnDispose"/>
        /// </summary>
        protected void OnDestroy() { }
    }

    public class Singleton : MonoBehaviour
    {
        // STATIC

        public static bool isQuitting { get; private set; }

        // PARAMS

        [SerializeField]
        protected bool _persistent = false;

        // PROPERTIES

        public bool isPersistent => _persistent;

        // UNITY

        protected virtual void Awake()
        {
            isQuitting = false;
        }

        /// <summary>
        /// DO NOT OVERRIDE: Use <see cref="OnAppQuit"/>
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            isQuitting = true;
            OnAppQuit();
        }

        /// <summary>
        /// Replace standard OnApplicationQuit() method in singleton
        /// </summary>
        protected virtual void OnAppQuit() { }
    }
}