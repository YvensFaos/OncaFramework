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

        public AgentData(List<Gene> genes)
        {
            GenesDictionary = new Dictionary<string, Gene>();
            for (int i = 0; i < genes.Count; i++)
            {
                GenesDictionary[genes[i].Name] = genes[i];
            }
        }
    }

    [Serializable]
    public class AgentProperty
    {
        public String Name;
        public float Value;

        public AgentProperty(string name, float value)
        {
            Name = name;
            Value = value;
        }

        public void IncrementValue(float increment)
        {
            Value += increment;
        }
        
        public void SetValue(float newValue)
        {
            Value = newValue;
        }
    }

    [RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(AgentColliderHandler))]
    [RequireComponent(typeof(AgentAction), typeof(PerformanceEvaluator))]
    public abstract class AgentLogic : MonoBehaviour, IComparable
    {
        private Vector3 _movingDirection;
        private Rigidbody _rigidbody;

        [SerializeField] protected float performance;
        private bool _isAwake;

        [Header("Properties")] [SerializeField] protected List<AgentProperty> properties;
        protected Dictionary<string, AgentProperty> _properties;
        
        [Header("Genes")] [SerializeField] protected List<Gene> genes;
        protected Dictionary<string, Gene> _genes;

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

        [Header("Collider / Trigger Handlers")] [SerializeField]
        private AgentColliderHandler colliderHandler;

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

            _genes = new Dictionary<string, Gene>();
            
            genes.Add(new Gene("steps", steps));
            genes.Add(new Gene("sightDivisions", sightDivisions));
            genes.Add(new Gene("sight", sight));
            genes.Add(new Gene("movingSpeed", movingSpeed));
            genes.Add(new Gene("maxUtilityChoiceChance", maxUtilityChoiceChance));

            foreach (Gene gene in genes)
            {
                _genes[gene.Name] = gene;
            }
            
            _properties = new Dictionary<string, AgentProperty>();
            CreateProperties();
            foreach (AgentProperty agentProperty in properties)
            {
                _properties[agentProperty.Name] = agentProperty;
            }
        }

        public void Birth(AgentData agentData)
        {
            _genes.Clear();
            foreach (KeyValuePair<string,Gene> gene in agentData.GenesDictionary)
            {
                _genes.Add(gene.Key, new Gene(gene.Value));
            }

            AssignSpecialGenes();
            CalculateProperties();
        }

        protected abstract void CreateProperties();
        protected abstract void CalculateProperties();

        private void AssignSpecialGenes()
        {
            steps = _genes["steps"].Value;
            sightDivisions = _genes["sightDivisions"].Value;
            sight = _genes["sight"].Value;
            movingSpeed = _genes["movingSpeed"].Value;
            maxUtilityChoiceChance = _genes["maxUtilityChoiceChance"].Value;
        }

        /// <summary>
        /// Has a mutationChance ([0%, 100%]) of causing a mutationFactor [-mutationFactor, +mutationFactor] to each gene.
        /// </summary>
        /// <param name="mutationFactor">How much a gene / weight can change (-mutationFactor, +mutationFactor)</param>
        /// <param name="mutationChance">Chance of a mutation happening per gene / weight.</param>
        public void Mutate(float mutationFactor, float mutationChance)
        {
            genes.RemoveRange(0, genes.Count);
            foreach (KeyValuePair<string, Gene> gene in _genes)
            {
                if (Random.Range(0.0f, 100.0f) <= mutationChance)
                {
                    gene.Value.Mutate(Random.Range(-mutationFactor, +mutationFactor));
                }
                genes.Add(gene.Value);
            }
            AssignSpecialGenes();
        }

        protected void Update()
        {
            // _rigidbody.velocity = Vector3.zero;
            if (_isAwake)
            {
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
                bool hasHitSomething = Physics.Raycast(selfPosition, rayDirection, out RaycastHit raycastHit, sight);
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
                agentAction.CalculateUtility(_genes, _properties, lookData);
            }
            
            Array.Sort(agentActions);
            int index = 0;
            if (agentActions.Length > 1)
            {
                index = (Random.Range(0.0f, 100.0f) < maxUtilityChoiceChance) ? 0 : 1;    
            }
            return agentActions[index].Act(_genes);
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

        public float GetProperty(string property)
        {
            return _properties[property].Value;
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

        public void CalculatePerformance(Environment environment)
        {
            performance = evaluator.EvaluatePerfomance(this, environment);
        }

        /// <summary>
        /// Returns the AgentData of this Agent.
        /// </summary>
        /// <returns></returns>
        public AgentData GetData()
        {
            return new AgentData(genes);
        }

        private void OnCollisionEnter(Collision other)
        {
            colliderHandler.AgentCollisionEnter(_genes, _properties, other);
        }

        private void OnCollisionExit(Collision other)
        {
            colliderHandler.AgentCollisionExit(_genes, _properties, other);
        }

        private void OnCollisionStay(Collision other)
        {
            colliderHandler.AgentCollisionStay(_genes, _properties, other);
        }

        private void OnTriggerEnter(Collider other)
        {
            colliderHandler.AgentTriggerEnter(_genes, _properties, other);
        }

        private void OnTriggerExit(Collider other)
        {
            colliderHandler.AgentTriggerExit(_genes, _properties, other);
        }

        private void OnTriggerStay(Collider other)
        {
            colliderHandler.AgentTriggerStay(_genes, _properties, other);
        }
    }
}