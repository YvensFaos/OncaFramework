using System.Collections.Generic;
using Onca;
using UnityEngine;

public class ShipProgram : AgentProgram
{
    public override float UtilityFunction(Dictionary<string, Gene> genes, Dictionary<string, AgentProperty> properties,
        AgentLookData lookData)
    {
        RaycastHit raycastHit = lookData.Hit;
        float utility = -1.0f;
        
        if (raycastHit.transform != null)
        {
            string gameObjectTag = raycastHit.transform.gameObject.tag;
            
            float distance = raycastHit.distance;
            float distanceFactor = 1.0f - (distance / genes["sight"].Value);
            
            switch (gameObjectTag)
            {
                case "BPBox":
                    utility = distanceFactor * genes["Dbox"].Value + genes["Wbox"].Value;
                    break;
                case "BPBoat":
                    utility = distanceFactor * genes["Dboat"].Value + genes["Wboat"].Value;
                    break;
                case "BPPirate":
                    utility = distanceFactor * genes["Dpirate"].Value + genes["Wpirate"].Value;
                    break;
            } 
            Debug.Log("Here! Found a " + gameObjectTag + " with U(x) of " + utility);
        }
        return utility;
    }
}