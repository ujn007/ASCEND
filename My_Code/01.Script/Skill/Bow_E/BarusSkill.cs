using Sirenix.OdinInspector;
using System.Collections;
using DG.Tweening;
using PJH.Agent.Player;
using PJH.Core;
using PJH.Manager;
using UnityEngine;

namespace PJH.EquipmentSkillSystem
{
    public class BarusSkill : EquipmentSkill
    {
        [SerializeField] private Vector3 clampPoint1, clampPoint2;

        [TabGroup("Info")] [SerializeField] private float arrowRainDelayTime;
        [TabGroup("Info")] [SerializeField] private float skillDis;

        private PoolManagerSO poolManager;
        [TabGroup("Pool")] [SerializeField] private PoolTypeSO arrowSkillType, arrowRainType;
        [SerializeField] private DecalProjectorPool _targetPointDecal;
        private Vector3 mousePos;

        private DecalProjectorPool decalProjector;

        public override void Init(Player player, Equipment.Equipment equipment)
        {
            base.Init(player, equipment);
            poolManager = Managers.Addressable.Load<PoolManagerSO>("PoolManager");

            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.ShotBarusSkillArrowEvent += HandleShotBarusSkillArrowEvent;
            decalProjector = Instantiate(_targetPointDecal);
            decalProjector.gameObject.SetActive(false);
        }

        private void HandleShotBarusSkillArrowEvent()
        {
            float mouseDis = Vector3.Distance(mousePos, _player.transform.position);
            BowSkillArrow arrow = poolManager.Pop(arrowSkillType) as BowSkillArrow;
            arrow.transform.position = _player.transform.position;
            arrow.ShotUp(mousePos, (90 - mouseDis * 2));

            StartCoroutine(Shot(arrow));
        }

        public override void UseSkill(bool isHolding)
        {
            if (isHolding) return;
            base.UseSkill(isHolding);
            mousePos = _player.PlayerInput.GetWorldMousePosition();
            DectectWall();
            decalProjector.gameObject.SetActive(true);
            decalProjector.ShowDecal();
            decalProjector.transform.position = mousePos + Vector3.up;

            _player.transform.DOLookAt(mousePos, .5f, AxisConstraint.Y);
            var animatorCompo = _player.GetCompo<PlayerAnimator>();

            animatorCompo.SetRootMotion(true);
            animatorCompo.PlaySkillAnimation(1);
            animatorCompo.EndUseSkillEvent += HandleEndUseSkillEvent;
        }

        private void HandleEndUseSkillEvent()
        {
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.EndUseSkillEvent -= HandleEndUseSkillEvent;
            animatorCompo.SetRootMotion(false);
        }

        public override bool AttemptUseSkill(bool isHolding = false)
        {
            mousePos = _player.PlayerInput.GetWorldMousePosition();
            float mouseDis = Vector3.Distance(mousePos, _player.transform.position);

            if (mouseDis > skillDis) return false;
            return base.AttemptUseSkill(isHolding);
        }

        private IEnumerator Shot(BowSkillArrow arrow)
        {
            yield return new WaitForSeconds(arrowRainDelayTime);
            decalProjector.HideDecal();
            ArrowRain arrowRain = poolManager.Pop(arrowRainType) as ArrowRain;
            arrowRain.transform.position = mousePos;

            poolManager.Push(arrow);
        }

        private void DectectWall()
        {
            float minX = Mathf.Min(clampPoint1.x, clampPoint2.x);
            float maxX = Mathf.Max(clampPoint1.x, clampPoint2.x);
            float minZ = Mathf.Min(clampPoint1.z, clampPoint2.z);
            float maxZ = Mathf.Max(clampPoint1.z, clampPoint2.z);

            float clampedX = Mathf.Clamp(mousePos.x, minX, maxX);
            float clampedZ = Mathf.Clamp(mousePos.z, minZ, maxZ);

            mousePos = new Vector3(clampedX, mousePos.y, clampedZ);
        }
    }
}