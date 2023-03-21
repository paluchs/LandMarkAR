using Controllers.Asset;
using Managers;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using UnityEngine;

namespace AssetFactories
{
    public class AssetTextFactory : AssetFactory
    {
        public AssetTextFactory()
        {
            AssetPrefab = AssetManager.GetAssetText();
        }

        public override GameObject InstantiateAsset(Vector3 position, Quaternion orientation, Transform parent)
        {
            var asset = InstantiateAsset<AssetTextController>(position, orientation, parent);

            var boundsControl = asset.GetComponent<BoundsControl>();
            boundsControl.FlattenAxis = FlattenModeType.FlattenZ;
            boundsControl.ScaleHandlesConfig.ScaleBehavior = HandleScaleMode.Uniform;

            return asset;
        }
    }
}