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
        private DropdownField _qualityDropdown;
        private Toggle _fullscreenToggle;
        private Toggle _vsyncToggle;
        private Slider _volumeSlider;
        private Slider _vivoxVolumeSlider;
        private Label _volumeLabel;
        private Button _closeButton;

        [SerializeField] private int targetFrameRate = 60;
        private Resolution[] _resolutions;

        // Initialize the options menu, UI elements, and settings
        public void Initialize()
        {
            if (_optionsDocument == null) return;

            _root = _optionsDocument.rootVisualElement;
            _root.style.display = DisplayStyle.None;

            InitializeUIElements();
            SetupResolutionOptions();
            SetupQualityOptions();
            LoadCurrentSettings();
            RegisterCallbacks();
        }

        // Initialize all the UI elements in the options menu
        private void InitializeUIElements()
        {
            _resolutionDropdown = _root.Q<DropdownField>("resolution-dropdown");
            _fpsDropdown = _root.Q<DropdownField>("fps-dropdown");
            _qualityDropdown = _root.Q<DropdownField>("quality-dropdown");
            _fullscreenToggle = _root.Q<Toggle>("fullscreen-toggle");
            _vsyncToggle = _root.Q<Toggle>("vsync-toggle");
            _volumeSlider = _root.Q<Slider>("game-volume");
            _vivoxVolumeSlider = _root.Q<Slider>("vivox-volume");
            _volumeLabel = _root.Q<Label>("volume-value");
            _closeButton = _root.Q<Button>("close-settings");
        }

        // Set up the quality options dropdown
        private void SetupQualityOptions()
        {
            string[] qualityLevels = QualitySettings.names;
            _qualityDropdown.choices = qualityLevels.ToList();

            int currentQuality = QualitySettings.GetQualityLevel();
            _qualityDropdown.value = qualityLevels[currentQuality];
        }

        // Set up the resolution and FPS options dropdowns
        private void SetupResolutionOptions()
        {
            var fpsOptions = new List<string> { "30", "60", "120", "Unlimited" };
            _fpsDropdown.choices = fpsOptions;
            _fpsDropdown.value = "60"; // Default to 60 FPS

            _resolutions = Screen.resolutions;
            var options = _resolutions.Select(res =>
                $"{res.width}x{res.height}").ToList();
            _resolutionDropdown.choices = options;

            Resolution currentResolution = Screen.currentResolution;
            string currentRes = $"{currentResolution.width}x{currentResolution.height}";
            _resolutionDropdown.value = currentRes;
        }

        // Load the current settings for quality, volume, and fullscreen
        private void LoadCurrentSettings()
        {
            _fullscreenToggle.value = Screen.fullScreen;
            _vsyncToggle.value = QualitySettings.vSyncCount > 0;
            _volumeSlider.value = AudioListener.volume;

            string[] qualityLevels = QualitySettings.names;
            _qualityDropdown.value = qualityLevels[QualitySettings.GetQualityLevel()];

            UpdateLabels();
        }

        // Register callback methods for UI elements (dropdowns, sliders, buttons)
        private void RegisterCallbacks()
        {
            _resolutionDropdown.RegisterValueChangedCallback(evt => ApplyResolution());
            _fullscreenToggle.RegisterValueChangedCallback(evt => ApplyFullscreen());
            _vsyncToggle.RegisterValueChangedCallback(evt => ApplyVSync());
            _qualityDropdown.RegisterValueChangedCallback(evt => ApplyQualitySettings());

            _volumeSlider.RegisterValueChangedCallback(evt => {
                AudioListener.volume = evt.newValue;
                _volumeLabel.text = $"{(evt.newValue * 100):F0}%";
            });

            _fpsDropdown.RegisterValueChangedCallback(evt => ApplyFPSLimit());
            _closeButton.clicked += OnCloseClicked;
        }

        // Apply the selected quality settings
        private void ApplyQualitySettings()
        {
            string[] qualityLevels = QualitySettings.names;
            int selectedQuality = System.Array.IndexOf(qualityLevels, _qualityDropdown.value);
            if (selectedQuality >= 0)
            {
                QualitySettings.SetQualityLevel(selectedQuality, true);
            }
        }

        // Apply the selected resolution and fullscreen setting
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

        // Apply the fullscreen setting
        private void ApplyFullscreen()
        {
            Screen.fullScreen = _fullscreenToggle.value;
        }

        // Apply the VSync setting
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

        // Apply the selected FPS limit
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

        // Update the volume labels for the game and Vivox volume sliders
        private void UpdateLabels()
        {
            _volumeLabel.text = $"{(_volumeSlider.value * 100):F0}%";
        }

        // Show the options menu and load current settings
        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
            LoadCurrentSettings();
        }

        // Handle the close button click to hide the options menu and show the pause menu
        private void OnCloseClicked()
        {
            Hide();
            pauseMenuController.ShowPauseMenu();
        }

        // Hide the options menu and lock the cursor
        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }

        // Check if the options menu is currently visible
        public bool IsVisible()
        {
            return _root.style.display == DisplayStyle.Flex;
        }

        // Cleanup event listeners when the object is destroyed
        private void OnDestroy()
        {
            if (_closeButton != null) _closeButton.clicked -= OnCloseClicked;
        }
    }
}
