using Controllers.Asset;
using Controllers.Functional;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.WorldLocking.Tools;
using TMPro;
using UI.Interactions;
using UnityEngine;
#if WINDOWS_UWP
using System;
#endif

namespace UI.Menus.Asset
{
    public class AssetTextMenu : AssetMenu
    {
        // Menu Buttons
        [SerializeField] private Interactable textButton;
        [SerializeField] private Interactable fillButton;
        [SerializeField] private Interactable orientationToggle;
        
        [Header("Controllers")]
        [SerializeField] private SystemKeyboardController systemKeyboardController;
        
        [Header("Sub Menus")]
        [SerializeField] private GameObject textMenu;
        [SerializeField] private GameObject fillSelectionMenu;
        
        [Header("Text Sub Menu")]
        [SerializeField] private Interactable changeTextButton;
        [SerializeField] private Interactable loadTextButton;
        [SerializeField] private Interactable italicCheckbox;
        [SerializeField] private Interactable boldCheckbox;
        [SerializeField] private Interactable serifCheckbox;
        [SerializeField] private PinchSlider widthSlider;

        [Header("Fill Sub Menu")]
        [SerializeField] private PinchSlider hue;
        [SerializeField] private PinchSlider saturation;
        [SerializeField] private PinchSlider value;

        public void Start()
        {
            // These click listeners are independent from the selected gameObject and therefore only need to be attached once
            textButton.OnClick.AddListener(ToggleTextOptions);
            fillButton.OnClick.AddListener(ToggleColorOptions);
        }

        protected override void ResetMenu()
        {
            SimpleConsole.AddLine(8, "something");
            base.ResetMenu();
            textMenu.SetActive(false);
            fillSelectionMenu.SetActive(false);
        }


        protected override void AddEventListeners()
        {
            base.AddEventListeners();
            var assetController = currentAsset.GetComponent<AssetTextController>();
            
            orientationToggle.OnClick.AddListener(() => ToggleOrientToUser(assetController));

            AddTextOptionsEventListeners();
            AddFillOptionsEventListeners();
        }

        private void ToggleOrientToUser(AssetTextController assetController)
        {
            assetController.FaceUser = !assetController.FaceUser;
        }


        private void AddTextOptionsEventListeners()
        {
            var assetController = currentAsset.GetComponent<AssetTextController>();
            changeTextButton.OnClick.AddListener(() => StartChangingText(assetController.Asset));

            boldCheckbox.IsToggled = assetController.Bold;
            boldCheckbox.OnClick.AddListener(() => assetController.Bold = !assetController.Bold);

            italicCheckbox.IsToggled = assetController.Italic;
            italicCheckbox.OnClick.AddListener(() => assetController.Italic = !assetController.Italic);

            serifCheckbox.IsToggled = assetController.Serif;
            serifCheckbox.OnClick.AddListener(() => assetController.Serif = !assetController.Serif);
            
            loadTextButton.OnClick.AddListener(() => OpenTxtFile(assetController));
            
            Debug.Log($"Rect width is {assetController.Asset.GetComponent<RectTransform>().rect.width} on instantiation.");
            widthSlider.SliderValue = assetController.Asset.GetComponent<RectTransform>().rect.width / 20 ;
            widthSlider.OnValueUpdated.AddListener(eventData => assetController.ChangeWidth(eventData.NewValue * 20));
        }
        
        private void AddFillOptionsEventListeners()
        {
            var assetController = currentAsset.GetComponent<AssetTextController>();
            
            // Get the objects current hsv value
            Color.RGBToHSV(assetController.Asset.GetComponent<TMP_Text>().color, H: out var h, S: out var s, V: out var v);
            
            // Add the change listeners to the color sliders and update their values to the current value
            hue.OnValueUpdated.AddListener(eventData => assetController.ChangeHue(eventData.NewValue));
            hue.SliderValue = h;

            saturation.OnValueUpdated.AddListener(eventData => assetController.ChangeSaturation(eventData.NewValue));
            saturation.SliderValue = s;

            value.OnValueUpdated.AddListener(eventData => assetController.ChangeValue(eventData.NewValue));
            value.SliderValue = v;
        }

        private void StartChangingText(GameObject asset)
        {
            systemKeyboardController.KeyboardTextChange = asset.GetComponent<KeyboardTextChange>();
            systemKeyboardController.OpenSystemKeyboard();
        }
        

        protected override void RemoveEventListeners()
        {
            base.RemoveEventListeners();
            RemoveTextOptionsEventListeners();
            RemoveTextColorSliderEventListeners();
        }

        private void RemoveTextOptionsEventListeners()
        {
            changeTextButton.OnClick.RemoveAllListeners();
            italicCheckbox.OnClick.RemoveAllListeners();
            boldCheckbox.OnClick.RemoveAllListeners();
            serifCheckbox.OnClick.RemoveAllListeners();
            widthSlider.OnValueUpdated.RemoveAllListeners();
        }

        private void RemoveTextColorSliderEventListeners()
        {
            hue.OnValueUpdated.RemoveAllListeners();
            saturation.OnValueUpdated.RemoveAllListeners();
            value.OnValueUpdated.RemoveAllListeners();
        }

        private void ToggleTextOptions()
        {
            textMenu.SetActive(!textMenu.activeSelf);
            fillSelectionMenu.SetActive(false);
        }

        private void ToggleColorOptions()
        {
            fillSelectionMenu.SetActive(!fillSelectionMenu.activeSelf);
            textMenu.SetActive(false);
        }

        

# if WINDOWS_UWP 
        private void OpenTxtFile(AssetTextController assetController)
        {
            var keyboardTextChange = assetController.Asset.GetComponent<KeyboardTextChange>();
            UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
            {
                SimpleConsole.AddLine(8, "Trying to load from .txt file..");
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                SimpleConsole.AddLine(8, "Created a text picker");
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                picker.FileTypeFilter.Add(".txt");
                var currentFile = await picker.PickSingleFileAsync();
                SimpleConsole.AddLine(8, "Opened file picker..");
                UnityEngine.WSA.Application.InvokeOnAppThread( async () =>
                {
                    var text = "";
                    if (currentFile != null)
                    {
                        // Application now has read/write access to the picked file
                        text = await Windows.Storage.FileIO.ReadTextAsync(currentFile);
                    }
                    keyboardTextChange.Text = text;
                }, true);
            }, false);
        }
#else
        private void OpenTxtFile(AssetTextController assetController)
        {
            Debug.Log(assetController);
            Debug.Log("Opening a .txt file only works on HoloLens");
        }
#endif
    }
}