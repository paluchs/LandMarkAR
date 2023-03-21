using UnityEngine;
using UnityEngine.XR;

namespace Controllers.Functional
{
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Get a rotation towards the player
        /// </summary>
        /// <param name="position">The position of an object</param>
        /// <returns>A quaternion to rotate an object such that it faces the player</returns>
        public Quaternion GetOrientationTowardsHead(Vector3 position)
        {
            if (!InputDevices.GetDeviceAtXRNode(XRNode.Head)
                    .TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
            {
                headPosition = Vector3.zero;
            }

            Quaternion orientationTowardsHead = Quaternion.LookRotation(position - headPosition, Vector3.up);
            return orientationTowardsHead;
        }

        /// <summary>
        /// Get a position on the line of sight of the player
        /// </summary>
        /// <returns>Returns a position 1.5metres away from the player in direction their viewing</returns>
        public Vector3 GetPositionTowardsHead()
        {
            var gameObjectTransform = transform;
            var playerPos = gameObjectTransform.position;
            var playerDir = gameObjectTransform.forward;
            const float spawnDistance = 1.5f;

            Vector3 position = playerPos + playerDir * spawnDistance;
            return position;
        }
    }
}
