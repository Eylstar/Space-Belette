using UnityEditor;
using System.IO;
using UnityEngine;

public class SkillTemplateCreator
{
    private const string templatePath = "Assets/Editor/Templates/SkillTemplate.txt";

    [MenuItem("Assets/Create/Pilot Skill Script", false, 12)]
    public static void CreateSkillScript()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
            templatePath,
            "NewSkill.cs"
        );
    }
}
