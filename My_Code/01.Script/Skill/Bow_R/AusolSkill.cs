using DG.Tweening;
using FMODUnity;
using Game.Events;
using PJH.Agent.Player;
using PJH.Equipment;
using PJH.Equipment.Weapon;
using PJH.EquipmentSkillSystem;
using PJH.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

public class AusolSkill : EquipmentSkill
{
    private PoolManagerSO poolManager;
    [SerializeField] private GameEventChannelSO eventSO;
    [TabGroup("Info")] [SerializeField] private float animatorSlowSpeed;

    [TabGroup("Pool")] [SerializeField] private PoolTypeSO arrowType, chargeEffectType;
    [TabGroup("Sound")] [SerializeField] private EventReference _chargingEvnetReference;
    private Vector3 mousePos;
    private Transform shotPoint;

    private PlayerAnimator animatorCompo;

    public override void Init(Player player, Equipment equipment)
    {
        base.Init(player, equipment);
        poolManager = Managers.Addressable.Load<PoolManagerSO>("PoolManager");
        animatorCompo = _player.GetCompo<PlayerAnimator>();
        animatorCompo.ShotAusolSkillSlowAnimEvent += HandleShotAusolSkillSlowAnimEvent;
        animatorCompo.ShotAusolSkillArrowEvent += HandleShotAusolSkillArrowEvent;
    }

    private void HandleShotAusolSkillSlowAnimEvent()
    {
        Bow bow = _player.GetCompo<PlayerEquipmentController>().GetWeapon() as Bow;
        shotPoint = bow.ShootPointTrm;
        RuntimeManager.PlayOneShot(_chargingEvnetReference, _player.transform.position);
        animatorCompo.AnimatorCompo.speed = animatorSlowSpeed;

        ChargeEffect chargeEffect = poolManager.Pop(chargeEffectType) as ChargeEffect;
        chargeEffect.transform.SetParent(shotPoint);
        chargeEffect.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        chargeEffect.ScaleUp();
    }

    private void HandleShotAusolSkillArrowEvent()
    {
        animatorCompo.AnimatorCompo.speed = 1;

        AusolArrow arrow = poolManager.Pop(arrowType) as AusolArrow;
        arrow.transform.position = shotPoint.position;
        arrow.ShotFront(mousePos);
    }

    public override void UseSkill(bool isHolding)
    {
        if (isHolding) return;
        base.UseSkill(isHolding);
        mousePos = _player.PlayerInput.GetWorldMousePosition();

        _player.transform.DOLookAt(mousePos, .5f, AxisConstraint.Y);

        animatorCompo.SetRootMotion(true);
        animatorCompo.PlaySkillAnimation(2);
        animatorCompo.EndUseSkillEvent += HandleEndUseSkillEvent;
    }

    private void HandleEndUseSkillEvent()
    {
        animatorCompo.EndUseSkillEvent -= HandleEndUseSkillEvent;
        animatorCompo.SetRootMotion(false);
    }
}