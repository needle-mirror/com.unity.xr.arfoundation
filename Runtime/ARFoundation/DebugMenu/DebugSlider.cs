using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Slider used for debug options in <see cref="ARDebugMenu"/>.
    /// </summary>
    [Obsolete("DebugSlider is deprecated in AR Foundation 6.7, and will not be replaced by any new API.")]
    [HelpUrl(typeof(ARDebugMenu))]
    public class DebugSlider: Slider
    {
        /// <summary>
        /// Use this callback to detect pointer down events.
        /// </summary>
        /// <param name="eventData">The pointer down event data.</param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if(interactable)
            {
                value = (value == 1) ? 0 : 1;
            }
        }
    }
}
