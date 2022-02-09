using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Slider used for debug options in <see cref="ARDebugMenu"/>.
    /// </summary>
    public class DebugSlider: Slider
    {
        /// <inheritdoc />
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if(value == 1)
            {
                value = 0;
            }
            else
            {
                value = 1;
            }
        }
    }
}
