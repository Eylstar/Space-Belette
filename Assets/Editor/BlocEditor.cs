using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Bloc))]
public class BlocEditor : Editor
{
    private SerializedProperty blocTypeProp, IDProp, IconProp, CostProp, collidersProp;
    private SerializedProperty wallNorthProp, wallSouthProp, wallEastProp, wallWestProp, bulletSpawnProp, roofProp;

    private void OnEnable()
    {
        blocTypeProp = serializedObject.FindProperty("blocType");
        IDProp = serializedObject.FindProperty("ID");
        IconProp = serializedObject.FindProperty("Icon");
        CostProp = serializedObject.FindProperty("Cost");
        collidersProp = serializedObject.FindProperty("colliders");

        wallNorthProp = serializedObject.FindProperty("WallNorth");
        wallSouthProp = serializedObject.FindProperty("WallSouth");
        wallEastProp = serializedObject.FindProperty("WallEast");
        wallWestProp = serializedObject.FindProperty("WallWest");
        bulletSpawnProp = serializedObject.FindProperty("BulletSpawn");
        roofProp = serializedObject.FindProperty("Roof");
    }

    public override void OnInspectorGUI()
    {
        Bloc bloc = (Bloc)target;

        serializedObject.Update();

        // BlocType
        EditorGUILayout.PropertyField(blocTypeProp);

        // Champs standards
        EditorGUILayout.PropertyField(IDProp);
        EditorGUILayout.PropertyField(IconProp);
        EditorGUILayout.PropertyField(CostProp);

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

        if (type == Bloc.BlocType.Weapon || type == Bloc.BlocType.Engine)
        {
            EditorGUILayout.PropertyField(bulletSpawnProp);
        }

        if (type == Bloc.BlocType.Floor || type == Bloc.BlocType.Engine || type == Bloc.BlocType.Weapon)
        {
            EditorGUILayout.PropertyField(roofProp);
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(bloc);
        }
    }
}