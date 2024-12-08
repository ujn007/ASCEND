using DG.Tweening;
using FMODUnity;
using Game.Events;
using INab.Dissolve;
using PJH.Agent.Player;
using PJH.Core;
using PJH.EquipmentSkillSystem;
using PJH.Manager;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PJH.Equipment;
using UnityEngine;
using YTH.Boss;

public class BigSwordSkill : EquipmentSkill
{
    [SerializeField] private GameEventChannelSO eventSO;
    private PoolManagerSO poolManager;

    [TabGroup("SwordInfo")] [SerializeField]
    private float swordSpawnRadius;

    [TabGroup("SwordInfo")] [SerializeField]
    private float swordSpawnTime;

    [TabGroup("SwordInfo")] [SerializeField]
    private float swordSpawnMoveDuration;

    [TabGroup("SwordInfo")] [SerializeField]
    private int swordSpawnCount;

    [TabGroup("SwordInfo")] [SerializeField]
    private int swordPlusIndex;

    [TabGroup("SwordInfo")] [SerializeField]
    private float dissolveTime;

    [TabGroup("PF")] [SerializeField] private SwordSkillCollision swordPointParent;

    [TabGroup("Pool")] [SerializeField] private PoolTypeSO bigSwordType, cloneType, lastBigSwordType;

    [TabGroup("Sound")] [SerializeField]
    private EventReference _spawnMirrorEventReference, _mirrorSlashEventReference, _spawnBigSwordEventReference;

    private int swordIndex;

    private int bossSpeed;
    private Vector3 spawnPos;

    private EnemySpawnManager enemySpawnManager => EnemySpawnManager.Instance;

    private List<Mirror> mirrorList = new();
    private List<Enemy> enemyList = new();
    private List<BossEnemy> bossList = new();
    public override void Init(Player player, Equipment equipment)
    {
        base.Init(player, equipment);
        poolManager = Managers.Addressable.Load<PoolManagerSO>("PoolManager");

    }

    public override void UseSkill(bool isHolding)
    {
        if (isHolding) return;
        base.UseSkill(isHolding);
        Vector3 mousePos = _player.PlayerInput.GetWorldMousePosition();

        SwordSkillCollision swordPar = Instantiate(swordPointParent);
        swordPar.Initialize(enemySpawnManager.EnemyList.Count, mousePos);
        spawnPos = swordPar.transform.position;

        enemyList = enemySpawnManager.EnemyList.OfType<Enemy>().ToList();
        bossList = enemySpawnManager.EnemyList.OfType<BossEnemy>().ToList();

        bossList.ForEach(e => bossSpeed = e.BossStat.moveSpeed);

        SetEnemySlow(true);

        _player.PlayerInput.EnablePlayerInput(false);

        SpawnSword(swordPar.transform);
        StartCoroutine(Moveing(swordPar));
    }

    private async void SpawnSword(Transform parent)
    {
        for (int i = 0; i < swordSpawnCount; i++)
        {
            float angle = i * Mathf.PI * 2f / swordSpawnCount;

            Vector3 position = new Vector3(
                Mathf.Cos(angle) * swordSpawnRadius + parent.position.x,
                parent.position.y,
                Mathf.Sin(angle) * swordSpawnRadius + parent.position.z
            );

            Mirror sword = poolManager.Pop(bigSwordType) as Mirror;
            RuntimeManager.PlayOneShot(_spawnMirrorEventReference, position);
            sword.transform.DOMove(position, swordSpawnMoveDuration).ChangeStartValue(position + Vector3.up * 50)
                .SetEase(Ease.Linear);
            sword.transform.LookAt(parent);
            sword.transform.rotation = Quaternion.Euler(0, sword.transform.rotation.eulerAngles.y, 0);

            mirrorList.Add(sword);

            var evt = GameEvents.CameraImpulse;
            evt.strength = 0.2f;
            eventSO.RaiseEvent(evt);

            await Task.Delay((int)(swordSpawnTime * 1000));
        }
    }

    private IEnumerator Moveing(SwordSkillCollision par)
    {
        yield return YieldCache.WaitForSeconds(swordSpawnTime * swordSpawnCount + 1);

        PlayerDissolver(true);

        yield return YieldCache.WaitForSeconds(1);

        MoveTwoPos moveTwoPos = new MoveTwoPos();

        var evt = GameEvents.CameraPerlin;
        evt.strength = 2;
        evt.increaseDur = 0.2f;
        eventSO.RaiseEvent(evt);

        for (int i = 0; i < 70; i++)
        {
            SwordPlayerClone s = poolManager.Pop(cloneType) as SwordPlayerClone;

            GetNextSword(moveTwoPos);
            s.MoveToWhere(moveTwoPos);
            RuntimeManager.PlayOneShot(_mirrorSlashEventReference, moveTwoPos.endPos);

            if (i % 6 == 0)
                par.DetectEnemy();

            yield return YieldCache.WaitForSeconds(0.05f);
        }

        evt.strength = 0;
        eventSO.RaiseEvent(evt);
        Destroy(par.gameObject);

        SpawnLastBigSword();
    }

    private void GetNextSword(MoveTwoPos moveTwoPos)
    {
        Mirror swordStart = mirrorList[swordIndex];

        swordIndex += swordPlusIndex;
        if (swordIndex >= swordSpawnCount) swordIndex -= swordSpawnCount;

        Mirror swordNext = mirrorList[swordIndex];

        moveTwoPos.startPos = swordStart.transform.position;
        moveTwoPos.endPos = swordNext.transform.position;
    }

    private void SpawnLastBigSword()
    {
        PlayerDissolver(false);
        _player.PlayerInput.EnablePlayerInput(true);
        _player.IsUsingSkill = false;
        LastBigSword s = poolManager.Pop(lastBigSwordType) as LastBigSword;
        Sequence sq = DOTween.Sequence();
        RuntimeManager.PlayOneShot(_spawnBigSwordEventReference, spawnPos);

        sq.Append(s.transform.DOMove(spawnPos, 0.2f).ChangeStartValue(spawnPos + Vector3.up * 30).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                mirrorList.ForEach(s => s.Bomb());

                var evt = GameEvents.CameraImpulse;
                evt.strength = 4;
                eventSO.RaiseEvent(evt);

                SetEnemySlow(false);

                s.DectectEnemy();
            }));
        sq.AppendInterval(3);
        sq.AppendCallback(() =>
        {
            Dissolve();
            ResetLastBigSword(s);
        });
    }

    private void Dissolve()
    {
        mirrorList.ForEach(s => { s.Dissolve(); });

        mirrorList.Clear();
    }

    private async void ResetLastBigSword(LastBigSword sword)
    {
        if (sword.TryGetComponent(out Dissolver dissolver))
            dissolver.Dissolve();
        await Task.Delay(2000);
        poolManager.Push(sword);
    }

    private void PlayerDissolver(bool v)
    {
        Dissolver playerDissolve = _player.transform.Find("Dissolve").GetComponent<Dissolver>();
        Dissolver weaponDissolve = _player.GetCompo<PlayerEquipmentController>().GetWeapon().DissolverCompo;

        if (v)
        {
            playerDissolve.Dissolve();
            weaponDissolve?.Dissolve();
        }
        else
        {
            playerDissolve.Materialize();
            weaponDissolve?.Materialize();
        }
    }

    private void SetEnemySlow(bool v)
    {
        if (v)
        {
            enemyList.ForEach(e => e.SetSlow(true, 0.2f));
            foreach (BossEnemy boss in bossList)
            {
                boss.AnimatorCompo.speed = 0.2f;
                boss.BossStat.moveSpeed = 1;
            }
        }
        else
        {
            enemyList.ForEach(e => e.SetSlow(false));
            foreach (BossEnemy boss in bossList)
            {
                boss.AnimatorCompo.speed = 1;
                boss.BossStat.moveSpeed = bossSpeed;
            }
        }
    }
}