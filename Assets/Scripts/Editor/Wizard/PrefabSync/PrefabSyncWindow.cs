using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sc.Editor.Wizard.PrefabSync
{
    /// <summary>
    /// 프리팹 ↔ 코드 동기화를 위한 통합 에디터 윈도우.
    /// </summary>
    public class PrefabSyncWindow : EditorWindow
    {
        private const string PREFAB_FOLDER = "Assets/Prefabs/UI";
        private const string SPEC_FOLDER = "Assets/Scripts/Editor/Wizard/PrefabSync/Specs";
        private const string BUILDER_FOLDER = "Assets/Scripts/Editor/Wizard/Generators";

        private GameObject _selectedPrefab;
        private TextAsset _selectedSpec;
        private Vector2 _scrollPos;
        private string _lastAnalyzedPath;
        private string _lastGeneratedPath;
        private string _logMessage;
        private MessageType _logType;

        [MenuItem("Tools/ProjectSC/PrefabSync/Sync Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<PrefabSyncWindow>("Prefab Sync");
            window.minSize = new Vector2(400, 500);
        }

        private void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            DrawHeader();
            DrawPrefabSection();
            DrawSpecSection();
            DrawWorkflowSection();
            DrawLogSection();

            EditorGUILayout.EndScrollView();
        }

        #region UI Sections

        private void DrawHeader()
        {
            EditorGUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label("Prefab ↔ Code Sync System", headerStyle);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.HelpBox(
                "프리팹 구조를 분석하여 JSON Spec을 생성하고,\n" +
                "Spec으로부터 Builder 코드를 자동 생성합니다.",
                MessageType.Info);

            EditorGUILayout.Space(10);
        }

        private void DrawPrefabSection()
        {
            EditorGUILayout.LabelField("1. Prefab → JSON Spec", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            // Prefab 선택
            _selectedPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Target Prefab",
                _selectedPrefab,
                typeof(GameObject),
                false);

            EditorGUILayout.Space(5);

            // 빠른 선택 버튼
            EditorGUILayout.LabelField("Quick Select:", EditorStyles.miniLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("LobbyScreen", EditorStyles.miniButton))
            {
                _selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Prefabs/UI/Screens/LobbyScreen.prefab");
            }

            if (GUILayout.Button("TitleScreen", EditorStyles.miniButton))
            {
                _selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Prefabs/UI/Screens/TitleScreen.prefab");
            }

            if (GUILayout.Button("Browse...", EditorStyles.miniButton))
            {
                var path = EditorUtility.OpenFilePanel("Select Prefab", PREFAB_FOLDER, "prefab");
                if (!string.IsNullOrEmpty(path))
                {
                    path = "Assets" + path.Substring(Application.dataPath.Length);
                    _selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Analyze 버튼
            GUI.enabled = _selectedPrefab != null;
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);

            if (GUILayout.Button("Analyze Prefab → Generate JSON Spec", GUILayout.Height(30)))
            {
                AnalyzePrefab();
            }

            GUI.backgroundColor = Color.white;
            GUI.enabled = true;

            // 결과 표시
            if (!string.IsNullOrEmpty(_lastAnalyzedPath))
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Last Generated:", EditorStyles.miniLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField(_lastAnalyzedPath, EditorStyles.miniTextField);

                if (GUILayout.Button("Open", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(_lastAnalyzedPath);
                    if (asset != null)
                    {
                        Selection.activeObject = asset;
                        EditorGUIUtility.PingObject(asset);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
        }

        private void DrawSpecSection()
        {
            EditorGUILayout.LabelField("2. JSON Spec → Builder Code", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            // Spec 선택
            _selectedSpec = (TextAsset)EditorGUILayout.ObjectField(
                "Spec File (.json)",
                _selectedSpec,
                typeof(TextAsset),
                false);

            EditorGUILayout.Space(5);

            // Spec 파일 목록
            EditorGUILayout.LabelField("Available Specs:", EditorStyles.miniLabel);

            if (Directory.Exists(SPEC_FOLDER))
            {
                var specFiles = Directory.GetFiles(SPEC_FOLDER, "*.structure.json");

                EditorGUILayout.BeginHorizontal();

                foreach (var file in specFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file).Replace(".structure", "");

                    if (GUILayout.Button(fileName, EditorStyles.miniButton))
                    {
                        _selectedSpec = AssetDatabase.LoadAssetAtPath<TextAsset>(file);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("No spec files found. Analyze a prefab first.", MessageType.Warning);
            }

            EditorGUILayout.Space(10);

            // Generate 버튼
            GUI.enabled = _selectedSpec != null;
            GUI.backgroundColor = new Color(0.4f, 0.6f, 1f);

            if (GUILayout.Button("Generate Builder Code from Spec", GUILayout.Height(30)))
            {
                GenerateBuilder();
            }

            GUI.backgroundColor = Color.white;
            GUI.enabled = true;

            // 결과 표시
            if (!string.IsNullOrEmpty(_lastGeneratedPath))
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Last Generated:", EditorStyles.miniLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField(_lastGeneratedPath, EditorStyles.miniTextField);

                if (GUILayout.Button("Open", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(_lastGeneratedPath);
                    if (asset != null)
                    {
                        AssetDatabase.OpenAsset(asset);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
        }

        private void DrawWorkflowSection()
        {
            EditorGUILayout.LabelField("3. One-Click Workflow", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.HelpBox(
                "선택된 프리팹을 분석하고 Builder 코드까지 한 번에 생성합니다.",
                MessageType.None);

            GUI.enabled = _selectedPrefab != null;
            GUI.backgroundColor = new Color(1f, 0.8f, 0.4f);

            if (GUILayout.Button("Full Sync: Prefab → Spec → Builder", GUILayout.Height(35)))
            {
                FullSync();
            }

            GUI.backgroundColor = Color.white;
            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        private void DrawLogSection()
        {
            if (!string.IsNullOrEmpty(_logMessage))
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox(_logMessage, _logType);
            }
        }

        #endregion

        #region Actions

        private void AnalyzePrefab()
        {
            if (_selectedPrefab == null) return;

            var prefabPath = AssetDatabase.GetAssetPath(_selectedPrefab);

            try
            {
                _lastAnalyzedPath = PrefabStructureAnalyzer.Analyze(prefabPath);

                if (!string.IsNullOrEmpty(_lastAnalyzedPath))
                {
                    _selectedSpec = AssetDatabase.LoadAssetAtPath<TextAsset>(_lastAnalyzedPath);
                    Log($"Spec generated: {_lastAnalyzedPath}", MessageType.Info);
                }
                else
                {
                    Log("Failed to analyze prefab", MessageType.Error);
                }
            }
            catch (System.Exception e)
            {
                Log($"Error: {e.Message}", MessageType.Error);
                Debug.LogException(e);
            }
        }

        private void GenerateBuilder()
        {
            if (_selectedSpec == null) return;

            var specPath = AssetDatabase.GetAssetPath(_selectedSpec);

            try
            {
                _lastGeneratedPath = PrefabBuilderGenerator.Generate(specPath);

                if (!string.IsNullOrEmpty(_lastGeneratedPath))
                {
                    Log($"Builder generated: {_lastGeneratedPath}", MessageType.Info);
                }
                else
                {
                    Log("Failed to generate builder", MessageType.Error);
                }
            }
            catch (System.Exception e)
            {
                Log($"Error: {e.Message}", MessageType.Error);
                Debug.LogException(e);
            }
        }

        private void FullSync()
        {
            if (_selectedPrefab == null) return;

            Log("Starting full sync...", MessageType.Info);

            // Step 1: Analyze
            AnalyzePrefab();

            if (_selectedSpec == null)
            {
                Log("Sync failed at analyze step", MessageType.Error);
                return;
            }

            // Step 2: Generate
            GenerateBuilder();

            if (!string.IsNullOrEmpty(_lastGeneratedPath))
            {
                Log($"Full sync completed!\nSpec: {_lastAnalyzedPath}\nBuilder: {_lastGeneratedPath}",
                    MessageType.Info);
            }
        }

        private void Log(string message, MessageType type)
        {
            _logMessage = message;
            _logType = type;
            Repaint();
        }

        #endregion
    }
}