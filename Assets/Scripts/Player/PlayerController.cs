using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f; // Movement speed
    [SerializeField] float rotationSpeed = 500f; // Rotation speed

    [SerializeField] float groundCheckRadius = 0.2f; // Ground check radius
    [SerializeField] Vector3 groundCheckOffset; // Offset for ground check
    [SerializeField] LayerMask groundLayer; // Layer for ground detection

    bool isGrounded; // Whether the player is grounded

    bool isDefending; // Whether the player is defending

    float ySpeed; // Y-axis speed (for gravity)

    bool isDie; // Whether the player is dead

    bool isWalking = false; // Whether the player is walking

    Quaternion targetRotation; // The target rotation for the player
    CameraController cameraController; // Reference to the camera controller
    Animator animator; // Reference to the animator
    CharacterController characterController; // Reference to the character controller
    MeeleFighter meeleFighter; // Reference to the melee fighter

    // Audio variables
    public AudioClip walkSound; // Walking sound effect
    public AudioClip runSound;  // Running sound effect
    private AudioSource audioSource; // Audio source component

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>(); // Get camera controller
        animator = GetComponent<Animator>(); // Get animator
        characterController = GetComponent<CharacterController>(); // Get character controller
        meeleFighter = GetComponent<MeeleFighter>(); // Get melee fighter
        audioSource = GetComponent<AudioSource>(); // Get AudioSource component
        GetComponent<MeeleFighter>().OnFighterDied += OnDie; // Subscribe to OnDie event
    }

    private void OnDie()
    {
        isDie = true; // Set player as dead
    }

    void Update()
    {
        if (isDie) return; // If player is dead, stop processing

        GroundCheck(); // Check if player is grounded

        // If attacking, do not process movement logic
        if (meeleFighter != null && meeleFighter.InAction)
        {
            animator.SetFloat("forwardSpeed", 0f, 0.2f, Time.deltaTime); // Set forward speed to 0 while attacking
            return;
        }

        if (characterController.isGrounded)
        {
            ySpeed = -0.5f; // Reset Y speed if grounded
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime; // Apply gravity
        }

        var moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized; // Get movement input
        var moveDir = cameraController.GetMoveDirection(moveInput); // Get direction from camera controller

        var velocity = moveDir * moveSpeed; // Calculate velocity
        velocity.y = ySpeed; // Apply Y speed (gravity)

        characterController.Move(velocity * Time.deltaTime); // Move the character

        // Rotate the player based on movement input
        if (moveInput.magnitude > 0.1f)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Check if Shift key is held to run
        float currentMoveAmount = moveInput.magnitude * 0.3f; // Default walking, max = 0.3f
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentMoveAmount = Mathf.Clamp01(moveInput.magnitude); // Max move amount = 1 when running
        }

        // Set forward speed parameter for animation transition
        animator.SetFloat("forwardSpeed", currentMoveAmount, 0.2f, Time.deltaTime);

        // Play walking and running sound effects
        HandleMovementSounds(moveInput.magnitude);
    }

    void GroundCheck()
    {
        Vector3 checkPos = transform.TransformPoint(groundCheckOffset); // Get the position to check for ground
        isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundLayer); // Check if grounded
        if (isGrounded) ySpeed = Mathf.Max(ySpeed, -0.5f); // Correct Y speed if grounded
    }

    // Handle the playback of walking and running sounds
    void HandleMovementSounds(float moveAmount)
    {
        if (moveAmount > 0.1f) // If moving
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) // If running
            {
                if (!audioSource.isPlaying || audioSource.clip != runSound)
                {
                    audioSource.clip = runSound; // Set run sound effect
                    audioSource.Play(); // Play run sound effect
                    isWalking = true; // Set as walking
                }
            }
            else // If walking
            {
                if (!audioSource.isPlaying || audioSource.clip != walkSound)
                {
                    audioSource.clip = walkSound; // Set walk sound effect
                    audioSource.Play(); // Play walk sound effect
                    isWalking = true; // Set as walking
                }
            }
        }
        else // If not moving
        {
            if (isWalking && audioSource.isPlaying)
            {
                audioSource.Stop(); // Stop walking or running sound effect
                isWalking = false; // Set as not walking or running
            }
        }
    }

    private void OnDestroy()
    {
        GetComponent<MeeleFighter>().OnFighterDied -= OnDie; // Unsubscribe from OnDie event
    }
}
