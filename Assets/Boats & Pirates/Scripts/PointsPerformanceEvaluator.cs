using Onca;
using UnityEngine;

public class PointsPerformanceEvaluator : PerformanceEvaluator
{
    public override float EvaluatePerfomance(AgentLogic agent, Environment environment)
    {
        return agent.GetProperty("points");
    }
}