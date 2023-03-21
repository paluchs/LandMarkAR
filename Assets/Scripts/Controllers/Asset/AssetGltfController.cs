using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers.Behavioral;
using GLTFast;
using Managers;
using Microsoft.MixedReality.WorldLocking.Tools;
using UnityEngine;

namespace Controllers.Asset
{
    public class AssetGltfController : AssetController
    {

        private string _gltfUrl;
        private bool _assetLoaded;

        public string GltfUrl
        {
            get => _gltfUrl;
            set
            {
                LoadGltf(value);
                _gltfUrl = value;
            }
        }
        
        
        public new void Awake()
        {
            base.Awake();
            ExperimentAssetMenu = ExperimentController.experimentGltfMenu;
        }

        public new void Start()
        {
            base.Start();
        }

        public override void OnSelect()
        {
            Asset.GetComponent<AnimationController>().ResetToStartingPosition();
            base.OnSelect();
        }

        private async void LoadGltf(string url)
        {
            StartCoroutine(nameof(OnGltfLoaded));
            
            var gltfAsset = Asset.GetComponent<GltfAsset>();
            
            // gltfAsset.url = url;
            // ReSharper disable once UseConfigureAwaitFalse
            var success = await gltfAsset.Load(url);
        
            if (success)
            {
                _assetLoaded = true;
            }
            else
            {
                // Todo: Give user some feedback that it didn't work!
                OnDelete();
            }
            
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator OnGltfLoaded()
        {
            while (!_assetLoaded)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            SetCenter();
            Asset.transform.localScale /= ShapeManger.GetScaleFactor(Asset.GetComponentInChildren<Renderer>(), 0.25f);
            UpdateBoxCollider();
        }

        protected override void UpdateBoxCollider()
        {
            var shapeRenderer = Asset.GetComponentInChildren<Renderer>();
            if (shapeRenderer != null)
            {
                ShapeManger.UpdateBoxCollider(transform, shapeRenderer);
            }
        }

        private void SetCenter()
        {
            var shapeRenderer = Asset.GetComponentInChildren<Renderer>();
            if (shapeRenderer == null) return;
            
            var bounds = shapeRenderer.bounds;
            
            // In world-space!
            var center = bounds.center;
            Debug.Log(center);
            
            // Converted to local space of the container
            center = transform.InverseTransformPoint(center);
            Debug.Log(center);

            var pos = Asset.transform.localPosition;
            var x = pos.x - center.x;
            var y = pos.y - center.y;
            var z = pos.z - center.z;
            Debug.Log(pos);

            Asset.transform.localPosition = new Vector3(x, y, z);
        }
        
        public override Dictionary<string, string> GetAnchorProps()
        {
            var baseAnchorProps = base.GetAnchorProps();
            
            var childAnchorProps = new Dictionary<string, string>
            {
                [@"asset-type"] = "Asset3D",
                ["factory-type"] = "AssetGltfFactory",
                [@"url"] = GltfUrl,

                [@"wiggle-x"] = Asset.GetComponent<AnimationController>().wiggleX.ToString(),
                [@"wiggle-y"] = Asset.GetComponent<AnimationController>().wiggleY.ToString(),
                [@"wiggle-z"] = Asset.GetComponent<AnimationController>().wiggleZ.ToString(),
                [@"rotate-x"] = Asset.GetComponent<AnimationController>().rotateX.ToString(),
                [@"rotate-y"] = Asset.GetComponent<AnimationController>().rotateY.ToString(),
                [@"rotate-z"] = Asset.GetComponent<AnimationController>().rotateZ.ToString(),
            };

            var anchorProps = baseAnchorProps.Concat(childAnchorProps).ToDictionary(x=>x.Key,x=>x.Value);
            
            return anchorProps;
        }

        public override void SetAnchorProps(IDictionary<string, string> anchorProps)
        {
            base.SetAnchorProps(anchorProps);

            GltfUrl = anchorProps["url"];

            //Animation
            var animationController = Asset.GetComponent<AnimationController>();
            animationController.wiggleX = bool.Parse(anchorProps["wiggle-x"]);
            animationController.wiggleY = bool.Parse(anchorProps["wiggle-y"]);
            animationController.wiggleZ = bool.Parse(anchorProps["wiggle-z"]);
            animationController.rotateX = bool.Parse(anchorProps["rotate-x"]);
            animationController.rotateY = bool.Parse(anchorProps["rotate-y"]);
            animationController.rotateZ = bool.Parse(anchorProps["rotate-z"]);
        }
    }
}
