using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseMoveSpeed = 4f;
    public float baseJumpForce = 7.5f;
    public float baseAttackDamage = 10f;

    [Header("Current Stats")]
    public float moveSpeed;
    public float jumpForce;
    public float attackDamage;

    private void Awake()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        moveSpeed = baseMoveSpeed;
        jumpForce = baseJumpForce;
        attackDamage = baseAttackDamage;
    }
}