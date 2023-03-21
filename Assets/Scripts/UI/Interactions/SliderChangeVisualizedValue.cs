using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

namespace UI.Interactions
{
    public class SliderChangeVisualizedValue : MonoBehaviour
    {
        private TextMeshPro _textMesh;

        public void OnSliderUpdated(SliderEventData eventData)
        {
            if (_textMesh == null)
            {
                _textMesh = GetComponent<TextMeshPro>();
            }

            if (_textMesh != null)
            {
                _textMesh.text = $"{eventData.NewValue:F2}";
            }
        }
    }
}
