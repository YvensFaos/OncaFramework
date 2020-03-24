using System.Collections.Generic;
using Onca;
using UnityEngine;

public class PirateCollider : AgentColliderHandler
{
    public override void AgentCollisionEnter(Dictionary<string, Gene> genes,
        Dictionary<string, AgentProperty> properties, Collision other)
    {
        if (other.gameObject.CompareTag("BPBoat"))
        {
            properties["points"].IncrementValue(5.0f);
            Destroy(other.gameObject);
        }
    }

    public override void AgentTriggerEnter(Dictionary<string, Gene> genes, Dictionary<string, AgentProperty> properties,
        Collider other)
    {
        if (other.gameObject.CompareTag("BPBox"))
        {
            properties["points"].IncrementValue(0.1f);
            Destroy(other.gameObject);
        }
    }
}