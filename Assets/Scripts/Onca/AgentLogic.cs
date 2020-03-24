using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Onca
{
    /// <summary>
    /// This struct stores all genes / weights from an Agent.
    /// It is used to pass this information along to other Agents, instead of using the MonoBehavior itself.
    /// Also, it makes it easier to inspect since it is a Serializable struct.
    /// </summary>
    [Serializable]
    public struct AgentData
    {
        public Dictionary<string, Gene> GenesDictionary { get; }

        public AgentData(Gene[] genes)
        {
            GenesDictionary = new Dictionary<string, Gene>();
            for (int i = 0; i < genes.Length; i++)
            {
                GenesDictionary[genes[i].Name] = genes[i];
            }
        }
    }

    [RequireComponent(typeof(Rigidbody), typeof(AgentAction), typeof(PerformanceEvaluator))]
    public class AgentLogic : MonoBehaviour, IComparable
    {
        private Vector3 _movingDirection;
        private Rigidbody _rigidbody;

        [SerializeField] protected float performance;
        private bool _isAwake;

        [Header("Genes")] [SerializeField] private Gene[] genes;
        private Dictionary<string, Gene> _genesDictionary;

        [Header("Special Genes")] [SerializeField, Tooltip("Steps for the area of sight.")]
        private float steps;

        [SerializeField, Range(0.0f, 360.0f), Tooltip("Divides the 360˚ view of the Agent into rayRadius steps.")]
        private float sightDivisions = 16;

        [SerializeField, Tooltip("Ray distance.")]
        private float sight = 10.0f;

        [SerializeField] private float movingSpeed;
        [SerializeField] private float maxUtilityChoiceChance;

        [Header("Agent Actions")] [SerializeField]
        private AgentAction[] agentActions;

        [Header("Performance Evaluator")] [SerializeField]
        private PerformanceEvaluator evaluator;

        [Space(10)] [Header("Debug & Help")] [SerializeField]
        private Color visionColor;

        [SerializeField] private Color foundColor;
        [SerializeField] private Color directionColor;

        [SerializeField, Tooltip("Shows visualization rays.")]
        private bool debug;

        private void Awake()
        {
            Initiate();

            agentActions = GetComponents<AgentAction>();
        }

        /// <summary>
        /// Initiate the values for this Agent, TODO
        /// </summary>
        private void Initiate()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _genesDictionary = new Dictionary<string, Gene>();

            foreach (Gene gene in genes)
            {
                _genesDictionary[gene.Name] = gene;
            }

            _genesDictionary["steps"] = new Gene("steps", steps);
            _genesDictionary["sightDivisions"] = new Gene("sightDivisions", sightDivisions);
            _genesDictionary["sight"] = new Gene("sight", sight);
            _genesDictionary["movingSpeed"] = new Gene("movingSpeed", movingSpeed);
            _genesDictionary["maxUtilityChoiceChance"] = new Gene("maxUtilityChoiceChance", maxUtilityChoiceChance);
        }

        public void Birth(AgentData agentData)
        {
            _genesDictionary = agentData.GenesDictionary;

            AssignSpecialGenes();
        }

        private void AssignSpecialGenes()
        {
            steps = _genesDictionary["steps"].Value;
            sightDivisions = _genesDictionary["sightDivisions"].Value;
            sight = _genesDictionary["sight"].Value;
            movingSpeed = _genesDictionary["movingSpeed"].Value;
            maxUtilityChoiceChance = _genesDictionary["maxUtilityChoiceChance"].Value;
        }

        /// <summary>
        /// Has a mutationChance ([0%, 100%]) of causing a mutationFactor [-mutationFactor, +mutationFactor] to each gene.
        /// </summary>
        /// <param name="mutationFactor">How much a gene / weight can change (-mutationFactor, +mutationFactor)</param>
        /// <param name="mutationChance">Chance of a mutation happening per gene / weight.</param>
        public void Mutate(float mutationFactor, float mutationChance)
        {
            foreach (Gene gene in genes)
            {
                if (Random.Range(0.0f, 100.0f) <= mutationChance)
                {
                    gene.Mutate(Random.Range(-mutationFactor, +mutationFactor));
                }
            }

            AssignSpecialGenes();
        }

        private void Update()
        {
            if (_isAwake)
            {
                _rigidbody.velocity = Vector3.zero;

                List<AgentLookData> lookData = LookAround();
                Vector3 actionDirection = Act(lookData);
            
                if (debug)
                {
                    Debug.DrawRay(transform.position, actionDirection * (sight * 1.5f), directionColor);
                }
            }
        }

        private List<AgentLookData> LookAround()
        {
            Transform selfTransform = transform;
            Vector3 forward = selfTransform.forward;
            forward.y = 0.0f;
            forward.Normalize();
            Vector3 selfPosition = selfTransform.position;

            //Initiate the rayDirection on the opposite side of the spectrum.
            Vector3 rayDirection = Quaternion.Euler(0, -1.0f * steps * (sightDivisions / 2.0f), 0) * forward;

            List<AgentLookData> objectsSeen = new List<AgentLookData>();
            for (uint i = 0; i <= (uint) sightDivisions; i++)
            {
                bool hasHitSomething = Physics.Raycast(selfPosition, rayDirection, out RaycastHit raycastHit);
                objectsSeen.Add(new AgentLookData(rayDirection, hasHitSomething, raycastHit));
                if (debug)
                {
                    if (hasHitSomething)
                    {
                        Debug.DrawLine(selfPosition, raycastHit.point, foundColor);
                    }
                    else
                    {
                        Debug.DrawRay(selfPosition, rayDirection * sight, visionColor);
                    }
                }
                rayDirection = Quaternion.Euler(0, steps, 0) * rayDirection;
            }

            return objectsSeen;
        }

        private Vector3 Act(List<AgentLookData> lookData)
        {
            foreach (AgentAction agentAction in agentActions)
            {
                agentAction.CalculateUtility(_genesDictionary, lookData);
            }
            
            Array.Sort(agentActions);
            int index = Random.Range(0.0f, 100.0f) < maxUtilityChoiceChance ? 0 : 1;
            return agentActions[index].Act(_genesDictionary);
        }

        public void WakeUp()
        {
            _isAwake = true;
        }

        public void Sleep()
        {
            _isAwake = false;
            _rigidbody.velocity = Vector3.zero;
        }

        public float GetPerformance()
        {
            return performance;
        }

        /// <summary>
        /// Compares the points of two agents. When used on Sort function will make the highest performance to be on top.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            AgentLogic otherAgent = obj as AgentLogic;
            if (otherAgent != null)
            {
                return otherAgent.GetPerformance().CompareTo(GetPerformance());
            }
            else
            {
                throw new ArgumentException("Object is not an AgentLogic");
            }
        }

        public float CalculatePerformance(Environment environment)
        {
            return evaluator.EvaluatePerfomance(this, environment);
        }

        /// <summary>
        /// Returns the AgentData of this Agent.
        /// </summary>
        /// <returns></returns>
        public AgentData GetData()
        {
            return new AgentData(genes);
        }
    }
}