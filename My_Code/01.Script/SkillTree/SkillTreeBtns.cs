using UnityEngine;
using UnityEngine.UI;

public class SkillTreeBtns : MonoBehaviour
{
    [SerializeField] private GameObject parentObj;
    [SerializeField] private Button closeBtn;

    private void Awake()
    {
        closeBtn.onClick.AddListener(HandleCloseBtn);
    }

    private void HandleCloseBtn()
    {
        parentObj.SetActive(false);
    }
}
