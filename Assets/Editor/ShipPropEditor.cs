using UnityEditor;
using UnityEngine;

// Custom editor pour ShipProp qui affiche dynamiquement les champs selon le type sélectionné
[CustomEditor(typeof(ShipProp))]
public class ShipPropEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Synchronise l'objet sérialisé avec l'objet réel
        serializedObject.Update();

        // Affiche le champ Type (toujours visible)
        SerializedProperty typeProp = serializedObject.FindProperty("Type");
        EditorGUILayout.PropertyField(typeProp);

        // Récupère la valeur sélectionnée de l'enum PropType
        ShipProp.PropType type = (ShipProp.PropType)typeProp.enumValueIndex;

        // Affiche dynamiquement les champs selon le type choisi
        switch (type)
        {
            case ShipProp.PropType.Weapon:
                // Pour Weapon : points de tir, bonus dégâts, coût
                EditorGUILayout.PropertyField(serializedObject.FindProperty("PropName"), new GUIContent("PropName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ShootPoints"), new GUIContent("Shoot Points"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BonusDamage"), new GUIContent("Bonus Damage"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Cost"), new GUIContent("Cost"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsStartProp"), new GUIContent("IsStartProp"));
                break;

            case ShipProp.PropType.Engine:
                // Pour Engine : bonus vie, bonus vitesse, coût
                EditorGUILayout.PropertyField(serializedObject.FindProperty("PropName"), new GUIContent("PropName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BonusLife"), new GUIContent("Bonus Life"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("EngineCount"), new GUIContent("EngineCount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Cost"), new GUIContent("Cost"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsStartProp"), new GUIContent("IsStartProp"));
                break;

            case ShipProp.PropType.Utility:
                // Pour Utility : tous les bonus + coût
                EditorGUILayout.PropertyField(serializedObject.FindProperty("PropName"), new GUIContent("PropName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BonusLife"), new GUIContent("Bonus Life"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BonusDamage"), new GUIContent("Bonus Damage"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BonusShield"), new GUIContent("Bonus Shield"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("LifeRegen"), new GUIContent("Life Regen"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Cost"), new GUIContent("Cost"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsStartProp"), new GUIContent("IsStartProp"));
                break;
        }

        // Applique les modifications au serializedObject
        serializedObject.ApplyModifiedProperties();
    }
}
