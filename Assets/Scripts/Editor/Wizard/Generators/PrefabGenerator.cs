using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sc.Common.UI;
using Sc.Common.UI.Attributes;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Sc.Editor.Wizard.Generators
{
    /// <summary>
    /// Screen/Popup 프리팹 자동 생성 및 Addressables 등록.
    /// </summary>
    public static class PrefabGenerator
    {
        private const string PREFAB_ROOT = "Assets/Prefabs/UI";
        private const string SCREEN_PATH = PREFAB_ROOT + "/Screens";
        private const string POPUP_PATH = PREFAB_ROOT + "/Popups";
        private const string ADDRESSABLE_GROUP = "UI";

        #region Public API

        /// <summary>
        /// 모든 프리팹을 삭제 후 재생성.
        /// </summary>
        public static (int screens, int popups) RegenerateAllPrefabs()
        {
            // 기존 프리팹 삭제
            DeleteAllPrefabs(SCREEN_PATH);
            DeleteAllPrefabs(POPUP_PATH);

            AssetDatabase.Refresh();

            // 재생성
            var screens = GenerateAllScreenPrefabs();
            var popups = GenerateAllPopupPrefabs();

            Debug.Log($"[PrefabGenerator] 재생성 완료: Screen {screens}개, Popup {popups}개");
            return (screens, popups);
        }

        /// <summary>
        /// 특정 경로의 모든 프리팹 삭제.
        /// </summary>
        public static void DeleteAllPrefabs(string path)
        {
            if (!Directory.Exists(path))
                return;

            var prefabs = Directory.GetFiles(path, "*.prefab");
            foreach (var prefab in prefabs)
            {
                AssetDatabase.DeleteAsset(prefab.Replace("\\", "/"));
            }

            Debug.Log($"[PrefabGenerator] {prefabs.Length}개 프리팹 삭제: {path}");
        }

        /// <summary>
        /// 모든 Screen 타입을 스캔하여 프리팹 생성.
        /// </summary>
        public static int GenerateAllScreenPrefabs()
        {
            var screenTypes = FindAllScreenTypes();
            var createdCount = 0;

            foreach (var type in screenTypes)
            {
                if (GenerateScreenPrefab(type))
                    createdCount++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[PrefabGenerator] Screen 프리팹 생성 완료: {createdCount}/{screenTypes.Length}개");
            return createdCount;
        }

        /// <summary>
        /// 모든 Popup 타입을 스캔하여 프리팹 생성.
        /// </summary>
        public static int GenerateAllPopupPrefabs()
        {
            var popupTypes = FindAllPopupTypes();
            var createdCount = 0;

            foreach (var type in popupTypes)
            {
                if (GeneratePopupPrefab(type))
                    createdCount++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[PrefabGenerator] Popup 프리팹 생성 완료: {createdCount}/{popupTypes.Length}개");
            return createdCount;
        }

        /// <summary>
        /// 특정 Screen 프리팹 생성.
        /// </summary>
        public static bool GenerateScreenPrefab(Type screenType)
        {
            if (!IsValidScreenType(screenType))
            {
                Debug.LogWarning($"[PrefabGenerator] {screenType.Name}는 유효한 Screen 타입이 아님");
                return false;
            }

            EnsureDirectoryExists(SCREEN_PATH);

            var prefabPath = $"{SCREEN_PATH}/{screenType.Name}.prefab";

            // 이미 존재하면 스킵
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log($"[PrefabGenerator] 이미 존재: {screenType.Name}");
                return false;
            }

            // GameObject 생성
            var go = CreateScreenGameObject(screenType);

            // 프리팹 저장
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            UnityEngine.Object.DestroyImmediate(go);

            // Addressables 등록
            RegisterToAddressables(prefabPath, $"UI/Screens/{screenType.Name}");

            Debug.Log($"[PrefabGenerator] Screen 생성: {screenType.Name}");
            return true;
        }

        /// <summary>
        /// 특정 Popup 프리팹 생성.
        /// </summary>
        public static bool GeneratePopupPrefab(Type popupType)
        {
            if (!IsValidPopupType(popupType))
            {
                Debug.LogWarning($"[PrefabGenerator] {popupType.Name}는 유효한 Popup 타입이 아님");
                return false;
            }

            EnsureDirectoryExists(POPUP_PATH);

            var prefabPath = $"{POPUP_PATH}/{popupType.Name}.prefab";

            // 이미 존재하면 스킵
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log($"[PrefabGenerator] 이미 존재: {popupType.Name}");
                return false;
            }

            // GameObject 생성
            var go = CreatePopupGameObject(popupType);

            // 프리팹 저장
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            UnityEngine.Object.DestroyImmediate(go);

            // Addressables 등록
            RegisterToAddressables(prefabPath, $"UI/Popups/{popupType.Name}");

            Debug.Log($"[PrefabGenerator] Popup 생성: {popupType.Name}");
            return true;
        }

        #endregion

        #region GameObject Creation

        private static GameObject CreateScreenGameObject(Type screenType)
        {
            // 1. PrefabBuilder 클래스가 있는지 리플렉션으로 확인
            var go = TryBuildWithPrefabBuilder(screenType);
            if (go != null)
            {
                return go;
            }

            // 2. ScreenTemplateAttribute 기반 팩토리 사용
            return ScreenTemplateFactory.Create(screenType);
        }

        private static GameObject CreatePopupGameObject(Type popupType)
        {
            // 1. PrefabBuilder 클래스가 있는지 리플렉션으로 확인
            var go = TryBuildWithPrefabBuilder(popupType);
            if (go != null)
            {
                return go;
            }

            // 2. PopupTemplateAttribute 기반 팩토리 사용
            return PopupTemplateFactory.Create(popupType);
        }

        /// <summary>
        /// 리플렉션으로 {TypeName}PrefabBuilder.Build() 메서드를 찾아 실행.
        /// </summary>
        private static GameObject TryBuildWithPrefabBuilder(Type targetType)
        {
            var builderTypeName = $"Sc.Editor.Wizard.Generators.{targetType.Name}PrefabBuilder";

            // 현재 어셈블리에서 Builder 타입 검색
            var builderType = typeof(PrefabGenerator).Assembly.GetType(builderTypeName);

            if (builderType == null)
            {
                return null;
            }

            // Build() static 메서드 찾기
            var buildMethod = builderType.GetMethod("Build", BindingFlags.Public | BindingFlags.Static);

            if (buildMethod == null)
            {
                Debug.LogWarning($"[PrefabGenerator] {builderTypeName}에 Build() 메서드가 없음");
                return null;
            }

            // 반환 타입 확인
            if (buildMethod.ReturnType != typeof(GameObject))
            {
                Debug.LogWarning($"[PrefabGenerator] {builderTypeName}.Build()의 반환 타입이 GameObject가 아님");
                return null;
            }

            // Build() 메서드 호출
            try
            {
                var result = buildMethod.Invoke(null, null) as GameObject;
                if (result != null)
                {
                    Debug.Log($"[PrefabGenerator] Builder 사용: {builderTypeName}");
                }
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"[PrefabGenerator] {builderTypeName}.Build() 실행 실패: {e.Message}");
                return null;
            }
        }

        #endregion

        #region Type Discovery

        private static Type[] FindAllScreenTypes()
        {
            var screenTypes = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(IsValidScreenType)
                        .ToArray();
                    screenTypes.AddRange(types);
                }
                catch (Exception)
                {
                    // 일부 어셈블리는 타입 로드 실패할 수 있음
                }
            }

            return screenTypes.ToArray();
        }

        private static Type[] FindAllPopupTypes()
        {
            var popupTypes = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(IsValidPopupType)
                        .ToArray();
                    popupTypes.AddRange(types);
                }
                catch (Exception)
                {
                    // 일부 어셈블리는 타입 로드 실패할 수 있음
                }
            }

            return popupTypes.ToArray();
        }

        private static bool IsValidScreenType(Type type)
        {
            return type != null
                   && !type.IsAbstract
                   && type.IsClass
                   && IsSubclassOfGeneric(type, typeof(ScreenWidget<,>));
        }

        private static bool IsValidPopupType(Type type)
        {
            return type != null
                   && !type.IsAbstract
                   && type.IsClass
                   && IsSubclassOfGeneric(type, typeof(PopupWidget<,>));
        }

        private static bool IsSubclassOfGeneric(Type type, Type genericType)
        {
            while (type != null && type != typeof(object))
            {
                var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (genericType == cur)
                    return true;
                type = type.BaseType;
            }

            return false;
        }

        #endregion

        #region Addressables

        private static void RegisterToAddressables(string assetPath, string address)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogWarning("[PrefabGenerator] Addressables 설정을 찾을 수 없음. 수동 등록 필요.");
                return;
            }

            // 그룹 찾기 또는 생성
            var group = settings.FindGroup(ADDRESSABLE_GROUP);
            if (group == null)
            {
                group = settings.CreateGroup(ADDRESSABLE_GROUP, false, false, true, null,
                    typeof(UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema),
                    typeof(UnityEditor.AddressableAssets.Settings.GroupSchemas.ContentUpdateGroupSchema));
            }

            // 에셋 등록
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = settings.CreateOrMoveEntry(guid, group, false, false);
            entry.address = address;

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
        }

        #endregion

        #region Helpers

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }
        }

        #endregion
    }
}
