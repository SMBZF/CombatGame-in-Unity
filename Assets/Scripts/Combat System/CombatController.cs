using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatController : MonoBehaviour
{
    MeeleFighter meeleFighter;
    public CameraController CameraController;

    [Header("UI")]
    public Image energyUI;
    public Image aimUI;

    [Header("UI - Icon Images")]
    public Image swordShieldIcon;       // Sword and shield icon
    public Image bowIcon;               // Bow icon

    [Header("Image Resources")]
    public Sprite swordShieldSelected;  // Sword and shield selected icon
    public Sprite swordShieldDefault;   // Sword and shield default icon
    public Sprite bowSelected;         // Bow selected icon
    public Sprite bowDefault;          // Bow default icon

    [Header("Energy Settings")]
    public string PreAttack = "Slash Pre";
    [Tooltip("Maximum Energy")]
    public float maxEnergy; // Maximum energy
    [Tooltip("Energy consumed per attack")]
    public float energyConsumedByEachAttack = 15f; // Energy consumed per attack
    [Tooltip("Energy consumed per second during charge up")]
    public float chargeUpEnergyConsume = 1f; // Energy consumed per second during charge-up
    [Tooltip("Energy recovered per second")]
    public float resumeEnergy; // Energy recovered per second

    [Header("Attack Settings")]
    public bool shootAttack = false;
    public float shootRadius = 10f;

    public GameObject sword;
    public GameObject dun;
    public GameObject bow;
    public GameObject arrow;
    public string shootAttackAnim = "ShootAttack";

    public float currentEnergy;

    bool isDie;

    // Camera smooth transition variables
    private Vector2 targetFramingOffset;
    private float targetDistance;
    private float smoothSpeed = 10f; // Smooth transition speed

    [Header("Attack Sound Effects")]
    public AudioClip shootAttackSound; // Shoot attack sound effect
    private AudioSource audioSource;

    private void Awake()
    {
        meeleFighter = GetComponent<MeeleFighter>();
        currentEnergy = maxEnergy;
        GetComponent<MeeleFighter>().OnFighterDied += OnDie;

        // Get AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Initialize camera target parameters
        targetFramingOffset = CameraController.framingOffset;
        targetDistance = CameraController.distance;

        // Default to attack mode
        shootAttack = false; // Default to melee attack mode
        sword.SetActive(true);
        dun.SetActive(true);
        bow.SetActive(false);
        aimUI.gameObject.SetActive(false);

        // Update icon images to selected state
        swordShieldIcon.sprite = swordShieldSelected;
        bowIcon.sprite = bowDefault;
    }

    private void OnDie()
    {
        isDie = true;
    }

    bool isAttackState;
    float timer = 0f;

    void Update()
    {
        if (isDie) return;

        // Switch attack modes
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            shootAttack = false;
            sword.SetActive(true);
            dun.SetActive(true);
            bow.SetActive(false);
            aimUI.gameObject.SetActive(false);

            targetFramingOffset = new Vector2(0, 1);
            targetDistance = 5;

            // Update icon images
            swordShieldIcon.sprite = swordShieldSelected;
            bowIcon.sprite = bowDefault;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            shootAttack = true;
            dun.SetActive(false);
            bow.SetActive(true);
            sword.SetActive(false);
            aimUI.gameObject.SetActive(true);

            targetFramingOffset = new Vector2(0, 2);
            targetDistance = 2;

            // Update icon images
            swordShieldIcon.sprite = swordShieldDefault;
            bowIcon.sprite = bowSelected;
        }

        // Smoothly transition camera parameters
        CameraController.framingOffset = Vector2.Lerp(CameraController.framingOffset, targetFramingOffset, Time.deltaTime * smoothSpeed);
        CameraController.distance = Mathf.Lerp(CameraController.distance, targetDistance, Time.deltaTime * smoothSpeed);

        // Attack logic
        if (currentEnergy - energyConsumedByEachAttack > 0 && !shootAttack) // Melee attack logic
        {
            if (Input.GetButton("Attack") && (meeleFighter.AttackState == AttackStates.Idle || meeleFighter.AttackState == AttackStates.Windup))
            {
                meeleFighter.WindUpAttack(PreAttack);
                isAttackState = true;
                timer += Time.deltaTime;
                energyUI.fillAmount = (currentEnergy - timer * chargeUpEnergyConsume) / maxEnergy;
            }
            if (Input.GetButtonUp("Attack") && isAttackState)
            {
                currentEnergy -= energyConsumedByEachAttack;
                energyUI.fillAmount = currentEnergy / maxEnergy;
                meeleFighter.TryToAttack(AfterAttackAnimLogic);
            }
        }
        else if (currentEnergy - energyConsumedByEachAttack > 0 && shootAttack) // Ranged attack logic
        {
            if (Input.GetButtonDown("Attack"))
            {
                currentEnergy -= energyConsumedByEachAttack;
                energyUI.fillAmount = currentEnergy / maxEnergy;
                meeleFighter.TryToShootAttack(shootAttackAnim);

                // Play shoot sound effect
                if (shootAttackSound != null)
                {
                    audioSource.PlayOneShot(shootAttackSound);
                }

                var arrowInstance = Instantiate(arrow);
                Vector3 dir = ((transform.position + transform.forward * shootRadius) - bow.transform.position).normalized;
                arrowInstance.transform.LookAt(dir);
                arrowInstance.transform.position = bow.transform.position;
                arrowInstance.GetComponent<AlongPathMove>().Init(20, bow.transform.position, bow.transform.position + Vector3.up, bow.transform.position + transform.forward * shootRadius);
                arrowInstance.GetComponent<HitBox>().Init(meeleFighter);
            }
        }

        // Recover energy
        if (!isAttackState)
        {
            if (currentEnergy < maxEnergy)
                currentEnergy += resumeEnergy * Time.deltaTime;

            energyUI.fillAmount = currentEnergy / maxEnergy;
        }

        // Kick logic
        if (Input.GetKeyDown(KeyCode.X))
        {
            meeleFighter.TryToKick();
        }
    }

    private void AfterAttackAnimLogic()
    {
        timer = 0f;
        isAttackState = false;
    }

    private void OnDestroy()
    {
        GetComponent<MeeleFighter>().OnFighterDied -= OnDie;
    }
}
