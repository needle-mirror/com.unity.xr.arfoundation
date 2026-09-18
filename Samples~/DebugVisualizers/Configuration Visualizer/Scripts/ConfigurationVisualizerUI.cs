using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ConfigurationVisualizerUI : MonoBehaviour
    {
        const int k_ColumnsToSkip = 1;

        XRSessionSubsystem m_SessionSubsystem;
        bool m_ConfigVisualizerSetup;

        [SerializeField]
        RectTransform m_RowsRootTransform;

        [SerializeField]
        ConfigurationRowUI m_LabelRow;

        [SerializeField]
        ConfigurationRowUI[] m_Rows;

        [SerializeField]
        ConfigurationRowUI m_PrototypeRow;

        void LateUpdate()
        {
            if (!m_ConfigVisualizerSetup)
            {
                SetupConfigurationDisplay();
                m_ConfigVisualizerSetup = true;
            }
        }

        void SetupConfigurationDisplay()
        {
            m_SessionSubsystem = GetSessionSubsystem();
            if (m_SessionSubsystem == null)
            {
                Debug.LogWarning($"As there is no active XRSessionSubsystem available, the {gameObject.name}'s configuration will not be populated.", this);
                return;
            }

            var supportedConfigIndicesByFeature = new Dictionary<string, int[]>();
            var descriptors = m_SessionSubsystem.GetConfigurationDescriptors(Allocator.Temp);

            for (int i = 0; i < descriptors.Length; i++)
            {
                string capabilities = descriptors[i].capabilities.ToStringList();
                string[] features = capabilities.Split(", ");

                foreach (var feature in features)
                {
                    if (!supportedConfigIndicesByFeature.ContainsKey(feature))
                    {
                        supportedConfigIndicesByFeature.TryAdd(feature, new int[descriptors.Length]);
                    }
                    supportedConfigIndicesByFeature[feature][i] = 1;
                }
            }
            CreateConfigurationEntries(supportedConfigIndicesByFeature, descriptors.Length);
        }

        XRSessionSubsystem GetSessionSubsystem()
        {
            if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
            {
                var loader = XRGeneralSettings.Instance.Manager.activeLoader;
                if (loader != null)
                {
                    return loader.GetLoadedSubsystem<XRSessionSubsystem>();
                }
            }

            return null;
        }

        void CreateConfigurationEntries(Dictionary<string, int[]> graph, int configurationsLength)
        {
            if (m_PrototypeRow == null)
            {
                Debug.LogError($"This is no prototype row set, the {gameObject.name}'s configuration will not be populated.", this);
            }

            if (m_LabelRow != null)
            {
                m_LabelRow.FitColumnCountToConfigurationCount(configurationsLength);
                m_LabelRow.SetEntriesToDisplayNumbers();
            }

            var features = new List<string>(graph.Keys);
            features.Sort();
            foreach (var feature in features)
            {
                var newRow = GameObject.Instantiate(m_PrototypeRow.gameObject,m_RowsRootTransform).GetComponent<ConfigurationRowUI>();
                newRow.gameObject.SetActive(true);

                newRow.SetBackingImageEnabled(newRow.transform.GetSiblingIndex() % 2 == 0);
                newRow.SetTextLabel(feature);

                newRow.FitColumnCountToConfigurationCount(configurationsLength);

                for (int i = 0; i < configurationsLength; i++)
                {
                    bool supported = graph[feature][i] == 1;
                    newRow.SetSpriteForConfiguration(i, supported);
                }
            }
        }
    }
}
