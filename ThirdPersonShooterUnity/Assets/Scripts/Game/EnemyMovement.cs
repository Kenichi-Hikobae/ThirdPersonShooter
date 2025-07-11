using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 2.5f;      //  Speed when the enemy is patrol
    [SerializeField]
    private float runSpeed = 4.0f;   //  Speed when the enemy is chasing the player
    [SerializeField]
    private float radiusMovement = 25.0f;    //  Radius of the movement when is in the patrol state
    [SerializeField]
    private float waitTime = 3.0f;   //  Waiting time to the next movement
    [SerializeField]
    private float viewRadius = 20.0f;    //  How far the enemy detects the player
    [SerializeField] 
    [Range(0, 360)]
    private float viewAngle = 90;    //  View angle of the enemy
    [SerializeField]
    private LayerMask playerMask;    //  Player mask to detect the player by raycasting
    [SerializeField]
    private LayerMask obstacleMask;  //  Obstacule mask to detect the Obstacules by raycasting

    [HideInInspector]
    public bool playerInRange;                  //  Check if the player is in range

    private Transform player;                           //  Position of the player
    private NavMeshAgent navMeshAgent;                  //  Enemy Navmesh componet
    private Animator m_Animator;                        //  Enemy Animator component
    private EnemyHealth enemyHealth;                    //  Enemy health
    private float m_WaitTimer;

    private void Start()
    {
        //  Get the player from the scene
        player = GameObject.FindGameObjectWithTag("Player").transform;

        //  Assign components
        m_Animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();

        //  Initialize Variables
        navMeshAgent.speed = walkSpeed;
        m_WaitTimer = waitTime;

        //  Spawn randomly enemies in the scene
        SpawnEnemyRandom();
    }

    private void Update()
    {
        if (!enemyHealth.IsDeath)
        {
            //  Check if the player is in range
            EnviromentView();
            if (playerInRange)
            {
                //  Chase the player forever
                ChasingPlayer();
            }
            else
            {
                //  Patrol randomly around the position
                PatrolMovement();
            }
            //  Set the speed in the blend tree of the animator
            m_Animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        }
        else
        {
            //  If the enemy is dead disable the navmes agent to not follow the player
            navMeshAgent.speed = walkSpeed;
            navMeshAgent.enabled = false;
        }
    }

    private void SpawnEnemyRandom()
    {
        //  Set the initial position of every enemy in the scena to a random position that hits with the navmesh area
        transform.position = RandomDestination(Vector3.zero, 50.0f);
    }

    private void PatrolMovement()
    {
        //  Set the velocity of the navmesh agent to walk
        navMeshAgent.speed = walkSpeed;

        //  If the enemy arrive to the navmesh detination, then wait for some second
        if (Vector3.Distance(navMeshAgent.destination, transform.position) <= navMeshAgent.stoppingDistance)
        {
            if (m_WaitTimer <= 0)
            {
                //  Set another destination
                navMeshAgent.SetDestination(RandomDestination(transform.position, radiusMovement));
                m_WaitTimer = waitTime;
            }
            else
            {
                m_WaitTimer -= Time.deltaTime;
            }
        }
    }

    private void ChasingPlayer()
    {
        //  Set the speed of the navmesh agent to run speed
        navMeshAgent.speed = runSpeed;
        //  Set the destination of the enemy to the player position
        navMeshAgent.SetDestination(player.position);
    }

    private Vector3 RandomDestination(Vector3 center, float distance)
    {
        //  Get a random position from the center of the object and around a radius
        Vector3 randomPosition = center + Random.insideUnitSphere * distance;
        NavMeshHit hit;

        //  If the random position is outside of the navmesh are, then assign the nearest point of the navmesh area
        NavMesh.SamplePosition(randomPosition, out hit, distance, NavMesh.AllAreas);

        return hit.position;
    }

    private void EnviromentView()
    {
        //  Check if the player is around the view radius of the enemy
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        for (int i = 0; i < playerInRange.Length; i++)
        {
            //  Get the player position in the radius and calculate the direction where the player is
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                //  Get the distance of the player
                float dstToPlayer = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    //  if the raycast does not hit to an obstacules, then the player is in range
                    this.playerInRange = true;
                }
            }
        }
    }
}
