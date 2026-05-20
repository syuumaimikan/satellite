using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTreeUI :
    MonoBehaviour
{
    [SerializeField]
    private GameObject skillTreeCanvas;

    private bool isOpen = false;

    void Update()
    {
        if (
            Keyboard.current.tabKey
            .wasPressedThisFrame)
        {
            ToggleSkillTree();
        }
    }

    public void ToggleSkillTree()
    {
        isOpen = !isOpen;

        skillTreeCanvas
            .SetActive(isOpen);

        // ゲーム停止
        Time.timeScale =
            isOpen ? 0f : 1f;
    }
}