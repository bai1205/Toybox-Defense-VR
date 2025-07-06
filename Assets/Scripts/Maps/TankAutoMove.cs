using UnityEngine;
using UnityEngine.AI;

public class TankAutoMove : MonoBehaviour
{
    public Transform goal; // aim in here
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(goal.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tower")) // Tag Tower
        {
            Debug.Log("Tank arrived");
            
        }
    }
}

