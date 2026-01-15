using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sc.Editor.Data
{
    /// <summary>
    /// 데이터 시스템 테스트 씬 자동 셋업
    /// </summary>
    public static class DataTestSceneSetup
    {
        private const string TestScenePath = "Assets/Scenes/Test/DataTestScene.unity";
        private const string GeneratedPath = "Assets/Data/Generated";

        [MenuItem("SC/Data/Create Test Scene", false, 100)]
        public static void CreateTestScene()
        {
            // 1. 마스터 데이터 존재 확인
            if (!ValidateMasterData())
            {
                if (EditorUtility.DisplayDialog("마스터 데이터 없음",
                    "생성된 마스터 데이터가 없습니다.\n먼저 Master Data Generator에서 데이터를 생성하시겠습니까?",
                    "생성하기", "취소"))
                {
                    MasterDataGeneratorWindow.ShowWindow();
                }
                return;
            }

            // 2. 현재 씬 저장 확인
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            // 3. 새 씬 생성
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // 4. DataManager 생성 및 설정
            var dataManagerGo = CreateDataManager();

            // 5. GameBootstrap 생성
            var bootstrapGo = CreateGameBootstrap();

            // 6. 씬 폴더 생성
            EnsureDirectory("Assets/Scenes/Test");

            // 7. 씬 저장
            EditorSceneManager.SaveScene(scene, TestScenePath);

            // 8. 선택
            Selection.activeGameObject = bootstrapGo;

            Debug.Log($"[DataTestSceneSetup] 테스트 씬 생성 완료: {TestScenePath}");
            EditorUtility.DisplayDialog("완료",
                "테스트 씬이 생성되었습니다.\n\nPlay 버튼을 눌러 테스트를 시작하세요.\n\n" +
                "GameBootstrap Inspector에서:\n" +
                "- Initialize Game: 초기화\n" +
                "- Test Gacha: 가챠 테스트\n" +
                "- Log Current State: 상태 확인",
                "확인");
        }

        [MenuItem("SC/Data/Setup Current Scene", false, 101)]
        public static void SetupCurrentScene()
        {
            // 마스터 데이터 존재 확인
            if (!ValidateMasterData())
            {
                if (EditorUtility.DisplayDialog("마스터 데이터 없음",
                    "생성된 마스터 데이터가 없습니다.\n먼저 Master Data Generator에서 데이터를 생성하시겠습니까?",
                    "생성하기", "취소"))
                {
                    MasterDataGeneratorWindow.ShowWindow();
                }
                return;
            }

            // 기존 DataManager 확인
            var existingDataManager = Object.FindFirstObjectByType<Sc.Core.DataManager>();
            if (existingDataManager != null)
            {
                if (!EditorUtility.DisplayDialog("DataManager 존재",
                    "이미 DataManager가 씬에 있습니다. 재설정하시겠습니까?",
                    "재설정", "취소"))
                {
                    return;
                }
                Object.DestroyImmediate(existingDataManager.gameObject);
            }

            // 기존 GameBootstrap 확인
            var existingBootstrap = Object.FindFirstObjectByType<Sc.Core.GameBootstrap>();
            if (existingBootstrap != null)
            {
                Object.DestroyImmediate(existingBootstrap.gameObject);
            }

            // 생성
            CreateDataManager();
            var bootstrapGo = CreateGameBootstrap();

            Selection.activeGameObject = bootstrapGo;
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            Debug.Log("[DataTestSceneSetup] 현재 씬에 테스트 환경 설정 완료");
        }

        private static GameObject CreateDataManager()
        {
            var go = new GameObject("[DataManager]");
            var dataManager = go.AddComponent<Sc.Core.DataManager>();

            // Database SO 자동 할당
            var characterDb = AssetDatabase.LoadAssetAtPath<Sc.Data.CharacterDatabase>($"{GeneratedPath}/CharacterDatabase.asset");
            var skillDb = AssetDatabase.LoadAssetAtPath<Sc.Data.SkillDatabase>($"{GeneratedPath}/SkillDatabase.asset");
            var itemDb = AssetDatabase.LoadAssetAtPath<Sc.Data.ItemDatabase>($"{GeneratedPath}/ItemDatabase.asset");
            var stageDb = AssetDatabase.LoadAssetAtPath<Sc.Data.StageDatabase>($"{GeneratedPath}/StageDatabase.asset");
            var gachaPoolDb = AssetDatabase.LoadAssetAtPath<Sc.Data.GachaPoolDatabase>($"{GeneratedPath}/GachaPoolDatabase.asset");

            // SerializedObject를 통해 private 필드 설정
            var serializedObject = new SerializedObject(dataManager);
            serializedObject.FindProperty("_characterDatabase").objectReferenceValue = characterDb;
            serializedObject.FindProperty("_skillDatabase").objectReferenceValue = skillDb;
            serializedObject.FindProperty("_itemDatabase").objectReferenceValue = itemDb;
            serializedObject.FindProperty("_stageDatabase").objectReferenceValue = stageDb;
            serializedObject.FindProperty("_gachaPoolDatabase").objectReferenceValue = gachaPoolDb;
            serializedObject.ApplyModifiedProperties();

            Undo.RegisterCreatedObjectUndo(go, "Create DataManager");

            Debug.Log("[DataTestSceneSetup] DataManager 생성 완료");
            return go;
        }

        private static GameObject CreateGameBootstrap()
        {
            var go = new GameObject("[GameBootstrap]");
            var bootstrap = go.AddComponent<Sc.Core.GameBootstrap>();

            // 기본 설정
            var serializedObject = new SerializedObject(bootstrap);
            serializedObject.FindProperty("_autoInitialize").boolValue = true;
            serializedObject.FindProperty("_testNickname").stringValue = "TestPlayer";
            serializedObject.ApplyModifiedProperties();

            Undo.RegisterCreatedObjectUndo(go, "Create GameBootstrap");

            Debug.Log("[DataTestSceneSetup] GameBootstrap 생성 완료");
            return go;
        }

        private static bool ValidateMasterData()
        {
            var characterDb = AssetDatabase.LoadAssetAtPath<Sc.Data.CharacterDatabase>($"{GeneratedPath}/CharacterDatabase.asset");
            var skillDb = AssetDatabase.LoadAssetAtPath<Sc.Data.SkillDatabase>($"{GeneratedPath}/SkillDatabase.asset");
            var itemDb = AssetDatabase.LoadAssetAtPath<Sc.Data.ItemDatabase>($"{GeneratedPath}/ItemDatabase.asset");
            var stageDb = AssetDatabase.LoadAssetAtPath<Sc.Data.StageDatabase>($"{GeneratedPath}/StageDatabase.asset");
            var gachaPoolDb = AssetDatabase.LoadAssetAtPath<Sc.Data.GachaPoolDatabase>($"{GeneratedPath}/GachaPoolDatabase.asset");

            return characterDb != null && skillDb != null && itemDb != null && stageDb != null && gachaPoolDb != null;
        }

        private static void EnsureDirectory(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;

            var parts = path.Split('/');
            var currentPath = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                var nextPath = $"{currentPath}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, parts[i]);
                }
                currentPath = nextPath;
            }
        }
    }
}
