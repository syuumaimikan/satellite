using UnityEngine;
using UnityEngine.UI;

public class SkillButton :
    MonoBehaviour
{
    public SkillData skill;

    private Button button;

    void Start()
    {
        button =
            GetComponent<Button>();
    }

    void Update()
    {
        bool canUnlock =
            skill.requiredSkill == null
            || skill.requiredSkill.unlocked;

        button.interactable =
            !skill.unlocked &&
            canUnlock;
    }
}