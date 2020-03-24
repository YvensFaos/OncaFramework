using System;

namespace Onca
{
    [Serializable]
    public class Gene
    {
        public string Name;
        public float Value;

        public Gene(string name, float value)
        {
            Name = name;
            Value = value;
        }

        public Gene(Gene anotherGene)
        {
            Name = anotherGene.Name;
            Value = anotherGene.Value;
        }

        public void Mutate(float mutateBy)
        {
            Value += mutateBy;
        }
    }
}
