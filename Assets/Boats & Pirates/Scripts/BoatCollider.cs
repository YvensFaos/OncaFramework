using System.Collections.Generic;
using Onca;
using UnityEngine;

public class BoatCollider : AgentColliderHandler
{
    public override void AgentTriggerEnter(Dictionary<string, Gene> genes, Dictionary<string, AgentProperty> properties,
        Collider other)
    {
        if (other.gameObject.CompareTag("BPBox"))
        {
            properties["points"].IncrementValue(1.0f);
            Destroy(other.gameObject);
        }
    }
}