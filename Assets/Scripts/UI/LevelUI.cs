using System.Collections;
using Load;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;
using Utilities.Observables;

namespace UI
{
    public class LevelUI : MonoBehaviour
    {
        public readonly Property<int> Score = new();
        public readonly Property<float> Timer = new();

        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Pause Menu")]
        [SerializeField] private CanvasGroup pauseMenu;
        [SerializeField] private Button pauseButton;

        public readonly Property<bool> Paused = new();
        private PlayerInput _playerInput;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            _playerInput = FindAnyObjectByType<PlayerInput>();

            pauseMenu.alpha = 0;
            pauseMenu.blocksRaycasts = false;
            pauseMenu.interactable = false;

            Score.AddListener((_, _) =>
            {
                if (scoreText != null) scoreText.text = $"Score: {Score.Value}";
            });

            Timer.AddListener((_, _) =>
            {
                if (timerText != null) timerText.text = Timer.Value.FormatToTime();
            });

            Paused.AddListener((_, change) =>
            {
                Cursor.lockState = change.NewValue ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = change.NewValue;

                if (_playerInput != null)
                {
                    _playerInput.SwitchCurrentActionMap(change.NewValue ? "UI" : "Player");
                }

                if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = StartCoroutine(FadeCoroutine(pauseMenu, Paused.Value ? 1f : 0f, 1));
            });
        }

        private void Update()
        {
            if (_playerInput.actions["Pause"].WasPressedThisFrame()) OnPauseButton();
        }

        public void OnPauseButton()
        {
            Paused.Value = !Paused.Value;
        }

        public void OnExitButton()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            LoadManager.LoadScene(LoadManager.MainMenuSceneIndex, LoadSceneMode.Single);
        }

        private IEnumerator FadeCoroutine(CanvasGroup canvasGroup, float targetAlpha, float duration)
        {
            var elapsedTime = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

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
            _fadeCoroutine = null;
        }
    }
}
