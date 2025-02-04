using UnityEditor;

[CustomEditor(typeof(RecipeStepSO))]
public class RecipeStepEditor : Editor
{
    SerializedProperty action;
    SerializedProperty stirAmount;
    SerializedProperty stirSprite;
    SerializedProperty stirBool;
    SerializedProperty ingredient; // this will need to be renamed
    SerializedProperty ingredientSprite;
    SerializedProperty nameOfStep;

    private void OnEnable()
    {
        // Cache serialized properties
        nameOfStep = serializedObject.FindProperty("stepName");
        action = serializedObject.FindProperty("action");
        stirAmount = serializedObject.FindProperty("stirAmount");
        stirSprite = serializedObject.FindProperty("stirSprite");
        ingredient = serializedObject.FindProperty("ingredient");
        stirBool = serializedObject.FindProperty("isClockwise");
        ingredientSprite = serializedObject.FindProperty("ingredientSprite");
    }

    public override void OnInspectorGUI()
    {
        // Update serialized object
        serializedObject.Update();

        // Show step ingredient and allow selection
        EditorGUILayout.PropertyField(action);
        EditorGUILayout.PropertyField(nameOfStep);

        // Conditionally show other fields based on step ingredient
        if (action.enumValueIndex == (int)RecipeStepSO.ActionType.AddIngredient)
        {
            // Show ingredient ingredient
            EditorGUILayout.PropertyField(ingredient);
            EditorGUILayout.PropertyField(ingredientSprite);
        }
        else if (action.enumValueIndex == (int)RecipeStepSO.ActionType.Stir)
        {
            // Show stir amount
            EditorGUILayout.PropertyField(stirAmount);
            EditorGUILayout.PropertyField(stirSprite);
            EditorGUILayout.PropertyField(stirBool);
        }

        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}