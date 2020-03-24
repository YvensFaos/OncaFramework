using System.Collections.Generic;
using UnityEngine;

namespace Onca
{
    [RequireComponent(typeof(Collider))]
    public abstract class AgentColliderHandler : MonoBehaviour
    {
        public virtual void AgentCollisionEnter(Dictionary<string, Gene> genes,
            Dictionary<string, AgentProperty> properties, Collision other)
        {
        }

        public virtual void AgentCollisionStay(Dictionary<string, Gene> genes,
            Dictionary<string, AgentProperty> properties, Collision other)
        {
        }

        public virtual void AgentCollisionExit(Dictionary<string, Gene> genes,
            Dictionary<string, AgentProperty> properties, Collision other)
        {
        }

        public virtual void AgentTriggerEnter(Dictionary<string, Gene> genes,
            Dictionary<string, AgentProperty> properties, Collider other)
        {
        }

        public virtual void AgentTriggerStay(Dictionary<string, Gene> genes,
            Dictionary<string, AgentProperty> properties, Collider other)
        {
        }

        public virtual void AgentTriggerExit(Dictionary<string, Gene> genes,
            Dictionary<string, AgentProperty> properties, Collider other)
        {
        }
    }
}