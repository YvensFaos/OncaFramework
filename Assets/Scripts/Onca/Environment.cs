using System;
using System.Collections.Generic;
using UnityEngine;

namespace Onca
{
    [Serializable]
    public struct EnvironmentProperty
    {
        public string name;
        public float Value { get; set; }

        public EnvironmentProperty(string name, float value)
        {
            this.name = name;
            Value = value;
        }

        public void ChangeValue(float newValue)
        {
            Value = newValue;
        }
    }

    public class Environment : MonoBehaviour
    {
        [SerializeField]
        private EnvironmentProperty[] properties;

        private Dictionary<string, EnvironmentProperty> _environmentProperties;

        public void Awake()
        {
            _environmentProperties = new Dictionary<string, EnvironmentProperty>();

            foreach (EnvironmentProperty environmentProperty in properties)
            {
                _environmentProperties[environmentProperty.name] = environmentProperty;
            }
        }

        public float GetEnvironmentProperty(string propertyName)
        {
            return _environmentProperties[propertyName].Value;
        }
    
        public void UpdateEnvironmentPropertyValue(string propertyName, float newValue)
        {
            _environmentProperties[propertyName].ChangeValue(newValue);
        }
    }
}