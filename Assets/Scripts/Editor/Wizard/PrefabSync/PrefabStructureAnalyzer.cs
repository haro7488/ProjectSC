using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.Plastic.Newtonsoft.Json;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Editor.Wizard.PrefabSync
{
    /// <summary>
    /// 프리팹 구조를 분석하여 JSON Spec으로 변환하는 Analyzer.
    /// </summary>
    public static class PrefabStructureAnalyzer
    {
        private const string OUTPUT_FOLDER = "Assets/Scripts/Editor/Wizard/PrefabSync/Specs";

        // 분석할 주요 컴포넌트 타입들
        private static readonly HashSet<Type> TrackedComponentTypes = new()
        {
            // Unity UI
            typeof(Image),
            typeof(RawImage),
            typeof(Button),
            typeof(Toggle),
            typeof(Slider),
            typeof(Scrollbar),
            typeof(ScrollRect),
            typeof(InputField),
            typeof(Dropdown),
            typeof(Mask),
            typeof(RectMask2D),

            // TextMeshPro
            typeof(TextMeshProUGUI),
            typeof(TMP_InputField),
            typeof(TMP_Dropdown),

            // Layout
            typeof(VerticalLayoutGroup),
            typeof(HorizontalLayoutGroup),
            typeof(GridLayoutGroup),
            typeof(LayoutElement),
            typeof(ContentSizeFitter),
            typeof(AspectRatioFitter),

            // Canvas
            typeof(Canvas),
            typeof(CanvasGroup),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        };

        // 무시할 컴포넌트
        private static readonly HashSet<Type> IgnoredComponentTypes = new()
        {
            typeof(Transform),
            typeof(RectTransform)
        };

        #region Public API

        /// <summary>
        /// 프리팹을 분석하여 JSON Spec 파일 생성.
        /// </summary>
        public static string Analyze(string prefabPath)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found: {prefabPath}");
                return null;
            }

            var spec = AnalyzePrefab(prefab, prefabPath);
            var json = SerializeSpec(spec);
            var outputPath = SaveSpec(spec.metadata.prefabName, json);

            Debug.Log($"Prefab analyzed: {prefabPath}\nSpec saved to: {outputPath}");
            return outputPath;
        }

        /// <summary>
        /// LobbyScreen 프리팹 분석 (테스트용 메뉴).
        /// </summary>
        [MenuItem("Tools/ProjectSC/PrefabSync/Analyze LobbyScreen")]
        public static void AnalyzeLobbyScreen()
        {
            Analyze("Assets/Prefabs/UI/Screens/LobbyScreen.prefab");
        }

        /// <summary>
        /// 선택된 프리팹 분석.
        /// </summary>
        [MenuItem("Tools/ProjectSC/PrefabSync/Analyze Selected Prefab")]
        public static void AnalyzeSelected()
        {
            var selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogError("No prefab selected");
                return;
            }

            var path = AssetDatabase.GetAssetPath(selected);
            if (string.IsNullOrEmpty(path) || !path.EndsWith(".prefab"))
            {
                Debug.LogError("Selected object is not a prefab");
                return;
            }

            Analyze(path);
        }

        #endregion

        #region Analysis

        private static PrefabStructureSpec AnalyzePrefab(GameObject prefab, string prefabPath)
        {
            var mainComponent = FindMainComponent(prefab);
            var colorPalette = new Dictionary<string, string>();
            var constants = new Dictionary<string, float>();

            var spec = new PrefabStructureSpec
            {
                metadata = new MetadataInfo
                {
                    prefabName = prefab.name,
                    prefabPath = prefabPath,
                    componentType = mainComponent?.GetType().Name ?? "",
                    componentNamespace = mainComponent?.GetType().Namespace ?? "",
                    generatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    builderPath = GuessBuilderPath(prefab.name)
                },
                hierarchy = AnalyzeHierarchy(prefab.transform, "", colorPalette, constants),
                serializedFields = mainComponent != null
                    ? AnalyzeSerializedFields(mainComponent, prefab.transform)
                    : new List<SerializedFieldMapping>(),
                theme = new ThemeInfo
                {
                    colors = colorPalette,
                    constants = constants,
                    fonts = new Dictionary<string, string>()
                }
            };

            return spec;
        }

        private static HierarchyNode AnalyzeHierarchy(
            Transform transform,
            string parentPath,
            Dictionary<string, string> colorPalette,
            Dictionary<string, float> constants)
        {
            var go = transform.gameObject;
            var currentPath = string.IsNullOrEmpty(parentPath) ? go.name : $"{parentPath}/{go.name}";

            var node = new HierarchyNode
            {
                name = go.name,
                path = currentPath,
                active = go.activeSelf,
                rect = RectInfoExtensions.FromRectTransform(go.GetComponent<RectTransform>()),
                components = AnalyzeComponents(go, colorPalette, constants),
                children = new List<HierarchyNode>()
            };

            // 위젯 컴포넌트가 있으면 SerializeField 분석
            var widgetComponent = FindWidgetComponent(go, isRoot: string.IsNullOrEmpty(parentPath));
            if (widgetComponent != null)
            {
                node.widgetFields = AnalyzeWidgetFields(widgetComponent, transform);
            }

            // 자식 순회
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                node.children.Add(AnalyzeHierarchy(child, currentPath, colorPalette, constants));
            }

            return node;
        }

        private static List<ComponentInfo> AnalyzeComponents(
            GameObject go,
            Dictionary<string, string> colorPalette,
            Dictionary<string, float> constants)
        {
            var result = new List<ComponentInfo>();
            var components = go.GetComponents<Component>();

            foreach (var comp in components)
            {
                if (comp == null) continue;

                var type = comp.GetType();

                // 무시할 컴포넌트
                if (IgnoredComponentTypes.Contains(type)) continue;

                var info = new ComponentInfo
                {
                    type = type.Name,
                    fullType = type.FullName,
                    isMainComponent = IsMainComponent(type),
                    properties = new Dictionary<string, object>()
                };

                // 주요 컴포넌트 속성 추출
                ExtractComponentProperties(comp, info, colorPalette, constants);

                result.Add(info);
            }

            return result;
        }

        private static void ExtractComponentProperties(
            Component comp,
            ComponentInfo info,
            Dictionary<string, string> colorPalette,
            Dictionary<string, float> constants)
        {
            switch (comp)
            {
                case Image img:
                    info.properties["color"] = ColorToHex(img.color);
                    info.properties["raycastTarget"] = img.raycastTarget;
                    if (img.sprite != null)
                        info.properties["sprite"] = AssetDatabase.GetAssetPath(img.sprite);
                    info.properties["type"] = img.type.ToString();
                    TrackColor(colorPalette, img.color);
                    break;

                case TextMeshProUGUI tmp:
                    info.properties["text"] = tmp.text;
                    info.properties["fontSize"] = tmp.fontSize;
                    info.properties["color"] = ColorToHex(tmp.color);
                    info.properties["alignment"] = tmp.alignment.ToString();
                    info.properties["fontStyle"] = tmp.fontStyle.ToString();
                    info.properties["raycastTarget"] = tmp.raycastTarget;
                    if (tmp.font != null)
                        info.properties["font"] = AssetDatabase.GetAssetPath(tmp.font);
                    TrackColor(colorPalette, tmp.color);
                    constants[$"FontSize_{tmp.fontSize}"] = tmp.fontSize;
                    break;

                case Button btn:
                    info.properties["interactable"] = btn.interactable;
                    info.properties["transition"] = btn.transition.ToString();
                    break;

                case VerticalLayoutGroup vlg:
                    info.properties["spacing"] = vlg.spacing;
                    info.properties["padding"] = new[]
                        { vlg.padding.left, vlg.padding.right, vlg.padding.top, vlg.padding.bottom };
                    info.properties["childAlignment"] = vlg.childAlignment.ToString();
                    info.properties["childControlWidth"] = vlg.childControlWidth;
                    info.properties["childControlHeight"] = vlg.childControlHeight;
                    info.properties["childForceExpandWidth"] = vlg.childForceExpandWidth;
                    info.properties["childForceExpandHeight"] = vlg.childForceExpandHeight;
                    constants[$"Spacing_{vlg.spacing}"] = vlg.spacing;
                    break;

                case HorizontalLayoutGroup hlg:
                    info.properties["spacing"] = hlg.spacing;
                    info.properties["padding"] = new[]
                        { hlg.padding.left, hlg.padding.right, hlg.padding.top, hlg.padding.bottom };
                    info.properties["childAlignment"] = hlg.childAlignment.ToString();
                    info.properties["childControlWidth"] = hlg.childControlWidth;
                    info.properties["childControlHeight"] = hlg.childControlHeight;
                    info.properties["childForceExpandWidth"] = hlg.childForceExpandWidth;
                    info.properties["childForceExpandHeight"] = hlg.childForceExpandHeight;
                    constants[$"Spacing_{hlg.spacing}"] = hlg.spacing;
                    break;

                case GridLayoutGroup glg:
                    info.properties["cellSize"] = new[] { glg.cellSize.x, glg.cellSize.y };
                    info.properties["spacing"] = new[] { glg.spacing.x, glg.spacing.y };
                    info.properties["startCorner"] = glg.startCorner.ToString();
                    info.properties["startAxis"] = glg.startAxis.ToString();
                    info.properties["childAlignment"] = glg.childAlignment.ToString();
                    info.properties["constraint"] = glg.constraint.ToString();
                    info.properties["constraintCount"] = glg.constraintCount;
                    info.properties["padding"] = new[]
                        { glg.padding.left, glg.padding.right, glg.padding.top, glg.padding.bottom };
                    break;

                case LayoutElement le:
                    if (le.minWidth >= 0) info.properties["minWidth"] = le.minWidth;
                    if (le.minHeight >= 0) info.properties["minHeight"] = le.minHeight;
                    if (le.preferredWidth >= 0) info.properties["preferredWidth"] = le.preferredWidth;
                    if (le.preferredHeight >= 0) info.properties["preferredHeight"] = le.preferredHeight;
                    if (le.flexibleWidth >= 0) info.properties["flexibleWidth"] = le.flexibleWidth;
                    if (le.flexibleHeight >= 0) info.properties["flexibleHeight"] = le.flexibleHeight;
                    break;

                case ScrollRect sr:
                    info.properties["horizontal"] = sr.horizontal;
                    info.properties["vertical"] = sr.vertical;
                    info.properties["movementType"] = sr.movementType.ToString();
                    if (sr.viewport != null)
                        info.properties["viewport"] = GetRelativePath(sr.transform, sr.viewport);
                    if (sr.content != null)
                        info.properties["content"] = GetRelativePath(sr.transform, sr.content);
                    break;

                case Mask mask:
                    info.properties["showMaskGraphic"] = mask.showMaskGraphic;
                    break;

                case CanvasGroup cg:
                    info.properties["alpha"] = cg.alpha;
                    info.properties["interactable"] = cg.interactable;
                    info.properties["blocksRaycasts"] = cg.blocksRaycasts;
                    break;

                case ContentSizeFitter csf:
                    info.properties["horizontalFit"] = csf.horizontalFit.ToString();
                    info.properties["verticalFit"] = csf.verticalFit.ToString();
                    break;
            }
        }

        #endregion

        #region SerializedField Analysis

        /// <summary>
        /// 위젯 컴포넌트 찾기 (루트가 아닌 하위 노드에서만).
        /// </summary>
        private static Component FindWidgetComponent(GameObject go, bool isRoot)
        {
            // 루트 노드는 스킵 (serializedFields에서 처리됨)
            if (isRoot) return null;

            foreach (var comp in go.GetComponents<Component>())
            {
                if (comp == null) continue;
                var type = comp.GetType();
                var ns = type.Namespace ?? "";

                // Sc.* 네임스페이스의 위젯 타입
                if (ns.StartsWith("Sc.") && IsWidgetType(type.Name))
                {
                    return comp;
                }
            }

            return null;
        }

        /// <summary>
        /// 위젯 타입인지 확인.
        /// </summary>
        private static bool IsWidgetType(string typeName)
        {
            return typeName.EndsWith("Widget") ||
                   typeName.EndsWith("Carousel") ||
                   typeName.EndsWith("Button") ||
                   typeName.EndsWith("Display");
        }

        /// <summary>
        /// 위젯 컴포넌트의 SerializeField 분석 (위젯 기준 상대 경로 사용).
        /// </summary>
        private static List<SerializedFieldMapping> AnalyzeWidgetFields(Component widgetComponent, Transform widgetRoot)
        {
            var result = new List<SerializedFieldMapping>();
            var type = widgetComponent.GetType();
            var so = new SerializedObject(widgetComponent);

            // SerializeField 속성이 있는 private 필드들 찾기
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<SerializeField>() != null)
                .ToList();

            foreach (var field in fields)
            {
                var prop = so.FindProperty(field.Name);
                if (prop == null) continue;

                // ObjectReference 타입만 처리 (Unity 오브젝트 참조)
                if (!prop.isArray && prop.propertyType != SerializedPropertyType.ObjectReference)
                    continue;

                var mapping = new SerializedFieldMapping
                {
                    fieldName = field.Name,
                    fieldType = field.FieldType.Name,
                    isArray = prop.isArray
                };

                if (prop.isArray)
                {
                    // 배열 요소 타입 확인
                    var elementType = field.FieldType.IsArray
                        ? field.FieldType.GetElementType()
                        : field.FieldType.GenericTypeArguments.FirstOrDefault();

                    if (elementType == null || !IsUnityObjectType(elementType))
                        continue;

                    mapping.arrayPaths = new List<string>();
                    for (int i = 0; i < prop.arraySize; i++)
                    {
                        var element = prop.GetArrayElementAtIndex(i);
                        var path = GetPathFromProperty(element, widgetRoot);
                        if (path != null)
                            mapping.arrayPaths.Add(path);
                    }

                    if (mapping.arrayPaths.Count > 0)
                    {
                        mapping.targetType = elementType.Name;
                        result.Add(mapping);
                    }
                }
                else
                {
                    mapping.targetPath = GetPathFromProperty(prop, widgetRoot);

                    if (prop.objectReferenceValue != null)
                    {
                        mapping.targetType = prop.objectReferenceValue.GetType().Name;
                    }

                    // 유효한 매핑만 추가
                    if (!string.IsNullOrEmpty(mapping.targetPath))
                    {
                        result.Add(mapping);
                    }
                }
            }

            return result.Count > 0 ? result : null;
        }

        /// <summary>
        /// Unity 오브젝트 타입인지 확인.
        /// </summary>
        private static bool IsUnityObjectType(Type type)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(type);
        }

        private static List<SerializedFieldMapping> AnalyzeSerializedFields(Component mainComponent, Transform root)
        {
            var result = new List<SerializedFieldMapping>();
            var type = mainComponent.GetType();
            var so = new SerializedObject(mainComponent);

            // SerializeField 속성이 있는 private 필드들 찾기
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<SerializeField>() != null)
                .ToList();

            foreach (var field in fields)
            {
                var prop = so.FindProperty(field.Name);
                if (prop == null) continue;

                var mapping = new SerializedFieldMapping
                {
                    fieldName = field.Name,
                    fieldType = field.FieldType.Name,
                    isArray = prop.isArray
                };

                if (prop.isArray)
                {
                    mapping.arrayPaths = new List<string>();
                    for (int i = 0; i < prop.arraySize; i++)
                    {
                        var element = prop.GetArrayElementAtIndex(i);
                        var path = GetPathFromProperty(element, root);
                        if (path != null)
                            mapping.arrayPaths.Add(path);
                    }

                    mapping.targetPath = mapping.arrayPaths.Count > 0 ? mapping.arrayPaths[0] : null;
                }
                else
                {
                    mapping.targetPath = GetPathFromProperty(prop, root);
                }

                // ObjectReference 타입인 경우에만 objectReferenceValue 접근
                if (prop.propertyType == SerializedPropertyType.ObjectReference &&
                    prop.objectReferenceValue != null)
                {
                    mapping.targetType = prop.objectReferenceValue.GetType().Name;
                }

                // 유효한 매핑만 추가
                if (!string.IsNullOrEmpty(mapping.targetPath) ||
                    (mapping.arrayPaths != null && mapping.arrayPaths.Count > 0))
                {
                    result.Add(mapping);
                }
            }

            return result;
        }

        private static string GetPathFromProperty(SerializedProperty prop, Transform root)
        {
            if (prop.propertyType != SerializedPropertyType.ObjectReference)
                return null;

            var obj = prop.objectReferenceValue;
            if (obj == null) return null;

            Transform target = null;

            if (obj is GameObject go)
                target = go.transform;
            else if (obj is Component comp)
                target = comp.transform;

            if (target == null) return null;

            return GetRelativePath(root, target);
        }

        #endregion

        #region Utilities

        private static Component FindMainComponent(GameObject prefab)
        {
            // Screen, Popup, Widget 등 주요 컴포넌트 찾기
            var components = prefab.GetComponents<Component>();

            foreach (var comp in components)
            {
                if (comp == null) continue;
                if (IsMainComponent(comp.GetType()))
                    return comp;
            }

            return null;
        }

        private static bool IsMainComponent(Type type)
        {
            if (type == null) return false;

            var name = type.Name;
            var ns = type.Namespace ?? "";

            // Sc 네임스페이스의 Screen, Popup, Widget
            if (ns.StartsWith("Sc."))
            {
                if (name.EndsWith("Screen") || name.EndsWith("Popup") ||
                    name.EndsWith("Widget") || name.EndsWith("Button") ||
                    name.EndsWith("Carousel"))
                    return true;
            }

            return false;
        }

        private static string GetRelativePath(Transform root, Transform target)
        {
            if (target == null || root == null) return null;
            if (target == root) return "";

            var path = target.name;
            var current = target.parent;

            while (current != null && current != root)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            if (current == null) return null; // target이 root의 자식이 아님
            return path;
        }

        private static string ColorToHex(Color color)
        {
            return $"#{ColorUtility.ToHtmlStringRGBA(color)}";
        }

        private static void TrackColor(Dictionary<string, string> palette, Color color)
        {
            var hex = ColorToHex(color);
            var key = GenerateColorName(color);

            if (!palette.ContainsKey(key))
                palette[key] = hex;
        }

        private static string GenerateColorName(Color color)
        {
            // 알파 기반 이름
            if (color.a < 0.1f) return $"Transparent_{color.GetHashCode():X8}";
            if (color.a < 0.5f) return $"Glass_{color.GetHashCode():X8}";

            // 흑백 계열
            if (color.r < 0.15f && color.g < 0.15f && color.b < 0.15f)
                return $"Dark_{color.GetHashCode():X8}";
            if (color.r > 0.9f && color.g > 0.9f && color.b > 0.9f)
                return $"Light_{color.GetHashCode():X8}";

            // 색상 계열
            var max = Mathf.Max(color.r, color.g, color.b);
            if (color.r == max && color.r > color.g + 0.2f && color.r > color.b + 0.2f)
                return $"Red_{color.GetHashCode():X8}";
            if (color.g == max && color.g > color.r + 0.2f && color.g > color.b + 0.2f)
                return $"Green_{color.GetHashCode():X8}";
            if (color.b == max && color.b > color.r + 0.2f && color.b > color.g + 0.2f)
                return $"Blue_{color.GetHashCode():X8}";

            return $"Color_{color.GetHashCode():X8}";
        }

        private static string GuessBuilderPath(string prefabName)
        {
            return $"Assets/Scripts/Editor/Wizard/Generators/{prefabName}PrefabBuilder.cs";
        }

        #endregion

        #region Serialization

        private static string SerializeSpec(PrefabStructureSpec spec)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(spec, settings);
        }

        private static string SaveSpec(string prefabName, string json)
        {
            if (!Directory.Exists(OUTPUT_FOLDER))
                Directory.CreateDirectory(OUTPUT_FOLDER);

            var outputPath = $"{OUTPUT_FOLDER}/{prefabName}.structure.json";
            File.WriteAllText(outputPath, json);
            AssetDatabase.Refresh();

            return outputPath;
        }

        #endregion
    }
}