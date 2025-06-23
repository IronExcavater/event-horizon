using System;
using System.Collections;
using System.IO;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Load
{
    [DoNotDestroySingleton]
    public class LoadManager : Singleton<LoadManager>
    {
        [Header("Game Data")]
        public static Data GameData = new();
        public string gameDataPath = "/save.json";

        public static event Action OnDataSaved;
        public static event Action OnDataLoaded;

        public static event Action<Scene> OnSceneLoaded;
        public static event Action<float> OnSceneUnloaded; // float => buildIndex

        public static bool IsLoading;

        public class Data
        {
            public float MasterVolume;
            public float MusicVolume;
            public float SfxVolume;
            public bool VSync;
            public FullScreenMode FullScreenMode = FullScreenMode.FullScreenWindow;

            public float MaxScore;
            public float MaxTime;
        }

        public static void SaveNewScore(float score, float timeInSeconds)
        {
            if (score < GameData.MaxScore) return;
            GameData.MaxScore = score;
            GameData.MaxTime = timeInSeconds;
            SaveData();
        }

        protected override void Awake()
        {
            base.Awake();
            LoadData();

            OnSceneLoaded += scene => Debug.Log($"Loaded scene {scene.buildIndex}: {scene.name}");
            OnSceneUnloaded += buildIndex => Debug.Log($"Unloaded scene {buildIndex}");
        }

        public static void SaveData()
        {
            var json = JsonUtility.ToJson(GameData, true);
            File.WriteAllText(Application.persistentDataPath + Instance.gameDataPath, json);
            OnDataSaved?.Invoke();
        }

        private static void LoadData()
        {
            var path = Application.persistentDataPath + Instance.gameDataPath;
            if (!File.Exists(path)) return;

            var json = File.ReadAllText(path);
            GameData = JsonUtility.FromJson<Data>(json);
            OnDataLoaded?.Invoke();
        }

        public static void LoadScene(int buildIndex, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            Instance.StartCoroutine(Instance.LoadSceneCoroutine(buildIndex, mode));
        }

        private IEnumerator LoadSceneCoroutine(int buildIndex, LoadSceneMode loadMode = LoadSceneMode.Additive)
        {
            if (!IsValidBuildIndex(buildIndex)) yield break;
            IsLoading = true;
            Debug.Log($"Loading scene {buildIndex}");
            if (loadMode == LoadSceneMode.Single && TransitionUI.Instance != null)
                yield return TransitionUI.FadeTransition(false);

            yield return SceneManager.LoadSceneAsync(buildIndex, loadMode);
            var scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            IsLoading = false;
            OnSceneLoaded?.Invoke(scene);

            Debug.Log($"Loaded scene {buildIndex}");
            if (loadMode == LoadSceneMode.Single && TransitionUI.Instance != null)
                yield return TransitionUI.FadeTransition(true);
        }

        public static void UnloadScene(int buildIndex)
        {
            Instance.StartCoroutine(Instance.UnloadSceneCoroutine(buildIndex));
        }

        private IEnumerator UnloadSceneCoroutine(int buildIndex)
        {
            if (!IsValidBuildIndex(buildIndex)) yield break;
            Debug.Log($"Unloading scene {buildIndex}");
            yield return SceneManager.UnloadSceneAsync(buildIndex);
            OnSceneUnloaded?.Invoke(buildIndex);

            Debug.Log($"Unloaded scene {buildIndex}");
        }

        private static bool IsValidBuildIndex(int buildIndex)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            return !string.IsNullOrEmpty(path);
        }
    }
}
