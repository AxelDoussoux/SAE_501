using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace TomAg
{
    public class OptionsMenuController : MonoBehaviour
    {
        [SerializeField] private UIDocument _optionsDocument;
        [SerializeField] private PauseMenuController pauseMenuController;

        private VisualElement _root;
        private DropdownField _resolutionDropdown;
        private DropdownField _fpsDropdown;
        private Toggle _fullscreenToggle;
        private Toggle _vsyncToggle;
        private Slider _volumeSlider;
        private Slider _vivoxVolumeSlider;
        private Label _volumeLabel;
        private Label _vivoxVolumeLabel;
        private Button _closeButton;

        [SerializeField] private int targetFrameRate = 60;
        private Resolution[] _resolutions;

        public void Initialize()
        {
            if (_optionsDocument == null) return;

            _root = _optionsDocument.rootVisualElement;
            _root.style.display = DisplayStyle.None;

            InitializeUIElements();
            SetupResolutionOptions();
            LoadCurrentSettings();
            RegisterCallbacks();
        }

        private void InitializeUIElements()
        {
            _resolutionDropdown = _root.Q<DropdownField>("resolution-dropdown");
            _fpsDropdown = _root.Q<DropdownField>("fps-dropdown");
            _fullscreenToggle = _root.Q<Toggle>("fullscreen-toggle");
            _vsyncToggle = _root.Q<Toggle>("vsync-toggle");
            _volumeSlider = _root.Q<Slider>("game-volume");
            _vivoxVolumeSlider = _root.Q<Slider>("vivox-volume");
            _volumeLabel = _root.Q<Label>("volume-value");
            _vivoxVolumeLabel = _root.Q<Label>("vivox-volume-value");
            _closeButton = _root.Q<Button>("close-settings");
        }

        private void SetupResolutionOptions()
        {
            // Setup FPS options
            var fpsOptions = new List<string> { "30", "60", "120", "Unlimited" };
            _fpsDropdown.choices = fpsOptions;
            _fpsDropdown.value = "60"; // Default to 60 FPS

            // Setup resolutions
            _resolutions = Screen.resolutions;
            var options = _resolutions.Select(res =>
                $"{res.width}x{res.height} @{res.refreshRate}Hz").ToList();
            _resolutionDropdown.choices = options;

            Resolution currentResolution = Screen.currentResolution;
            string currentRes = $"{currentResolution.width}x{currentResolution.height} @{currentResolution.refreshRate}Hz";
            _resolutionDropdown.value = currentRes;
        }

        private void LoadCurrentSettings()
        {
            _fullscreenToggle.value = Screen.fullScreen;
            _vsyncToggle.value = QualitySettings.vSyncCount > 0;
            _volumeSlider.value = AudioListener.volume;
            _vivoxVolumeSlider.value = PlayerPrefs.GetFloat("VivoxVolume", 1.0f);

            UpdateLabels();
        }

        private void RegisterCallbacks()
        {
            _resolutionDropdown.RegisterValueChangedCallback(evt => ApplyResolution());
            _fullscreenToggle.RegisterValueChangedCallback(evt => ApplyFullscreen());
            _vsyncToggle.RegisterValueChangedCallback(evt => ApplyVSync());

            _volumeSlider.RegisterValueChangedCallback(evt => {
                AudioListener.volume = evt.newValue;
                _volumeLabel.text = $"{(evt.newValue * 100):F0}%";
            });

            _vivoxVolumeSlider.RegisterValueChangedCallback(evt => {
                float newValue = evt.newValue;
                PlayerPrefs.SetFloat("VivoxVolume", newValue);
                _vivoxVolumeLabel.text = $"{(newValue * 100):F0}%";
                PlayerPrefs.Save();
            });

            _fpsDropdown.RegisterValueChangedCallback(evt => ApplyFPSLimit());
            _closeButton.clicked += OnCloseClicked;
        }

        private void ApplyResolution()
        {
            string[] resParts = _resolutionDropdown.value.Split(new[] { 'x', '@' });
            if (resParts.Length >= 2)
            {
                int width = int.Parse(resParts[0]);
                int height = int.Parse(resParts[1].Split(' ')[0]);
                Screen.SetResolution(width, height, _fullscreenToggle.value);
            }
        }

        private void ApplyFullscreen()
        {
            Screen.fullScreen = _fullscreenToggle.value;
        }

        private void ApplyVSync()
        {
            if (_vsyncToggle.value)
            {
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1; // Let VSync control frame rate
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = targetFrameRate;
            }
        }

        private void ApplyFPSLimit()
        {
            string selectedValue = _fpsDropdown.value;
            if (selectedValue == "Unlimited")
            {
                Application.targetFrameRate = -1;
            }
            else
            {
                Application.targetFrameRate = int.Parse(selectedValue);
            }
        }

        private void UpdateLabels()
        {
            _volumeLabel.text = $"{(_volumeSlider.value * 100):F0}%";
            _vivoxVolumeLabel.text = $"{(_vivoxVolumeSlider.value * 100):F0}%";
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
            LoadCurrentSettings();
        }

        private void OnCloseClicked()
        {
            Hide();
            pauseMenuController.ShowPauseMenu();
        }

        public void Hide()
{
    _root.style.display = DisplayStyle.None;
    UnityEngine.Cursor.visible = false;
    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
}

        public bool IsVisible()
        {
            return _root.style.display == DisplayStyle.Flex;
        }

        private void OnDestroy()
        {
            if (_closeButton != null) _closeButton.clicked -= OnCloseClicked;
        }
    }
}
