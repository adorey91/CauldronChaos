using UnityEditor;

[CustomEditor(typeof(RecipeStepSO))]
public class RecipeStepEditor : Editor
{
    SerializedProperty stepType;
    SerializedProperty stirAmount;
    SerializedProperty ingredient1; // this will need to be renamed
    SerializedProperty sprite;

    private void OnEnable()
    {
        // Cache serialized properties
        stepType = serializedObject.FindProperty("stepType");
        ingredient1 = serializedObject.FindProperty("ingredient1");
        sprite = serializedObject.FindProperty("sprite");
        stirAmount = serializedObject.FindProperty("stirAmount");
    }

    public override void OnInspectorGUI()
    {
        // Update serialized object
        serializedObject.Update();

        // Show step type and allow selection
        EditorGUILayout.PropertyField(stepType);

        // Conditionally show other fields based on step type
        if (stepType.enumValueIndex == (int)RecipeStepSO.StepType.AddIngredient)
        {
            // Show ingredient type
            EditorGUILayout.PropertyField(ingredient1);
            EditorGUILayout.PropertyField(sprite);
        }
        else if (stepType.enumValueIndex == (int)RecipeStepSO.StepType.StirClockwise || stepType.enumValueIndex == (int)RecipeStepSO.StepType.StirCounterClockwise)
        {
            // Show stir amount
            EditorGUILayout.PropertyField(stirAmount);
        }

        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
