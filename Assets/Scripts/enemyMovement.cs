using UnityEngine;
using UnityEngine.AI;

public class enemyMovement : MonoBehaviour
{
    public Transform player;
    public float chaseRadius = 10f;
    public EnemyType enemyType = EnemyType.Knockback;
    private NavMeshAgent navMeshAgent;
    private GameController gameController;
    public Vector3 startingPosition;

    public enum EnemyType
    {
        Knockback,
        Timer,
        Stealing
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        startingPosition = transform.position;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("Player not found in the scene!");
            }
        }

        gameController = FindFirstObjectByType<GameController>();
        if (gameController == null)
        {
            Debug.LogError("GameController not found in the scene!");
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= chaseRadius)
            {
                navMeshAgent.SetDestination(player.position);
            }
            else
            {
                navMeshAgent.ResetPath();
            }

        }
    }

    public void Respawn()
    {
        navMeshAgent.ResetPath();
        transform.position = startingPosition;
    }
}