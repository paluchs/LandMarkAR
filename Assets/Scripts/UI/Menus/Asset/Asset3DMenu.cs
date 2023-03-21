using System;
using System.Collections.Generic;
using Controllers.Asset;
using Controllers.Asset.Enums;
using Controllers.Functional;
using Managers;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.WorldLocking.Tools;
using UnityEngine;

namespace UI.Menus.Asset
{
    public class Asset3DMenu : AssetAnimateableMenu
    {
        // Menu Buttons
        [SerializeField] private Interactable shapeButton;
        [SerializeField] private Interactable fillButton;
        
        
        
        [Header("Sub Menus")]
        [SerializeField] private GameObject shapeSelectionMenu;
        [SerializeField] private GameObject appearanceMenu;

        [Header("Shape Sub Menu")]
        [SerializeField] private GridObjectCollection shapeSelectionGridObjectCollection;
        private List<GameObject> _shapePreviews;
        
        [Header("Appearance Sub Menu")]
        [SerializeField] private PinchSlider hue;
        [SerializeField] private PinchSlider saturation;
        [SerializeField] private PinchSlider value;
        [SerializeField] private PinchSlider alpha;
        [SerializeField] private Interactable regularRadioButton;
        [SerializeField] private Interactable outlinedRadioButton;
        [SerializeField] private Interactable glowRadioButton;
        


        // Info
        private bool _shapeMenuLoaded;

        public new void Start()
        {
            base.Start();
            // These click listeners are independent from the selected gameObject and therefore only need to be attached once
            shapeButton.OnClick.AddListener(ShowShapeOptions);
            fillButton.OnClick.AddListener(ShowFillOptions);
        }

        public override void OpenMenu(GameObject selectedAsset3D)
        {
            LoadShapeMenu();
            base.OpenMenu(selectedAsset3D);
        }

        protected override void ResetMenu()
        {
            base.ResetMenu();
            shapeSelectionMenu.SetActive(false);
            appearanceMenu.SetActive(false);
        }


        // -------------- Event Listeners Add ---------------
        protected override void AddEventListeners()
        {
            SimpleConsole.AddLine(8, "Experiment3DAssetMenu.AddEventListeners called");
            base.AddEventListeners();

            AddFillColorSliderEventListeners();
            AddMaterialSelectionEventListeners();
            AddShapeSelectionClickListeners();
            SimpleConsole.AddLine(8, "Experiment3DAssetMenu.AddEventListeners finished");
        }


        private void AddFillColorSliderEventListeners()
        {
            var asset3DController = currentAsset.GetComponent<Asset3DController>();
            
            // Get the objects current hsv value
            Color.RGBToHSV(asset3DController.Asset.GetComponent<Renderer>().material.color, H: out var h, S: out var s, V: out var v);
            // Get the objects current alpha value
            float a = asset3DController.Asset.GetComponent<Renderer>().material.color.a;

            // Add the change listeners to the color sliders and update their values to the current value
            hue.OnValueUpdated.AddListener(eventData => asset3DController.ChangeHue(eventData.NewValue));
            hue.SliderValue = h;

            saturation.OnValueUpdated.AddListener(eventData => asset3DController.ChangeSaturation(eventData.NewValue));
            saturation.SliderValue = s;

            value.OnValueUpdated.AddListener(eventData => asset3DController.ChangeValue(eventData.NewValue));
            value.SliderValue = v;

            alpha.OnValueUpdated.AddListener(eventData => asset3DController.ChangeAlpha(eventData.NewValue));
            alpha.SliderValue = a;
        }
        
        private void AddMaterialSelectionEventListeners()
        {
            var asset3DController = currentAsset.GetComponent<Asset3DController>();
            
            // Set the existing Material Button
            switch (asset3DController.MaterialType)
            {
                case MaterialType.Regular:
                    regularRadioButton.IsToggled = true;
                    break;
                case MaterialType.Outlined:
                    outlinedRadioButton.IsToggled = true;
                    break;
                case MaterialType.Glow:
                    glowRadioButton.IsToggled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            regularRadioButton.OnClick.AddListener(() => OnMaterialRadioButtonClick(asset3DController, MaterialType.Regular));
            outlinedRadioButton.OnClick.AddListener(() => OnMaterialRadioButtonClick(asset3DController, MaterialType.Outlined));
            glowRadioButton.OnClick.AddListener(() => OnMaterialRadioButtonClick(asset3DController, MaterialType.Glow));
        }

        private void OnMaterialRadioButtonClick(Asset3DController asset3DController, MaterialType materialType)
        {
            switch (materialType)
            {
                case MaterialType.Regular:
                    asset3DController.MaterialType = MaterialType.Regular;
                    outlinedRadioButton.IsToggled = false;
                    glowRadioButton.IsToggled = false;
                    break;
                case MaterialType.Outlined:
                    asset3DController.MaterialType = MaterialType.Outlined;
                    regularRadioButton.IsToggled = false;
                    glowRadioButton.IsToggled = false;
                    break;
                case MaterialType.Glow:
                    asset3DController.MaterialType = MaterialType.Glow;
                    regularRadioButton.IsToggled = false;
                    outlinedRadioButton.IsToggled = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(materialType), materialType, null);
            }
        }

        private void AddShapeSelectionClickListeners()
        {
            if (!_shapeMenuLoaded) return;

            var asset3DController = currentAsset.GetComponent<Asset3DController>();
            foreach (var preview in _shapePreviews)
            {
                preview.GetComponent<PreviewController>().interactable.OnClick.AddListener(() => OnAssetPreviewClicked(asset3DController, preview));
            }
        }

        // ---------------------- Event Listeners Remove -----------------------

        protected override void RemoveEventListeners()
        {
            RemoveFillColorSliderEventListeners();
            RemoveMaterialSelectionEventListeners();
            RemoveShapeSelectionEventListeners();

            base.RemoveEventListeners();
        }



        private void RemoveShapeSelectionEventListeners()
        {
            if (!_shapeMenuLoaded) return;
            foreach (var preview in _shapePreviews)
            {
                preview.GetComponent<PreviewController>().interactable.OnClick.RemoveAllListeners();
            }
        }
        
        private void RemoveMaterialSelectionEventListeners()
        {
            regularRadioButton.OnClick.RemoveAllListeners();
            outlinedRadioButton.OnClick.RemoveAllListeners();
            glowRadioButton.OnClick.RemoveAllListeners();
        }

        private void RemoveFillColorSliderEventListeners()
        {
            // If I remove all listeners the the SliderChangeVisualizedValueListener is not removed because it is assigned in the editor. Therefore this works
            hue.OnValueUpdated.RemoveAllListeners();
            saturation.OnValueUpdated.RemoveAllListeners();
            value.OnValueUpdated.RemoveAllListeners();
            alpha.OnValueUpdated.RemoveAllListeners();
        }

        private void LoadShapeMenu()
        {
            if (_shapeMenuLoaded) return;

            _shapePreviews = new List<GameObject>();
            
            foreach (var mesh in AssetManager.Experiment3DAssetMeshes)
            {
                var preview = Instantiate(AssetManager.ShapeSelectionButton,  shapeSelectionGridObjectCollection.transform);
                var previewController = preview.GetComponent<PreviewController>();
                previewController.meshName = mesh.name;
                previewController.Mesh = mesh;

                _shapePreviews.Add(preview);
            }

            shapeSelectionGridObjectCollection.UpdateCollection();
            _shapeMenuLoaded = true;
        }

        protected override void HideAllSubMenus()
        {
            base.HideAllSubMenus();
            appearanceMenu.SetActive(false);
            shapeSelectionMenu.SetActive(false);
        }


        private void ShowShapeOptions()
        {
            var active = !shapeSelectionMenu.activeSelf;
            HideAllSubMenus();
            shapeSelectionMenu.SetActive(active);
        }

        private void ShowFillOptions()
        {
            var active = !appearanceMenu.activeSelf;
            HideAllSubMenus();
            appearanceMenu.SetActive(active);
        }

        private void OnAssetPreviewClicked(Asset3DController asset3DController, GameObject preview)
        {
            var meshName = preview.GetComponent<PreviewController>().meshName;
            Debug.Log(meshName);
            var mesh = AssetManager.GetMeshByName(meshName);
            Debug.Log(mesh);
            asset3DController.ChangeMesh(mesh);
        }
    }
}
