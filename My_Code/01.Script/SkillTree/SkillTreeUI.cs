using UnityEngine;
using UnityEngine.UI;

public delegate void SkillUpgrade(int currentCount);
public class SkillTreeUI : MonoBehaviour
{
    [SerializeField] private string _skillUpgradeName;
    [TextArea][SerializeField] private string _skillUgradeDescription;

    [SerializeField] private SkillTreeUI[] _shouldBeUnlocked; //�� ��ų�� �̸��� ���� �����Ǿ�� �ϴ� ��ų

    [SerializeField] private Color _lockedSkillColor;

     private Image _skillImage;
    public bool unlocked;

    private int _currentUpgradeCount;

    public event SkillUpgrade UpgradeEvent;

    private void OnValidate()
    {
        gameObject.name = $"SkillTreeSlotUI-[ {_skillUpgradeName} ]";
    }

    private void Start()
    {
        _skillImage = GetComponent<Image>();
        _skillImage.color = _lockedSkillColor;
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (unlocked)
        {
            _skillImage.color = Color.white;
        }
    }

    public void UnlockSkillSlot()
    {
        if (unlocked) return;

        for (int i = 0; i < _shouldBeUnlocked.Length; ++i)
        {
            if (_shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("Can not unlock this skill");
                return;
            }
        }

        //for (int i = 0; i < _shouldBeLocked.Length; ++i)
        //{
        //    if (_shouldBeLocked[i].unlocked)
        //    {
        //        Debug.Log("Can not unlock this skill");
        //        return;
        //    }
        //}

        if (TestSkillPoint.Instance.CanSpendSkillPoint())
        {
            if (!unlocked)
            {
                unlocked = true;
            }

            ++_currentUpgradeCount;
            UpgradeEvent?.Invoke(_currentUpgradeCount); 
            UpdateUI();
        }
    }
}
