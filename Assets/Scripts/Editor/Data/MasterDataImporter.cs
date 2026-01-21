using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sc.Editor.Data
{
    /// <summary>
    /// JSON 파일 변경 감지 및 SO 자동 생성
    /// </summary>
    public class MasterDataImporter : AssetPostprocessor
    {
        private const string JsonPath = "Assets/Data/MasterData";
        private const string OutputPath = "Assets/Data/Generated";

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (var assetPath in importedAssets)
            {
                if (!assetPath.StartsWith(JsonPath) || !assetPath.EndsWith(".json"))
                    continue;

                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                if (fileName == "README") continue;

                try
                {
                    ProcessJsonFile(assetPath, fileName);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[MasterDataImporter] {fileName}.json 처리 실패: {e.Message}");
                }
            }
        }

        private static void ProcessJsonFile(string assetPath, string fileName)
        {
            var json = File.ReadAllText(assetPath);

            switch (fileName)
            {
                case "Character":
                    ImportCharacters(json);
                    break;
                case "Skill":
                    ImportSkills(json);
                    break;
                case "Item":
                    ImportItems(json);
                    break;
                case "Stage":
                    ImportStages(json);
                    break;
                case "GachaPool":
                    ImportGachaPools(json);
                    break;
                case "ScreenHeaderConfig":
                    ImportScreenHeaderConfigs(json);
                    break;
                default:
                    Debug.LogWarning($"[MasterDataImporter] 알 수 없는 데이터 타입: {fileName}");
                    break;
            }
        }

        #region Character Import

        [Serializable]
        private class CharacterJsonWrapper
        {
            public string version;
            public string exportedAt;
            public CharacterJson[] data;
        }

        [Serializable]
        private class CharacterJson
        {
            public string Id;
            public string Name;
            public string NameEn;
            public string Rarity;
            public string Class;
            public string Element;
            public int BaseHP;
            public int BaseATK;
            public int BaseDEF;
            public int BaseSPD;
            public float CritRate;
            public float CritDamage;
            public string[] SkillIds;
            public string Description;
        }

        private static void ImportCharacters(string json)
        {
            var wrapper = JsonUtility.FromJson<CharacterJsonWrapper>(json);
            if (wrapper?.data == null) return;

            EnsureOutputDirectory();
            var dataDir = $"{OutputPath}/Characters";
            EnsureDirectory(dataDir);

            var database = LoadOrCreateDatabase<Sc.Data.CharacterDatabase>($"{OutputPath}/CharacterDatabase.asset");
            database.Clear();

            foreach (var item in wrapper.data)
            {
                var assetPath = $"{dataDir}/{item.Id}.asset";
                var data = LoadOrCreateAsset<Sc.Data.CharacterData>(assetPath);

                data.Initialize(
                    item.Id,
                    item.Name,
                    item.NameEn,
                    ParseEnum<Sc.Data.Rarity>(item.Rarity),
                    ParseEnum<Sc.Data.CharacterClass>(item.Class),
                    ParseEnum<Sc.Data.Element>(item.Element),
                    item.BaseHP,
                    item.BaseATK,
                    item.BaseDEF,
                    item.BaseSPD,
                    item.CritRate,
                    item.CritDamage,
                    item.SkillIds ?? Array.Empty<string>(),
                    item.Description ?? ""
                );

                EditorUtility.SetDirty(data);
                database.Add(data);
            }

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"[MasterDataImporter] Character {wrapper.data.Length}개 임포트 완료");
        }

        #endregion

        #region Skill Import

        [Serializable]
        private class SkillJsonWrapper
        {
            public string version;
            public string exportedAt;
            public SkillJson[] data;
        }

        [Serializable]
        private class SkillJson
        {
            public string Id;
            public string Name;
            public string NameEn;
            public string Type;
            public string TargetType;
            public string Element;
            public int Power;
            public int CoolDown;
            public int ManaCost;
            public string Description;
        }

        private static void ImportSkills(string json)
        {
            var wrapper = JsonUtility.FromJson<SkillJsonWrapper>(json);
            if (wrapper?.data == null) return;

            EnsureOutputDirectory();
            var dataDir = $"{OutputPath}/Skills";
            EnsureDirectory(dataDir);

            var database = LoadOrCreateDatabase<Sc.Data.SkillDatabase>($"{OutputPath}/SkillDatabase.asset");
            database.Clear();

            foreach (var item in wrapper.data)
            {
                var assetPath = $"{dataDir}/{item.Id}.asset";
                var data = LoadOrCreateAsset<Sc.Data.SkillData>(assetPath);

                data.Initialize(
                    item.Id,
                    item.Name,
                    item.NameEn,
                    ParseEnum<Sc.Data.SkillType>(item.Type),
                    ParseEnum<Sc.Data.TargetType>(item.TargetType),
                    ParseEnum<Sc.Data.Element>(item.Element),
                    item.Power,
                    item.CoolDown,
                    item.ManaCost,
                    item.Description ?? ""
                );

                EditorUtility.SetDirty(data);
                database.Add(data);
            }

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"[MasterDataImporter] Skill {wrapper.data.Length}개 임포트 완료");
        }

        #endregion

        #region Item Import

        [Serializable]
        private class ItemJsonWrapper
        {
            public string version;
            public string exportedAt;
            public ItemJson[] data;
        }

        [Serializable]
        private class ItemJson
        {
            public string Id;
            public string Name;
            public string NameEn;
            public string Type;
            public string Rarity;
            public int ATKBonus;
            public int DEFBonus;
            public int HPBonus;
            public string Description;
        }

        private static void ImportItems(string json)
        {
            var wrapper = JsonUtility.FromJson<ItemJsonWrapper>(json);
            if (wrapper?.data == null) return;

            EnsureOutputDirectory();
            var dataDir = $"{OutputPath}/Items";
            EnsureDirectory(dataDir);

            var database = LoadOrCreateDatabase<Sc.Data.ItemDatabase>($"{OutputPath}/ItemDatabase.asset");
            database.Clear();

            foreach (var item in wrapper.data)
            {
                var assetPath = $"{dataDir}/{item.Id}.asset";
                var data = LoadOrCreateAsset<Sc.Data.ItemData>(assetPath);

                data.Initialize(
                    item.Id,
                    item.Name,
                    item.NameEn,
                    ParseEnum<Sc.Data.ItemType>(item.Type),
                    ParseEnum<Sc.Data.Rarity>(item.Rarity),
                    item.ATKBonus,
                    item.DEFBonus,
                    item.HPBonus,
                    item.Description ?? ""
                );

                EditorUtility.SetDirty(data);
                database.Add(data);
            }

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"[MasterDataImporter] Item {wrapper.data.Length}개 임포트 완료");
        }

        #endregion

        #region Stage Import

        [Serializable]
        private class StageJsonWrapper
        {
            public string version;
            public string exportedAt;
            public StageJson[] data;
        }

        [Serializable]
        private class StageJson
        {
            public string Id;
            public string Name;
            public string NameEn;
            public int Chapter;
            public int StageNumber;
            public string Difficulty;
            public int StaminaCost;
            public int RecommendedPower;
            public string[] EnemyIds;
            public int RewardGold;
            public int RewardExp;
            public string[] RewardItemIds;
            public float[] RewardItemRates;
            public string Description;
        }

        private static void ImportStages(string json)
        {
            var wrapper = JsonUtility.FromJson<StageJsonWrapper>(json);
            if (wrapper?.data == null) return;

            EnsureOutputDirectory();
            var dataDir = $"{OutputPath}/Stages";
            EnsureDirectory(dataDir);

            var database = LoadOrCreateDatabase<Sc.Data.StageDatabase>($"{OutputPath}/StageDatabase.asset");
            database.Clear();

            foreach (var item in wrapper.data)
            {
                var assetPath = $"{dataDir}/{item.Id}.asset";
                var data = LoadOrCreateAsset<Sc.Data.StageData>(assetPath);

                data.Initialize(
                    item.Id,
                    item.Name,
                    item.NameEn,
                    item.Chapter,
                    item.StageNumber,
                    ParseEnum<Sc.Data.Difficulty>(item.Difficulty),
                    item.StaminaCost,
                    item.RecommendedPower,
                    item.EnemyIds ?? Array.Empty<string>(),
                    item.RewardGold,
                    item.RewardExp,
                    item.RewardItemIds ?? Array.Empty<string>(),
                    item.RewardItemRates ?? Array.Empty<float>(),
                    item.Description ?? ""
                );

                EditorUtility.SetDirty(data);
                database.Add(data);
            }

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"[MasterDataImporter] Stage {wrapper.data.Length}개 임포트 완료");
        }

        #endregion

        #region GachaPool Import

        [Serializable]
        private class GachaPoolJsonWrapper
        {
            public string version;
            public string exportedAt;
            public GachaPoolJson[] data;
        }

        [Serializable]
        private class GachaPoolJson
        {
            public string Id;
            public string Name;
            public string NameEn;
            public string Type;
            public string CostType;
            public int CostAmount;
            public int CostAmount10;
            public int PityCount;
            public string[] CharacterIds;
            public GachaRatesJson Rates;
            public string RateUpCharacterId;
            public float RateUpBonus;
            public bool IsActive;
            public string StartDate;
            public string EndDate;
            public string BannerImagePath;
            public int DisplayOrder;
            public int PitySoftStart;
            public float PitySoftRateBonus;
            public string Description;
        }

        [Serializable]
        private class GachaRatesJson
        {
            public float SSR;
            public float SR;
            public float R;
        }

        private static void ImportGachaPools(string json)
        {
            var wrapper = JsonUtility.FromJson<GachaPoolJsonWrapper>(json);
            if (wrapper?.data == null) return;

            EnsureOutputDirectory();
            var dataDir = $"{OutputPath}/GachaPools";
            EnsureDirectory(dataDir);

            var database = LoadOrCreateDatabase<Sc.Data.GachaPoolDatabase>($"{OutputPath}/GachaPoolDatabase.asset");
            database.Clear();

            foreach (var item in wrapper.data)
            {
                var assetPath = $"{dataDir}/{item.Id}.asset";
                var data = LoadOrCreateAsset<Sc.Data.GachaPoolData>(assetPath);

                var rates = new Sc.Data.GachaRates
                {
                    SSR = item.Rates?.SSR ?? 0f,
                    SR = item.Rates?.SR ?? 0f,
                    R = item.Rates?.R ?? 0f
                };

                data.Initialize(
                    item.Id,
                    item.Name,
                    item.NameEn,
                    ParseEnum<Sc.Data.GachaType>(item.Type),
                    ParseEnum<Sc.Data.CostType>(item.CostType),
                    item.CostAmount,
                    item.CostAmount10,
                    item.PityCount,
                    item.CharacterIds ?? Array.Empty<string>(),
                    rates,
                    item.RateUpCharacterId ?? "",
                    item.RateUpBonus,
                    item.IsActive,
                    item.StartDate ?? "",
                    item.EndDate ?? "",
                    item.BannerImagePath ?? "",
                    item.DisplayOrder,
                    item.PitySoftStart,
                    item.PitySoftRateBonus,
                    item.Description ?? ""
                );

                EditorUtility.SetDirty(data);
                database.Add(data);
            }

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"[MasterDataImporter] GachaPool {wrapper.data.Length}개 임포트 완료");
        }

        #endregion

        #region ScreenHeaderConfig Import

        [Serializable]
        private class ScreenHeaderConfigJsonWrapper
        {
            public string version;
            public string exportedAt;
            public ScreenHeaderConfigJson[] data;
        }

        [Serializable]
        private class ScreenHeaderConfigJson
        {
            public string Id;
            public string Title;
            public bool ShowBackButton;
            public bool ShowProfileButton;
            public bool ShowMenuButton;
            public bool ShowMailButton;
            public bool ShowNoticeButton;
            public bool ShowCurrency;
            public string[] CurrencyTypes;
        }

        private static void ImportScreenHeaderConfigs(string json)
        {
            var wrapper = JsonUtility.FromJson<ScreenHeaderConfigJsonWrapper>(json);
            if (wrapper?.data == null) return;

            EnsureOutputDirectory();
            var dataDir = $"{OutputPath}/ScreenHeaderConfigs";
            EnsureDirectory(dataDir);

            var database = LoadOrCreateDatabase<Sc.Data.ScreenHeaderConfigDatabase>($"{OutputPath}/ScreenHeaderConfigDatabase.asset");
            database.Clear();

            foreach (var item in wrapper.data)
            {
                var assetPath = $"{dataDir}/{item.Id}.asset";
                var data = LoadOrCreateAsset<Sc.Data.ScreenHeaderConfigData>(assetPath);

                data.Initialize(
                    item.Id,
                    item.Title ?? "",
                    item.ShowBackButton,
                    item.ShowProfileButton,
                    item.ShowMenuButton,
                    item.ShowMailButton,
                    item.ShowNoticeButton,
                    item.ShowCurrency,
                    item.CurrencyTypes != null ? new List<string>(item.CurrencyTypes) : new List<string>()
                );

                EditorUtility.SetDirty(data);
                database.Add(data);
            }

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"[MasterDataImporter] ScreenHeaderConfig {wrapper.data.Length}개 임포트 완료");
        }

        #endregion

        #region Utility Methods

        private static void EnsureOutputDirectory()
        {
            EnsureDirectory(OutputPath);
        }

        private static void EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
                var folder = Path.GetFileName(path);
                if (!string.IsNullOrEmpty(parent) && !string.IsNullOrEmpty(folder))
                {
                    EnsureDirectory(parent);
                    AssetDatabase.CreateFolder(parent, folder);
                }
            }
        }

        private static T LoadOrCreateAsset<T>(string assetPath) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, assetPath);
            }
            return asset;
        }

        private static T LoadOrCreateDatabase<T>(string assetPath) where T : ScriptableObject
        {
            return LoadOrCreateAsset<T>(assetPath);
        }

        private static T ParseEnum<T>(string value) where T : struct, Enum
        {
            if (Enum.TryParse<T>(value, true, out var result))
                return result;

            Debug.LogWarning($"[MasterDataImporter] Enum 파싱 실패: {typeof(T).Name}.{value}");
            return default;
        }

        #endregion
    }
}
