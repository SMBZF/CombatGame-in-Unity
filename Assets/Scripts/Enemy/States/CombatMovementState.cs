using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AICombatStates { Idle, Chase, Circling}

public class CombatMovementState : State<EnemyController>
{
    [SerializeField] float circlingSpeed = 10f;
    [SerializeField] float distanceToStand = 4f;
    [SerializeField] float adjustDistanceThreshold = 1f;
    [SerializeField] Vector2 idleTimeRange = new Vector2(1, 2);
    [SerializeField] Vector2 circlingTimeRange = new Vector2(1, 2);

    float timer = 0f;

    int circlingDir = 1;

    AICombatStates state;

    EnemyController enemy;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;

        enemy.NavAgent.stoppingDistance = distanceToStand;
        enemy.CombatMovementTimer = 0f;

        enemy.Animator.SetBool("combatMode", true);
    }

    public override void Execute()
    {
        if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) > distanceToStand + adjustDistanceThreshold)
            StartChase();
        if (state == AICombatStates.Idle)
        {
            if (timer <= 0)
            {
                if (Random.Range(0, 2) == 0)
                    StartIdle();
                else
                    StartCircling();
            }
        }
        else if (state == AICombatStates.Chase)
        {
            if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= distanceToStand + 0.03f)
            {
                StartIdle();
                return;
            }
                enemy.NavAgent.SetDestination(enemy.Target.transform.position);
        }
        
        else if (state == AICombatStates.Circling)
        {
            if (timer <= 0)
            {
                StartIdle();
                return;
            }
            
            var vecToTarget = enemy.transform.position - enemy.Target.transform.position;
            var rotatedPos = Quaternion.Euler(0, circlingDir * circlingSpeed * Time.deltaTime, 0) * vecToTarget;

            enemy.NavAgent.Move(rotatedPos - vecToTarget);
            enemy.transform.rotation = Quaternion.LookRotation(-rotatedPos);
        }

        if (timer > 0)
            timer -= Time.deltaTime;

        enemy.CombatMovementTimer += Time.deltaTime;
    }

    public void StartChase()
    {
        state = AICombatStates.Chase;
    }


    void StartIdle()
    {
        state = AICombatStates.Idle;
        timer = Random.Range(idleTimeRange.x, idleTimeRange.y);
    }

    void StartCircling()
    {
        state = AICombatStates.Circling;

       // enemy.NavAgent.ResetPath();
        timer = Random.Range(circlingTimeRange.x, circlingTimeRange.y);

        circlingDir = Random.Range(0, 2) == 0 ? 1 : -1;
    }

    public override void Exit()
    {
        enemy.CombatMovementTimer = 0f;
    }
}
