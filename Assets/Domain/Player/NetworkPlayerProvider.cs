using Domain.UI;
using UnityEngine;

namespace Domain.Player
{
    public sealed class NetworkPlayerProvider : MonoBehaviour
    {
        public PlayerProvider PlayerProvider;
        public CreatureInspectorProvider Inspector;
    }
}