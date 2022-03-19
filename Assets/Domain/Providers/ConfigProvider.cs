using UnityEngine;

namespace Domain.Providers
{
    [CreateAssetMenu(menuName = "Config Provider", fileName = "ConfigProvider", order = 0)]
    public sealed class ConfigProvider : ScriptableObject
    {
        public float PlayerSpeed;
        public int BasePlayerHealth;
    }
}