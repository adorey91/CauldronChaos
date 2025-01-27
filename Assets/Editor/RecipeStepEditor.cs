using UnityEditor;

[CustomEditor(typeof(RecipeStepSO))]
public class RecipeStepEditor : Editor
{
    SerializedProperty stepType;
    SerializedProperty stirAmount;
    SerializedProperty stirSprite;
    SerializedProperty ingredient; // this will need to be renamed
    SerializedProperty ingredientSprite;

    private void OnEnable()
    {
        // Cache serialized properties
        stepType = serializedObject.FindProperty("stepType");
        stirAmount = serializedObject.FindProperty("stirAmount");
        stirSprite = serializedObject.FindProperty("stirSprite");
        ingredient = serializedObject.FindProperty("ingredient");
        ingredientSprite = serializedObject.FindProperty("ingredientSprite");
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
            EditorGUILayout.PropertyField(ingredient);
            EditorGUILayout.PropertyField(ingredientSprite);
        }
        else if (stepType.enumValueIndex == (int)RecipeStepSO.StepType.StirClockwise || stepType.enumValueIndex == (int)RecipeStepSO.StepType.StirCounterClockwise)
        {
            // Show stir amount
            EditorGUILayout.PropertyField(stirAmount);
            EditorGUILayout.PropertyField(stirSprite);
        }

        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}