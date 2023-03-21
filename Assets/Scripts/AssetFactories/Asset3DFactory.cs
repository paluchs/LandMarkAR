using Controllers.Asset;
using Managers;
using UnityEngine;

namespace AssetFactories
{
    public class Asset3DFactory : AssetFactory
    {
        public Asset3DFactory()
        {
            AssetPrefab = AssetManager.GetAsset3D();
        }
        
        public override GameObject InstantiateAsset(Vector3 position, Quaternion orientation, Transform parent)
        {
            return InstantiateAsset<Asset3DController>(position, orientation, parent);
        }
    }
}