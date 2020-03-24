using System.Collections.Generic;
using UnityEngine;

namespace Onca
{
    public abstract class AgentProgram : MonoBehaviour
    {
        public abstract float UtilityFunction(Dictionary<string, Gene> genes, AgentLookData lookData);
    }
}