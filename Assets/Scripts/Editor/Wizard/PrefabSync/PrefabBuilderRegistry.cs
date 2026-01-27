using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Sc.Editor.Wizard.PrefabSync
{
    /// <summary>
    /// 프리팹 빌더 통합 레지스트리.
    /// Generated/Manual 빌더를 검색하고 실행하는 단일 진입점.
    /// PrefabGenerator와 PrefabSyncWindow 모두 이 클래스를 사용합니다.
    /// </summary>
    public static class PrefabBuilderRegistry
    {
        private const string BUILDER_NAMESPACE = "Sc.Editor.Wizard.Generators";
        private const string SCREEN_PREFAB_PATH = "Assets/Prefabs/UI/Screens";
        private const string POPUP_PREFAB_PATH = "Assets/Prefabs/UI/Popups";

        /// <summary>
        /// 빌더 유형
        /// </summary>
        public enum BuilderType
        {
            None,
            Generated, // {Name}PrefabBuilder_Generated
            Manual // {Name}PrefabBuilder
        }

        /// <summary>
        /// 빌더 정보
        /// </summary>
        public class BuilderInfo
        {
            public string PrefabName { get; set; }
            public string TypeName { get; set; }
            public Type BuilderClass { get; set; }
            public MethodInfo BuildMethod { get; set; }
            public BuilderType Type { get; set; }
            public bool IsScreen { get; set; }
            public string ExpectedPrefabPath { get; set; }

            /// <summary>
            /// 빌더 실행
            /// </summary>
            public GameObject Build()
            {
                if (BuildMethod == null)
                    return null;

                try
                {
                    return BuildMethod.Invoke(null, null) as GameObject;
                }
                catch (Exception e)
                {
                    var innerMsg = e.InnerException?.Message ?? e.Message;
                    var innerStack = e.InnerException?.StackTrace ?? e.StackTrace;
                    Debug.LogError($"[PrefabBuilderRegistry] {TypeName}.Build() 실행 실패: {innerMsg}\n{innerStack}");
                    return null;
                }
            }
        }

        /// <summary>
        /// 빌더 검색 결과
        /// </summary>
        public class BuilderSearchResult
        {
            public BuilderInfo Generated { get; set; }
            public BuilderInfo Manual { get; set; }

            public bool HasGenerated => Generated != null;
            public bool HasManual => Manual != null;
            public bool HasAny => HasGenerated || HasManual;

            /// <summary>
            /// 우선순위에 따른 최적 빌더 (Generated > Manual)
            /// </summary>
            public BuilderInfo Best => Generated ?? Manual;
        }

        #region 검색 API

        /// <summary>
        /// 특정 프리팹에 대한 모든 빌더 검색 (Generated + Manual)
        /// </summary>
        public static BuilderSearchResult FindBuilders(string prefabName)
        {
            var assembly = GetEditorAssembly();

            return new BuilderSearchResult
            {
                Generated = FindBuilderByType(assembly, prefabName, BuilderType.Generated),
                Manual = FindBuilderByType(assembly, prefabName, BuilderType.Manual)
            };
        }

        /// <summary>
        /// 특정 프리팹에 대한 Generated 빌더 검색
        /// </summary>
        public static BuilderInfo FindGeneratedBuilder(string prefabName)
        {
            return FindBuilderByType(GetEditorAssembly(), prefabName, BuilderType.Generated);
        }

        /// <summary>
        /// 특정 프리팹에 대한 Manual 빌더 검색
        /// </summary>
        public static BuilderInfo FindManualBuilder(string prefabName)
        {
            return FindBuilderByType(GetEditorAssembly(), prefabName, BuilderType.Manual);
        }

        /// <summary>
        /// 우선순위에 따른 최적 빌더 검색 (Generated > Manual)
        /// </summary>
        public static BuilderInfo FindBestBuilder(string prefabName)
        {
            return FindBuilders(prefabName).Best;
        }

        /// <summary>
        /// 모든 Manual 빌더 검색
        /// </summary>
        public static BuilderInfo[] FindAllManualBuilders()
        {
            return FindAllBuildersByType(BuilderType.Manual);
        }

        /// <summary>
        /// 모든 Generated 빌더 검색
        /// </summary>
        public static BuilderInfo[] FindAllGeneratedBuilders()
        {
            return FindAllBuildersByType(BuilderType.Generated);
        }

        /// <summary>
        /// 모든 빌더 검색 (Generated + Manual)
        /// </summary>
        public static BuilderInfo[] FindAllBuilders()
        {
            var assembly = GetEditorAssembly();
            var results = new List<BuilderInfo>();

            try
            {
                var builderTypes = assembly.GetTypes()
                    .Where(t => t.Namespace == BUILDER_NAMESPACE
                                && (t.Name.EndsWith("PrefabBuilder") || t.Name.EndsWith("PrefabBuilder_Generated"))
                                && t.IsClass)
                    // static class는 IsAbstract=true && IsSealed=true이므로 !IsAbstract 조건 제거
                    .ToArray();

                foreach (var builderType in builderTypes)
                {
                    var info = CreateBuilderInfo(builderType);
                    if (info != null)
                    {
                        results.Add(info);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[PrefabBuilderRegistry] Failed to find builders: {e.Message}");
            }

            return results.ToArray();
        }

        #endregion

        #region 실행 API

        /// <summary>
        /// 빌더를 실행하여 GameObject 생성
        /// </summary>
        public static GameObject ExecuteBuilder(BuilderInfo builder)
        {
            if (builder == null)
                return null;

            var result = builder.Build();
            if (result != null)
            {
                Debug.Log($"[PrefabBuilderRegistry] {builder.Type} 빌더 실행: {builder.TypeName}");
            }

            return result;
        }

        /// <summary>
        /// 프리팹에 대한 최적 빌더를 찾아 실행 (Generated > Manual)
        /// </summary>
        public static (GameObject result, BuilderType usedType) ExecuteBestBuilder(string prefabName)
        {
            var searchResult = FindBuilders(prefabName);

            if (searchResult.HasGenerated)
            {
                var result = ExecuteBuilder(searchResult.Generated);
                if (result != null)
                    return (result, BuilderType.Generated);
            }

            if (searchResult.HasManual)
            {
                var result = ExecuteBuilder(searchResult.Manual);
                if (result != null)
                    return (result, BuilderType.Manual);
            }

            return (null, BuilderType.None);
        }

        #endregion

        #region Internal

        private static Assembly GetEditorAssembly()
        {
            return typeof(PrefabBuilderRegistry).Assembly;
        }

        private static BuilderInfo FindBuilderByType(Assembly assembly, string prefabName, BuilderType type)
        {
            var className = type == BuilderType.Generated
                ? $"{prefabName}PrefabBuilder_Generated"
                : $"{prefabName}PrefabBuilder";

            var fullTypeName = $"{BUILDER_NAMESPACE}.{className}";
            var builderType = assembly.GetType(fullTypeName);

            if (builderType == null)
                return null;

            return CreateBuilderInfo(builderType);
        }

        private static BuilderInfo[] FindAllBuildersByType(BuilderType type)
        {
            var assembly = GetEditorAssembly();
            var results = new List<BuilderInfo>();

            try
            {
                var suffix = type == BuilderType.Generated ? "PrefabBuilder_Generated" : "PrefabBuilder";
                var excludeSuffix = type == BuilderType.Manual ? "PrefabBuilder_Generated" : null;

                var builderTypes = assembly.GetTypes()
                    .Where(t => t.Namespace == BUILDER_NAMESPACE
                                && t.Name.EndsWith(suffix)
                                && (excludeSuffix == null || !t.Name.EndsWith(excludeSuffix))
                                && t.IsClass)
                    // static class는 IsAbstract=true && IsSealed=true이므로 !IsAbstract 조건 제거
                    .ToArray();

                foreach (var builderType in builderTypes)
                {
                    var info = CreateBuilderInfo(builderType);
                    if (info != null)
                    {
                        results.Add(info);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[PrefabBuilderRegistry] Failed to find {type} builders: {e.Message}");
            }

            return results.ToArray();
        }

        private static BuilderInfo CreateBuilderInfo(Type builderType)
        {
            var buildMethod = builderType.GetMethod("Build", BindingFlags.Public | BindingFlags.Static);

            if (buildMethod == null || buildMethod.ReturnType != typeof(GameObject))
                return null;

            // 빌더 타입 판별
            var isGenerated = builderType.Name.EndsWith("PrefabBuilder_Generated");
            var type = isGenerated ? BuilderType.Generated : BuilderType.Manual;

            // 프리팹 이름 추출
            var prefabName = builderType.Name
                .Replace("PrefabBuilder_Generated", "")
                .Replace("PrefabBuilder", "");

            // Screen/Popup 판별
            var isScreen = prefabName.EndsWith("Screen") || prefabName.EndsWith("Dashboard");
            var prefabPath = isScreen
                ? $"{SCREEN_PREFAB_PATH}/{prefabName}.prefab"
                : $"{POPUP_PREFAB_PATH}/{prefabName}.prefab";

            return new BuilderInfo
            {
                PrefabName = prefabName,
                TypeName = builderType.FullName,
                BuilderClass = builderType,
                BuildMethod = buildMethod,
                Type = type,
                IsScreen = isScreen,
                ExpectedPrefabPath = prefabPath
            };
        }

        #endregion
    }
}