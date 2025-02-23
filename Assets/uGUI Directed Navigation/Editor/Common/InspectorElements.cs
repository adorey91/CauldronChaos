using UnityEditor;
using UnityEngine;

namespace IEVO.UI.uGUIDirectedNavigation.Common
{
    static public class InspectorElements
    {
        private static GUIContent s_LabelReset = EditorGUIUtility.TrTextContent(Texts.Button_Reset);


        static public void DrawDirectivityProperty(SerializedProperty directivityProperty)
        {
            EditorGUILayout.LabelField(Texts.Field_Directivity);

            EditorGUILayout.BeginHorizontal();

            // Slider
            float sliderValue;
            if (directivityProperty.floatValue < 0)
                sliderValue = -Utils.ExpNormalizeInverse(-directivityProperty.floatValue, 1f, Config.DirectivityMax);
            else
                sliderValue = Utils.ExpNormalizeInverse(directivityProperty.floatValue, 1f, Config.DirectivityMax);

            sliderValue = EditorGUILayout.Slider(sliderValue, -1f, 1f);

            if (sliderValue < 0)
                directivityProperty.floatValue = -Utils.ExpNormalize(-sliderValue, 1f, Config.DirectivityMax);
            else
                directivityProperty.floatValue = Utils.ExpNormalize(sliderValue, 1f, Config.DirectivityMax);
            // Slider

            // Button Reset
            if (GUILayout.Button(s_LabelReset, GUILayout.Width(50)))
            {
                directivityProperty.floatValue = 0f;
            }
            // Button Reset

            EditorGUILayout.EndHorizontal();
        }
    }
}
