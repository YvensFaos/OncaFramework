using UnityEngine;
using Random = UnityEngine.Random;

namespace Onca
{
    /// <summary>
    /// Script to generate objects in an given area.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Renderer))]
    public class GenerateObjectsInArea : MonoBehaviour
    {
        private Bounds _bounds;

        [Header("Objects")]
        [SerializeField, Tooltip("Possible objects to be created in the area.")]
        private GameObject[] gameObjectToBeCreated;
        [SerializeField, Tooltip("Number of objects to be created.")]
        protected uint count;

        [Space(10)]
        [Header("Variation")]
        [SerializeField]
        private Vector3 randomRotationMinimal;
        [SerializeField]
        private Vector3 randomRotationMaximal;

        private void Awake()
        {
            _bounds = GetComponent<Renderer>().bounds;
        }

        /// <summary>
        /// Remove all children objects. Uses DestroyImmediate.
        /// </summary>
        public void RemoveChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    
        /// <summary>
        /// Destroy all objects in the area (that belongs to this script) and creates them again.
        /// The list of newly created objects is returned.
        /// </summary>
        /// <returns></returns>
        public void RegenerateObjects()
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        
            for (uint i = 0; i < count; i++)
            {
                GameObject created = Instantiate(gameObjectToBeCreated[Random.Range(0, gameObjectToBeCreated.Length)], 
                    GenerateUtils.GetRandomPositionInWorldBounds(_bounds), GenerateUtils.GetRandomRotation(randomRotationMinimal, randomRotationMaximal));
                created.transform.parent = transform;
            }
        }
    }
}
