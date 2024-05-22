using System;
using System.IO;
using UnityEngine;

namespace UnityEditor.XR.ARSubsystems
{
    /// <summary>
    /// An Editor window that can assist you to correctly build AssetBundles containing AR Foundation data.
    /// </summary>
    /// <remarks>
    /// If your AssetBundle contains any reference image library data, you must call <see cref="ARBuildProcessor.PreprocessBuild"/>
    /// before you call <see cref="BuildPipeline.BuildAssetBundles(string, BuildAssetBundleOptions, BuildTarget)"/>.
    /// This Editor window allows you to abide by the required API contract without writing any custom code.
    /// </remarks>
    class BuildAssetBundlesWindow : EditorWindow
    {
        const string k_EditorPrefsAssetBundleOutputDirectoryKey = "UnityEditor.XR.ARSubsystems.BuildAssetBundlesWindow.OutputDirectory";

        const int k_WindowWidth = 500;
        const int k_WindowHeight = 120;
        static readonly Vector2 k_WindowDimensions = new(k_WindowWidth, k_WindowHeight);

        const float k_IndentWidth = 10f;
        const float k_LabelWidth = 144f;
        const float k_FolderButtonWidth = 25f;

        // Can't initialize these Rects until OnEnable because it's unsafe to access necessary Editor APIs until then
        static Rect s_BuildForTargetLabelRect;
        static Rect s_BuildForTargetFieldRect;
        static Rect s_OutputDirectoryLabelRect;
        static Rect s_OutputDirectoryFieldRect;
        static Rect s_OutputDirectoryButtonRect;
        static Rect s_CleanOutputDirectoryLabelRect;
        static Rect s_CleanOutputDirectoryToggleRect;
        static Rect s_BuildAssetBundlesButtonRect;

        static readonly GUIContent k_BuildForTargetContent = new(
            "Build for Target",
            "The target platform for the AssetBundles.");

        static readonly GUIContent k_OutputDirectoryContent = new(
            "Output Directory",
            "The directory in which to save the built AssetBundles.");

        static readonly GUIContent k_CleanOutputDirectoryContent = new(
            "Clean Output Directory",
            "If enabled, delete the contents of the output directory before building AssetBundles.");

        static Texture2D s_FolderIcon;

        static readonly string[] k_SupportedBuildTargets = { nameof(BuildTarget.Android), nameof(BuildTarget.iOS) };
        static readonly int[] k_SupportedBuildTargetValues = { (int)BuildTarget.Android, (int)BuildTarget.iOS };

        static readonly string k_ProjectRepository = Directory.GetParent(Application.dataPath)!.ToString();

        static bool staticsAreInitialized => s_BuildForTargetLabelRect != default;

        [SerializeField]
        BuildTarget m_BuildForTarget;

        [SerializeField]
        string m_OutputDirectory;

        [SerializeField]
        bool m_CleanOutputDirectory;

        [MenuItem("Assets/AR Foundation/Build AssetBundles...", false, 700)]
        static void ShowWindow()
        {
            BuildAssetBundlesWindow window = GetWindow<BuildAssetBundlesWindow>();
            window.titleContent = new GUIContent("Build AssetBundles");
            window.minSize = k_WindowDimensions;
            window.maxSize = k_WindowDimensions;
        }

        void OnEnable()
        {
            m_BuildForTarget = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS ? BuildTarget.iOS : BuildTarget.Android;
            m_OutputDirectory = EditorPrefs.GetString(k_EditorPrefsAssetBundleOutputDirectoryKey);

            if (!staticsAreInitialized)
                InitStatics();
        }

        static void InitStatics()
        {
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var lineSpace = EditorGUIUtility.standardVerticalSpacing;
            var safeWidth = k_WindowWidth - 2 * k_IndentWidth;
            var x = k_IndentWidth;
            var y = lineSpace;
            var horizontalSpace = 2;

            s_BuildForTargetLabelRect = new Rect(x, y, k_LabelWidth, lineHeight);
            s_BuildForTargetFieldRect = new Rect(
                s_BuildForTargetLabelRect.xMax + horizontalSpace,
                y,
                safeWidth - k_LabelWidth - horizontalSpace,
                lineHeight);

            y += lineHeight + lineSpace;

            s_OutputDirectoryLabelRect = new Rect(x, y, k_LabelWidth, lineHeight);
            s_OutputDirectoryFieldRect = new Rect(
                s_OutputDirectoryLabelRect.xMax + horizontalSpace,
                y,
                safeWidth - k_LabelWidth - k_FolderButtonWidth - 2 * horizontalSpace,
                lineHeight);
            s_OutputDirectoryButtonRect = new Rect(s_OutputDirectoryFieldRect.xMax + horizontalSpace, y, k_FolderButtonWidth, lineHeight);

            s_FolderIcon = EditorGUIUtility.IconContent("d_Folder Icon").image as Texture2D;

            y += lineHeight + lineSpace;

            s_CleanOutputDirectoryLabelRect = new Rect(x, y, k_LabelWidth, lineHeight);
            s_CleanOutputDirectoryToggleRect = new Rect(
                s_CleanOutputDirectoryLabelRect.xMax + horizontalSpace,
                y,
                lineHeight,
                lineHeight);

            y += lineHeight + lineSpace;

            s_BuildAssetBundlesButtonRect = new Rect(x, y + 10, safeWidth, 2 * lineHeight);
        }

        void OnGUI()
        {
            EditorGUI.LabelField(s_BuildForTargetLabelRect, k_BuildForTargetContent);

            // Use an IntPopup to avoid boxing operations when working with Enum types
            m_BuildForTarget = (BuildTarget)EditorGUI.IntPopup(
                s_BuildForTargetFieldRect,
                (int)m_BuildForTarget,
                k_SupportedBuildTargets,
                k_SupportedBuildTargetValues);

            EditorGUI.LabelField(s_OutputDirectoryLabelRect, k_OutputDirectoryContent);
            m_OutputDirectory = EditorGUI.TextField(s_OutputDirectoryFieldRect, m_OutputDirectory);
            if (GUI.Button(s_OutputDirectoryButtonRect, s_FolderIcon))
            {
                string defaultDirectory = Directory.Exists(m_OutputDirectory) ? m_OutputDirectory : k_ProjectRepository;
                var outputDir = EditorUtility.OpenFolderPanel(
                "Choose the AssetBundles output directory",
                defaultDirectory,
                string.Empty);

                if (!string.IsNullOrEmpty(outputDir))
                    m_OutputDirectory = outputDir;
            }

            EditorGUI.LabelField(s_CleanOutputDirectoryLabelRect, k_CleanOutputDirectoryContent);
            m_CleanOutputDirectory = EditorGUI.Toggle(s_CleanOutputDirectoryToggleRect, m_CleanOutputDirectory);

            if (GUI.Button(s_BuildAssetBundlesButtonRect, "Build AssetBundles"))
            {
                BuildAssetBundles();
            }
        }

        void BuildAssetBundles()
        {
            if (!Directory.Exists(m_OutputDirectory))
            {
                Directory.CreateDirectory(m_OutputDirectory);
            }
            else if (m_CleanOutputDirectory)
            {
                Directory.Delete(m_OutputDirectory, true);
                Directory.CreateDirectory(m_OutputDirectory);
            }

            EditorPrefs.SetString(k_EditorPrefsAssetBundleOutputDirectoryKey, m_OutputDirectory);

            ARBuildProcessor.PreprocessBuild(m_BuildForTarget);
            var manifest = BuildPipeline.BuildAssetBundles(
                m_OutputDirectory,
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                m_BuildForTarget);
            if (manifest == null)
            {
                Debug.LogWarning("Cannot build AssetBundles because no Assets in the project are assigned to an AssetBundle.");
                return;
            }

            var count = manifest.GetAllAssetBundles()?.Length ?? 0;

            // Refresh the Project window in case AssetBundles were exported within unity project.
            AssetDatabase.Refresh();

            Debug.Log($"Wrote {count} asset bundle{(count == 1 ? "" : "s")} to {m_OutputDirectory}");
        }
    }
}
