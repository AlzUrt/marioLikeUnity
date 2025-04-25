using System.Collections;
using UnityEngine;

public class CrushingWallsTrigger : MonoBehaviour
{
    [Header("Walls Configuration")]
    [Tooltip("Les deux murs qui vont se rapprocher")]
    public GameObject leftWall;
    public GameObject rightWall;

    [Header("Movement Settings")]
    [Tooltip("Distance finale entre les deux murs (en unités)")]
    public float finalGap = 0.5f;

    [Tooltip("Vitesse de déplacement des murs")]
    public float moveSpeed = 5f;

    [Tooltip("Peut-on déclencher ce piège plusieurs fois?")]
    public bool canTriggerMultipleTimes = false;

    [Tooltip("Délai avant que les murs reviennent à leur position d'origine (0 = ne reviennent pas)")]
    public float returnDelay = 0f;

    // Variables privées
    private Vector3 leftWallOriginalPosition;
    private Vector3 rightWallOriginalPosition;
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
        else
        {
            // Créer un collider si aucun n'existe
            BoxCollider2D triggerCollider = gameObject.AddComponent<BoxCollider2D>();
            triggerCollider.isTrigger = true;
        }

        // Stocker les positions originales des murs
        if (leftWall != null && rightWall != null)
        {
            leftWallOriginalPosition = leftWall.transform.position;
            rightWallOriginalPosition = rightWall.transform.position;
        }
        else
        {
            Debug.LogError("CrushingWallsTrigger: Murs gauche ou droit non assignés!");
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

                // Si les murs ne sont pas déjà en mouvement
                if (!isMoving && leftWall != null && rightWall != null)
                {
                    StartCoroutine(MoveWalls());
                }
            }
        }
    }

    private IEnumerator MoveWalls()
    {
        isMoving = true;

        // Calculer les positions cibles
        float currentDistance = Vector3.Distance(leftWall.transform.position, rightWall.transform.position);
        float targetDistance = finalGap;
        float distanceToMove = (currentDistance - targetDistance) / 2f;

        Vector3 direction = (rightWall.transform.position - leftWall.transform.position).normalized;
        Vector3 leftWallTargetPosition = leftWallOriginalPosition + (direction * distanceToMove);
        Vector3 rightWallTargetPosition = rightWallOriginalPosition - (direction * distanceToMove);

        // Déplacer les murs vers leurs positions cibles
        float elapsedTime = 0f;
        float duration = distanceToMove / moveSpeed;
        Vector3 leftWallStartPosition = leftWall.transform.position;
        Vector3 rightWallStartPosition = rightWall.transform.position;

        // Vérifier périodiquement pendant le déplacement si le joueur est écrasé
        while (elapsedTime < duration)
        {
            leftWall.transform.position = Vector3.Lerp(leftWallStartPosition, leftWallTargetPosition, elapsedTime / duration);
            rightWall.transform.position = Vector3.Lerp(rightWallStartPosition, rightWallTargetPosition, elapsedTime / duration);

            // Vérifier tous les 0.1 secondes si le joueur est écrasé pendant le mouvement
            if (Mathf.FloorToInt(elapsedTime * 10) != Mathf.FloorToInt((elapsedTime + Time.deltaTime) * 10))
            {
                CheckPlayerCrushed();
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // S'assurer que les murs sont exactement à leurs positions cibles
        leftWall.transform.position = leftWallTargetPosition;
        rightWall.transform.position = rightWallTargetPosition;

        // Vérification finale des collisions avec le joueur
        CheckPlayerCrushed();

        // Si un délai de retour est spécifié, attendre puis revenir
        if (returnDelay > 0)
        {
            yield return new WaitForSeconds(returnDelay);

            // Déplacer les murs vers leurs positions d'origine
            elapsedTime = 0f;
            duration = distanceToMove / moveSpeed;
            leftWallStartPosition = leftWall.transform.position;
            rightWallStartPosition = rightWall.transform.position;

            while (elapsedTime < duration)
            {
                leftWall.transform.position = Vector3.Lerp(leftWallStartPosition, leftWallOriginalPosition, elapsedTime / duration);
                rightWall.transform.position = Vector3.Lerp(rightWallStartPosition, rightWallOriginalPosition, elapsedTime / duration);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // S'assurer que les murs sont exactement à leurs positions d'origine
            leftWall.transform.position = leftWallOriginalPosition;
            rightWall.transform.position = rightWallOriginalPosition;

            // Réinitialiser le trigger si on peut l'utiliser plusieurs fois
            if (canTriggerMultipleTimes)
            {
                hasBeenTriggered = false;
            }
        }

        isMoving = false;
    }

    private void CheckPlayerCrushed()
    {
        // Vérifier si le joueur est entre les deux murs en utilisant une zone de détection
        // Cette méthode est plus fiable que les raycasts, surtout quand les murs sont proches

        // Calculer la zone entre les murs
        Vector3 leftPos = leftWall.transform.position;
        Vector3 rightPos = rightWall.transform.position;
        Vector3 center = (leftPos + rightPos) / 2f;

        // Obtenir les tailles approximatives des murs en utilisant leurs colliders ou renderers
        float leftWallWidth = GetObjectWidth(leftWall);
        float rightWallWidth = GetObjectWidth(rightWall);

        // Créer une zone de détection légèrement plus grande que l'espace entre les murs
        Vector2 direction = (rightPos - leftPos).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x); // Perpendiculaire à la direction

        float wallsDistance = Vector3.Distance(leftPos, rightPos);
        float boxWidth = wallsDistance + 0.2f; // Un peu plus large pour éviter les problèmes aux bords
        float boxHeight = 2f; // Hauteur suffisante pour détecter le joueur

        // Détecter tous les colliders dans la zone entre les murs
        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, new Vector2(boxWidth, boxHeight),
                                Vector2.Angle(direction, Vector2.right));

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Player player = collider.GetComponent<Player>();
                if (player != null && !player.starpower)
                {
                    // Vérifier si les murs sont assez proches pour écraser le joueur
                    if (wallsDistance <= finalGap * 1.2f) // Marge légèrement plus grande
                    {
                        Debug.Log("Joueur écrasé par les murs!");
                        player.Hit(); // Utilisez Hit() au lieu de Death() pour respecter la logique du jeu

                        // Déplacer légèrement le joueur pour éviter qu'il reste coincé
                        Vector3 playerPos = player.transform.position;

                        // Déplacer vers le haut pour le sortir du piège
                        player.transform.position = new Vector3(playerPos.x, playerPos.y + 1f, playerPos.z);
                    }
                }
            }
        }
    }

    private float GetObjectWidth(GameObject obj)
    {
        // Tente d'obtenir la largeur d'un objet en utilisant son collider ou renderer
        if (obj.TryGetComponent(out Collider2D collider))
        {
            return collider.bounds.size.x;
        }
        else if (obj.TryGetComponent(out Renderer renderer))
        {
            return renderer.bounds.size.x;
        }

        return 1f; // Valeur par défaut
    }
}