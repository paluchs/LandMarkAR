using Controllers.Asset;
using Managers;
using UnityEngine;

namespace AssetFactories
{
    public class AssetGltfFactory : AssetFactory
    {
        public AssetGltfFactory()
        {
            AssetPrefab = AssetManager.GetAssetGltf();
        }
        public override GameObject InstantiateAsset(Vector3 position, Quaternion orientation, Transform parent)
        {
            return InstantiateAsset<AssetGltfController>(position, orientation, parent);
        }
    }
}