using UnityEngine;

namespace Onca
{
    public abstract class PerformanceEvaluator : MonoBehaviour
    {
        public abstract float EvaluatePerfomance(AgentLogic agent, Environment environment);
    }
}