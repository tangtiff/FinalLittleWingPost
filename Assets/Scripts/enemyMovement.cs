using UnityEngine;
using UnityEngine.AI;

public class enemyMovement : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent navMeshAgent;
    private SceneController gameController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        // Find the SceneController in the scene
        gameController = FindObjectOfType<SceneController>();
        if (gameController == null)
        {
            Debug.LogError("SceneController not found in the scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) 
        {
            navMeshAgent.SetDestination(player.position);
        }  
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name}");
        
        // Check if we collided with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collision detected!");
            if (gameController != null)
            {
                Debug.Log("Applying time penalty!");
                gameController.ApplyTimePenalty(10f);
            }
            else
            {
                Debug.LogError("SceneController is null when trying to apply penalty!");
            }
        }
    }
}
