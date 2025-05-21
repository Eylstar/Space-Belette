using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Bloc))]
public class BlocEditor : Editor
{
    private SerializedProperty blocTypeProp, utilityTypeProp, IDProp, IconProp, CostProp, lifeBonusProp, blocWeightProp, weightGainProp, collidersProp;
    private SerializedProperty wallNorthProp, wallSouthProp, wallEastProp, wallWestProp, bulletSpawnProp, roofProp, playerSpawnProp;
    private SerializedProperty coordGridProp, levelProp;

    private void OnEnable()
    {
        blocTypeProp = serializedObject.FindProperty("blocType");
        utilityTypeProp = serializedObject.FindProperty("utilityType");
        IDProp = serializedObject.FindProperty("ID");
        IconProp = serializedObject.FindProperty("Icon");
        CostProp = serializedObject.FindProperty("Cost");
        lifeBonusProp = serializedObject.FindProperty("LifeBonus");
        blocWeightProp = serializedObject.FindProperty("BlocWeight");
        weightGainProp = serializedObject.FindProperty("WeightGain");
        levelProp = serializedObject.FindProperty("Level");
        collidersProp = serializedObject.FindProperty("colliders");

        wallNorthProp = serializedObject.FindProperty("WallNorth");
        wallSouthProp = serializedObject.FindProperty("WallSouth");
        wallEastProp = serializedObject.FindProperty("WallEast");
        wallWestProp = serializedObject.FindProperty("WallWest");
        bulletSpawnProp = serializedObject.FindProperty("BulletSpawn");
        roofProp = serializedObject.FindProperty("Roof");
        playerSpawnProp = serializedObject.FindProperty("PlayerSpawn");

        coordGridProp = serializedObject.FindProperty("CoordGrid");
    }

    public override void OnInspectorGUI()
    {
        Bloc bloc = (Bloc)target;

        serializedObject.Update();

        // BlocType
        EditorGUILayout.PropertyField(blocTypeProp);

        // Afficher UtilityType uniquement si le bloc est de type Utility
        if ((Bloc.BlocType)blocTypeProp.enumValueIndex == Bloc.BlocType.Utility)
        {
            EditorGUILayout.PropertyField(utilityTypeProp);
        }

        // Champs standards
        EditorGUILayout.PropertyField(IDProp);
        EditorGUILayout.PropertyField(IconProp);
        EditorGUILayout.PropertyField(CostProp);
        EditorGUILayout.PropertyField(lifeBonusProp);
        EditorGUILayout.PropertyField(levelProp);
        EditorGUILayout.PropertyField(coordGridProp);

        // Afficher WeightGain uniquement pour UtilityType.Engine
        if ((Bloc.BlocType)blocTypeProp.enumValueIndex == Bloc.BlocType.Utility &&
            (Bloc.UtilityType)utilityTypeProp.enumValueIndex == Bloc.UtilityType.Engine)
        {
            EditorGUILayout.PropertyField(weightGainProp);
        }
        // Afficher BlocWeight pour tous les autres types
        else
        {
            EditorGUILayout.PropertyField(blocWeightProp);
        }

        // SerializedDictionary (colliders)
        EditorGUILayout.PropertyField(collidersProp, true);

        // Champs conditionnels
        var type = (Bloc.BlocType)blocTypeProp.enumValueIndex;

        if (type == Bloc.BlocType.Floor || type == Bloc.BlocType.Weapon)
        {
            EditorGUILayout.PropertyField(wallNorthProp);
            EditorGUILayout.PropertyField(wallSouthProp);
            EditorGUILayout.PropertyField(wallEastProp);
            EditorGUILayout.PropertyField(wallWestProp);
        }

        if (type == Bloc.BlocType.Weapon ||
            (type == Bloc.BlocType.Utility && (Bloc.UtilityType)utilityTypeProp.enumValueIndex == Bloc.UtilityType.Cockpit))
        {
            EditorGUILayout.PropertyField(bulletSpawnProp);
        }

        if (type == Bloc.BlocType.Floor || type == Bloc.BlocType.Utility || type == Bloc.BlocType.Weapon)
        {
            EditorGUILayout.PropertyField(roofProp);
        }

        // Afficher PlayerSpawn uniquement pour UtilityType.Cockpit
        if (type == Bloc.BlocType.Utility && (Bloc.UtilityType)utilityTypeProp.enumValueIndex == Bloc.UtilityType.Cockpit)
        {
            EditorGUILayout.PropertyField(playerSpawnProp);
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(bloc);
        }
    }
}
