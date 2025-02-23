/* ---------------------------------------
 * Project: 'uGUI Directed Navigation'
 * Studio:          IEVO
 * Site:     https://ievo.games/
 * -------------------------------------*/
using IEVO.UI.uGUIDirectedNavigation.Common;
using IEVO.UI.uGUIDirectedNavigation.Layout;
using UnityEditor;

namespace IEVO.UI.uGUIDirectedNavigation
{
    [CustomEditor(typeof(DirectedNavigationGrid), true), CanEditMultipleObjects]
    public partial class DirectedNavigationGridEditor : Editor
    {
        protected DirectedNavigationGrid m_Target;

        protected SerializedProperty m_ConfigLeftProperty;
        protected SerializedProperty m_ConfigRightProperty;
        protected SerializedProperty m_ConfigUpProperty;
        protected SerializedProperty m_ConfigDownProperty;


        static protected bool s_LeftFoldoutState = true;
        static protected bool s_RightFoldoutState = true;
        static protected bool s_UpFoldoutState = true;
        static protected bool s_DownFoldoutState = true;


        protected virtual void OnEnable()
        {
            m_Target = target as DirectedNavigationGrid;

            m_ConfigLeftProperty = serializedObject.FindProperty("m_ConfigLeft");
            m_ConfigRightProperty = serializedObject.FindProperty("m_ConfigRight");
            m_ConfigUpProperty = serializedObject.FindProperty("m_ConfigUp");
            m_ConfigDownProperty = serializedObject.FindProperty("m_ConfigDown");

            m_Target.OnUpdateGrid += OnUpdateGrid;
            m_Target.UpdateGrid();
        }

        protected virtual void OnDisable()
        {
            m_Target.OnUpdateGrid -= OnUpdateGrid;
        }


        protected void OnUpdateGrid()
        {
            DirectedNavigationEditor.UpdateNavigationVisualization();
        }


        protected void DrawSide(SerializedProperty sideConfigProperty)
        {
            SerializedProperty outgoingTransitionsProperty = sideConfigProperty.FindPropertyRelative("m_OutgoingTransitions");
            SerializedProperty wrapAroundProperty = sideConfigProperty.FindPropertyRelative("m_WrapAround");
            SerializedProperty strictProperty = sideConfigProperty.FindPropertyRelative("m_Strict");
            SerializedProperty loopProperty = sideConfigProperty.FindPropertyRelative("m_Loop");
            SerializedProperty directivityProperty = sideConfigProperty.FindPropertyRelative("m_Directivity");

            EditorGUI.indentLevel++;

            outgoingTransitionsProperty.boolValue = EditorGUILayout.Toggle(Texts.Field_OutgoingTransitions, outgoingTransitionsProperty.boolValue);

            wrapAroundProperty.intValue = EditorGUILayout.Popup(Texts.Field_WrapAround, wrapAroundProperty.intValue, WrapAroundType.Names);
            if (wrapAroundProperty.intValue != WrapAroundType.Value.Disabled.ToInt())
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                strictProperty.boolValue = EditorGUILayout.Toggle(Texts.Field_Strict, strictProperty.boolValue);

                if (wrapAroundProperty.intValue == WrapAroundType.Value.Prev.ToInt() || wrapAroundProperty.intValue == WrapAroundType.Value.Next.ToInt())
                {
                    loopProperty.boolValue = EditorGUILayout.Toggle(Texts.Field_Loop, loopProperty.boolValue);
                }
                InspectorElements.DrawDirectivityProperty(directivityProperty);

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }


        public override void OnInspectorGUI()
        {
            if (m_Target.DirectedNavigations.Count == 0)
                m_Target.UpdateGrid();

            if (m_Target.DirectedNavigations.Count == 0)
            {
                EditorGUILayout.Space();
                EditorGUI.HelpBox(EditorGUILayout.GetControlRect(false, 50f), Texts.Notification_NoDirectedNavigationComponents, MessageType.Info);
                EditorGUILayout.Space();
            }

            bool gridLayoutGroupIsActive = m_Target.GridLayoutGroup.isActiveAndEnabled;

            if (!gridLayoutGroupIsActive)
            {
                EditorGUILayout.Space();
                EditorGUI.HelpBox(EditorGUILayout.GetControlRect(false, 40f), Texts.Notification_GridLayoutGroupNotActive, MessageType.Info);
                EditorGUILayout.Space();
            }

            EditorGUI.BeginDisabledGroup(!gridLayoutGroupIsActive);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();

            s_LeftFoldoutState = EditorGUILayout.Foldout(s_LeftFoldoutState, Texts.Label_Left, Style.Inspector.SideFoldoutStyle);
            if (s_LeftFoldoutState)
            {
                DrawSide(m_ConfigLeftProperty);
            }

            s_RightFoldoutState = EditorGUILayout.Foldout(s_RightFoldoutState, Texts.Label_Right, Style.Inspector.SideFoldoutStyle);
            if (s_RightFoldoutState)
            {
                DrawSide(m_ConfigRightProperty);
            }

            s_UpFoldoutState = EditorGUILayout.Foldout(s_UpFoldoutState, Texts.Label_Up, Style.Inspector.SideFoldoutStyle);
            if (s_UpFoldoutState)
            {
                DrawSide(m_ConfigUpProperty);
            }

            s_DownFoldoutState = EditorGUILayout.Foldout(s_DownFoldoutState, Texts.Label_Down, Style.Inspector.SideFoldoutStyle);
            if (s_DownFoldoutState)
            {
                DrawSide(m_ConfigDownProperty);
            }

            EditorGUILayout.Space();

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                m_Target.UpdateGrid();
            }
        }


    }
}
