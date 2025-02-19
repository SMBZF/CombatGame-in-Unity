using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EnemyStates { Idle, CombatMovement, Attack, RetreatAfterAttack }

public class EnemyController : MonoBehaviour
{
    [field: SerializeField] public float Fov { get; private set; } = 180f;

    public List<MeeleFighter> TargetsInRange { get; set; } = new List<MeeleFighter>();
    public MeeleFighter Target { get; set; }
    public float CombatMovementTimer { get; set; } = 0f;
    public StateMachine<EnemyController> StateMachine { get; private set; }
    Dictionary<EnemyStates, State<EnemyController>> stateDict;

    public UnityEngine.AI.NavMeshAgent NavAgent { get; private set; }
    public Animator Animator { get; private set; }
    public MeeleFighter Fighter { get; private set; }

    [SerializeField] private GameObject arrowUI; // 锁定目标箭头
    private bool isLockedOn = false;
    private Vector3 prevPos;

    public event Action<EnemyController> OnEnemyDied; // 传递自身引用 // 定义死亡事件


    private void Start()
    {
        NavAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Animator = GetComponent<Animator>();
        Fighter = GetComponent<MeeleFighter>();

        // 初始化箭头 UI
        if (arrowUI != null)
        {
            arrowUI.SetActive(false); // 默认隐藏箭头
        }

        stateDict = new Dictionary<EnemyStates, State<EnemyController>>()
        {
            { EnemyStates.Idle, GetComponent<IdleState>() },
            { EnemyStates.CombatMovement, GetComponent<CombatMovementState>() },
            { EnemyStates.Attack, GetComponent<AttackState>() },
            { EnemyStates.RetreatAfterAttack, GetComponent<RetreatAfterAttackState>() }
        };

        StateMachine = new StateMachine<EnemyController>(this);
        StateMachine.ChangeState(stateDict[EnemyStates.Idle]);

        Fighter.OnFighterDied += OnDie;
    }

    public void ShowArrowUI()
    {
        if (arrowUI != null)
        {
            arrowUI.SetActive(true); // 显示锁定箭头
        }
    }

    public void HideArrowUI()
    {
        if (arrowUI != null)
        {
            arrowUI.SetActive(false); // 隐藏锁定箭头
        }
    }

    public void SetLockOn(bool lockedOn)
    {
        isLockedOn = lockedOn;

        if (arrowUI != null)
        {
            arrowUI.SetActive(lockedOn); // 根据锁定状态显示或隐藏箭头
        }
        else
        {
            Debug.LogWarning("ArrowUI not found on the enemy.");
        }
    }

    public void ChangeState(EnemyStates state)
    {
        StateMachine.ChangeState(stateDict[state]);
    }

    public bool IsInState(EnemyStates state)
    {
        return StateMachine.CurrentState == stateDict[state];
    }

    private void OnDie()
    {
        // 通知 EnemyManager 移除敌人
        EnemyManager.i.RemoveEnemyInRange(this);

        // 触发死亡事件
        OnEnemyDied?.Invoke(this);  // 确保触发死亡事件

        // 销毁敌人对象
        Destroy(gameObject, 2f);  // 延时销毁，以便播放死亡动画
    }


    private void Update()
    {
        StateMachine.Execute();

        // 计算速度
        Vector3 deltaPos = Animator.applyRootMotion ? Vector3.zero : transform.position - prevPos;
        Vector3 velocity = deltaPos / Time.deltaTime;

        // 根据移动速度混合动画
        float forwardSpeed = Vector3.Dot(velocity, transform.forward);
        Animator.SetFloat("forwardSpeed", forwardSpeed / NavAgent.velocity.magnitude / NavAgent.speed);

        // 计算侧移速度
        float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
        float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
        Animator.SetFloat("strafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);

        prevPos = transform.position;
    }

    private void OnDestroy()
    {
        if (Fighter != null)
        {
            Fighter.OnFighterDied -= OnDie;
        }
    }
}
