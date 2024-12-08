using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PJH.Core.MInterface;
using Random = UnityEngine.Random;

public class SkillItem : MonoBehaviour, IInteractable
{
    [Header("Child")]
    [SerializeField] private GameObject popUpUI;
    [SerializeField] private ToolTip toolTip;

    [Header("Size")]
    [SerializeField] private float zeroDuration;

    private SkillItemsParent itemsParent;
    private SkillItemSO currentSkillItemSO;
    private int weaponIndex;
    private int skillIndex;
    public bool canGetSkill = false;
    private bool isGetSkill = false;

    private List<WeaponType> weaponTypeList;

    private List<ParticleSystem> orbParticle = new();

    public InteractInfo InteractInfo { get; set; }

    private void Awake()
    {
        orbParticle = GetComponentsInChildren<ParticleSystem>().ToList();
        itemsParent = GetComponentInParent<SkillItemsParent>();
    }

    private void OnEnable()
    {
        isGetSkill = false;
    }

    public void GetRandomSkillItem(List<WeaponType> wList, HashSet<(int, int)> usedCombinations)
    {
        weaponTypeList = wList;
        ClearItem();

        for (int attempt = 0; attempt <= 50; attempt++)
        {
            int randWeapon = GetRandom(0, weaponTypeList.Count);
            int randSkill = GetRandom(0, weaponTypeList[randWeapon].skillList.Count);

            if (weaponTypeList[randWeapon].skillList.Count <= 0) continue;

            if (!usedCombinations.Contains((randWeapon, randSkill)) || attempt >= 10)
            {
                usedCombinations.Add((randWeapon, randSkill));
                SetkillTypeSO(randWeapon, randSkill);
                transform.GetChild(randWeapon).gameObject.SetActive(true);
                return;
            }

        }
    }

    private void SetkillTypeSO(int index, int skillIndex)
    {
        currentSkillItemSO = weaponTypeList[index].skillList[skillIndex];
        weaponIndex = index;
        this.skillIndex = skillIndex;

        toolTip.SetToolTip(
            currentSkillItemSO.skillTypeName,
            currentSkillItemSO.skillDescription,
            currentSkillItemSO.videoClip
        );
    }

    public void ClearItem()
    {
        weaponIndex = skillIndex = 0;
        orbParticle.ForEach((x) => x.gameObject.SetActive(false));
    }

    public void Interact()
    {
        if (isGetSkill) return;
        if (canGetSkill)
        {
            canGetSkill = false;
            ToolTipActive(false);
            itemsParent.RaiseEventSkill(currentSkillItemSO);
            RemoveIndex();
            return;
        }

        PopUpUIActive(false);
        ToolTipActive(true);

        canGetSkill = true;
    }

    private void RemoveIndex()
    {
        weaponTypeList[weaponIndex].skillList.RemoveAt(skillIndex);
    }

    public void SizeOff()
    {
        transform.DOScale(transform.localScale * 1.5f, zeroDuration * 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            transform.DOScale(Vector3.zero, zeroDuration * 0.7f);
        });
    }

    private bool SOCount()
    {
        int cnt = 0;
        foreach (var item in weaponTypeList)
        {
            cnt += item.skillList.Count;
        }
        return cnt > 3;
    }

    public void PopUpUIActive(bool v) => popUpUI.SetActive(v);
    public void ToolTipActive(bool v) => toolTip.gameObject.SetActive(v);
    public int GetRandom(int a, int b) => Random.Range(a, b);
    public void IsGetSkill(bool v) => isGetSkill = v;
}

