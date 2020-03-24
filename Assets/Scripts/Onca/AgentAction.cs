using System;
using System.Collections.Generic;
using UnityEngine;

namespace Onca
{
    [RequireComponent(typeof(AgentProgram))]
    public abstract class AgentAction : MonoBehaviour, IComparable
    {
        [SerializeField]
        protected AgentProgram agentProgram;
        [SerializeField]
        protected float currentUtility;

        protected void Awake()
        {
            if (agentProgram == null)
            {
                Debug.LogError(name + " has an AgentAction without a proper AgentProgram.");
            }
        }
    
        public abstract float CalculateUtility(Dictionary<string, Gene> genes, Dictionary<string, AgentProperty> properties, List<AgentLookData> lookData);

        public abstract Vector3 Act(Dictionary<string, Gene> genes);
    
        public float GetCurrentUtility()
        {
            return currentUtility;
        }
    
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            AgentAction otherAgentAction = obj as AgentAction;
            if (otherAgentAction != null)
            {
                return otherAgentAction.GetCurrentUtility().CompareTo(GetCurrentUtility());
            }
            throw new ArgumentException("Object is not an AgentAction");
        }
    }
}