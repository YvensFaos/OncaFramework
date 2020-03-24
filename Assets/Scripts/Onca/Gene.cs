using System;

namespace Onca
{
    [Serializable]
    public struct Gene
    {
        public string Name { get; }
        public float Value { get; private set; }

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
