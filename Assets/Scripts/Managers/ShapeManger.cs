

using UnityEngine;

namespace Managers
{
    public static class ShapeManger
    {
        /// <summary>
        /// Updates the Box Collider of a a gameObject with a collider according to the shape of a given renderer
        /// </summary>
        /// <param name="colliderOwner"></param>
        /// <param name="assetRenderer"></param>
        /// <typeparam name="TTargetTransform"></typeparam>
        public static void UpdateBoxCollider(Transform colliderOwner, Renderer assetRenderer)
        {
            // Reset the rotation to zero so that the box collider gets a meaningful shape
            var oldRotation = colliderOwner.rotation;
            colliderOwner.rotation = new Quaternion(0, 0, 0, 0);
        
            var boxCollider = colliderOwner.GetComponent<BoxCollider>();

            var bounds = assetRenderer.bounds;
            // In world-space!
            var size = bounds.size;
            var center = bounds.center;
        
            Debug.Log($"Global size of assetRendererBounds: {size}");

            // converted to local space of the collider
            size = boxCollider.transform.InverseTransformVector(size);
            center = boxCollider.transform.InverseTransformPoint(center);

            Debug.Log($"Local size of assetRendererBounds: {size}");
        
            boxCollider.size = size;
            boxCollider.center = center;
        
            // Reset the rotation to it's original value
            colliderOwner.transform.rotation = oldRotation;
        }

        /// <summary>
        /// Implementations for rects (i.e., 2D Assets like text)
        /// </summary>
        /// <param name="colliderOwner"></param>
        /// <param name="boxCollider"></param>
        public static void UpdateBoxCollider(Transform colliderOwner, RectTransform assetTransform)
        {
            // Quickly reset the rotation to zero
            var oldRotation = colliderOwner.rotation;
            colliderOwner.rotation = new Quaternion(0, 0, 0, 0);

            var bounds = assetTransform.rect;
            // In world-space!
            var size = new Vector3(bounds.width, bounds.height, 0.01f);
            var center = new Vector3(bounds.center.x, bounds.center.y, 0);
        
            Debug.Log($"Potential new size {size}");

            // converted to local space of the collider
            // size = boxCollider.transform.InverseTransformVector(size);
            // center = boxCollider.transform.InverseTransformPoint(center);

            var boxCollider = colliderOwner.GetComponent<BoxCollider>();
            boxCollider.size = size;
            boxCollider.center = center;
        
            Debug.Log($"New Box Collider Size: {boxCollider.size}");
            // Reset the rotation to it's original value
            colliderOwner.rotation = oldRotation;
        }
        
        /// <summary>
        /// Find the scale factor to make a mesh the same size of the local scale of the Game Object
        /// </summary>
        /// <param name="preview"></param>
        /// <returns></returns>
        public static float GetScaleFactor(GameObject preview)
        {
            var size = preview.GetComponent<Renderer>().bounds.size;
            var maxExtent = Mathf.Max(Mathf.Max(size.x, size.y), size.z);
            var localScale = preview.transform.localScale.x;
            var scaleFactor = (maxExtent / localScale);
            return scaleFactor;
        }

        /// <summary>
        /// Find the scale factor to make a mesh the same size as the input gameObject
        /// </summary>
        /// <param name="preview"></param>
        /// <param name="targetMaxLongestEdge"> The max size on the longest edge (xyz) of the object </param>
        /// <returns></returns>
        public static float GetScaleFactor(Renderer target, float targetMaxLongestEdge)
        {
            var size = target.bounds.size;
            var maxExtent = Mathf.Max(Mathf.Max(size.x, size.y), size.z);
            var scaleFactor = (maxExtent / targetMaxLongestEdge);
            return scaleFactor;
        }
        
    }
}
