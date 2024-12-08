using System;
using Game.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillItemsParent : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO eventSO;
    private List<SkillItem> skillItemList = new List<SkillItem>();

    private void Awake()
    {
        eventSO.AddListener<InteractObjectInfo>(HandleCloseInteractEvent);
        skillItemList = GetComponentsInChildren<SkillItem>().ToList();
    }

    private void HandleCloseInteractEvent(InteractObjectInfo info)
    {
        ClearPopupUI();

        if (info.targetTrm == null) return;
        SkillItem skillItem = info.targetTrm.GetComponent<SkillItem>();
        skillItem.PopUpUIActive(true);
    }

    public void RaiseEventSkill(SkillItemSO so)
    {
        var evt = GameEvents.GetStoreSkill;
        evt.skillTypeName = so.skillTypeName;
        evt.keyIdx = so.keyIdx;
        eventSO.RaiseEvent(evt);

        DontGetSkill();
        SizeZero();
    }

    private void ClearPopupUI()
    {
        foreach (SkillItem skillItem in skillItemList)
        {
            skillItem.canGetSkill = false;
            skillItem.PopUpUIActive(false);
            skillItem.ToolTipActive(false);
        }
    }

    private void OnDestroy()
    {
        eventSO.RemoveListener<InteractObjectInfo>(HandleCloseInteractEvent);
    }

    private void SizeZero()
    {
        skillItemList.ForEach(s => s.SizeOff());
    }

    private void DontGetSkill()
    {
        skillItemList.ForEach(s => s.IsGetSkill(true));
    }
}