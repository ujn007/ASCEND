using System;
using UnityEngine;

public class TestSkillPoint : MonoSingleton<TestSkillPoint>
{
    [SerializeField] private int skillPoint;

    public event Action<int> SkillPointChanged;

    public int SkillPoint
    {
        get => skillPoint;
        private set
        {
            skillPoint = value;
            SkillPointChanged?.Invoke(skillPoint);
        }
    }

    public bool CanSpendSkillPoint()
    {
        if (SkillPoint <= 0) return false;

        SkillPoint -= 1;
        return true;
    }

    private void LevelUpProcess()
    {
        SkillPoint += 1;
    }
}
