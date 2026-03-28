using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    [Serializable]
    public class Stat
    {
        [SerializeField] private float baseValue;
        private List<Modifier> _modifiers = new();
        
        public event Action OnValueChanged;
        
        public Stat(float baseValue)
        {
            this.baseValue = baseValue;
        }
        
        public float Value
        {
            get
            {
                float flat = baseValue;
                float percentAdd = 0;
                float percentMult = 1;

                foreach (var mod in _modifiers)
                {
                    switch (mod.type)
                    {
                        case StatModType.Flat:
                            flat += mod.value;
                            break;

                        case StatModType.PercentAdd:
                            percentAdd += mod.value;
                            break;

                        case StatModType.PercentMult:
                            percentMult *= 1 + mod.value;
                            break;
                    }
                }

                return flat * (1 + percentAdd) * percentMult;
            }
            set
            {
                if (!Mathf.Approximately(baseValue, value))
                    baseValue = value;
            }
        }

        public void AddModifier(Modifier mod)
        {
            _modifiers.Add(mod);
            OnValueChanged?.Invoke();
        }

        public void RemoveModifier(Modifier mod)
        {
            _modifiers.Remove(mod);
            OnValueChanged?.Invoke();
        }

        public void RemoveModifiersFromSource(object source)
        {
            _modifiers.RemoveAll(m => m.Source == source);
            OnValueChanged?.Invoke();
        }
    }
}