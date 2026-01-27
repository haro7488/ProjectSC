using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sc.Editor.Wizard.PrefabSync
{
    /// <summary>
    /// 빌더 실행 및 프리팹 저장 파이프라인.
    /// PrefabBuilderRegistry를 사용하여 빌더를 검색하고,
    /// 프리팹 저장 → JSON 분석 → Generated 코드 생성까지의 파이프라인을 제공합니다.
    /// </summary>
    public static class ManualBuilderExecutor
    {
        /// <summary>
        /// 빌더 실행 결과
        /// </summary>
        public class BuildResult
        {
            public bool Success { get; set; }
            public string PrefabPath { get; set; }
            public string ErrorMessage { get; set; }
            public string BuilderTypeName { get; set; }
            public PrefabBuilderRegistry.BuilderType BuilderType { get; set; }
        }

        #region 검색 API (Registry 위임)

        /// <summary>
        /// 모든 Manual 빌더 검색 (Registry 위임)
        /// </summary>
        public static PrefabBuilderRegistry.BuilderInfo[] FindAllManualBuilders()
        {
            return PrefabBuilderRegistry.FindAllManualBuilders();
        }

        /// <summary>
        /// 특정 프리팹의 Manual 빌더 검색 (Registry 위임)
        /// </summary>
        public static PrefabBuilderRegistry.BuilderInfo FindBuilderForPrefab(string prefabName)
        {
            return PrefabBuilderRegistry.FindManualBuilder(prefabName);
        }

        #endregion

        #region 빌더 실행 + 프리팹 저장

        /// <summary>
        /// 빌더를 실행하여 프리팹을 생성/갱신합니다.
        /// </summary>
        public static BuildResult ExecuteBuilder(PrefabBuilderRegistry.BuilderInfo builderInfo, bool overwrite = true)
        {
            if (builderInfo == null)
            {
                return new BuildResult
                {
                    Success = false,
                    ErrorMessage = "Builder info is null"
                };
            }

            try
            {
                // 기존 프리팹이 있고 덮어쓰기 안 하는 경우 스킵
                if (!overwrite && File.Exists(builderInfo.ExpectedPrefabPath))
                {
                    return new BuildResult
                    {
                        Success = true,
                        PrefabPath = builderInfo.ExpectedPrefabPath,
                        BuilderTypeName = builderInfo.TypeName,
                        BuilderType = builderInfo.Type,
                        ErrorMessage = "Prefab already exists, skipped"
                    };
                }

                // 빌더 실행
                var gameObject = builderInfo.Build();

                if (gameObject == null)
                {
                    return new BuildResult
                    {
                        Success = false,
                        BuilderTypeName = builderInfo.TypeName,
                        BuilderType = builderInfo.Type,
                        ErrorMessage = "Build() returned null"
                    };
                }

                // 디렉토리 확인
                var directory = Path.GetDirectoryName(builderInfo.ExpectedPrefabPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    AssetDatabase.Refresh();
                }

                // 프리팹 저장
                var prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, builderInfo.ExpectedPrefabPath);
                UnityEngine.Object.DestroyImmediate(gameObject);

                if (prefab == null)
                {
                    return new BuildResult
                    {
                        Success = false,
                        BuilderTypeName = builderInfo.TypeName,
                        BuilderType = builderInfo.Type,
                        ErrorMessage = "Failed to save prefab"
                    };
                }

                Debug.Log($"[ManualBuilderExecutor] Built prefab ({builderInfo.Type}): {builderInfo.ExpectedPrefabPath}");

                return new BuildResult
                {
                    Success = true,
                    PrefabPath = builderInfo.ExpectedPrefabPath,
                    BuilderTypeName = builderInfo.TypeName,
                    BuilderType = builderInfo.Type
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"[ManualBuilderExecutor] Failed to execute {builderInfo.TypeName}: {e.Message}");
                return new BuildResult
                {
                    Success = false,
                    BuilderTypeName = builderInfo.TypeName,
                    BuilderType = builderInfo.Type,
                    ErrorMessage = e.Message
                };
            }
        }

        #endregion

        #region 파이프라인

        /// <summary>
        /// 빌더 실행 → 프리팹 저장 → JSON 분석까지 일괄 수행
        /// </summary>
        public static (BuildResult buildResult, string jsonPath) ExecuteBuilderAndAnalyze(
            PrefabBuilderRegistry.BuilderInfo builderInfo,
            bool overwritePrefab = true)
        {
            var buildResult = ExecuteBuilder(builderInfo, overwritePrefab);

            if (!buildResult.Success || string.IsNullOrEmpty(buildResult.PrefabPath))
            {
                return (buildResult, null);
            }

            // PrefabStructureAnalyzer를 사용하여 JSON 분석
            var jsonPath = PrefabStructureAnalyzer.Analyze(buildResult.PrefabPath);

            return (buildResult, jsonPath);
        }

        /// <summary>
        /// 빌더 실행 → 프리팹 → JSON → Generated 빌더까지 전체 파이프라인
        /// </summary>
        public static (BuildResult buildResult, string jsonPath, string generatedPath) ExecuteFullPipeline(
            PrefabBuilderRegistry.BuilderInfo builderInfo,
            bool overwritePrefab = true)
        {
            var (buildResult, jsonPath) = ExecuteBuilderAndAnalyze(builderInfo, overwritePrefab);

            if (!buildResult.Success || string.IsNullOrEmpty(jsonPath))
            {
                return (buildResult, jsonPath, null);
            }

            // PrefabBuilderGenerator를 사용하여 Generated 코드 생성
            var generatedPath = PrefabBuilderGenerator.Generate(jsonPath, forceOverwrite: true);

            return (buildResult, jsonPath, generatedPath);
        }

        /// <summary>
        /// 모든 Manual 빌더에 대해 전체 파이프라인 실행
        /// </summary>
        public static void ExecuteAllManualBuilders(bool overwritePrefab = true, bool generateCode = true)
        {
            var builders = FindAllManualBuilders();

            Debug.Log($"[ManualBuilderExecutor] Found {builders.Length} manual builders");

            int successCount = 0;
            int failCount = 0;

            foreach (var builder in builders)
            {
                try
                {
                    if (generateCode)
                    {
                        var (buildResult, jsonPath, generatedPath) = ExecuteFullPipeline(builder, overwritePrefab);

                        if (buildResult.Success)
                        {
                            successCount++;
                            Debug.Log($"[ManualBuilderExecutor] Full pipeline completed: {builder.PrefabName}");
                        }
                        else
                        {
                            failCount++;
                            Debug.LogWarning(
                                $"[ManualBuilderExecutor] Failed: {builder.PrefabName} - {buildResult.ErrorMessage}");
                        }
                    }
                    else
                    {
                        var (buildResult, jsonPath) = ExecuteBuilderAndAnalyze(builder, overwritePrefab);

                        if (buildResult.Success)
                        {
                            successCount++;
                        }
                        else
                        {
                            failCount++;
                        }
                    }
                }
                catch (Exception e)
                {
                    failCount++;
                    Debug.LogError($"[ManualBuilderExecutor] Exception for {builder.PrefabName}: {e.Message}");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[ManualBuilderExecutor] Completed: {successCount} success, {failCount} failed");
        }

        #endregion
    }
}
