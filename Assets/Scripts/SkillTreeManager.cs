using UnityEngine;
using TMPro;

public class SkillTreeManager :
    MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private HeroKnight hero;

    [SerializeField]
    private PlayerStats stats;

    [SerializeField]
    private TMP_Text pointText;

    [Header("Points")]
    public int skillPoints = 5;

    [Header("Skills")]
    public SkillData speedSkill;
    public SkillData tripleJumpSkill;
    public SkillData dodgeSkill;

    void Update()
    {
        pointText.text =
            "SP : " + skillPoints;
    }

    public void UnlockSpeed()
    {
        UnlockSkill(
            speedSkill,
            () =>
            {
                stats.moveSpeed += 2f;
            });
    }

    public void UnlockTripleJump()
    {
        UnlockSkill(
            tripleJumpSkill,
            () =>
            {
                hero.SetMaxJumps(3);
            });
    }

    public void UnlockDodgeMaster()
    {
        UnlockSkill(
            dodgeSkill,
            () =>
            {
                hero.SetRollCooldown(0.35f);
            });
    }

    private void UnlockSkill(
        SkillData skill,
        System.Action onUnlock)
    {
        if (skill.unlocked)
            return;

        // 前提条件
        if (skill.requiredSkill != null
            && !skill.requiredSkill.unlocked)
        {
            Debug.Log(
                "必要スキル不足"
            );
            return;
        }

        // ポイント不足
        if (skillPoints < skill.cost)
        {
            Debug.Log(
                "スキルポイント不足"
            );
            return;
        }

        skill.unlocked = true;
        skillPoints -= skill.cost;

        onUnlock?.Invoke();

        Debug.Log(
            skill.skillName
            + " 解放"
        );
    }
}