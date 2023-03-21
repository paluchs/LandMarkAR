using Controllers.Asset;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Controllers.Behavioral
{
    public class AnimationController : MonoBehaviour
    {
        public float maxWiggleDistance = 0.1f;
        public float rotationSpeed = 80f;
        
        public bool wiggleX;
        public bool wiggleY;
        public bool wiggleZ;
        public bool rotateX;
        public bool rotateY;
        public bool rotateZ;

        private bool _isGrabbedOrHovered;
        
        public AnimationCurve wiggleCurve;
        
        private AssetController _assetController;

        public void Start()
        {
            SetupStopAnimationWhenGrabbingOrHovering();
            _assetController = GetComponentInParent<AssetController>();
        }

        public void Update()
        {
            if (_assetController is not null)
            {
                if (_assetController.IsSelected) return;
            }
            
            if (_isGrabbedOrHovered) return;

            var position = transform.localPosition;
            var x = position.x;
            var y = position.y;
            var z = position.z;

            if (wiggleX) { x = wiggleCurve.Evaluate(Time.time % wiggleCurve.length) * maxWiggleDistance; }
            if (wiggleY) { y = wiggleCurve.Evaluate(Time.time % wiggleCurve.length) * maxWiggleDistance; }
            if (wiggleZ) { z = wiggleCurve.Evaluate(Time.time % wiggleCurve.length) * maxWiggleDistance; }

            transform.localPosition = new Vector3(x, y, z);

            if (rotateX) { transform.Rotate(new Vector3(Time.deltaTime * rotationSpeed, 0, 0));}
            if (rotateY) { transform.Rotate(new Vector3(0, Time.deltaTime * rotationSpeed, 0));}
            if (rotateZ) { transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotationSpeed));}
        }

        public void ResetToStartingPosition()
        {
            var t = transform;
            t.localPosition = Vector3.zero;
            t.rotation = new Quaternion();
        }
        
        private void SetupStopAnimationWhenGrabbingOrHovering()
        {
            var manipulator = GetComponentInParent<ObjectManipulator>();
            if (!manipulator) return;
            
            manipulator.OnManipulationStarted.AddListener(_ => { _isGrabbedOrHovered = true; });
            manipulator.OnManipulationEnded.AddListener(_ => { _isGrabbedOrHovered = false; });
            manipulator.OnHoverEntered.AddListener(_ => { _isGrabbedOrHovered = true; });
            manipulator.OnHoverExited.AddListener(_ => { _isGrabbedOrHovered = false; });
        }
    }
}
