using UnityEngine;

public class SpikeBlock : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Specify a particle system to play when player hits spikes (optional)")]
    public ParticleSystem hitEffect;

    [Tooltip("How much knockback to apply to the player when hit (optional)")]
    public float knockbackForce = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the colliding object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                // Optionally apply knockback before death
                if (knockbackForce > 0)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    collision.rigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }

                // Play hit effect if assigned
                if (hitEffect != null)
                {
                    hitEffect.Play();
                }

                // Kill the player immediately
                player.Death();

                // Log for debugging
                Debug.Log("Player hit spike block and died");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Alternative trigger method in case the spike uses trigger collider instead
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // Play hit effect if assigned
                if (hitEffect != null)
                {
                    hitEffect.Play();
                }

                // Kill the player immediately
                player.Death();

                // Log for debugging
                Debug.Log("Player triggered spike block and died");
            }
        }
    }
}