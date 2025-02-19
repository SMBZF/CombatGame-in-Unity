using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget; // The player's Transform
    public float distance = 5;
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float minVerticalAngle = -45;
    [SerializeField] float maxVerticalAngle = 45;
    public Vector2 framingOffset;
    public float rotationX;
    public float rotationY;
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;
    float invertXVal;
    float invertYVal;

    [SerializeField] Transform enemyTarget; // The enemy's Transform
    [SerializeField] bool lockOnEnemy = false; // Whether to lock onto the enemy
    [SerializeField] float smoothFactor = 0.1f; // Smoothing factor
    [SerializeField] float lockRange = 10f; // The range to lock onto an enemy
    //[SerializeField] float lockOnTransitionSpeed = 5f; // The transition speed when locking onto the enemy

    private Vector3 initialCameraPosition; // The initial camera position before lock-on
    private Quaternion initialCameraRotation; // The initial camera rotation before lock-on

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        initialCameraPosition = transform.position;
        initialCameraRotation = transform.rotation;
    }

    private void Update()
    {
        // Locking and unlocking the enemy
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!lockOnEnemy)
            {
                // Find the nearest enemy within range
                FindNearestEnemy();
                if (enemyTarget != null)
                {
                    var enemyController = enemyTarget.GetComponent<EnemyController>();
                    if (enemyController != null)
                    {
                        enemyController.SetLockOn(true); // Show the arrow when locked on
                    }
                }
            }
            else
            {
                // Unlock the enemy
                if (enemyTarget != null)
                {
                    var enemyController = enemyTarget.GetComponent<EnemyController>();
                    if (enemyController != null)
                    {
                        enemyController.SetLockOn(false); // Hide the arrow when unlocking
                    }
                }

                lockOnEnemy = false;
                enemyTarget = null;
            }
        }

        // Check if the enemy is dead or destroyed each frame, if so, unlock
        if (lockOnEnemy && (enemyTarget == null || !enemyTarget.gameObject.activeInHierarchy))
        {
            // The enemy is dead or destroyed, unlock
            lockOnEnemy = false;
            enemyTarget = null;
        }

        if (!lockOnEnemy)
        {
            invertXVal = (invertX) ? -1 : 1;
            invertYVal = (invertY) ? -1 : 1;

            rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
            rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

            rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;

            var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
            var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);

            transform.position = focusPosition - targetRotation * new Vector3(0, 0, distance);
            transform.rotation = targetRotation;
            followTarget.rotation = Quaternion.Euler(0, rotationY, 0);
        }
        else
        {
            // If locked onto an enemy
            if (enemyTarget != null)
            {
                Vector3 targetToEnemy = enemyTarget.position - followTarget.position;
                targetToEnemy.y = 0;
                Quaternion targetRotationToEnemy = Quaternion.LookRotation(targetToEnemy);

                invertYVal = (invertY) ? -1 : 1;
                rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
                rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

                var finalRotation = Quaternion.Euler(rotationX, targetRotationToEnemy.eulerAngles.y, 0);
                Vector3 cameraBehindTarget = followTarget.position + new Vector3(framingOffset.x, framingOffset.y) - finalRotation * Vector3.forward * distance;

                // Smooth interpolation for camera transitions
                transform.position = Vector3.Lerp(transform.position, cameraBehindTarget, smoothFactor);
                transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, smoothFactor);
                followTarget.rotation = Quaternion.Euler(0, finalRotation.eulerAngles.y, 0);
            }
        }
    }

    private Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);

    public Vector3 GetMoveDirection(Vector3 moveInput)
    {
        if (!lockOnEnemy)
        {
            return PlanarRotation * moveInput;
        }
        else
        {
            Vector3 relativeMoveInput = followTarget.rotation * moveInput;
            return relativeMoveInput;
        }
    }

    private void FindNearestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(followTarget.position, lockRange);
        float minDistance = Mathf.Infinity;
        Transform nearestEnemy = null;
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector3.Distance(followTarget.position, collider.transform.position);
                if (distanceToEnemy < minDistance)
                {
                    minDistance = distanceToEnemy;
                    nearestEnemy = collider.transform;
                }
            }
        }
        if (nearestEnemy != null)
        {
            enemyTarget = nearestEnemy;
            lockOnEnemy = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(followTarget.position, lockRange);
    }
}
