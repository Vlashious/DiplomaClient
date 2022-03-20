using UnityEngine;

namespace Domain.UI
{
    public sealed class UIProvider : MonoBehaviour
    {
        public CreatureInspectorProvider PlayerInspectorProvider;
        public CreatureInspectorProvider CreatureInspectorProvider;
        public AbilityButtonProvider FirstAbility;
        public AbilityButtonProvider SecondAbility;
        public AbilityButtonProvider SpecialAbility;
    }
}