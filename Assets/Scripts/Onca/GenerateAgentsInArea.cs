using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Onca
{
    /// <summary>
    /// Script to generate objects in an given area.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Renderer))]
    public class GenerateAgentsInArea : MonoBehaviour
    {
        private Bounds _bounds;

        [Header("Objects")]
        [SerializeField, Tooltip("Possible agents to be created in the area.")]
        private AgentLogic[] agentsToBeCreated;
        [SerializeField, Tooltip("Number of objects to be created.")]
        protected uint count;

        [Space(10)]
        [Header("Variation")]
        [SerializeField]
        private Vector3 randomRotationMinimal;
        [SerializeField]
        private Vector3 randomRotationMaximal;

        private List<AgentLogic> _activeAgents;
    
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
        /// Destroy all agents in the area (that belongs to this script) and creates them again.
        /// The list of newly created objects is returned.
        /// </summary>
        /// <returns></returns>
        public List<AgentLogic> RegenerateObjects()
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        
            _activeAgents = new List<AgentLogic>();
            for (uint i = 0; i < count; i++)
            {
                AgentLogic created = Instantiate<AgentLogic>(agentsToBeCreated[Random.Range(0, agentsToBeCreated.Length)], 
                    GenerateUtils.GetRandomPositionInWorldBounds(_bounds), GenerateUtils.GetRandomRotation(randomRotationMinimal, randomRotationMaximal));
                created.transform.parent = transform;
                _activeAgents.Add(created);
            }

            return _activeAgents;
        }

        public List<AgentLogic> GetActiveAgents()
        {
            return _activeAgents;
        }
    }
}
