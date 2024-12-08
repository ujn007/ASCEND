using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

[Serializable]
public class WeaponType
{
    public List<SkillItemSO> skillList = new();
}

public class SkillitemDealer : MonoBehaviour
{
    [SerializeField] private Transform itemParent;
    [SerializeField] private Transform spawnTrm;
    [SerializeField] private EventReference _visibleSkillItemEvenReference;
    private Transform item;

    public List<WeaponType> weaponTypeList = new();
    private List<SkillItem> skillItem = new();

    private HashSet<(int, int)> usedCombinations = new HashSet<(int, int)>();

    private bool stopFollowing = false;


    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        FollowSpawnTrm();
    }

    public int GetWeaponSkillsCount()
    {
        int cnt = 0;
        weaponTypeList.ForEach(e => cnt += e.skillList.Count);
        return cnt;
    }

    public void SpawnItemAnimaotion(bool v)
    {
        animator.SetBool("IsThrow", v);
    }

    private void FollowSpawnTrm()
    {
        if (item == null || stopFollowing) return;

        item.position = spawnTrm.position;
    }

    private void OnDisable()
    {
        stopFollowing = false;
        SpawnItemAnimaotion(false);
    }

    private void SpawnSkillItem()
    {
        usedCombinations.Clear();
        item = Instantiate(itemParent, spawnTrm.position, spawnTrm.localRotation);
        skillItem = item.GetComponentsInChildren<SkillItem>().ToList();
        RuntimeManager.PlayOneShot(_visibleSkillItemEvenReference, transform.position);

        foreach (SkillItem skillItem in skillItem) 
            skillItem.GetRandomSkillItem(weaponTypeList, usedCombinations);
    }

    public void DestroyMe()
    {
        gameObject.SetActive(false);
    }

    private void StopFollow() => stopFollowing = true;
}