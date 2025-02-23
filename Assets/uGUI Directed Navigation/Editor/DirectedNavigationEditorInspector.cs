/* ---------------------------------------
 * Project: 'uGUI Directed Navigation'
 * Studio:          IEVO
 * Site:     https://ievo.games/
 * -------------------------------------*/
using IEVO.UI.uGUIDirectedNavigation.Common;
using IEVO.UI.uGUIDirectedNavigation.Layout;
using UnityEditor;
using UnityEngine;

namespace IEVO.UI.uGUIDirectedNavigation
{
    public partial class DirectedNavigationEditor : Editor
    {
        private static SideFlagsEditor s_sideFlagsLeft = new SideFlagsEditor();
        private static SideFlagsEditor s_sideFlagsRigt = new SideFlagsEditor();
        private static SideFlagsEditor s_sideFlagsUp = new SideFlagsEditor();
        private static SideFlagsEditor s_sideFlagsDown = new SideFlagsEditor();
        
        private static bool s_ShowGizmos = true;

        private static GUIContent s_LabelShowGizmos = EditorGUIUtility.TrTextContent(Texts.Button_ShowGizmos, Texts.Button_ShowGizmosHint);        
        private static GUIContent s_LabelGizmosColor = EditorGUIUtility.TrTextContent(Texts.Field_GizmosColor);
        private static GUIContent s_LabelReset = EditorGUIUtility.TrTextContent(Texts.Button_Reset);
        private static GUIContent s_LabelList = EditorGUIUtility.TrTextContent(Texts.Field_List);
        private static GUIContent s_Value = EditorGUIUtility.TrTextContent(Texts.Field_Value);


        public override void OnInspectorGUI()
        {
            s_ShowNavigation = EditorPrefs.GetBool(s_ShowNavigationKey, true);
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            m_ActiveProperty.boolValue = EditorGUILayout.Toggle(Texts.Field_Active, m_ActiveProperty.boolValue);
            s_AdvancedSettings = EditorGUILayout.Toggle(Texts.Field_AdvancedSettings, s_AdvancedSettings);
            EditorGUILayout.EndHorizontal();

            //  DirectedNavigationGrid
            DirectedNavigationGrid directedNavigationGrid = m_Target.GetComponentInParent<DirectedNavigationGrid>();
            bool controlledByDirectedNavigationGrid = directedNavigationGrid != null && m_Target.transform.parent == directedNavigationGrid.transform;

            if (controlledByDirectedNavigationGrid)
            {
                EditorGUILayout.Space();

                EditorGUI.HelpBox(EditorGUILayout.GetControlRect(false, 40f), Texts.Notification_SettingsControlledByDirNav, MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);

                EditorGUILayout.ObjectField(directedNavigationGrid, typeof(DirectedNavigationGrid), false);

                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }
            //  DirectedNavigationGrid


            // BeginDisabledGroup
            EditorGUI.BeginDisabledGroup(!m_Target.Active || controlledByDirectedNavigationGrid);
            // BeginDisabledGroup

            EditorGUILayout.Space();

            // Left Foldout
            s_sideFlagsLeft.Foldout = EditorGUILayout.Foldout(s_sideFlagsLeft.Foldout, Texts.Label_Left, Style.Inspector.SideFoldoutStyle);
            if (s_sideFlagsLeft.Foldout)
            {
                DrawSideFoldoutContent(m_ConfigLeftProperty, s_sideFlagsLeft);
            }
            // Left Foldout

            // Right Foldout
            s_sideFlagsRigt.Foldout = EditorGUILayout.Foldout(s_sideFlagsRigt.Foldout, Texts.Label_Right, Style.Inspector.SideFoldoutStyle);
            if (s_sideFlagsRigt.Foldout)
            {
                DrawSideFoldoutContent(m_ConfigRightProperty, s_sideFlagsRigt);
            }
            // Right Foldout

            // Up Foldout
            s_sideFlagsUp.Foldout = EditorGUILayout.Foldout(s_sideFlagsUp.Foldout, Texts.Label_Up, Style.Inspector.SideFoldoutStyle);
            if (s_sideFlagsUp.Foldout)
            {
                DrawSideFoldoutContent(m_ConfigUpProperty, s_sideFlagsUp);
            }
            // Up Foldout

            // Down Foldout
            s_sideFlagsDown.Foldout = EditorGUILayout.Foldout(s_sideFlagsDown.Foldout, Texts.Label_Down, Style.Inspector.SideFoldoutStyle);
            if (s_sideFlagsDown.Foldout)
            {
                DrawSideFoldoutContent(m_ConfigDownProperty, s_sideFlagsDown);
            }
            // Down Foldout

            EditorGUILayout.Space();

            // EndDisabledGroup
            EditorGUI.EndDisabledGroup();
            // EndDisabledGroup

            // Show Gizmos
            Rect toggleRect = EditorGUILayout.GetControlRect();
            toggleRect.xMin += EditorGUIUtility.labelWidth;
            s_ShowGizmos = GUI.Toggle(toggleRect, s_ShowGizmos, s_LabelShowGizmos, EditorStyles.miniButton);
            // Show Gizmos

            // Gizmos Color
            EditorGUI.BeginDisabledGroup(!s_ShowGizmos);

            EditorGUILayout.BeginHorizontal();

            s_GizmosColorPrimary = s_GizmosColorSecondary = EditorGUILayout.ColorField(s_LabelGizmosColor, s_GizmosColorPrimary, true, false, false);
            s_GizmosColorSecondary = Style.Gizmos.GetGizmosSecondaryColor(s_GizmosColorPrimary);

            if (GUILayout.Button(s_LabelReset))
            {
                s_GizmosColorPrimary = Style.Gizmos.DefaultColor;
                s_GizmosColorSecondary = Style.Gizmos.GetGizmosSecondaryColor(s_GizmosColorPrimary);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();
            // Gizmos Color

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }

            ChildClassPropertiesGUI();

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawSideFoldoutContent(SerializedProperty configSerializedProperty, SideFlagsEditor flags)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.Space();

            SerializedProperty typeProperty = configSerializedProperty.FindPropertyRelative("m_Type");

            SerializedProperty anchorSection = configSerializedProperty.FindPropertyRelative("m_Anchor");

            SerializedProperty wrapAroundProperty = configSerializedProperty.FindPropertyRelative("m_wrapAround");
            SerializedProperty useEdgesProperty = configSerializedProperty.FindPropertyRelative("m_UseEdges");
            SerializedProperty omnidirectionalProperty = configSerializedProperty.FindPropertyRelative("m_Omnidirectional");
            SerializedProperty directivityProperty = configSerializedProperty.FindPropertyRelative("m_Directivity");

            SerializedProperty sectorSection = configSerializedProperty.FindPropertyRelative("m_Sector");
            SerializedProperty rectTransformSection = configSerializedProperty.FindPropertyRelative("m_RectTransform");
            SerializedProperty selectableListSection = configSerializedProperty.FindPropertyRelative("m_SelectableList");

            typeProperty.intValue = EditorGUILayout.Popup(Texts.Field_Type, typeProperty.intValue, DirectedNavigationType.Names);

            EditorGUILayout.Space();

            if (typeProperty.intValue != DirectedNavigationType.Value.Disabled.ToInt())
            {
                SerializedProperty useListOrderProperty = selectableListSection.FindPropertyRelative("m_UseListOrder");

                // DisabledGroup
                bool disableCommonOptionsControls = typeProperty.intValue == DirectedNavigationType.Value.SelectableList.ToInt() && useListOrderProperty.boolValue;
                EditorGUI.BeginDisabledGroup(disableCommonOptionsControls);

                DrawAnchorSection(anchorSection, flags);

                if(typeProperty.intValue != DirectedNavigationType.Value.Sector.ToInt())
                    wrapAroundProperty.boolValue = EditorGUILayout.Toggle(Texts.Field_WrapAround, wrapAroundProperty.boolValue);

                useEdgesProperty.boolValue = EditorGUILayout.Toggle(Texts.Field_UseEdges, useEdgesProperty.boolValue);

                if (s_AdvancedSettings)
                    omnidirectionalProperty.boolValue = EditorGUILayout.Toggle(Texts.Field_Omnidirectional, omnidirectionalProperty.boolValue);

                InspectorElements.DrawDirectivityProperty(directivityProperty);

                EditorGUI.EndDisabledGroup();
                // DisabledGroup

                EditorGUILayout.Space();
            }

            if (typeProperty.intValue == DirectedNavigationType.Value.Sector.ToInt())
            {
                DrawSectorProperties(sectorSection, omnidirectionalProperty);
            }
            else if (typeProperty.intValue == DirectedNavigationType.Value.RectTransform.ToInt())
            {
                DrawRectTransformProperties(rectTransformSection);
            }
            else if (typeProperty.intValue == DirectedNavigationType.Value.SelectableList.ToInt())
            {
                DrawSelectableListProperties(selectableListSection);
            }

            EditorGUI.indentLevel--;
        }


        private void DrawAnchorSection(SerializedProperty anchorSection, SideFlagsEditor flags)
        {
            if (s_AdvancedSettings)
            {
                flags.FoldoutAnchor = EditorGUILayout.Foldout(flags.FoldoutAnchor, Texts.Field_Anchor, Style.Inspector.FoldoutStyle);

                if (flags.FoldoutAnchor)
                {
                    EditorGUI.indentLevel++;

                    SerializedProperty anchorTypeProperty = anchorSection.FindPropertyRelative("m_Type");
                    anchorTypeProperty.intValue = EditorGUILayout.Popup(Texts.Field_Type, anchorTypeProperty.intValue, Anchor.Names);

                    EditorGUILayout.BeginHorizontal();

                    if (anchorTypeProperty.intValue == Anchor.Type.RectTransform.ToInt())
                    {
                        SerializedProperty rectTransformProperty = anchorSection.FindPropertyRelative("m_RectTransform");
                        rectTransformProperty.objectReferenceValue = EditorGUILayout.ObjectField(Texts.Field_RectTransform, rectTransformProperty.objectReferenceValue, typeof(RectTransform), true);

                        // Button Reset
                        if (GUILayout.Button(s_LabelReset, GUILayout.Width(50)))
                            rectTransformProperty.objectReferenceValue = null;
                        // Button Reset
                    }
                    else if (anchorTypeProperty.intValue == Anchor.Type.Shift.ToInt())
                    {
                        SerializedProperty shiftProperty = anchorSection.FindPropertyRelative("m_Shift");
                        EditorGUILayout.PropertyField(shiftProperty, s_Value);

                        // Button Reset
                        if (GUILayout.Button(s_LabelReset, GUILayout.Width(50)))
                            shiftProperty.vector3Value = Vector3.zero;
                        // Button Reset
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
            }
        }


        private void DrawSectorProperties(SerializedProperty serializedProperty, SerializedProperty omnidirectionalProperty)
        {
            EditorGUILayout.LabelField(Texts.Field_Sector);
            DrawSectorMinMaxSlider(serializedProperty, omnidirectionalProperty);

            EditorGUILayout.LabelField(Texts.Field_Radius);
            DrawRadiusSlider(serializedProperty);
        }


        private void DrawSectorMinMaxSlider(SerializedProperty serializedProperty, SerializedProperty omnidirectionalProperty)
        {
            SerializedProperty minAngleProperty = serializedProperty.FindPropertyRelative("m_MinAngle");
            SerializedProperty maxAngleProperty = serializedProperty.FindPropertyRelative("m_MaxAngle");

            float minAngle = minAngleProperty.floatValue;
            float maxAngle = maxAngleProperty.floatValue;

            if(omnidirectionalProperty.boolValue)
                EditorGUILayout.MinMaxSlider(ref minAngle, ref maxAngle, Config.SectorConfig.MinAngleLimitOmnidirectional, Config.SectorConfig.MaxAngleLimitOmnidirectional);
            else
                EditorGUILayout.MinMaxSlider(ref minAngle, ref maxAngle, Config.SectorConfig.MinAngleLimit, Config.SectorConfig.MaxAngleLimit);

            minAngleProperty.floatValue = minAngle;
            maxAngleProperty.floatValue = maxAngle;
        }


        private void DrawRadiusSlider(SerializedProperty serializedProperty)
        {
            SerializedProperty radiusProperty = serializedProperty.FindPropertyRelative("m_Radius");

            radiusProperty.floatValue = EditorGUILayout.Slider(radiusProperty.floatValue, 0.1f, s_RadiusLimit);
        }


        private void DrawRectTransformProperties(SerializedProperty serializedProperty)
        {
            SerializedProperty rectTransformProperty = serializedProperty.FindPropertyRelative("m_RectTransform");
            rectTransformProperty.objectReferenceValue = EditorGUILayout.ObjectField(Texts.Field_Area, rectTransformProperty.objectReferenceValue, typeof(RectTransform), true);
        }


        private void DrawSelectableListProperties(SerializedProperty serializedProperty)
        {
            SerializedProperty useListOrderProperty = serializedProperty.FindPropertyRelative("m_UseListOrder");
            useListOrderProperty.boolValue = EditorGUILayout.Toggle(Texts.Field_UseListOrder, useListOrderProperty.boolValue);

            SerializedProperty selectableListProperty = serializedProperty.FindPropertyRelative("m_SelectableList");
            EditorGUILayout.PropertyField(selectableListProperty, s_LabelList);
        }


        private class SideFlagsEditor
        {
            public bool Foldout = true;
            public bool FoldoutAnchor = false;
        }
    }
}
