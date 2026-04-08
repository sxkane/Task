using UnityEngine;

namespace Weapons
{
    public class Effect : ScriptableObject
    {
        [Header("Presentation")]
        [SerializeField] private string displayName;
        [SerializeField] private string description;

        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;

        public virtual void ExecuteEffect()
        {
            Debug.Log("Effect executed");
        }

        public virtual string BuildDescription()
        {
            if (!string.IsNullOrWhiteSpace(description))
                return description.Trim();

            return DisplayName;
        }
    }
}
