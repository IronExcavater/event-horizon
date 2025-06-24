using System.Collections;
using Load;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utilities;
using Utilities.Observables;

namespace UI
{
    public class LevelUI : MonoBehaviour
    {
        public readonly Property<float> Score = new();
        public readonly Property<float> Timer = new();

        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Pause Menu")]
        [SerializeField] private CanvasGroup pauseMenu;

        public readonly Property<bool> Paused = new();
        private PlayerInput _playerInput;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            _playerInput = FindAnyObjectByType<PlayerInput>();

            pauseMenu.alpha = 0;
            pauseMenu.blocksRaycasts = false;
            pauseMenu.interactable = false;

            if (scoreText != null) scoreText.text = Score.Value.ToString("F2");
            if (timerText != null) timerText.text = Timer.Value.FormatToTime();

            Score.AddListener((_, _) =>
            {
                if (scoreText != null) scoreText.text = Score.Value.ToString("F2");
            });

            Timer.AddListener((_, _) =>
            {
                if (timerText != null) timerText.text = Timer.Value.FormatToTime();
            });

            Paused.AddListener((_, change) =>
            {
                if (_playerInput != null)
                {
                    _playerInput.SwitchCurrentActionMap(change.NewValue ? "UI" : "Player");
                }

                if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = StartCoroutine(FadeCanvasGroup(pauseMenu, Paused.Value ? 1f : 0f, 1));
            });
        }

        private void Update()
        {
            if (_playerInput == null) FindAnyObjectByType<PlayerInput>();
            if (_playerInput != null && _playerInput.actions["Pause"].WasPressedThisFrame())
            {
                AudioManager.PlaySfxOneShot(AudioManager.Audio.click);
                OnPauseButton();
            }
        }

        public void OnPauseButton()
        {
            AudioManager.PlaySfxOneShot(AudioManager.Audio.click);
            Paused.Value = !Paused.Value;
        }

        public void OnExitButton()
        {
            AudioManager.PlaySfxOneShot(AudioManager.Audio.click);
            LoadManager.SaveNewScore(Level.GetScore, Level.GetTime);
            //Paused.Value = false;
            LoadManager.LoadScene(0, LoadSceneMode.Single);
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
        {
            var elapsedTime = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.gameObject.SetActive(true);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                var t = Mathf.Lerp(canvasGroup.alpha, targetAlpha, elapsedTime / duration);
                canvasGroup.alpha = t;
                Time.timeScale = -t + 1;
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            canvasGroup.interactable = targetAlpha.Equals(1f);
            canvasGroup.blocksRaycasts = targetAlpha.Equals(1f);
            if (targetAlpha == 0) canvasGroup.gameObject.SetActive(false);
            _fadeCoroutine = null;
        }
    }
}
