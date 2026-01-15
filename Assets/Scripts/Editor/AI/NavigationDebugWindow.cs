using UnityEngine;
using UnityEditor;
using Sc.Common.UI;
using Sc.Common.UI.Tests;

namespace Sc.Editor.AI
{
    /// <summary>
    /// UI Navigation 상태를 시각적으로 표시하는 에디터 윈도우.
    /// 통합 스택 구조 표시.
    /// </summary>
    public class NavigationDebugWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private bool _autoRepaint = true;
        private float _lastRepaintTime;

        // 스타일
        private GUIStyle _headerStyle;
        private GUIStyle _stackItemStyle;
        private GUIStyle _screenItemStyle;
        private GUIStyle _popupItemStyle;
        private GUIStyle _currentItemStyle;
        private GUIStyle _emptyStyle;
        private bool _stylesInitialized;

        [MenuItem("SC Tools/UI Test/Navigation Debug Window", priority = 110)]
        public static void ShowWindow()
        {
            var window = GetWindow<NavigationDebugWindow>("Navigation Debug");
            window.minSize = new Vector2(320, 450);
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            Repaint();
        }

        private void InitStyles()
        {
            if (_stylesInitialized) return;

            _headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                margin = new RectOffset(5, 5, 10, 5)
            };

            _stackItemStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 8, 8),
                margin = new RectOffset(5, 5, 2, 2)
            };

            _screenItemStyle = new GUIStyle(_stackItemStyle);
            _screenItemStyle.normal.background = MakeTexture(2, 2, new Color(0.15f, 0.25f, 0.4f, 0.3f));

            _popupItemStyle = new GUIStyle(_stackItemStyle);
            _popupItemStyle.normal.background = MakeTexture(2, 2, new Color(0.4f, 0.25f, 0.15f, 0.3f));

            _currentItemStyle = new GUIStyle(_stackItemStyle);
            _currentItemStyle.normal.background = MakeTexture(2, 2, new Color(0.2f, 0.5f, 0.3f, 0.4f));

            _emptyStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 11,
                padding = new RectOffset(10, 10, 20, 20)
            };

            _stylesInitialized = true;
        }

        private void OnGUI()
        {
            InitStyles();

            // 헤더
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            _autoRepaint = GUILayout.Toggle(_autoRepaint, "Auto Refresh", EditorStyles.toolbarButton);
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                Repaint();
            }
            EditorGUILayout.EndHorizontal();

            // Play 모드 체크
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Play 모드에서만 Navigation 상태를 확인할 수 있습니다.", MessageType.Info);
                return;
            }

            // UITestSetup 체크
            var testSetup = UITestSetup.Instance;
            if (testSetup == null)
            {
                EditorGUILayout.HelpBox("UITestSetup이 씬에 없습니다.\nSC Tools > UI Test > Setup Test Scene을 실행하세요.", MessageType.Warning);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // 통합 스택 표시
            DrawNavigationStack(testSetup);

            EditorGUILayout.Space(10);

            // Controls
            DrawControls(testSetup);

            EditorGUILayout.EndScrollView();

            // Auto repaint
            if (_autoRepaint && Application.isPlaying)
            {
                if (Time.realtimeSinceStartup - _lastRepaintTime > 0.1f)
                {
                    _lastRepaintTime = Time.realtimeSinceStartup;
                    Repaint();
                }
            }
        }

        private void DrawNavigationStack(UITestSetup testSetup)
        {
            EditorGUILayout.LabelField("Navigation Stack (Unified)", _headerStyle);

            // 범례
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUI.backgroundColor = new Color(0.15f, 0.25f, 0.4f, 0.8f);
            GUILayout.Box(" [S] Screen ", GUILayout.Width(80));
            GUI.backgroundColor = new Color(0.4f, 0.25f, 0.15f, 0.8f);
            GUILayout.Box(" [P] Popup ", GUILayout.Width(80));
            GUI.backgroundColor = new Color(0.2f, 0.5f, 0.3f, 0.8f);
            GUILayout.Box(" Current ", GUILayout.Width(70));
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            var stack = testSetup.NavigationStack;
            if (stack.Count == 0)
            {
                EditorGUILayout.LabelField("(Empty)", _emptyStyle);
                return;
            }

            // 역순으로 표시 (최상위가 위로)
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                var entry = stack[i];
                if (entry?.Widget == null) continue;

                bool isCurrent = i == stack.Count - 1;

                // 스타일 선택
                GUIStyle style;
                if (isCurrent)
                    style = _currentItemStyle;
                else if (entry.IsScreen)
                    style = _screenItemStyle;
                else
                    style = _popupItemStyle;

                EditorGUILayout.BeginHorizontal(style);

                // 인덱스 + 타입
                var typeMarker = entry.IsScreen ? "[S]" : "[P]";
                EditorGUILayout.LabelField($"{i} {typeMarker}", GUILayout.Width(45));

                // 이름
                string displayName = entry.Widget.gameObject.name;
                if (isCurrent) displayName += " ◀ TOP";
                EditorGUILayout.LabelField(displayName);

                // Visible 상태
                var visibleIcon = entry.Widget.IsVisible ? "d_scenevis_visible_hover" : "d_scenevis_hidden_hover";
                GUILayout.Label(EditorGUIUtility.IconContent(visibleIcon), GUILayout.Width(20));

                // Select 버튼
                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    Selection.activeGameObject = entry.Widget.gameObject;
                }

                // Close 버튼 (Popup만)
                if (entry.IsPopup)
                {
                    if (GUILayout.Button("X", GUILayout.Width(22)))
                    {
                        testSetup.PopPopup(entry.AsPopup);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawControls(UITestSetup testSetup)
        {
            EditorGUILayout.LabelField("Controls", _headerStyle);

            // Push 버튼
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Push Screen", GUILayout.Height(30)))
            {
                testSetup.PushScreen();
            }
            if (GUILayout.Button("Push Popup", GUILayout.Height(30)))
            {
                testSetup.PushPopup();
            }
            EditorGUILayout.EndHorizontal();

            // Pop 버튼
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = testSetup.StackCount > 0;
            if (GUILayout.Button("Pop (Unified)", GUILayout.Height(30)))
            {
                testSetup.Pop();
            }
            GUI.enabled = true;
            if (GUILayout.Button("Clear Popups", GUILayout.Height(30)))
            {
                testSetup.ClearPopups();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Full Flow Test
            if (GUILayout.Button("Test Full Flow", GUILayout.Height(28)))
            {
                // 기존 데이터 정리 후 테스트
                var method = testSetup.GetType().GetMethod("TestFullFlow",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method?.Invoke(testSetup, null);
            }

            EditorGUILayout.Space(10);

            // 상태 요약
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Summary", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total Stack: {testSetup.StackCount}");
            EditorGUILayout.LabelField($"Screens: {testSetup.ScreenCount}");
            EditorGUILayout.LabelField($"Popups: {testSetup.StackCount - testSetup.ScreenCount}");
            EditorGUILayout.Space(3);

            var currentEntry = testSetup.CurrentEntry;
            if (currentEntry != null)
            {
                var typeStr = currentEntry.IsScreen ? "Screen" : "Popup";
                EditorGUILayout.LabelField($"Top: [{typeStr}] {currentEntry.Widget.gameObject.name}");
            }
            else
            {
                EditorGUILayout.LabelField("Top: None");
            }

            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Stack:", EditorStyles.miniLabel);
            EditorGUILayout.LabelField(testSetup.GetStackDebugString(), EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();
        }

        private static Texture2D MakeTexture(int width, int height, Color color)
        {
            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            var texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}
