using System.Collections;
using UnityEngine;

public class MovingPlatformTrigger : MonoBehaviour
{
    [Header("Platform References")]
    [Tooltip("La plateforme à déplacer")]
    public GameObject platformToMove;

    [Header("Movement Settings")]
    [Tooltip("Distance de déplacement sur l'axe X (négatif = gauche)")]
    public float moveDistanceX = -5f;

    [Tooltip("Vitesse de déplacement de la plateforme")]
    public float moveSpeed = 3f;

    [Tooltip("Peut-on déclencher ce piège plusieurs fois?")]
    public bool canTriggerMultipleTimes = false;

    [Tooltip("Délai avant que la plateforme revienne à sa position d'origine (0 = ne revient pas)")]
    public float returnDelay = 0f;

    // Variables privées
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool hasBeenTriggered = false;
    private bool isMoving = false;

    private void Start()
    {
        // S'assurer que le collider est en mode trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        // Stocker la position originale de la plateforme
        if (platformToMove != null)
        {
            originalPosition = platformToMove.transform.position;
            targetPosition = originalPosition + new Vector3(moveDistanceX, 0, 0);
        }
        else
        {
            Debug.LogError("MovingPlatformTrigger: Aucune plateforme assignée!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifier si c'est le joueur qui a touché le trigger
        if (other.CompareTag("Player"))
        {
            // Si on peut déclencher plusieurs fois OU si c'est la première fois
            if (canTriggerMultipleTimes || !hasBeenTriggered)
            {
                // Marquer comme déclenché
                hasBeenTriggered = true;

                // Si la plateforme n'est pas déjà en mouvement
                if (!isMoving && platformToMove != null)
                {
                    StartCoroutine(MovePlatform());
                }
            }
        }
    }

    private IEnumerator MovePlatform()
    {
        isMoving = true;

        // Déplacer la plateforme vers la position cible
        float elapsedTime = 0f;
        float duration = Vector3.Distance(platformToMove.transform.position, targetPosition) / moveSpeed;
        Vector3 startPosition = platformToMove.transform.position;

        while (elapsedTime < duration)
        {
            platformToMove.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // S'assurer que la plateforme est exactement à la position cible
        platformToMove.transform.position = targetPosition;

        // Si un délai de retour est spécifié, attendre puis revenir
        if (returnDelay > 0)
        {
            yield return new WaitForSeconds(returnDelay);

            // Déplacer la plateforme vers sa position d'origine
            elapsedTime = 0f;
            duration = Vector3.Distance(platformToMove.transform.position, originalPosition) / moveSpeed;
            startPosition = platformToMove.transform.position;

            while (elapsedTime < duration)
            {
                platformToMove.transform.position = Vector3.Lerp(startPosition, originalPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // S'assurer que la plateforme est exactement à sa position d'origine
            platformToMove.transform.position = originalPosition;

            // Réinitialiser le trigger si on peut l'utiliser plusieurs fois
            if (canTriggerMultipleTimes)
            {
                hasBeenTriggered = false;
            }
        }

        isMoving = false;
    }

    // Pour le débogage - rendre visible le trigger invisible dans l'éditeur
    private void OnDrawGizmos()
    {
        // Dessiner le volume du trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Orange semi-transparent
            Gizmos.DrawCube(transform.position, collider.bounds.size);
        }

        // Dessiner la trajectoire de la plateforme
        if (platformToMove != null)
        {
            Gizmos.color = Color.red;
            Vector3 startPos = Application.isPlaying ? originalPosition : platformToMove.transform.position;
            Vector3 endPos = startPos + new Vector3(moveDistanceX, 0, 0);
            Gizmos.DrawLine(startPos, endPos);
            Gizmos.DrawSphere(endPos, 0.2f);
        }
    }
}