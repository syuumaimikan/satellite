using UnityEngine;

[System.Serializable]
public class SkillData
{
    public string skillName;

    public int cost = 1;

    public bool unlocked;

    public SkillData requiredSkill;
}