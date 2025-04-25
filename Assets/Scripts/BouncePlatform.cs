using System.Collections;
using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [SerializeField] private float bounceForce = 40f;

    // Référence au SpriteRenderer pour changer la couleur
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        // Récupérer le SpriteRenderer et sauvegarder la couleur d'origine
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out Player player))
        {
            if (collision.transform.DotTest(transform, Vector2.down))
            {
                if (player.movement != null)
                {
                    player.movement.velocity = new Vector2(player.movement.velocity.x, bounceForce);

                    // Changer la couleur en rouge
                    ChangeColor();
                }
            }
        }
    }

    private void ChangeColor()
    {
        if (spriteRenderer != null)
        {
            // Arrêter toutes les coroutines précédentes pour éviter des problèmes si le joueur rebondit plusieurs fois rapidement
            StopAllCoroutines();

            // Changer la couleur en rouge
            spriteRenderer.color = Color.black;

            // Démarrer la coroutine pour restaurer la couleur originale après 1 seconde
            StartCoroutine(RestoreColor());
        }
    }

    private IEnumerator RestoreColor()
    {
        // Attendre 1 seconde
        yield return new WaitForSeconds(2f);

        // Restaurer la couleur d'origine
        spriteRenderer.color = originalColor;
    }
}