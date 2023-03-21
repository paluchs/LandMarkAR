using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using UI.Menus.Asset;
using UnityEngine;

namespace Controllers.Asset
{
    public abstract class AssetController : MonoBehaviour
    {
        public bool IsSelected { get; private set; }

        // Controllers
        protected ExperimentController ExperimentController;

        //Matching Menu
        protected AssetMenu ExperimentAssetMenu;

        //Manipulation components
        private BoundsControl _boundsControl;
        private ObjectManipulator _objectManipulator;
        private NearInteractionGrabbable _nearInteractionGrabbable;

        private Interactable _interactable;
        private NearInteractionTouchable _nearInteractionTouchable;
        
        // Visuals
        [SerializeField] private GameObject axes;
        
        private GameObject _asset;
        public GameObject Asset
        {
            set
            {
                _asset = value;
                UpdateBoxCollider();
            }
            get => _asset;
        }


        public void Awake()
        {
            IsSelected = false;
            ExperimentController = GameObject.Find("ExperimentView").GetComponent<ExperimentController>();
            
            axes = transform.Find("Axes").gameObject;
            axes.SetActive(false);
        }

        /// <summary>
        /// Assigns all the components of the AssetContainer on Instantiation so that they can be manipulated later
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Start()
        {
            _boundsControl = GetComponent<BoundsControl>();
            _objectManipulator = GetComponent<ObjectManipulator>();
            _nearInteractionGrabbable = GetComponent<NearInteractionGrabbable>();

            _interactable = GetComponent<Interactable>();
            _nearInteractionTouchable = GetComponent<NearInteractionTouchable>();
        }
        
        public virtual void OnSelect()
        {
            ExperimentController.AssetSelected = true;
            
            // At the moment I delete the ASA, because I think there might be unwanted side effects e.g. when moving the asset
            ExperimentController.RemoveAsset(gameObject);

            UpdateBoxCollider();
            EnableObjectManipulation();
            DisableSelectInteraction();
            IsSelected = true;
            axes.SetActive(true);
            
            ExperimentAssetMenu.OpenMenu(gameObject);
        }

        public virtual void OnSave()
        {
            ExperimentController.AddAsset(gameObject);
            
            DisableObjectManipulation();
            EnableSelectInteraction();
            IsSelected = false;
            axes.SetActive(false);

            ExperimentAssetMenu.CloseMenu();
            
            ExperimentController.AssetSelected = false;
        }
        
        public void OnDelete()
        {
            Destroy(gameObject);
            
            ExperimentAssetMenu.CloseMenu();
            ExperimentController.AssetSelected = false;
        }

        /// <summary>
        /// Returns the properties of the Asset so that they can be saved as AzureSpatialAnchor properties and retrieved later
        /// Always call this in the override of the child
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, string> GetAnchorProps()
        {
            var localScale = transform.localScale;
            var anchorProps = new Dictionary<string, string>
            {
                [@"x"] = localScale.x.ToString(CultureInfo.InvariantCulture),
                [@"y"] = localScale.y.ToString(CultureInfo.InvariantCulture),
                [@"z"] = localScale.z.ToString(CultureInfo.InvariantCulture),
            };
            return anchorProps;
        }

        /// <summary>
        /// Sets the Properties given from an AzureSpatialAnchor on the Asset
        /// Always call this in the override of the child
        /// </summary>
        /// <param name="anchorProps"></param>
        public virtual void SetAnchorProps(IDictionary<string, string> anchorProps)
        {
            //Scale
            transform.localScale = new Vector3(float.Parse(anchorProps["x"]),
                float.Parse(anchorProps["y"]), float.Parse(anchorProps["z"]));
            UpdateBoxCollider();
        }

        protected abstract void UpdateBoxCollider();
        
        public void EnableSelectInteraction()
        {
            if (_interactable == null)
            {
                _interactable = GetComponent<Interactable>();
            }
            _interactable.enabled = true;

            if (_nearInteractionTouchable == null)
            {
                _nearInteractionTouchable = GetComponent<NearInteractionTouchable>();
            }
            _nearInteractionTouchable.enabled = true;
            
            // Todo: Not quite sure if I need to update the boxcollider here again
            // UpdateBoxCollider();
        }
    
        public void DisableSelectInteraction()
        {
            if (_interactable == null)
            {
                _interactable = GetComponent<Interactable>();
            }
            _interactable.enabled = false;
        
            if (_nearInteractionTouchable == null)
            {
                _nearInteractionTouchable = GetComponent<NearInteractionTouchable>();
            }
            _nearInteractionTouchable.enabled = false;
        }

        private void EnableObjectManipulation()
        {
            if (_boundsControl == null)
            {
                _boundsControl = GetComponent<BoundsControl>();
            }
            _boundsControl.enabled = true;

            if (_objectManipulator == null)
            {
                _objectManipulator = GetComponent<ObjectManipulator>();
            }
            _objectManipulator.enabled = true;

            if (_nearInteractionGrabbable == null)
            {
                _nearInteractionGrabbable = GetComponent<NearInteractionGrabbable>();
            }
            _nearInteractionGrabbable.enabled = true;
        }

        private void DisableObjectManipulation()
        {
            if (_boundsControl == null)
            {
                _boundsControl = GetComponent<BoundsControl>();
            }
            _boundsControl.enabled = false;
        
            if (_objectManipulator == null)
            {
                _objectManipulator = GetComponent<ObjectManipulator>();
            }
            _objectManipulator.enabled = false;
        
            if (_nearInteractionGrabbable == null)
            {
                _nearInteractionGrabbable = GetComponent<NearInteractionGrabbable>();
            }
            _nearInteractionGrabbable.enabled = false;
        }
    }
}
