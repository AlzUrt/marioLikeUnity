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

            // Vérifier si le SpikeBlock existe déjà avant d'en ajouter un nouveau
            SpikeBlock spikeBlock = spikePlatform.GetComponent<SpikeBlock>();
            if (spikeBlock == null)
            {
                spikeBlock = spikePlatform.AddComponent<SpikeBlock>();
            }

            // Configurer les colliders pour qu'ils fonctionnent correctement
            ConfigureColliders(spikePlatform);
        }
    }

    private void ConfigureColliders(GameObject obj)
    {
        Collider2D[] colliders = obj.GetComponents<Collider2D>();

        // S'assurer qu'au moins un collider n'est pas un trigger pour la physique
        bool hasNonTriggerCollider = false;

        foreach (Collider2D collider in colliders)
        {
            // Garder au moins un collider comme non-trigger pour la physique
            if (!hasNonTriggerCollider)
            {
                collider.isTrigger = false;
                hasNonTriggerCollider = true;
            }
            else
            {
                collider.isTrigger = true;
            }
        }

        // Si aucun collider n'a été trouvé, en ajouter un pour la détection
        if (colliders.Length == 0)
        {
            BoxCollider2D newCollider = obj.AddComponent<BoxCollider2D>();
            newCollider.isTrigger = false;

            // Ajouter un second collider pour la détection du joueur
            BoxCollider2D triggerCollider = obj.AddComponent<BoxCollider2D>();
            triggerCollider.isTrigger = true;
        }
        else if (colliders.Length == 1)
        {
            // Si un seul collider existe, ajouter un second en trigger
            BoxCollider2D triggerCollider = obj.AddComponent<BoxCollider2D>();
            triggerCollider.size = colliders[0].bounds.size;
            triggerCollider.offset = colliders[0].offset;
            triggerCollider.isTrigger = true;
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
            if (player != null && !player.starpower)
            {
                player.Death();
            }
        }
    }
}