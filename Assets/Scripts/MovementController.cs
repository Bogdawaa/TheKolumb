using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using Unity.VisualScripting;
using System.Runtime.Serialization;

public class MovementController: MonoBehaviour
{
	private NavMeshAgent agent;

	[Header("Movement Settings")]
	public float moveSpeed = 10f;

	[Header("Input settings")]
	[SerializeField] float sampleDistanse = 0.5f;
	[SerializeField] LayerMask groundLayer;

	public static event System.Action<Vector3> OnGroundTouched;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
		agent.speed = moveSpeed;
    }

    void Update()
	{
		if (Mouse.current.leftButton.wasPressedThisFrame)
		{
			Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
 
    		RaycastHit hit;
       
			if (Physics.Raycast(ray, out hit, groundLayer))
			{
				if (NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, sampleDistanse, NavMesh.AllAreas))
				{
					agent.SetDestination(navMeshHit.position);
					OnGroundTouched?.Invoke(navMeshHit.position);
				} else
				{
					Debug.Log("Clicked on not walkable position");
				}
			}
    	}	
	}
}
