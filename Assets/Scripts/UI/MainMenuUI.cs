using System.Collections;
using Load;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private float fadeDuration;

        [SerializeField] private CanvasGroup mainCanvasGroup;
        [SerializeField] private CanvasGroup creditsCanvasGroup;
        [SerializeField] private CanvasGroup settingsCanvasGroup;

        private CanvasGroup _currentCanvasGroup;

        public CanvasGroup CurrentCanvasGroup
        {
            get => _currentCanvasGroup;
            set
            {
                if (_currentCanvasGroup == value) return;

                StartCoroutine(FadeCanvasGroup(_currentCanvasGroup, 0, fadeDuration));
                _currentCanvasGroup = value;
                StartCoroutine(FadeCanvasGroup(_currentCanvasGroup, 1, fadeDuration));
            }
        }

        public void OnStart()
        {
            LoadManager.LoadScene(1, LoadSceneMode.Single);
        }

        public void OnBack()
        {
            CurrentCanvasGroup = creditsCanvasGroup;
        }

        public void OnCredits()
        {
            CurrentCanvasGroup = creditsCanvasGroup;
        }

        public void OnSettings()
        {
            CurrentCanvasGroup = settingsCanvasGroup;
        }

        public void OnExit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_STANDALONE
                Application.Quit();
            #elif UNITY_WEBGL
                Application.ExternalEval("window.close();");
            #endif
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
        {
            if (canvasGroup == null) yield break;

            var elapsedTime = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.gameObject.SetActive(true);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                var t = Mathf.Lerp(canvasGroup.alpha, targetAlpha, elapsedTime / duration);
                canvasGroup.alpha = t;
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            canvasGroup.interactable = targetAlpha.Equals(1f);
            canvasGroup.blocksRaycasts = targetAlpha.Equals(1f);
            if (targetAlpha == 0) canvasGroup.gameObject.SetActive(false);
        }
    }
}
