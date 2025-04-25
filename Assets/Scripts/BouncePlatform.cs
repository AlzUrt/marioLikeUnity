using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [SerializeField] private float bounceForce = 40f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out Player player))
        {
            if (collision.transform.DotTest(transform, Vector2.down))
            {
                if (player.movement != null)
                {
                    player.movement.velocity = new Vector2(player.movement.velocity.x, bounceForce);
                }
            }
        }
    }
}