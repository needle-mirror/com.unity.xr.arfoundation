using UnityEngine.UI;
using TMPro;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// A class representing a single entry in a configuration visualizer
    /// This class must be placed on the root transform of the entry
    /// </summary>
    public class ConfigurationEntryUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text m_TextDisplay;

        [SerializeField]
        Image m_BackingImage;

        [SerializeField]
        GameObject m_EnabledIndicator;

        [SerializeField]
        GameObject m_DisabledIndicator;

        public void SetSpriteForConfiguration(bool enabled)
        {
            m_EnabledIndicator?.SetActive(enabled);
            m_DisabledIndicator?.SetActive(!enabled);
        }

        public void SetBackingImageEnabled(bool enabled)
        {
            if (m_BackingImage != null)
                m_BackingImage.enabled = enabled;
        }

        public void SetDisplayText(string text)
        {
            if (m_TextDisplay != null)
                m_TextDisplay.text = text;
        }

        public void SetTextEnabled(bool enabled)
        {
            if (m_TextDisplay != null)
                m_TextDisplay.enabled = enabled;
        }
    }
}
