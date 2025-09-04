using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;
using System.Reflection;

public class SkillAssetAutoCreator : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (var assetPath in importedAssets)
        {
            if (!assetPath.EndsWith(".cs")) continue;

            string scriptName = Path.GetFileNameWithoutExtension(assetPath);

            // Tenter de trouver le Type en recherchant dans toutes les assemblies
            var type = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == scriptName && typeof(ScriptableObject).IsAssignableFrom(t));

            if (type == null) continue;

            // Verifier que la classe herite bien de Skill ou de NewWeapon
            bool isSkill = typeof(Skill).IsAssignableFrom(type);
            bool isNewWeapon = typeof(NewWeapon).IsAssignableFrom(type);

            if (!isSkill && !isNewWeapon) continue;

            // Determiner le type pour la recherche d'asset existant
            string typeFilter = isSkill ? "Skill" : "NewWeapon";

            // Vérifier si un asset existe deja
            string folder = Path.GetDirectoryName(assetPath);
            string[] existingAssets = AssetDatabase.FindAssets($"{scriptName} t:{typeFilter}", new[] { folder });

            if (existingAssets.Length > 0) continue; // Ne pas recreer s'il existe deja

            // Créer l'instance
            ScriptableObject instance = ScriptableObject.CreateInstance(type);
            string assetPathSO = Path.Combine(folder, scriptName + ".asset");

            AssetDatabase.CreateAsset(instance, assetPathSO);
            AssetDatabase.SaveAssets();

            Debug.Log($"ScriptableObject '{scriptName}.asset' cree automatiquement dans {folder}");
        }
    }
}
