using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sc.Editor.Data
{
    /// <summary>
    /// 마스터 데이터 수동 생성 에디터 윈도우
    /// </summary>
    public class MasterDataGeneratorWindow : EditorWindow
    {
        private const string JsonPath = "Assets/Data/MasterData";
        private const string OutputPath = "Assets/Data/Generated";

        private Vector2 _scrollPosition;
        private bool _showJsonFiles = true;
        private bool _showGeneratedAssets = true;

        [MenuItem("SC/Data/Master Data Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<MasterDataGeneratorWindow>("Master Data Generator");
            window.minSize = new Vector2(400, 300);
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawHeader();
            DrawJsonFilesSection();
            DrawGeneratedAssetsSection();
            DrawActions();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Master Data Generator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "JSON 파일을 ScriptableObject로 변환합니다.\n" +
                "JSON 파일 변경 시 자동으로 변환되지만, 수동 재생성도 가능합니다.",
                MessageType.Info);
            EditorGUILayout.Space(10);
        }

        private void DrawJsonFilesSection()
        {
            _showJsonFiles = EditorGUILayout.Foldout(_showJsonFiles, "JSON 파일", true);
            if (!_showJsonFiles) return;

            EditorGUI.indentLevel++;

            var jsonDir = new DirectoryInfo(JsonPath);
            if (jsonDir.Exists)
            {
                var files = jsonDir.GetFiles("*.json");
                foreach (var file in files)
                {
                    if (file.Name == "README.json") continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(file.Name, GUILayout.Width(200));
                    EditorGUILayout.LabelField($"{file.Length / 1024f:F1} KB", GUILayout.Width(80));

                    if (GUILayout.Button("열기", GUILayout.Width(60)))
                    {
                        var asset = AssetDatabase.LoadAssetAtPath<TextAsset>($"{JsonPath}/{file.Name}");
                        if (asset != null)
                            EditorGUIUtility.PingObject(asset);
                    }

                    if (GUILayout.Button("변환", GUILayout.Width(60)))
                    {
                        ImportSingleFile(file.Name);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (files.Length == 0)
                {
                    EditorGUILayout.HelpBox("JSON 파일이 없습니다.", MessageType.Warning);
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"폴더가 없습니다: {JsonPath}", MessageType.Warning);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(10);
        }

        private void DrawGeneratedAssetsSection()
        {
            _showGeneratedAssets = EditorGUILayout.Foldout(_showGeneratedAssets, "생성된 에셋", true);
            if (!_showGeneratedAssets) return;

            EditorGUI.indentLevel++;

            var outputDir = new DirectoryInfo(OutputPath);
            if (outputDir.Exists)
            {
                DrawDatabaseAsset<Sc.Data.CharacterDatabase>("CharacterDatabase");
                DrawDatabaseAsset<Sc.Data.SkillDatabase>("SkillDatabase");
                DrawDatabaseAsset<Sc.Data.ItemDatabase>("ItemDatabase");
                DrawDatabaseAsset<Sc.Data.StageDatabase>("StageDatabase");
                DrawDatabaseAsset<Sc.Data.GachaPoolDatabase>("GachaPoolDatabase");
            }
            else
            {
                EditorGUILayout.HelpBox($"폴더가 없습니다: {OutputPath}", MessageType.Warning);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(10);
        }

        private void DrawDatabaseAsset<T>(string name) where T : ScriptableObject
        {
            var assetPath = $"{OutputPath}/{name}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            var dataFolderName = name.Replace("Database", "s"); // CharacterDatabase -> Characters
            var dataFolderPath = $"{OutputPath}/{dataFolderName}";
            var hasDataFolder = AssetDatabase.IsValidFolder(dataFolderPath);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(name, GUILayout.Width(180));

            if (asset != null)
            {
                EditorGUILayout.LabelField("✓", GUILayout.Width(20));
                if (GUILayout.Button("선택", GUILayout.Width(50)))
                {
                    Selection.activeObject = asset;
                    EditorGUIUtility.PingObject(asset);
                }

                GUI.backgroundColor = new Color(1f, 0.7f, 0.7f);
                if (GUILayout.Button("삭제", GUILayout.Width(50)))
                {
                    DeleteDatabaseAsset(name, assetPath, dataFolderPath, hasDataFolder);
                }
                GUI.backgroundColor = Color.white;
            }
            else
            {
                EditorGUILayout.LabelField("✗", GUILayout.Width(20));
                EditorGUILayout.LabelField("", GUILayout.Width(50));
                EditorGUILayout.LabelField("", GUILayout.Width(50));
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DeleteDatabaseAsset(string name, string assetPath, string dataFolderPath, bool hasDataFolder)
        {
            var message = hasDataFolder
                ? $"{name}.asset 및 관련 데이터 폴더를 삭제하시겠습니까?"
                : $"{name}.asset를 삭제하시겠습니까?";

            if (!EditorUtility.DisplayDialog("확인", message, "삭제", "취소"))
                return;

            // 데이터 폴더 삭제
            if (hasDataFolder)
            {
                AssetDatabase.DeleteAsset(dataFolderPath);
            }

            // Database 에셋 삭제
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.Refresh();

            Debug.Log($"[MasterDataGenerator] {name} 삭제 완료");
        }

        private void DrawActions()
        {
            EditorGUILayout.LabelField("작업", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("전체 재생성", GUILayout.Height(30)))
            {
                ImportAllFiles();
            }

            if (GUILayout.Button("생성 폴더 열기", GUILayout.Height(30)))
            {
                var folder = AssetDatabase.LoadAssetAtPath<Object>(OutputPath);
                if (folder != null)
                {
                    Selection.activeObject = folder;
                    EditorGUIUtility.PingObject(folder);
                }
                else
                {
                    Debug.LogWarning($"폴더가 없습니다: {OutputPath}");
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("생성된 에셋 모두 삭제", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("확인",
                    $"{OutputPath} 폴더의 모든 에셋을 삭제하시겠습니까?",
                    "삭제", "취소"))
                {
                    DeleteAllGeneratedAssets();
                }
            }
            GUI.backgroundColor = Color.white;
        }

        private void ImportSingleFile(string fileName)
        {
            var assetPath = $"{JsonPath}/{fileName}";
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
            Debug.Log($"[MasterDataGenerator] {fileName} 변환 완료");
        }

        private void ImportAllFiles()
        {
            var jsonDir = new DirectoryInfo(JsonPath);
            if (!jsonDir.Exists)
            {
                Debug.LogWarning($"JSON 폴더가 없습니다: {JsonPath}");
                return;
            }

            var files = jsonDir.GetFiles("*.json");
            foreach (var file in files)
            {
                if (file.Name == "README.json") continue;
                AssetDatabase.ImportAsset($"{JsonPath}/{file.Name}", ImportAssetOptions.ForceUpdate);
            }

            AssetDatabase.Refresh();
            Debug.Log($"[MasterDataGenerator] 전체 변환 완료 ({files.Length}개 파일)");
        }

        private void DeleteAllGeneratedAssets()
        {
            if (AssetDatabase.IsValidFolder(OutputPath))
            {
                AssetDatabase.DeleteAsset(OutputPath);
                AssetDatabase.Refresh();
                Debug.Log($"[MasterDataGenerator] {OutputPath} 삭제 완료");
            }
        }
    }
}
