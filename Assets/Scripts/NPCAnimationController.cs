using UnityEngine;
using UnityEngine.AI;

public class NPCAnimationController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private float walkThreshold = 0.1f;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponentInParent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (agent == null || animator == null)
            return;

        bool isWalking =
            agent.velocity.magnitude > walkThreshold &&
            agent.remainingDistance > agent.stoppingDistance;

        animator.SetBool("IsWalking", isWalking);
    }
}

