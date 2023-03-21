using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Managers
{
    public static class AssetManager
    {
        // 3D Primitives
        private static readonly Mesh[] StandardAssets = { 
            Resources.GetBuiltinResource<Mesh>("Sphere.fbx"), 
            Resources.GetBuiltinResource<Mesh>("Cube.fbx"), 
            Resources.GetBuiltinResource<Mesh>("Capsule.fbx"),
            Resources.GetBuiltinResource<Mesh>("Cylinder.fbx"),
            Resources.GetBuiltinResource<Mesh>("Plane.fbx")
        };
        
        public static readonly List<Mesh> Experiment3DAssetMeshes = new(Resources.LoadAll<Mesh>("Asset3DMeshes").Concat(StandardAssets));
        public static readonly GameObject ShapeSelectionButton = Resources.Load<GameObject>("ShapeSelectionButton");
        
        public static Mesh GetMeshByName(string meshName)
        {
            var mesh = Experiment3DAssetMeshes.Find(mesh => mesh.name == meshName);
            return mesh;
        }
        
        // --- Assets ---
        public static GameObject GetAssetContainer()
        {
            return (GameObject)Resources.Load(Path.Combine("AssetPrefabs", "AssetContainer"));
        }
        public static GameObject GetAsset3D()
        {
            return (GameObject)Resources.Load(Path.Combine("AssetPrefabs", "Asset3D"));
        }
        public static GameObject GetAssetText()
        {
            return (GameObject)Resources.Load(Path.Combine("AssetPrefabs", "AssetText"));
        }

        public static GameObject GetAssetGltf()
        {
            return (GameObject)Resources.Load(Path.Combine("AssetPrefabs", "AssetGltf"));
        }
        
        // --- Fonts ---
        public static TMP_FontAsset GetSerifFont()
        {
            return Resources.Load<TMP_FontAsset>(Path.Combine("Fonts", "Cambria_sdf"));
        }

        public static TMP_FontAsset GetNonSerifFont()
        {
            return Resources.Load<TMP_FontAsset>(Path.Combine("Fonts", "LiberationSans_SDF"));
        }
        
        // --- Materials ---
        public static Material GetStandardMaterial()
        {
            return Resources.Load<Material>(Path.Combine("Materials", "ExperimentAssetStandardMaterial"));
        }
        
        public static Material GetTransparentMaterial()
        {
            return Resources.Load<Material>(Path.Combine("Materials", "ExperimentAssetTransparencyMaterial"));
        }
        
        public static Material GetGlowingMaterial()
        {
            return Resources.Load<Material>(Path.Combine("Materials", "ExperimentAssetGlowingMaterial"));
        }
        
        public static Material GetWireframeMaterial()
        {
            return Resources.Load<Material>(Path.Combine("Materials", "ExperimentAssetWireframeMaterial"));
        }
    }
}
