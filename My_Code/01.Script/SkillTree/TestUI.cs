using TMPro;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointText;
    private TestSkillPoint testSkillPoint => TestSkillPoint.Instance;

    private void Awake()
    {
        testSkillPoint.SkillPointChanged += HandlePointChanged;
    }

    private void Start()
    {
        pointText.text = testSkillPoint.SkillPoint.ToString();
    }

    private void HandlePointChanged(int point)
    {
        pointText.text = point.ToString();
    }
}
