using Domain.Player;
using UnityEngine;

namespace Domain.Providers
{
    [CreateAssetMenu(menuName = "Prefab provider", fileName = "PrefabProvider", order = 0)]
    public sealed class PrefabProvider : ScriptableObject
    {
        public PlayerProvider Player;
    }
}