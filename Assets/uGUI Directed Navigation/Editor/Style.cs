/* ---------------------------------------
 * Project: 'uGUI Directed Navigation'
 * Studio:          IEVO
 * Site:     https://ievo.games/
 * -------------------------------------*/
using UnityEditor;
using UnityEngine;

namespace IEVO.UI.uGUIDirectedNavigation
{
    /// <summary>
    /// Style settings container.
    /// </summary>
    static public class Style
    {
        /// <summary>
        /// Container for 'Gizmos' style settings.
        /// </summary>
        static public class Gizmos
        {
            static private GUIStyle s_LabelStyle;

            public const float DottedLineSize = 5f;
            public const float AnchorDottedLineSize = 3f;
            static public readonly Color DefaultColor = new Color(1, 0.8f, 0.4f);
            static public GUIStyle LabelStyle 
            { 
                get 
                {
                    if(s_LabelStyle == null)
                    {
                        s_LabelStyle = new GUIStyle(EditorStyles.label);
                        s_LabelStyle.fontStyle = FontStyle.Bold;
                        s_LabelStyle.fontSize = 14;
                        s_LabelStyle.normal.textColor = DefaultColor;
                    }

                    return s_LabelStyle;
                } 
            }

            static public Color GetGizmosSecondaryColor(Color color)
            {
                color.a = Mathf.Lerp(0.4f, 0.08f, (color.r + color.g + color.b) / 0.6f);
                return color;
            }
        }

        /// <summary>
        /// Container for 'Inspector' style settings.
        /// </summary>
        static public class Inspector
        {
            static private GUIStyle s_FoldoutStyle;
            static private GUIStyle s_SideFoldoutStyle;

            static public GUIStyle SideFoldoutStyle
            {
                get
                {
                    if (s_SideFoldoutStyle == null)
                    {
                        s_SideFoldoutStyle = new GUIStyle(EditorStyles.foldout);
                        s_SideFoldoutStyle.fontStyle = FontStyle.Bold;
                        SetGuiStyleFontColor(s_SideFoldoutStyle, EditorGUIUtility.isProSkin ? Color.white : Color.black);
                    }

                    return s_SideFoldoutStyle;
                }
            }

            static public GUIStyle FoldoutStyle
            {
                get
                {
                    if (s_FoldoutStyle == null)
                    {
                        s_FoldoutStyle = new GUIStyle(EditorStyles.foldout);
                    }

                    return s_FoldoutStyle;
                }
            }

            static public void SetGuiStyleFontColor(GUIStyle guiStyle, Color color)
            {
                guiStyle.normal.textColor = color;
                guiStyle.hover.textColor = color;
                guiStyle.active.textColor = color;
                guiStyle.focused.textColor = color;
                guiStyle.onNormal.textColor = color;
                guiStyle.onHover.textColor = color;
                guiStyle.onActive.textColor = color;
                guiStyle.onFocused.textColor = color;
            }
        }


    }
}