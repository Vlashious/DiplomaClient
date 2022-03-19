using UnityEngine;

namespace Domain.UI
{
    [CreateAssetMenu]
    public sealed class UIProvider : ScriptableObject
    {
        public CreatureInspectorProvider CreatureInspectorProvider;
    }
}