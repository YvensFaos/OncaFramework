using System.Collections.Generic;
using UnityEngine;

namespace Onca
{
    [RequireComponent(typeof(Rigidbody))]
    public class MoveAction : AgentAction
    {
        private Rigidbody _rigidbody;
        private AgentLookData _highestData;
    
        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override float CalculateUtility(Dictionary<string, Gene> genes, List<AgentLookData> lookData)
        {
            float highestUtility = float.MinValue;
            lookData.ForEach(data =>
            {
                float utility = agentProgram.UtilityFunction(genes, data);
                if (utility > highestUtility)
                {
                    highestUtility = utility;
                    _highestData = data;
                }
            });
            return highestUtility;
        }

        public override Vector3 Act(Dictionary<string, Gene> genes)
        {
            Vector3 direction = _highestData.RayDirection;
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(direction), 0.1f);
            _rigidbody.velocity = direction * genes["movingSpeed"].Value;
        
            return _highestData.RayDirection;
        }
    }
}
