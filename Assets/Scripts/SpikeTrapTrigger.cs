using UnityEngine;

public class SpikeTrapTrigger : MonoBehaviour
{
    public GameObject spikePlatform;

    public float fallSpeed = 15f;

    private bool hasBeenTriggered = false;

    private void Start()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();

        if (colliders.Length == 1)
        {
            BoxCollider2D triggerCollider = gameObject.AddComponent<BoxCollider2D>();

            triggerCollider.size = colliders[0].size;
            triggerCollider.offset = colliders[0].offset;

            triggerCollider.size = new Vector2(triggerCollider.size.x, triggerCollider.size.y + 0.1f);

            triggerCollider.isTrigger = true;

            colliders[0].isTrigger = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasBeenTriggered && other.CompareTag("Player"))
        {
            DropSpikePlatform();

            hasBeenTriggered = true;

        }
    }

    private void DropSpikePlatform()
    {
        if (spikePlatform != null)
        {
            Rigidbody2D rb = spikePlatform.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = spikePlatform.AddComponent<Rigidbody2D>();
            }

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = fallSpeed / 9.81f;

            if (spikePlatform.GetComponent<SpikeBlock>() == null)
            {
                spikePlatform.AddComponent<SpikeBlock>();
            }

            ConfigureColliders(spikePlatform);
        }
    }

    private void ConfigureColliders(GameObject obj)
    {
        Collider2D[] colliders = obj.GetComponents<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            if (!collider.isTrigger)
            {
                collider.isTrigger = true;
            }
        }

        SpikeBlock spikeBlock = obj.GetComponent<SpikeBlock>();
        if (spikeBlock != null)
        {
            // Le SpikeBlock existe déjà et utilisera OnTriggerEnter2D maintenant
            // que le collider est configuré comme trigger
        }
        else
        {
            obj.AddComponent<SpikeBlockTrigger>();
        }
    }
}

public class SpikeBlockTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Death();
            }
        }
    }
}