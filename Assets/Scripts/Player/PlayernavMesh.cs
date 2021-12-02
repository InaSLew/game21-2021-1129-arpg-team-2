using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayernavMesh : MonoBehaviour
{

    [SerializeField] private Vector3Value playerPosition;
    [SerializeField] private Weapon weapon;
    [SerializeField] private GameObjectValue target;
    
    private NavMeshAgent navMeshAgent;
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerPosition.Vector3 = transform.position;
    }

    private void Update()
    {
        //navMeshAgent.destination = movePositionTransform.position;
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (hit.transform.CompareTag("Destructible") || hit.transform.CompareTag("Gate"))
                {
                    Debug.Log("Moving towards destructible stuff");
                    var position = hit.transform.position;
                    navMeshAgent.destination = position;
                    navMeshAgent.stoppingDistance = weapon.Range;
                    target.Value = hit.transform.gameObject;
                }
                else if (SetDestination(hit.transform.position) || hit.transform.CompareTag("Wall"))
                {
                    navMeshAgent.destination = hit.point;
                    navMeshAgent.stoppingDistance = 1;
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
}