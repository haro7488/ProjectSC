using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Sc.Editor.Wizard.PrefabSync
{
    /// <summary>
    /// 프리팹 ↔ 코드 동기화를 위한 통합 에디터 윈도우.
    /// Single 모드: 개별 프리팹 동기화
    /// Batch 모드: 폴더 내 프리팹 일괄 처리
    /// </summary>
    public class PrefabSyncWindow : EditorWindow
    {
        private const string PREFAB_FOLDER = "Assets/Prefabs/UI";
        private const string SCREENS_FOLDER = PREFAB_FOLDER + "/Screens";
        private const string POPUPS_FOLDER = PREFAB_FOLDER + "/Popups";
        private const string SPEC_FOLDER = "Assets/Scripts/Editor/Wizard/PrefabSync/Specs";
        private const string BUILDER_FOLDER = "Assets/Scripts/Editor/Wizard/Generators";

        private enum TabMode { Single, Batch }
        private TabMode _currentTab = TabMode.Batch;

        // Single mode
        private GameObject _selectedPrefab;
        private TextAsset _selectedSpec;
        private string _lastAnalyzedPath;
        private string _lastGeneratedPath;

        // Batch mode
        private List<PrefabEntry> _screenPrefabs = new();
        private List<PrefabEntry> _popupPrefabs = new();
        private bool _screensFoldout = true;
        private bool _popupsFoldout = true;

        // Common
        private Vector2 _scrollPos;
        private string _logMessage;
        private MessageType _logType;

        private class PrefabEntry
        {
            public string Path;
            public string Name;
            public bool Selected;
            public bool HasSpec;
            public bool HasBuilder;
            public bool HasManualBuilder; // 수동 빌더 존재 여부
        }

        private class ManualBuilderEntry
        {
            public PrefabBuilderRegistry.BuilderInfo Builder;
            public bool Selected;
        }

        // Manual Builder mode
        private List<ManualBuilderEntry> _manualBuilders = new();
        private bool _manualBuildersFoldout = true;

        [MenuItem("Tools/ProjectSC/PrefabSync/Sync Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<PrefabSyncWindow>("Prefab Sync");
            window.minSize = new Vector2(450, 600);
        }

        private void OnEnable()
        {
            RefreshPrefabList();
            RefreshManualBuilderList();
        }

        private void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            DrawHeader();
            DrawTabs();

            if (_currentTab == TabMode.Single)
            {
                DrawSingleMode();
            }
            else
            {
                DrawBatchMode();
            }

            DrawLogSection();

            EditorGUILayout.EndScrollView();
        }

        #region Header & Tabs

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

        private void DrawTabs()
        {
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = _currentTab == TabMode.Single ? Color.white : Color.gray;
            if (GUILayout.Button("Single Prefab", GUILayout.Height(25)))
            {
                _currentTab = TabMode.Single;
            }

            GUI.backgroundColor = _currentTab == TabMode.Batch ? Color.white : Color.gray;
            if (GUILayout.Button("Batch Sync", GUILayout.Height(25)))
            {
                _currentTab = TabMode.Batch;
                RefreshPrefabList();
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
        }

        #endregion

        #region Single Mode

        private void DrawSingleMode()
        {
            DrawPrefabSection();
            DrawSpecSection();
            DrawWorkflowSection();
            DrawSingleManualBuilderSection();
        }

        private void DrawSingleManualBuilderSection()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("4. Manual Builder", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            // 선택된 프리팹에 해당하는 Manual Builder가 있는지 확인
            string prefabName = _selectedPrefab != null ? _selectedPrefab.name : null;
            PrefabBuilderRegistry.BuilderInfo builderInfo = null;

            if (!string.IsNullOrEmpty(prefabName))
            {
                builderInfo = ManualBuilderExecutor.FindBuilderForPrefab(prefabName);
            }

            if (builderInfo != null)
            {
                EditorGUILayout.LabelField($"Found: {builderInfo.TypeName}", EditorStyles.miniLabel);

                EditorGUILayout.Space(5);

                // Build from Manual → Prefab → JSON
                GUI.backgroundColor = new Color(0.6f, 0.6f, 1f);
                if (GUILayout.Button("Build from Manual → Prefab + JSON", GUILayout.Height(28)))
                {
                    BuildSingleFromManual(builderInfo, generateCode: false);
                }

                EditorGUILayout.Space(3);

                // Full Pipeline
                GUI.backgroundColor = new Color(0.8f, 0.4f, 1f);
                if (GUILayout.Button("Full Pipeline → Prefab + JSON + Generated", GUILayout.Height(30)))
                {
                    BuildSingleFromManual(builderInfo, generateCode: true);
                }

                GUI.backgroundColor = Color.white;
            }
            else
            {
                EditorGUILayout.HelpBox(
                    prefabName != null
                        ? $"No manual builder found for '{prefabName}'"
                        : "Select a prefab to check for manual builder",
                    MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void BuildSingleFromManual(PrefabBuilderRegistry.BuilderInfo builderInfo, bool generateCode)
        {
            try
            {
                if (generateCode)
                {
                    var (buildResult, jsonPath, generatedPath) =
                        ManualBuilderExecutor.ExecuteFullPipeline(builderInfo, overwritePrefab: true);

                    if (buildResult.Success)
                    {
                        _lastAnalyzedPath = jsonPath;
                        _lastGeneratedPath = generatedPath;

                        // 프리팹 새로고침
                        _selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(buildResult.PrefabPath);
                        if (!string.IsNullOrEmpty(jsonPath))
                        {
                            _selectedSpec = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
                        }

                        Log($"Full pipeline completed!\nPrefab: {buildResult.PrefabPath}\nJSON: {jsonPath}\nGenerated: {generatedPath}",
                            MessageType.Info);
                    }
                    else
                    {
                        Log($"Failed: {buildResult.ErrorMessage}", MessageType.Error);
                    }
                }
                else
                {
                    var (buildResult, jsonPath) =
                        ManualBuilderExecutor.ExecuteBuilderAndAnalyze(builderInfo, overwritePrefab: true);

                    if (buildResult.Success)
                    {
                        _lastAnalyzedPath = jsonPath;

                        _selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(buildResult.PrefabPath);
                        if (!string.IsNullOrEmpty(jsonPath))
                        {
                            _selectedSpec = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
                        }

                        Log($"Build completed!\nPrefab: {buildResult.PrefabPath}\nJSON: {jsonPath}",
                            MessageType.Info);
                    }
                    else
                    {
                        Log($"Failed: {buildResult.ErrorMessage}", MessageType.Error);
                    }
                }

                AssetDatabase.Refresh();
            }
            catch (System.Exception e)
            {
                Log($"Error: {e.Message}", MessageType.Error);
                Debug.LogException(e);
            }
        }

        private void DrawPrefabSection()
        {
            EditorGUILayout.LabelField("1. Prefab → JSON Spec", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            _selectedPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Target Prefab",
                _selectedPrefab,
                typeof(GameObject),
                false);

            EditorGUILayout.Space(5);

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

            GUI.enabled = _selectedPrefab != null;
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);

            if (GUILayout.Button("Analyze Prefab → Generate JSON Spec", GUILayout.Height(30)))
            {
                AnalyzePrefab(_selectedPrefab);
            }

            GUI.backgroundColor = Color.white;
            GUI.enabled = true;

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

            _selectedSpec = (TextAsset)EditorGUILayout.ObjectField(
                "Spec File (.json)",
                _selectedSpec,
                typeof(TextAsset),
                false);

            EditorGUILayout.Space(5);

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

            GUI.enabled = _selectedSpec != null;
            GUI.backgroundColor = new Color(0.4f, 0.6f, 1f);

            if (GUILayout.Button("Generate Builder Code from Spec", GUILayout.Height(30)))
            {
                GenerateBuilder(_selectedSpec);
            }

            GUI.backgroundColor = Color.white;
            GUI.enabled = true;

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
                FullSyncSingle();
            }

            GUI.backgroundColor = Color.white;
            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Batch Mode

        private void DrawBatchMode()
        {
            // 상단 컨트롤
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Refresh List", EditorStyles.miniButton, GUILayout.Width(80)))
            {
                RefreshPrefabList();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Select All", EditorStyles.miniButton, GUILayout.Width(70)))
            {
                SetAllSelection(true);
            }

            if (GUILayout.Button("Deselect All", EditorStyles.miniButton, GUILayout.Width(80)))
            {
                SetAllSelection(false);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Screens 섹션
            _screensFoldout = EditorGUILayout.Foldout(_screensFoldout, $"Screens ({_screenPrefabs.Count})", true);

            if (_screensFoldout)
            {
                EditorGUILayout.BeginVertical("box");
                DrawPrefabList(_screenPrefabs, "Screens");
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(5);

            // Popups 섹션
            _popupsFoldout = EditorGUILayout.Foldout(_popupsFoldout, $"Popups ({_popupPrefabs.Count})", true);

            if (_popupsFoldout)
            {
                EditorGUILayout.BeginVertical("box");
                DrawPrefabList(_popupPrefabs, "Popups");
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(10);

            // 일괄 작업 버튼
            DrawBatchActions();
        }

        private void DrawPrefabList(List<PrefabEntry> prefabs, string category)
        {
            if (prefabs.Count == 0)
            {
                EditorGUILayout.HelpBox($"No prefabs found in {category} folder.", MessageType.Warning);
                return;
            }

            // 카테고리 전체 선택/해제
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);

            if (GUILayout.Button($"Select All {category}", EditorStyles.miniButton, GUILayout.Width(120)))
            {
                foreach (var p in prefabs) p.Selected = true;
            }

            if (GUILayout.Button("Deselect", EditorStyles.miniButton, GUILayout.Width(60)))
            {
                foreach (var p in prefabs) p.Selected = false;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(3);

            // 프리팹 목록
            foreach (var entry in prefabs)
            {
                EditorGUILayout.BeginHorizontal();

                // 체크박스
                entry.Selected = EditorGUILayout.Toggle(entry.Selected, GUILayout.Width(20));

                // 프리팹 이름
                var labelStyle = new GUIStyle(EditorStyles.label);
                if (entry.HasManualBuilder)
                {
                    labelStyle.normal.textColor = new Color(0.6f, 0.6f, 1f); // 파란색: 수동 빌더
                }
                else if (entry.HasSpec && entry.HasBuilder)
                {
                    labelStyle.normal.textColor = new Color(0.4f, 0.8f, 0.4f); // 녹색: 완료
                }
                else if (entry.HasSpec)
                {
                    labelStyle.normal.textColor = new Color(1f, 0.8f, 0.4f); // 노란색: Spec만 있음
                }

                EditorGUILayout.LabelField(entry.Name, labelStyle);

                // 상태 아이콘
                GUILayout.FlexibleSpace();

                var statusText = "";
                if (entry.HasManualBuilder) statusText = "🔧"; // 수동 빌더
                else
                {
                    if (entry.HasSpec) statusText += "📄";
                    if (entry.HasBuilder) statusText += "⚙️";
                    if (string.IsNullOrEmpty(statusText)) statusText = "—";
                }

                GUILayout.Label(statusText, GUILayout.Width(40));

                // View 버튼
                if (GUILayout.Button("View", EditorStyles.miniButton, GUILayout.Width(40)))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.Path);
                    if (prefab != null)
                    {
                        Selection.activeObject = prefab;
                        EditorGUIUtility.PingObject(prefab);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawBatchActions()
        {
            var selectedCount = GetSelectedCount();

            EditorGUILayout.LabelField("Batch Actions", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField($"Selected: {selectedCount} prefab(s)", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.Space(5);

            GUI.enabled = selectedCount > 0;

            // Analyze 버튼
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button($"Analyze Selected → JSON Specs ({selectedCount})", GUILayout.Height(28)))
            {
                BatchAnalyze();
            }

            EditorGUILayout.Space(3);

            // Generate 버튼
            GUI.backgroundColor = new Color(0.4f, 0.6f, 1f);
            if (GUILayout.Button($"Generate Builders from Specs ({selectedCount})", GUILayout.Height(28)))
            {
                BatchGenerateBuilders();
            }

            EditorGUILayout.Space(5);

            // Full Sync 버튼
            GUI.backgroundColor = new Color(1f, 0.8f, 0.4f);
            if (GUILayout.Button($"Full Batch Sync ({selectedCount})", GUILayout.Height(35)))
            {
                BatchFullSync();
            }

            GUI.backgroundColor = Color.white;
            GUI.enabled = true;

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // Manual Builder 섹션
            DrawManualBuilderSection();
        }

        private void DrawManualBuilderSection()
        {
            EditorGUILayout.LabelField("Manual Builder Actions", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.HelpBox(
                "수동 작성된 PrefabBuilder를 실행하여 프리팹을 재생성합니다.\n" +
                "Manual Builder → Prefab → JSON Spec → Generated Code",
                MessageType.None);

            EditorGUILayout.Space(5);

            // 새로고침 버튼
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh", GUILayout.Width(70)))
            {
                RefreshManualBuilderList();
            }
            EditorGUILayout.LabelField($"Found: {_manualBuilders.Count} builders", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Select All / Deselect All 버튼
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select All", GUILayout.Width(80)))
            {
                foreach (var entry in _manualBuilders) entry.Selected = true;
            }
            if (GUILayout.Button("Deselect All", GUILayout.Width(80)))
            {
                foreach (var entry in _manualBuilders) entry.Selected = false;
            }
            var selectedCount = _manualBuilders.Count(e => e.Selected);
            EditorGUILayout.LabelField($"Selected: {selectedCount}", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(3);

            // Manual Builder 목록 (체크박스)
            _manualBuildersFoldout = EditorGUILayout.Foldout(_manualBuildersFoldout, "Manual Builders", true);
            if (_manualBuildersFoldout)
            {
                EditorGUI.indentLevel++;
                foreach (var entry in _manualBuilders)
                {
                    EditorGUILayout.BeginHorizontal();
                    entry.Selected = EditorGUILayout.Toggle(entry.Selected, GUILayout.Width(20));

                    var labelStyle = new GUIStyle(EditorStyles.label);
                    labelStyle.normal.textColor = new Color(0.6f, 0.6f, 1f); // 파란색
                    EditorGUILayout.LabelField(entry.Builder.PrefabName, labelStyle);

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            GUI.enabled = selectedCount > 0;

            // Manual Builder → Prefab → JSON
            GUI.backgroundColor = new Color(0.6f, 0.6f, 1f);
            if (GUILayout.Button($"Build from Manual → Prefabs + JSON ({selectedCount})", GUILayout.Height(28)))
            {
                BatchBuildFromManual(generateCode: false);
            }

            EditorGUILayout.Space(3);

            // Full Pipeline: Manual Builder → Prefab → JSON → Generated
            GUI.backgroundColor = new Color(0.8f, 0.4f, 1f);
            if (GUILayout.Button($"Full Pipeline: Manual → Prefab → JSON → Generated ({selectedCount})", GUILayout.Height(35)))
            {
                BatchBuildFromManual(generateCode: true);
            }

            GUI.backgroundColor = Color.white;
            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Actions

        private void RefreshPrefabList()
        {
            _screenPrefabs = ScanPrefabFolder(SCREENS_FOLDER);
            _popupPrefabs = ScanPrefabFolder(POPUPS_FOLDER);
            Repaint();
        }


        private void RefreshManualBuilderList()
        {
            var builders = ManualBuilderExecutor.FindAllManualBuilders();
            _manualBuilders = builders.Select(b => new ManualBuilderEntry
            {
                Builder = b,
                Selected = false
            }).OrderBy(e => e.Builder.PrefabName).ToList();
        }

        private List<PrefabEntry> ScanPrefabFolder(string folder)
        {
            var list = new List<PrefabEntry>();

            if (!Directory.Exists(folder)) return list;

            var prefabFiles = Directory.GetFiles(folder, "*.prefab");

            foreach (var file in prefabFiles)
            {
                var assetPath = file.Replace("\\", "/");
                var name = Path.GetFileNameWithoutExtension(assetPath);

                var specPath = $"{SPEC_FOLDER}/{name}.structure.json";
                var builderPath = $"{BUILDER_FOLDER}/{name}PrefabBuilder.Generated.cs";
                var manualBuilderPath = $"{BUILDER_FOLDER}/{name}PrefabBuilder.cs";

                list.Add(new PrefabEntry
                {
                    Path = assetPath,
                    Name = name,
                    Selected = false,
                    HasSpec = File.Exists(specPath),
                    HasBuilder = File.Exists(builderPath),
                    HasManualBuilder = File.Exists(manualBuilderPath)
                });
            }

            return list.OrderBy(p => p.Name).ToList();
        }

        private void SetAllSelection(bool selected)
        {
            foreach (var p in _screenPrefabs) p.Selected = selected;
            foreach (var p in _popupPrefabs) p.Selected = selected;
        }

        private int GetSelectedCount()
        {
            return _screenPrefabs.Count(p => p.Selected) + _popupPrefabs.Count(p => p.Selected);
        }

        private List<PrefabEntry> GetSelectedPrefabs()
        {
            var selected = new List<PrefabEntry>();
            selected.AddRange(_screenPrefabs.Where(p => p.Selected));
            selected.AddRange(_popupPrefabs.Where(p => p.Selected));
            return selected;
        }

        // Single mode actions
        private void AnalyzePrefab(GameObject prefab)
        {
            if (prefab == null) return;

            var prefabPath = AssetDatabase.GetAssetPath(prefab);

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

        private void GenerateBuilder(TextAsset spec)
        {
            if (spec == null) return;

            var specPath = AssetDatabase.GetAssetPath(spec);

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

        private void FullSyncSingle()
        {
            if (_selectedPrefab == null) return;

            Log("Starting full sync...", MessageType.Info);

            AnalyzePrefab(_selectedPrefab);

            if (_selectedSpec == null)
            {
                Log("Sync failed at analyze step", MessageType.Error);
                return;
            }

            GenerateBuilder(_selectedSpec);

            if (!string.IsNullOrEmpty(_lastGeneratedPath))
            {
                Log($"Full sync completed!\nSpec: {_lastAnalyzedPath}\nBuilder: {_lastGeneratedPath}",
                    MessageType.Info);
            }
        }

        // Batch mode actions
        private void BatchAnalyze()
        {
            var selected = GetSelectedPrefabs();
            if (selected.Count == 0) return;

            var successCount = 0;
            var failCount = 0;

            try
            {
                EditorUtility.DisplayProgressBar("Batch Analyze", "Starting...", 0f);

                for (int i = 0; i < selected.Count; i++)
                {
                    var entry = selected[i];
                    EditorUtility.DisplayProgressBar("Batch Analyze",
                        $"Analyzing {entry.Name}...", (float)i / selected.Count);

                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.Path);
                    if (prefab == null)
                    {
                        failCount++;
                        continue;
                    }

                    var specPath = PrefabStructureAnalyzer.Analyze(entry.Path);
                    if (!string.IsNullOrEmpty(specPath))
                    {
                        entry.HasSpec = true;
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.Refresh();
            RefreshPrefabList();

            Log($"Batch analyze completed.\nSuccess: {successCount}, Failed: {failCount}", MessageType.Info);
        }

        private void BatchGenerateBuilders()
        {
            var selected = GetSelectedPrefabs();
            if (selected.Count == 0) return;

            var successCount = 0;
            var failCount = 0;
            var skippedCount = 0;

            try
            {
                EditorUtility.DisplayProgressBar("Batch Generate", "Starting...", 0f);

                for (int i = 0; i < selected.Count; i++)
                {
                    var entry = selected[i];
                    EditorUtility.DisplayProgressBar("Batch Generate",
                        $"Generating {entry.Name}...", (float)i / selected.Count);

                    var specPath = $"{SPEC_FOLDER}/{entry.Name}.structure.json";

                    if (!File.Exists(specPath))
                    {
                        skippedCount++;
                        continue;
                    }

                    var builderPath = PrefabBuilderGenerator.Generate(specPath);
                    if (!string.IsNullOrEmpty(builderPath))
                    {
                        entry.HasBuilder = true;
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.Refresh();
            RefreshPrefabList();

            Log($"Batch generate completed.\nSuccess: {successCount}, Failed: {failCount}, Skipped (no spec): {skippedCount}",
                MessageType.Info);
        }

        private void BatchFullSync()
        {
            var selected = GetSelectedPrefabs();
            if (selected.Count == 0) return;

            var successCount = 0;
            var failCount = 0;

            try
            {
                EditorUtility.DisplayProgressBar("Batch Full Sync", "Starting...", 0f);

                for (int i = 0; i < selected.Count; i++)
                {
                    var entry = selected[i];
                    var progress = (float)i / selected.Count;

                    // Step 1: Analyze
                    EditorUtility.DisplayProgressBar("Batch Full Sync",
                        $"[{i+1}/{selected.Count}] Analyzing {entry.Name}...", progress);

                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.Path);
                    if (prefab == null)
                    {
                        failCount++;
                        continue;
                    }

                    var specPath = PrefabStructureAnalyzer.Analyze(entry.Path);
                    if (string.IsNullOrEmpty(specPath))
                    {
                        failCount++;
                        continue;
                    }

                    entry.HasSpec = true;

                    // Step 2: Generate
                    EditorUtility.DisplayProgressBar("Batch Full Sync",
                        $"[{i+1}/{selected.Count}] Generating {entry.Name}...", progress + 0.5f / selected.Count);

                    var builderPath = PrefabBuilderGenerator.Generate(specPath);
                    if (!string.IsNullOrEmpty(builderPath))
                    {
                        entry.HasBuilder = true;
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.Refresh();
            RefreshPrefabList();

            Log($"Batch full sync completed.\nSuccess: {successCount}, Failed: {failCount}", MessageType.Info);
        }

        private void BatchBuildFromManual(bool generateCode)
        {
            // 선택된 빌더만 가져오기
            var selectedBuilders = _manualBuilders
                .Where(e => e.Selected)
                .Select(e => e.Builder)
                .ToArray();

            if (selectedBuilders.Length == 0)
            {
                Log("No manual builders selected.", MessageType.Warning);
                return;
            }

            var successCount = 0;
            var failCount = 0;

            try
            {
                var actionName = generateCode ? "Full Pipeline" : "Build from Manual";
                EditorUtility.DisplayProgressBar(actionName, "Starting...", 0f);

                for (int i = 0; i < selectedBuilders.Length; i++)
                {
                    var builder = selectedBuilders[i];
                    var progress = (float)i / selectedBuilders.Length;

                    EditorUtility.DisplayProgressBar(actionName,
                        $"[{i + 1}/{selectedBuilders.Length}] Processing {builder.PrefabName}...", progress);

                    try
                    {
                        if (generateCode)
                        {
                            // Full Pipeline: Manual → Prefab → JSON → Generated
                            var (buildResult, jsonPath, generatedPath) =
                                ManualBuilderExecutor.ExecuteFullPipeline(builder, overwritePrefab: true);

                            if (buildResult.Success && !string.IsNullOrEmpty(jsonPath))
                            {
                                successCount++;
                                Debug.Log($"[PrefabSync] Full pipeline completed: {builder.PrefabName}");
                            }
                            else
                            {
                                failCount++;
                                Debug.LogWarning($"[PrefabSync] Failed: {builder.PrefabName} - {buildResult.ErrorMessage}");
                            }
                        }
                        else
                        {
                            // Build + Analyze only
                            var (buildResult, jsonPath) =
                                ManualBuilderExecutor.ExecuteBuilderAndAnalyze(builder, overwritePrefab: true);

                            if (buildResult.Success && !string.IsNullOrEmpty(jsonPath))
                            {
                                successCount++;
                            }
                            else
                            {
                                failCount++;
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        failCount++;
                        Debug.LogError($"[PrefabSync] Exception for {builder.PrefabName}: {e.Message}");
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RefreshPrefabList();

            var operationType = generateCode ? "Full pipeline" : "Build from manual";
            Log($"{operationType} completed.\nSuccess: {successCount}, Failed: {failCount}", MessageType.Info);
        }

        private void Log(string message, MessageType type)
        {
            _logMessage = message;
            _logType = type;
            Debug.Log($"[PrefabSync] {message}");
            Repaint();
        }

        #endregion

        #region Log Section

        private void DrawLogSection()
        {
            if (!string.IsNullOrEmpty(_logMessage))
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox(_logMessage, _logType);
            }
        }

        #endregion
    }
}
