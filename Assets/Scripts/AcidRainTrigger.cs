using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidRainTrigger : MonoBehaviour
{
    public List<GameObject> clouds = new List<GameObject>();
    public float acidRainDuration = 5f;
    public float dropFrequency = 0.2f;
    public float acidDropSpeed = 8f;

    private bool isRaining = false;

    private void Start()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isRaining)
        {
            StartCoroutine(TriggerAcidRain());
        }
    }

    private IEnumerator TriggerAcidRain()
    {
        isRaining = true;

        // Change cloud colors
        foreach (GameObject cloud in clouds)
        {
            SpriteRenderer renderer = cloud.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = Color.red;
            }
        }

        // Start acid rain for each cloud
        foreach (GameObject cloud in clouds)
        {
            StartCoroutine(SpawnAcidDrops(cloud));
        }

        // Wait for acid rain duration
        yield return new WaitForSeconds(acidRainDuration);

        // Restore original cloud colors
        foreach (GameObject cloud in clouds)
        {
            SpriteRenderer renderer = cloud.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = Color.white;
            }
        }

        yield return new WaitForSeconds(1f);
        isRaining = false;
    }

    private IEnumerator SpawnAcidDrops(GameObject cloud)
    {
        int totalDrops = Mathf.FloorToInt(acidRainDuration / dropFrequency);

        for (int i = 0; i < totalDrops; i++)
        {
            if (isRaining)
            {
                SpawnAcidDrop(cloud);
                yield return new WaitForSeconds(dropFrequency);
            }
            else
            {
                break;
            }
        }
    }

    private void SpawnAcidDrop(GameObject cloud)
    {
        SpriteRenderer cloudRenderer = cloud.GetComponent<SpriteRenderer>();
        if (cloudRenderer == null) return;

        float cloudWidth = cloudRenderer.bounds.size.x;
        float randomX = Random.Range(-cloudWidth / 2, cloudWidth / 2);
        Vector3 dropPosition = cloud.transform.position + new Vector3(randomX, -0.5f, 0);

        // Create a simple acid drop
        GameObject acidDrop = new GameObject("AcidDrop");
        acidDrop.transform.position = dropPosition;

        // Add sprite renderer with RED color
        SpriteRenderer dropRenderer = acidDrop.AddComponent<SpriteRenderer>();
        dropRenderer.color = Color.red;
        dropRenderer.sprite = CreateSimpleSprite();

        // Make drops significantly larger - 3x larger than before
        acidDrop.transform.localScale = new Vector3(0.6f, 0.9f, 1f);

        // Add behavior
        AcidDrop dropBehavior = acidDrop.AddComponent<AcidDrop>();
        dropBehavior.fallSpeed = acidDropSpeed;

        // Clean up
        Destroy(acidDrop, 10f);
    }

    private Sprite CreateSimpleSprite()
    {
        // Create a larger texture for better quality
        Texture2D texture = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];

        for (int i = 0; i < colors.Length; i++)
        {
            int x = i % 32;
            int y = i / 32;
            float distanceFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
            colors[i] = distanceFromCenter < 14 ? Color.red : new Color(0, 0, 0, 0);
        }

        texture.SetPixels(colors);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
}

public class AcidDrop : MonoBehaviour
{
    public float fallSpeed = 8f;

    private void Start()
    {
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.3f; // Larger collider to match larger sprite
    }

    private void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null && !player.starpower)
            {
                player.Death();
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}