using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayernavMesh : MonoBehaviour
{
    
    [SerializeField] private Vector3Value playerPosition;
    [SerializeField] private Vector3Value playerStartPosition;
    [SerializeField] private Weapon weapon;
    [SerializeField] private GameObjectValue movementTarget;
    [SerializeField] private FloatValue stopFrontOfGateDistance;
    [SerializeField] private BooleanValue attackOnGoing;
    [SerializeField] private GameEvent attackStop;
    private NavMeshAgent navMeshAgent;
    private RaycastHit hit;
    Animator animator;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerPosition.Vector3 = transform.position;
        movementTarget.Value = null;
    }

    private void Start()
    {
        playerStartPosition.Vector3 = transform.position;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
      
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            attackOnGoing.BoolValue = false;
            attackStop.Raise();
            movementTarget.Value = null;
            
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                Debug.Log("We hit: "+hit.transform.gameObject);
                if (hit.transform.CompareTag("Destructible") || hit.transform.CompareTag("Enemy"))
                {
                    var position = hit.transform.position;
                    navMeshAgent.destination = position;
                    navMeshAgent.stoppingDistance = weapon.Range;
                    movementTarget.Value = hit.transform.gameObject;
                }
                else if (hit.transform.CompareTag("Gate"))
                {
                    var position = hit.transform.position;
                    navMeshAgent.destination = position;
                    navMeshAgent.stoppingDistance = stopFrontOfGateDistance.InitialValue;
                    movementTarget.Value = hit.transform.gameObject;
                }
                else if (SetDestination(hit.transform.position) || hit.transform.CompareTag("Wall"))
                {
                    navMeshAgent.destination = hit.point;
                    navMeshAgent.stoppingDistance = 1;
                    movementTarget.Value = null;
                }
            }
        }
        
        playerPosition.Vector3 = transform.position;
    }

    private bool SetDestination(Vector3 targetDestination)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetDestination, out hit, 1f, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
            return true;
        }
        return false;
    }

    public void ResetToStartPosition()
    {
        navMeshAgent.Warp(playerStartPosition.Vector3);
    }
}