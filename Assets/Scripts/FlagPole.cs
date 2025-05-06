using System.Collections;
using UnityEngine;

public class FlagPole : MonoBehaviour
{
    public Transform flag;
    public Transform poleBottom;
    public Transform castle;
    public float speed = 6f;
    public int nextWorld = 1;
    public int nextStage = 1;
    private LeaderboardManager leaderboardManager;

    private void Start()
    {
        // Trouver le LeaderboardManager dans la scène
        leaderboardManager = FindObjectOfType<LeaderboardManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            StartCoroutine(MoveTo(flag, poleBottom.position));
            StartCoroutine(LevelCompleteSequence(player));
        }
    }

    private IEnumerator LevelCompleteSequence(Player player)
    {
        player.movement.enabled = false;

        // Arrêter le timer
        GameManager.Instance.StopTimer();

        yield return MoveTo(player.transform, poleBottom.position);
        yield return MoveTo(player.transform, player.transform.position + Vector3.right);
        yield return MoveTo(player.transform, player.transform.position + Vector3.right + Vector3.down);
        yield return MoveTo(player.transform, castle.position);

        player.gameObject.SetActive(false);

        // Afficher le leaderboard après l'animation
        if (leaderboardManager != null)
        {
            leaderboardManager.ShowLeaderboard();
        }
        else
        {
            // Fallback si le leaderboardManager n'est pas trouvé
            yield return new WaitForSeconds(2f);
            GameManager.Instance.LoadLevel(nextWorld, nextStage);
        }
    }

    private IEnumerator MoveTo(Transform subject, Vector3 position)
    {
        while (Vector3.Distance(subject.position, position) > 0.125f)
        {
            subject.position = Vector3.MoveTowards(subject.position, position, speed * Time.deltaTime);
            yield return null;
        }

        subject.position = position;
    }
}