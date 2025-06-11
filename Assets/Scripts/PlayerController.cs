using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float maxReverseSpeed = -2.5f;
    public float acceleration = 2f;
    public float brakeDeceleration = 2f;
    public float reverseAcceleration = 1f;
    public float turnSpeed = 180f;
    public float tiltAmount = 4f;

    private float speed = 0f;
    private float angle = 0f;
    private float tilt = 0f;
    private Rigidbody rb;

    private PackageController packageController;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        packageController = FindFirstObjectByType<PackageController>();
    }

    private void Update()
    {
        HandleMovement();
        MoveCharacter();
    }

    private void HandleMovement()
    {
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveInput = 1f;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveInput = -1f;

        float currentTurnSpeed = turnSpeed * (speed / maxSpeed);
        if (speed < 0f) currentTurnSpeed = turnSpeed * (Mathf.Abs(speed) / Mathf.Abs(maxReverseSpeed));

        if (moveInput >= 0f)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                angle -= currentTurnSpeed * Time.deltaTime;
                tilt = Mathf.Lerp(tilt, tiltAmount, Time.deltaTime * 5f);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                angle += currentTurnSpeed * Time.deltaTime;
                tilt = Mathf.Lerp(tilt, -tiltAmount, Time.deltaTime * 5f);
            }
            else
            {
                tilt = Mathf.Lerp(tilt, 0f, Time.deltaTime * 5f);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                angle += currentTurnSpeed * Time.deltaTime;
                tilt = Mathf.Lerp(tilt, tiltAmount, Time.deltaTime * 5f);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                angle -= currentTurnSpeed * Time.deltaTime;
                tilt = Mathf.Lerp(tilt, -tiltAmount, Time.deltaTime * 5f);
            }
            else
            {
                tilt = Mathf.Lerp(tilt, 0f, Time.deltaTime * 5f);
            }
        }

        if (moveInput > 0f && speed < maxSpeed) speed += acceleration * Time.deltaTime;
        else if (moveInput < 0f)
        {
            if (speed <= 0f) speed -= reverseAcceleration * Time.deltaTime;
            else speed -= brakeDeceleration * Time.deltaTime;
        }

        if (moveInput == 0f)
        {
            if (speed > 0f) speed -= brakeDeceleration * Time.deltaTime;
            else if (speed < 0f) speed += brakeDeceleration * Time.deltaTime;
        }

        speed = Mathf.Clamp(speed, maxReverseSpeed, maxSpeed);
    }

    private void MoveCharacter()
    {
        Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0f, Mathf.Cos(Mathf.Deg2Rad * angle));
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        transform.localRotation = Quaternion.Euler(0f, angle, tilt);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Delivery"))
        {
            packageController.DeliverPackage(other.GetComponent<Mailbox>());
        }
        else if (other.gameObject.CompareTag("Package"))
        {
            Debug.Log("Found a package! Deliver it!");
            packageController.PickupPackage(other.gameObject);
        }
    }

    // Collision checker; handles most solid objects on map as well as enemies
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            enemyMovement enemyScript = collision.gameObject.GetComponent<enemyMovement>();
            if (enemyScript != null)
            {
                switch (enemyScript.enemyType)
                {
                    case enemyMovement.EnemyType.Knockback:
                        ApplyKnockback(collision.transform.position, 8f, 0.25f);
                        enemyScript.Respawn();
                        break;

                    case enemyMovement.EnemyType.Timer:
                        GameController gameController = FindFirstObjectByType<GameController>();
                        if (gameController != null)
                        {
                            gameController.ApplyTimePenalty(10f);
                            Debug.Log("Time penalty applied.");
                        }
                        enemyScript.Respawn();
                        break;

                    case enemyMovement.EnemyType.Stealing:
                        if (packageController != null)
                        {
                            packageController.StealPackage();
                            Debug.Log("Package stolen");
                        }
                        enemyScript.Respawn();
                        break;
                }
            }
        }

        else if (collision.gameObject.CompareTag("Solid"))
        {
            speed = 0f;
        }
    }

    private bool isKnockedBack = false;  // Knockback

    // Applies a small knockback to player; called on enemy contact
    public void ApplyKnockback(Vector3 sourcePosition, float distance, float duration)
    {
        if (!isKnockedBack)
        {
            Vector3 direction = (transform.position - sourcePosition).normalized;
            direction.y = 0f;
            StartCoroutine(DoKnockback(direction, distance, duration));
        }
    }

    private IEnumerator DoKnockback(Vector3 direction, float distance, float duration)
    {
        isKnockedBack = true;
        speed = 0f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + direction * distance;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isKnockedBack = false;
    }
}
