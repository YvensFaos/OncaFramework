using UnityEngine;

namespace Onca
{
    public class GenerateUtils
    {
        /// <summary>
        /// Gets a random position delimited by the bounds, using its extends and center.
        /// </summary>
        /// <returns>Returns a random position in the bounds of the area.</returns>
        public static Vector3 GetRandomPositionInWorldBounds(Bounds bounds)
        {
            Vector3 extents = bounds.extents;
            Vector3 center = bounds.center;
            return new Vector3(
                Random.Range(-extents.x, extents.x) + center.x,
                Random.Range(-extents.y, extents.y) + center.y,
                Random.Range(-extents.z, extents.z) + center.z
            );
        }

        /// <summary>
        /// Gets a random rotation (Quaternion) using the randomRotationMinimal and randomRotationMaximal.
        /// </summary>
        /// <returns>Returns a random rotation.</returns>
        public static Quaternion GetRandomRotation(Vector3 randomRotationMinimal, Vector3 randomRotationMaximal)
        {
            return Quaternion.Euler(Random.Range(randomRotationMinimal.x, randomRotationMaximal.x),
                Random.Range(randomRotationMinimal.y, randomRotationMaximal.y),
                Random.Range(randomRotationMinimal.z, randomRotationMaximal.z));
        }
    }
}
