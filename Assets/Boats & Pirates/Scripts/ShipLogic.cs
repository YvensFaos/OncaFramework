using System.Transactions;
using Onca;

public class ShipLogic : AgentLogic
{
    protected override void CreateProperties()
    {
        if (!_properties.ContainsKey("points"))
        {
            AgentProperty points = new AgentProperty("points", 0.0f);
            properties.Add(points);
            _properties["points"] = points;
        }
    }
    
    protected override void CalculateProperties()
    {
        _properties["points"].SetValue(0.0f);
    }
}