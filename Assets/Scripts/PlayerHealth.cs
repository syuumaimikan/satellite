using UnityEngine;

public class PlayerHealth :
    MonoBehaviour
{
    [SerializeField]
    private int maxHP = 100;

    private int currentHP;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        currentHP =
            Mathf.Clamp(
                currentHP,
                0,
                maxHP
            );

        Debug.Log(
            "HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;

        currentHP =
            Mathf.Clamp(
                currentHP,
                0,
                maxHP
            );
    }

    private void Die()
    {
        Debug.Log("Player Dead");
    }
}