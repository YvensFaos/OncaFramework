using UnityEngine;

namespace Onca
{
    public struct AgentLookData
    {
        public Vector3 RayDirection { get; }
        public bool HasHitSomething { get; }
        public RaycastHit Hit { get; }

        public AgentLookData(Vector3 rayDirection, bool hasHitSomething, RaycastHit raycastHit)
        {
            RayDirection = rayDirection;
            HasHitSomething = hasHitSomething;
            Hit = raycastHit;
        }
    }
}