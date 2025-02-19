using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public enum AttackStates { Idle, Windup, Impact, Cooldown }

public class MeeleFighter : MonoBehaviour
{
    public List<AttackData> attacks;
    [SerializeField] GameObject sword;

    [SerializeField] float maxHealth = 100;
    [SerializeField] float currentHealth;
    [SerializeField] Image healthBar;

    [SerializeField] AudioSource audioSource; // ��Ч����Դ

    [SerializeField] AudioClip defeatSound; // ʧ����Ч
    [SerializeField] GameObject gameOverUI;

    BoxCollider swordCollider;
    public SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;

    Animator animator;

    public UnityAction OnFighterDied;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // ȷ����һ�� AudioSource ���
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (sword != null)
        {
            sword.AddComponent<HitBox>().Init(this);
            swordCollider = sword.GetComponent<BoxCollider>();
            leftHandCollider = animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<SphereCollider>();
            if (leftHandCollider != null)
                leftHandCollider.AddComponent<HitBox>().Init(this);
            rightHandCollider = animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<SphereCollider>();
            if (rightHandCollider != null)
                rightHandCollider.AddComponent<HitBox>().Init(this);
            leftFootCollider = animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponent<SphereCollider>();
            if (leftFootCollider != null)
                leftFootCollider.AddComponent<HitBox>().Init(this);
            rightFootCollider = animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<SphereCollider>();
            if (rightFootCollider != null)
                rightFootCollider.AddComponent<HitBox>().Init(this);

            DisableAllColliders();
        }
    }

    public AttackStates AttackState { get; private set; }
    bool doCombo;
    public int comboCount = 0;

    public bool InAction { get; private set; } = false;

    public void TryToAttack(Action action = null)
    {
        if (!InAction)
        {
            StartCoroutine(Attack(action));
        }
        else if (AttackState == AttackStates.Impact || AttackState == AttackStates.Cooldown)
        {
            doCombo = true;
        }
    }

    public void TryToShootAttack(string shootAttackName)
    {
        animator.CrossFade(shootAttackName, .2f);
    }

    public void WindUpAttack(string preAttackName)
    {
        AttackState = AttackStates.Windup;

        animator.CrossFade(preAttackName, 0.2f);

        // ����׼��������Ч����̬��ȡ��
        if (attacks[comboCount].WindupSound != null)
        {
            audioSource.PlayOneShot(attacks[comboCount].WindupSound);
        }
    }

    IEnumerator Attack(Action action)
    {
        InAction = true;

        animator.CrossFade(attacks[comboCount].AnimName, 0.2f);
        AttackState = AttackStates.Windup;

        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            if (AttackState == AttackStates.Windup)
            {
                if (normalizedTime >= attacks[comboCount].ImpactStartTime)
                {
                    AttackState = AttackStates.Impact;
                    EnableHitbox(attacks[comboCount]);

                    // ���Ź�����Ч����̬��ȡ��
                    if (attacks[comboCount].ImpactSound != null)
                    {
                        audioSource.PlayOneShot(attacks[comboCount].ImpactSound);
                    }
                    Debug.Log(attacks[comboCount] + "111");
                }
            }
            else if (AttackState == AttackStates.Impact)
            {
                if (normalizedTime >= attacks[comboCount].ImpactEndTime)
                {
                    AttackState = AttackStates.Cooldown;
                    DisableAllColliders();
                }
            }
            else if (AttackState == AttackStates.Cooldown)
            {
                if (doCombo)
                {
                    doCombo = false;
                    comboCount = (comboCount + 1) % 2;
                    StartCoroutine(Attack(action));
                    yield break;
                }
            }
            yield return null;
        }

        AttackState = AttackStates.Idle;
        comboCount = 0;
        InAction = false;
        action?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitBox"))
        {
            Debug.Log(gameObject.name + "��������");
            StartCoroutine(PlayHitReaction(other.GetComponent<HitBox>().GetFighter()));
        }
    }

    IEnumerator PlayHitReaction(MeeleFighter fighter)
    {
        currentHealth -= fighter.attacks[fighter.comboCount].attackDamage;

        healthBar.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + "������");
            animator.CrossFade("Death", .2f);

            // ���������ֹͣ���нű�
            DisableAllScripts();

            // ֻ�е������tag��"Player"ʱ����ʾ��Ϸ����UI
            if (gameObject.CompareTag("Player"))
            {
                PlayDefeatSound(); // ����ʧ����Ч
                gameOverUI.SetActive(true);

                // ����������������¼����ȴ��κΰ����������˵�
                StartCoroutine(WaitForAnyKeyToReturn());
            }

            OnFighterDied?.Invoke();
        }

        animator.CrossFade("SwordImpact", 0.2f);
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        yield return new WaitForSeconds(animState.length * 0.7f);
    }

    // ����ʧ����Ч
    private void PlayDefeatSound()
    {
        if (defeatSound != null)
        {
            audioSource.PlayOneShot(defeatSound);
        }
        else
        {
            Debug.LogWarning("Defeat sound is not assigned!");
        }
    }

    // �ȴ��κΰ����������˵�
    IEnumerator WaitForAnyKeyToReturn()
    {
        while (!Input.anyKeyDown) // ���û�а����κμ���������ȴ�
        {
            yield return null;
        }

        // �����κμ�ʱ���� MainUI ����
        SceneManager.LoadScene("MainUI");
    }


    void DisableAllScripts()
    {
        this.enabled = false;

        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
        }

        //animator.enabled = false;
    }

    void EnableHitbox(AttackData attack)
    {
        switch (attack.HitboxToUse)
        {
            case AttackHitbox.LeftHand:
                leftHandCollider.enabled = true;
                break;
            case AttackHitbox.RightHand:
                rightHandCollider.enabled = true;
                break;
            case AttackHitbox.LeftFoot:
                leftFootCollider.enabled = true;
                break;
            case AttackHitbox.RightFoot:
                rightFootCollider.enabled = true;
                break;
            case AttackHitbox.Sword:
                swordCollider.enabled = true;
                break;
            default:
                break;
        }
    }

    void DisableAllColliders()
    {
        if (swordCollider != null)
            swordCollider.enabled = false;

        if (leftHandCollider != null)
            leftHandCollider.enabled = false;

        if (rightHandCollider != null)
            rightHandCollider.enabled = false;

        if (leftFootCollider != null)
            leftFootCollider.enabled = false;

        if (rightFootCollider != null)
            rightFootCollider.enabled = false;
    }

    public List<AttackData> Attacks => attacks;

    public void TryToKick()
    {
        if (!InAction)
        {
            StartCoroutine(Kick());
        }
    }

    IEnumerator Kick()
    {
        InAction = true;

        AttackState = AttackStates.Windup;

        animator.CrossFade(attacks[2].AnimName, 0.2f);

        // �����߻���Ч
        if (attacks[2].ImpactSound != null)
        {
            audioSource.PlayOneShot(attacks[2].ImpactSound);
        }

        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / animState.length;

            if (AttackState == AttackStates.Windup)
            {
                if (normalizedTime >= attacks[2].ImpactStartTime)
                {
                    AttackState = AttackStates.Impact;
                    EnableHitbox(attacks[2]);
                }
            }
            else if (AttackState == AttackStates.Impact)
            {
                if (normalizedTime >= attacks[2].ImpactEndTime)
                {
                    AttackState = AttackStates.Cooldown;
                    DisableAllColliders();
                }
            }
            yield return null;
        }

        AttackState = AttackStates.Idle;
        InAction = false;
    }
}
