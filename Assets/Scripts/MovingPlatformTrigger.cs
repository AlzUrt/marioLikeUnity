using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformTrigger : MonoBehaviour
{
    public enum MoveDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    [Header("Platform References")]
    [Tooltip("Les plateformes à déplacer")]
    public List<GameObject> platformsToMove = new List<GameObject>();

    [Header("Movement Settings")]
    [Tooltip("Direction de déplacement des plateformes")]
    public MoveDirection direction = MoveDirection.Left;

    [Tooltip("Distance de déplacement (valeur positive)")]
    public float moveDistance = 5f;

    [Tooltip("Vitesse de déplacement des plateformes")]
    public float moveSpeed = 3f;

    [Tooltip("Peut-on déclencher ce piège plusieurs fois?")]
    public bool canTriggerMultipleTimes = false;

    [Tooltip("Délai avant que les plateformes reviennent à leur position d'origine (0 = ne reviennent pas)")]
    public float returnDelay = 0f;

    // Variables privées
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> targetPositions = new Dictionary<GameObject, Vector3>();
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

        // Stocker les positions originales des plateformes
        if (platformsToMove.Count > 0)
        {
            foreach (GameObject platform in platformsToMove)
            {
                if (platform != null)
                {
                    originalPositions[platform] = platform.transform.position;

                    // Calculer la position cible selon la direction choisie
                    Vector3 moveVector = GetMoveVector();
                    targetPositions[platform] = originalPositions[platform] + moveVector;
                }
            }
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

                // Si les plateformes ne sont pas déjà en mouvement
                if (!isMoving && platformsToMove.Count > 0)
                {
                    StartCoroutine(MovePlatforms());
                }
            }
        }
    }

    private IEnumerator MovePlatforms()
    {
        isMoving = true;

        // Déplacer toutes les plateformes vers leur position cible
        float elapsedTime = 0f;

        // Calculer la durée basée sur la distance et la vitesse
        float maxDistance = 0f;
        foreach (GameObject platform in platformsToMove)
        {
            if (platform != null)
            {
                float distance = Vector3.Distance(platform.transform.position, targetPositions[platform]);
                maxDistance = Mathf.Max(maxDistance, distance);
            }
        }

        float duration = maxDistance / moveSpeed;
        Dictionary<GameObject, Vector3> startPositions = new Dictionary<GameObject, Vector3>();

        // Enregistrer les positions de départ pour chaque plateforme
        foreach (GameObject platform in platformsToMove)
        {
            if (platform != null)
            {
                startPositions[platform] = platform.transform.position;
            }
        }

        while (elapsedTime < duration)
        {
            foreach (GameObject platform in platformsToMove)
            {
                if (platform != null)
                {
                    platform.transform.position = Vector3.Lerp(startPositions[platform],
                        targetPositions[platform], elapsedTime / duration);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // S'assurer que les plateformes sont exactement à leur position cible
        foreach (GameObject platform in platformsToMove)
        {
            if (platform != null)
            {
                platform.transform.position = targetPositions[platform];
            }
        }

        // Si un délai de retour est spécifié, attendre puis revenir
        if (returnDelay > 0)
        {
            yield return new WaitForSeconds(returnDelay);

            // Déplacer les plateformes vers leur position d'origine
            elapsedTime = 0f;

            // Recalculer le maximum de distance pour le retour
            maxDistance = 0f;
            foreach (GameObject platform in platformsToMove)
            {
                if (platform != null)
                {
                    float distance = Vector3.Distance(platform.transform.position, originalPositions[platform]);
                    maxDistance = Mathf.Max(maxDistance, distance);
                }
            }

            duration = maxDistance / moveSpeed;

            // Mettre à jour les positions de départ pour le retour
            foreach (GameObject platform in platformsToMove)
            {
                if (platform != null)
                {
                    startPositions[platform] = platform.transform.position;
                }
            }

            while (elapsedTime < duration)
            {
                foreach (GameObject platform in platformsToMove)
                {
                    if (platform != null)
                    {
                        platform.transform.position = Vector3.Lerp(startPositions[platform],
                            originalPositions[platform], elapsedTime / duration);
                    }
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // S'assurer que les plateformes sont exactement à leur position d'origine
            foreach (GameObject platform in platformsToMove)
            {
                if (platform != null)
                {
                    platform.transform.position = originalPositions[platform];
                }
            }

            // Réinitialiser le trigger si on peut l'utiliser plusieurs fois
            if (canTriggerMultipleTimes)
            {
                hasBeenTriggered = false;
            }
        }

        isMoving = false;
    }

    // Fonction pour obtenir le vecteur de déplacement selon la direction choisie
    private Vector3 GetMoveVector()
    {
        switch (direction)
        {
            case MoveDirection.Left:
                return new Vector3(-moveDistance, 0, 0);
            case MoveDirection.Right:
                return new Vector3(moveDistance, 0, 0);
            case MoveDirection.Up:
                return new Vector3(0, moveDistance, 0);
            case MoveDirection.Down:
                return new Vector3(0, -moveDistance, 0);
            default:
                return Vector3.zero;
        }
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

        // Dessiner les trajectoires des plateformes
        foreach (GameObject platform in platformsToMove)
        {
            if (platform != null)
            {
                Gizmos.color = Color.red;
                Vector3 startPos = Application.isPlaying && originalPositions.ContainsKey(platform)
                    ? originalPositions[platform]
                    : platform.transform.position;

                Vector3 moveVector = GetMoveVector();
                Vector3 endPos = startPos + moveVector;

                Gizmos.DrawLine(startPos, endPos);
                Gizmos.DrawSphere(endPos, 0.2f);
            }
        }
    }
}