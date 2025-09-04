using UnityEditor;
using System.IO;
using UnityEngine;

public class WeaponTemplateCreator
{
    private const string templatePath = "Assets/Editor/Templates/WeaponTemplate.txt";

    [MenuItem("Assets/Create/WeaponUpgrade Script", false, 12)]
    public static void CreateSkillScript()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
            templatePath,
            "NewWeaponSO.cs"
        );
    }
}
