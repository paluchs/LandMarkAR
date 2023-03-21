using Controllers.Asset;
using Managers;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace AssetFactories
{
    /// <summary>
    /// Parent for all asset factories
    /// Whenever there is a new type of asset to be added to the project, create a new AssetFactory that derives from this class.
    /// This standardizes Instantiation and makes sure that you don't have to write Instantiation anew for each type of asset there will be.
    /// </summary>
    public abstract class AssetFactory
    {
        protected GameObject AssetPrefab;


        /// <summary>
        /// Visible function of Instantiate asset. Need to call implementation of instantiate asset in child
        /// </summary>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        /// <param name="parent"></param>
        public abstract GameObject InstantiateAsset(Vector3 position, Quaternion orientation, Transform parent);
        
        /// <summary>
        /// Implementation of InstantiateAsset
        /// </summary>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        /// <param name="parent"></param>
        protected GameObject InstantiateAsset<TAssetController>(Vector3 position, Quaternion orientation, Transform parent) where TAssetController : AssetController
        {
            var assetContainer = Object.Instantiate(AssetManager.GetAssetContainer(), position, orientation);
            var asset = Object.Instantiate(AssetPrefab, assetContainer.transform);

            // Ensure that the asset is not too large
            assetContainer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            
            // Add the correct assetController --> Setting the asset also updates the box collider
            var assetController = assetContainer.AddComponent<TAssetController>();
            assetController.Asset = asset;
            
            // Make sure the Asset is selectable
            var interactable = assetContainer.GetComponent<Interactable>();
            interactable.OnClick.AddListener(assetController.OnSelect);
            
            return assetContainer;
        }
    }
    
}
