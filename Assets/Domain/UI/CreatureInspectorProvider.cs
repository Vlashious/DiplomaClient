using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Domain.UI
{
    public sealed class CreatureInspectorProvider : MonoBehaviour
    {
        public TMP_Text Name;
        public Slider HealthBar;
        public TMP_Text HealthBarValue;

        public void SetValue(float currentValue, float maxValue)
        {
            HealthBarValue.SetText($"{currentValue}/{maxValue}");
            HealthBar.value = currentValue / maxValue;
        }
    }
}