using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FallingPlatform : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Time in seconds before the platform starts to fall after player steps on it")]
    public float delayBeforeFall = 0.2f;

    [Tooltip("How long the platform shakes before falling")]
    public float shakeDuration = 0.2f;

    [Tooltip("Intensity of the shake effect")]
    public float shakeIntensity = 0.1f;

    [Tooltip("How quickly the platform falls")]
    public float fallSpeed = 9.0f;

    [Tooltip("Time in seconds before platform is destroyed after falling")]
    public float destroyDelay = 0.5f;

    // Internal variables
    private Vector3 initialPosition;
    private bool isFalling = false;
    private bool playerDetected = false;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        initialPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        Debug.Log("FallingPlatform initialized on: " + gameObject.name);
    }

    private void Start()
    {
        Debug.Log("Platform position: " + transform.position);
        Debug.Log("Has SpriteRenderer: " + (spriteRenderer != null));
        Debug.Log("Has BoxCollider2D: " + (boxCollider != null));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        // Detect any collision with player
        if (!isFalling && !playerDetected && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player detected on platform");
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null)
            {
                Debug.Log("Starting fall sequence!");
                playerDetected = true;
                StartCoroutine(FallSequence());
            }
            else
            {
                Debug.Log("Player component not found on collision object");
            }
        }
    }

    private IEnumerator FallSequence()
    {
        Debug.Log("Waiting for " + delayBeforeFall + " seconds");
        yield return new WaitForSeconds(delayBeforeFall);

        // Shake effect before falling
        Debug.Log("Starting shake effect");
        float elapsed = 0;
        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitCircle * shakeIntensity;
            transform.position = initialPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Start falling
        Debug.Log("Platform is now falling");
        isFalling = true;

        // Optional: Disable collision with the player when falling
        boxCollider.isTrigger = true;

        // Falling physics
        while (true)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnBecameInvisible()
    {
        // Destroy the platform when it's off-screen
        if (isFalling)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    // Optional: Reset the platform if needed for reuse
    public void Reset()
    {
        isFalling = false;
        playerDetected = false;
        transform.position = initialPosition;

        boxCollider.isTrigger = false;

        StopAllCoroutines();
    }
}