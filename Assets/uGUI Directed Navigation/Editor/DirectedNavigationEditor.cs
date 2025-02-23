/* ---------------------------------------
 * Project: 'uGUI Directed Navigation'
 * Studio:          IEVO
 * Site:     https://ievo.games/
 * -------------------------------------*/
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace IEVO.UI.uGUIDirectedNavigation
{
    [CustomEditor(typeof(DirectedNavigation), true)]
    public partial class DirectedNavigationEditor : Editor
    {
        static protected List<DirectedNavigationEditor> s_Editors = new List<DirectedNavigationEditor>();

        static protected bool s_ShowNavigation = false;
        static protected string s_ShowNavigationKey = "SelectableEditor.ShowNavigation";

        static protected string s_GizmosColorPrimaryKey = "DirectedNavigationEditor.GizmosColor";

        static protected bool s_AdvancedSettings = false;
        static protected string s_AdvancedSettingsKey = "DirectedNavigationEditor.AdvancedSettings";

        static protected float s_RadiusLimit;


        protected string[] m_PropertyPathToExcludeForChildClasses;

        protected SerializedProperty m_ScriptProperty;

        protected SerializedProperty m_ActiveProperty;

        protected SerializedProperty m_ConfigLeftProperty;
        protected SerializedProperty m_ConfigRightProperty;
        protected SerializedProperty m_ConfigUpProperty;
        protected SerializedProperty m_ConfigDownProperty;

        protected DirectedNavigation m_Target;


        static DirectedNavigationEditor()
        {
            Selection.selectionChanged += RegisterOnDuringSceneGuiNavigation;
        }


        static private List<Selectable> s_ActiveGameObjectSelectable = new List<Selectable>();
        static private void RegisterOnDuringSceneGuiNavigation()
        {
            SceneView.duringSceneGui -= OnDuringSceneGuiNavigation;

            if (Selection.activeGameObject != null && Selection.activeGameObject.transform is RectTransform)
            {
                Selection.activeGameObject.GetComponents(s_ActiveGameObjectSelectable);

                if (s_ActiveGameObjectSelectable.Count > 0)
                    SceneView.duringSceneGui += OnDuringSceneGuiNavigation;
            }
        }


        static private void OnDuringSceneGuiNavigation(SceneView view)
        {
            s_ShowNavigation = EditorPrefs.GetBool(s_ShowNavigationKey, true);

            if (s_ShowNavigation)
                UpdateNavigationVisualization();
        }


        /// <summary>
        /// Asigning 'selectOnLeft', 'selectOnRight', 'selectOnUp', 'selectOnDown' is used only in editor, 
        /// for visualization of navigation pathes with native 'Selectable' mechanism.
        /// </summary>
        static public void UpdateNavigationVisualization()
        {
            if (s_SelectableBuffer.Length < Selectable.allSelectableCount)
                s_SelectableBuffer = new Selectable[Selectable.allSelectableCount];

            s_SelectableBufferCount = Selectable.AllSelectablesNoAlloc(s_SelectableBuffer);

            for (int i = 0; i < s_SelectableBufferCount; i++)
            {
                Selectable sel = s_SelectableBuffer[i];

                if (sel == null || Camera.current != null && !UnityEditor.SceneManagement.StageUtility.IsGameObjectRenderedByCamera(sel.gameObject, Camera.current))
                    continue;

                DirectedNavigation dirNav = sel.GetComponent<DirectedNavigation>();

                if (dirNav == null || !dirNav.Active)
                    continue;

                UnityEngine.UI.Navigation navigation = UnityEngine.UI.Navigation.defaultNavigation;
                navigation.mode = UnityEngine.UI.Navigation.Mode.Explicit;
                navigation.selectOnLeft = dirNav.FindSelectableLeft();
                navigation.selectOnRight = dirNav.FindSelectableRight();
                navigation.selectOnUp = dirNav.FindSelectableUp();
                navigation.selectOnDown = dirNav.FindSelectableDown();

                sel.navigation = navigation;
            }
        }


        protected virtual void OnEnable()
        {
            m_Target = target as DirectedNavigation;

            Canvas targetCanvas = m_Target.GetComponentInParent<Canvas>();
            Selectable targetSelectable = m_Target.GetComponent<Selectable>();

            if (targetCanvas != null)
            {
                RectTransform canvasRt = targetCanvas.transform as RectTransform;
                s_RadiusLimit = Mathf.Sqrt(Mathf.Pow(canvasRt.rect.width, 2) + Mathf.Pow(canvasRt.rect.height, 2));
            }

            s_AdvancedSettings = EditorPrefs.GetBool(s_AdvancedSettingsKey, false);

            string defaultColorString = ColorUtility.ToHtmlStringRGB(Style.Gizmos.DefaultColor);
            string gizmosColorString = "#" + EditorPrefs.GetString(s_GizmosColorPrimaryKey, defaultColorString);
            ColorUtility.TryParseHtmlString(gizmosColorString, out s_GizmosColorPrimary);

            m_ScriptProperty = serializedObject.FindProperty("m_Script");

            m_ActiveProperty = serializedObject.FindProperty("m_Active");

            m_ConfigLeftProperty = serializedObject.FindProperty("m_ConfigLeft");
            m_ConfigRightProperty = serializedObject.FindProperty("m_ConfigRight");
            m_ConfigUpProperty = serializedObject.FindProperty("m_ConfigUp");
            m_ConfigDownProperty = serializedObject.FindProperty("m_ConfigDown");
            
            m_PropertyPathToExcludeForChildClasses = new[]
            {
                m_ScriptProperty.propertyPath,
                m_ActiveProperty.propertyPath,
                m_ConfigLeftProperty.propertyPath,
                m_ConfigRightProperty.propertyPath,
                m_ConfigUpProperty.propertyPath,
                m_ConfigDownProperty.propertyPath,   
            };

            if (m_ActiveProperty.boolValue)
            {
                UnityEngine.UI.Navigation navigation = UnityEngine.UI.Navigation.defaultNavigation;
                navigation.mode = UnityEngine.UI.Navigation.Mode.Explicit;
                targetSelectable.navigation = navigation;
            }

            s_Editors.Add(this);
            RegisterOnDuringSceneGuiGizmos();
        }


        protected virtual void OnDisable()
        {
            EditorPrefs.SetString(s_GizmosColorPrimaryKey, ColorUtility.ToHtmlStringRGB(s_GizmosColorPrimary));
            EditorPrefs.SetBool(s_AdvancedSettingsKey, s_AdvancedSettings);

            s_Editors.Remove(this);
            RegisterOnDuringSceneGuiGizmos();
        }


        private void ChildClassPropertiesGUI()
        {
            if (IsDerivedDirectedNavigationEditor())
                return;

            DrawPropertiesExcluding(serializedObject, m_PropertyPathToExcludeForChildClasses);
        }


        private bool IsDerivedDirectedNavigationEditor()
        {
            return GetType() != typeof(DirectedNavigationEditor);
        }


    }
}
