using System;
using Controllers.Asset;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.WorldLocking.Tools;
using UnityEngine;

namespace UI.Menus.Asset
{
    public abstract class AssetMenu : MonoBehaviour
    {
        public GameObject currentAsset;

        [SerializeField] private Interactable pin;
        
        [Header("Menu Buttons")]
        [SerializeField] private Interactable saveButton;
        [SerializeField] private Interactable deleteButton;
        

        public virtual void OpenMenu(GameObject selectedAsset)
        {
            currentAsset = selectedAsset;
            AddEventListeners();
            
            gameObject.SetActive(true);
        }

        public void CloseMenu()
        {
            ResetMenu();
  
            RemoveEventListeners();
            currentAsset = null;
            gameObject.SetActive(false);
        }

        protected virtual void ResetMenu()
        {
            if (!pin.IsToggled) return;
            
            pin.OnClick.Invoke();
            pin.ResetAllStates();
        }

        protected virtual void AddEventListeners()
        {
            var assetController = currentAsset.GetComponent<AssetController>();
            saveButton.OnClick.AddListener(assetController.OnSave);
            deleteButton.OnClick.AddListener(assetController.OnDelete);
        }

        protected virtual void RemoveEventListeners()
        {
            saveButton.OnClick.RemoveAllListeners();
            deleteButton.OnClick.RemoveAllListeners();
        }
        

    }
}