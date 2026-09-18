using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// A class representing a row in a configuration visualizer, which holds a label and entries
    /// This class must be placed on the root transform of the row
    /// </summary>
    public class ConfigurationRowUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text m_RowLabel;

        [SerializeField]
        Image m_BackingImage;

        [SerializeField]
        List<ConfigurationEntryUI> m_ConfigurationEntries;

        [SerializeField]
        ConfigurationEntryUI m_PrototypeConfigurationEntry;

        public void SetBackingImageEnabled(bool enabled)
        {
            if (m_BackingImage != null)
                m_BackingImage.enabled = enabled;
        }

        public void SetSpriteForConfiguration(int configurationIndex, bool enabled)
        {
            if (configurationIndex >= m_ConfigurationEntries.Count)
                return;

            m_ConfigurationEntries[configurationIndex].SetSpriteForConfiguration(enabled);
        }

        public void FitColumnCountToConfigurationCount(int configurationCount)
        {
            if (m_ConfigurationEntries.Count == configurationCount)
                return;

            if (m_ConfigurationEntries.Count < configurationCount)
            {
                for (int i = m_ConfigurationEntries.Count; i < configurationCount; i++)
                {
                    var entry = GameObject.Instantiate(m_PrototypeConfigurationEntry.gameObject, transform).GetComponent<ConfigurationEntryUI>();
                    m_ConfigurationEntries.Add(entry);
                }
            }
            else
            {
                for (int i = m_ConfigurationEntries.Count; i > configurationCount; i--)
                    m_ConfigurationEntries[i-1].gameObject.SetActive(false);
            }
        }

        public void SetEntriesToDisplayNumbers()
        {
            for (int i = 0; i < m_ConfigurationEntries.Count; i++)
            {
                var entry = m_ConfigurationEntries[i];
                entry.SetDisplayText((i + 1).ToString());
                entry.SetTextEnabled(true);
            }
        }

        public void SetTextLabel(string labelText)
        {
            if (m_RowLabel != null)
                m_RowLabel.text = labelText;
        }
    }
}
