using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Controllers.Asset.Enums;
using Controllers.Behavioral;
using Managers;
using Microsoft.MixedReality.WorldLocking.Tools;
using UnityEngine;

namespace Controllers.Asset
{
    public class Asset3DController : AssetController
    {
        // Assigning this as a fixed variable is probably not ideal, it has to be assigned before the ASA is saved,
        // Otherwise the system crashes
        // Ideally there was a null check in GetAnchorProps for this variable
        // However it could be that this variable should be part of another component/controller at some point
        // Especially if 3D Shapes are needed for another type of asset.
        private string _meshName = "Cube";
        
        public new void Awake()
        {
            base.Awake();
            ExperimentAssetMenu = ExperimentController.experiment3DAssetMenu;
        }
        
        public override void OnSelect()
        {
            Asset.GetComponent<AnimationController>().ResetToStartingPosition();
            base.OnSelect();
        }

        // --- Anchor Properties ---
        public override Dictionary<string, string> GetAnchorProps()
        {
            var assetColor = Asset.GetComponent<Renderer>().material.color;
            Color.RGBToHSV(assetColor, H: out var h, S: out var s, V: out var v);
            
            
            Color.RGBToHSV(Color.white, H: out var wh, S: out var ws, V: out var wv);
            Color.RGBToHSV(Color.black, H: out var bh, S: out var bs, V: out var bv);

            SimpleConsole.AddLine(8, $"Saved hsv is: {h}, {s}, {v}");
            SimpleConsole.AddLine(8, $"White hsv is: {wh}, {ws}, {wv}");
            SimpleConsole.AddLine(8, $"White hsv is: {bh}, {bs}, {bv}");

            // Todo: Make sure this retains the scale well
            var baseAnchorProps = base.GetAnchorProps();
            
            var childAnchorProps = new Dictionary<string, string>
            {
                [@"asset-type"] = "Asset3D",
                ["factory-type"] = "Asset3DFactory",
                [@"mesh-name"] = _meshName,
                
                [@"hue"] = h.ToString(CultureInfo.InvariantCulture),
                [@"saturation"] = s.ToString(CultureInfo.InvariantCulture),
                [@"value"] = v.ToString(CultureInfo.InvariantCulture),
                [@"alpha"] = assetColor.a.ToString(CultureInfo.InvariantCulture),

                [@"wiggle-x"] = Asset.GetComponent<AnimationController>().wiggleX.ToString(),
                [@"wiggle-y"] = Asset.GetComponent<AnimationController>().wiggleY.ToString(),
                [@"wiggle-z"] = Asset.GetComponent<AnimationController>().wiggleZ.ToString(),
                [@"rotate-x"] = Asset.GetComponent<AnimationController>().rotateX.ToString(),
                [@"rotate-y"] = Asset.GetComponent<AnimationController>().rotateY.ToString(),
                [@"rotate-z"] = Asset.GetComponent<AnimationController>().rotateZ.ToString(),
                
                [@"material"] = _materialType.ToString()
            };

            var anchorProps = baseAnchorProps.Concat(childAnchorProps).ToDictionary(x=>x.Key,x=>x.Value);
            
            return anchorProps;
        }

        public override void SetAnchorProps(IDictionary<string, string> anchorProps)
        { 
            
            // Mesh --> Needs to be changed first, so that the scale stays the same when it's changed in base.SetAnchorProps
            var mesh = AssetManager.GetMeshByName(anchorProps["mesh-name"]);
            ChangeMesh(mesh);
            
            base.SetAnchorProps(anchorProps);
            
            // Color
            ChangeHSV(float.Parse(anchorProps["hue"], CultureInfo.InvariantCulture), float.Parse(anchorProps["saturation"], CultureInfo.InvariantCulture), float.Parse(anchorProps["value"], CultureInfo.InvariantCulture));
            ChangeAlpha(float.Parse(anchorProps["alpha"], CultureInfo.InvariantCulture));
            
            //Material
            var parse = Enum.TryParse<MaterialType>(anchorProps["material"], out var materialType);
            if (parse)
            {
                MaterialType = materialType;
            }
            
            //Animation
            var animationController = Asset.GetComponent<AnimationController>();
            animationController.wiggleX = bool.Parse(anchorProps["wiggle-x"]);
            animationController.wiggleY = bool.Parse(anchorProps["wiggle-y"]);
            animationController.wiggleZ = bool.Parse(anchorProps["wiggle-z"]);
            animationController.rotateX = bool.Parse(anchorProps["rotate-x"]);
            animationController.rotateY = bool.Parse(anchorProps["rotate-y"]);
            animationController.rotateZ = bool.Parse(anchorProps["rotate-z"]);
        }

        protected override void UpdateBoxCollider()
        {
            ShapeManger.UpdateBoxCollider(transform, Asset.GetComponent<Renderer>());
        }

        // --- Change Properties ---
        public void ChangeMesh(Mesh newMesh)
        {
            var oldSize = Asset.GetComponent<MeshRenderer>().bounds.size;
            Asset.GetComponent<MeshFilter>().mesh = newMesh;
            var newSize = Asset.GetComponent<MeshRenderer>().bounds.size;

            // Make sure that the size of the object stays more or less constant
            var scaleFactor = oldSize.x / newSize.x;
            Asset.transform.localScale *= scaleFactor;
            
            // Make the BoxCollider Fit and make sure the mesh name is correct
            UpdateBoxCollider();
            _meshName = newMesh.name;
        }

        public void ChangeHue(float newHue)
        {
            Color.RGBToHSV(Asset.GetComponent<Renderer>().material.color, H: out var h, S: out var s, V: out var v);
            h = newHue;
            Asset.GetComponent<Renderer>().material.color = Color.HSVToRGB(h, s, v);
        }

        public void ChangeSaturation(float newSaturation)
        {
            Color.RGBToHSV(Asset.GetComponent<Renderer>().material.color, H: out var h, S: out var s, V: out var v);
            s = newSaturation;
            Asset.GetComponent<Renderer>().material.color = Color.HSVToRGB(h, s, v);
        }

        public void ChangeValue(float newValue)
        {
            Color.RGBToHSV(Asset.GetComponent<Renderer>().material.color, H: out var h, S: out var s, V: out var v);
            v = newValue;
            Asset.GetComponent<Renderer>().material.color = Color.HSVToRGB(h, s, v);
        }

        public void ChangeHSV(float h, float s, float v)
        {
            Asset.GetComponent<Renderer>().material.color = Color.HSVToRGB(h, s, v);
        }

        public void ChangeAlpha(float newAlpha)
        {
            var renderComponent = Asset.GetComponent<Renderer>();
            
            var oldColor = renderComponent.material.color;
            var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, newAlpha);
            
            // If the alpha is so that the object should be transparent change the material to one that accepts transparency
            renderComponent.material = Math.Abs(newAlpha - 1) < 0.001 ? AssetManager.GetStandardMaterial() : AssetManager.GetTransparentMaterial();
            
            renderComponent.material.color = newColor;
        }

        private MaterialType _materialType;
        public MaterialType MaterialType
        {
            get => _materialType;
            set
            {
                _materialType = value;
                var renderComponent = Asset.GetComponent<Renderer>();

                // Retain the color after switching the material
                var oldColor = renderComponent.material.color;
                // Reset the alpha value, because glowing material cannot render alpha
                var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 1);

                var material = value switch
                {
                    MaterialType.Regular => AssetManager.GetStandardMaterial(),
                    MaterialType.Outlined => AssetManager.GetWireframeMaterial(),
                    MaterialType.Glow => AssetManager.GetGlowingMaterial(),
                    _ => (renderComponent.material)
                };
                renderComponent.material = material;

                material.color = newColor;
            }
        }
    }
}
