using Controllers.Asset;
using Controllers.Behavioral;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using UnityEngine;

namespace UI.Menus.Asset
{
    public abstract class AssetAnimateableMenu : AssetMenu
    {
        [SerializeField] private Interactable scaleModeToggle;
        [SerializeField] private Interactable animationButton;

        [Header("Sub Menus")] 
        [SerializeField] private GameObject animationMenu;
        
        [Header("Animation Sub Menu")]
        [SerializeField] private Interactable wiggleRedCheckbox;
        [SerializeField] private Interactable wiggleGreenCheckbox;
        [SerializeField] private Interactable wiggleBlueCheckbox;
        [SerializeField] private Interactable rotateRedCheckbox;
        [SerializeField] private Interactable rotateGreenCheckbox;
        [SerializeField] private Interactable rotateBlueCheckbox;
        [SerializeField] private PinchSlider speedSlider;

        public void Start()
        {
            // These click listeners are independent from the selected gameObject and therefore only need to be attached once
            scaleModeToggle.OnClick.AddListener(ToggleScaleMode);
            animationButton.OnClick.AddListener(ShowAnimationOptions);

        }
        
        protected override void ResetMenu()
        {
            base.ResetMenu();
            animationMenu.SetActive(false);
        }
        
        
        protected override void AddEventListeners()
        {
            base.AddEventListeners();
            AddAnimationClickListeners();
        }
        
        protected override void RemoveEventListeners()
        {
            ResetScaleModeToggle();
            RemoveAnimateClickListeners();
            
            base.RemoveEventListeners();
        }
        
        private void ToggleScaleMode()
        {
            currentAsset.GetComponent<BoundsControl>().ScaleHandlesConfig.ScaleBehavior = scaleModeToggle.IsToggled ? HandleScaleMode.Uniform : HandleScaleMode.NonUniform;
        }
        
        private void ResetScaleModeToggle()
        {
            currentAsset.GetComponent<BoundsControl>().ScaleHandlesConfig.ScaleBehavior = HandleScaleMode.NonUniform;
            if (scaleModeToggle.IsToggled)
            {
                scaleModeToggle.OnClick.Invoke();
            }
            scaleModeToggle.OnClick.RemoveAllListeners();
        }
        
        private void AddAnimationClickListeners()
        {
            var animationController = currentAsset.GetComponent<AssetController>().Asset.GetComponent<AnimationController>();
            
            wiggleRedCheckbox.IsToggled = animationController.wiggleX;
            wiggleGreenCheckbox.IsToggled = animationController.wiggleY;
            wiggleBlueCheckbox.IsToggled = animationController.wiggleZ;
            rotateRedCheckbox.IsToggled = animationController.rotateX;
            rotateGreenCheckbox.IsToggled = animationController.rotateY;
            rotateBlueCheckbox.IsToggled = animationController.rotateZ;

            wiggleRedCheckbox.OnClick.AddListener(() => { animationController.wiggleX = !animationController.wiggleX;});
            wiggleGreenCheckbox.OnClick.AddListener(() => { animationController.wiggleY = !animationController.wiggleY;});
            wiggleBlueCheckbox.OnClick.AddListener(() => { animationController.wiggleZ = !animationController.wiggleZ;});
            rotateRedCheckbox.OnClick.AddListener(() => { animationController.rotateX = !animationController.rotateX;});
            rotateGreenCheckbox.OnClick.AddListener(() => { animationController.rotateY = !animationController.rotateY;});
            rotateBlueCheckbox.OnClick.AddListener(() => { animationController.rotateZ = !animationController.rotateZ;});
        }
        
        private void RemoveAnimateClickListeners()
        {
            wiggleRedCheckbox.OnClick.RemoveAllListeners();
            wiggleGreenCheckbox.OnClick.RemoveAllListeners();
            wiggleBlueCheckbox.OnClick.RemoveAllListeners();
            rotateRedCheckbox.OnClick.RemoveAllListeners();
            rotateGreenCheckbox.OnClick.RemoveAllListeners();
            rotateBlueCheckbox.OnClick.RemoveAllListeners();
        }

        protected virtual void HideAllSubMenus()
        {
            animationMenu.SetActive(false);
        }

        private void ShowAnimationOptions()
        {
            var active = !animationMenu.activeSelf;
            HideAllSubMenus();
            animationMenu.SetActive(active);
        }
        
        
    }
}