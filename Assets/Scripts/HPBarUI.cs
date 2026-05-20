using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBarUI :
    MonoBehaviour
{
    [SerializeField]
    private Slider hpSlider;

    [SerializeField]
    private TMP_Text hpText;

    [SerializeField]
    private PlayerHealth playerHealth;

    void Update()
    {
        hpSlider.maxValue =
            playerHealth.MaxHP;

        hpSlider.value =
            playerHealth.CurrentHP;

        hpText.text =
            playerHealth.CurrentHP
            + " / "
            + playerHealth.MaxHP;
    }
}