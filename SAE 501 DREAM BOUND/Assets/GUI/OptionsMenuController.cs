using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace TomAg
{
    public class OptionsMenuController : MonoBehaviour
    {
        [SerializeField] private UIDocument _optionsDocument;

        private VisualElement _root;
        private DropdownField _resolutionDropdown;
        private Toggle _fullscreenToggle;
        private Toggle _vsyncToggle;
        private Slider _volumeSlider;
        private Slider _vivoxVolumeSlider;
        private Label _volumeLabel;
        private Label _vivoxVolumeLabel;
        private Button _applyButton;
        private Button _closeButton;

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
            _fullscreenToggle = _root.Q<Toggle>("fullscreen-toggle");
            _vsyncToggle = _root.Q<Toggle>("vsync-toggle");
            _volumeSlider = _root.Q<Slider>("game-volume");
            _vivoxVolumeSlider = _root.Q<Slider>("vivox-volume");
            _volumeLabel = _root.Q<Label>("volume-value");
            _vivoxVolumeLabel = _root.Q<Label>("vivox-volume-value");
            _applyButton = _root.Q<Button>("apply-settings");
            _closeButton = _root.Q<Button>("close-settings");
        }

        private void SetupResolutionOptions()
        {
            _resolutions = Screen.resolutions;
            var options = _resolutions.Select(res =>
                $"{res.width}x{res.height} @{res.refreshRate}Hz").ToList();
            _resolutionDropdown.choices = options;

            // Set current resolution
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
            _volumeSlider.RegisterValueChangedCallback(evt => {
                _volumeLabel.text = $"{(evt.newValue * 100):F0}%";
            });

            _vivoxVolumeSlider.RegisterValueChangedCallback(evt => {
                _vivoxVolumeLabel.text = $"{(evt.newValue * 100):F0}%";
            });

            _applyButton.clicked += ApplySettings;
            _closeButton.clicked += Hide;
        }

        private void ApplySettings()
        {
            // Apply Resolution
            string[] resParts = _resolutionDropdown.value.Split(new[] { 'x', '@' });
            if (resParts.Length >= 2)
            {
                int width = int.Parse(resParts[0]);
                int height = int.Parse(resParts[1].Split(' ')[0]);
                Screen.SetResolution(width, height, _fullscreenToggle.value);
            }

            // Apply Fullscreen
            Screen.fullScreen = _fullscreenToggle.value;

            // Apply VSync and Frame Rate settings
            if (_vsyncToggle.value)
            {
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1; // Let VSync control the frame rate
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 60; // Force 60 FPS when VSync is off
            }

            // Apply Volume
            AudioListener.volume = _volumeSlider.value;

            // Apply Vivox Volume
            float vivoxVolume = _vivoxVolumeSlider.value;
            PlayerPrefs.SetFloat("VivoxVolume", vivoxVolume);
            // Add your Vivox volume implementation here

            PlayerPrefs.Save();
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

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }

        private void OnDestroy()
        {
            if (_applyButton != null) _applyButton.clicked -= ApplySettings;
            if (_closeButton != null) _closeButton.clicked -= Hide;
        }
    }
}