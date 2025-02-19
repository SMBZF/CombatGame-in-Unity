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

    [SerializeField] private GameObject arrowUI; // ����Ŀ���ͷ
    private bool isLockedOn = false;
    private Vector3 prevPos;

    public event Action<EnemyController> OnEnemyDied; // ������������ // ���������¼�


    private void Start()
    {
        NavAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Animator = GetComponent<Animator>();
        Fighter = GetComponent<MeeleFighter>();

        // ��ʼ����ͷ UI
        if (arrowUI != null)
        {
            arrowUI.SetActive(false); // Ĭ�����ؼ�ͷ
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
            arrowUI.SetActive(true); // ��ʾ������ͷ
        }
    }

    public void HideArrowUI()
    {
        if (arrowUI != null)
        {
            arrowUI.SetActive(false); // ����������ͷ
        }
    }

    public void SetLockOn(bool lockedOn)
    {
        isLockedOn = lockedOn;

        if (arrowUI != null)
        {
            arrowUI.SetActive(lockedOn); // ��������״̬��ʾ�����ؼ�ͷ
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
        // ֪ͨ EnemyManager �Ƴ�����
        EnemyManager.i.RemoveEnemyInRange(this);

        // ���������¼�
        OnEnemyDied?.Invoke(this);  // ȷ�����������¼�

        // ���ٵ��˶���
        Destroy(gameObject, 2f);  // ��ʱ���٣��Ա㲥����������
    }


    private void Update()
    {
        StateMachine.Execute();

        // �����ٶ�
        Vector3 deltaPos = Animator.applyRootMotion ? Vector3.zero : transform.position - prevPos;
        Vector3 velocity = deltaPos / Time.deltaTime;

        // �����ƶ��ٶȻ�϶���
        float forwardSpeed = Vector3.Dot(velocity, transform.forward);
        Animator.SetFloat("forwardSpeed", forwardSpeed / NavAgent.velocity.magnitude / NavAgent.speed);

        // ��������ٶ�
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
