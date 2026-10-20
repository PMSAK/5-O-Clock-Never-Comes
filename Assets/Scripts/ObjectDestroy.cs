
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ObjectDestroy : MonoBehaviour
{
    [Header("Message")]
    [SerializeField] private GameObject leakMessage;
    [SerializeField] private float messageDuration = 3f;

    [Header("NPCs")]
    [SerializeField] private NavMeshAgent[] npcAgents;
    [SerializeField] private Transform inspectionPoint;
    [SerializeField] private float npcMovementDelay = 1.5f;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player"))
            return;

        triggered = true;

        if (leakMessage != null)
        {
            leakMessage.SetActive(true);
            StartCoroutine(HideMessage());
        }

        StartCoroutine(DelayNpcMovement());
    }

    private IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(messageDuration);

        if (leakMessage != null)
            leakMessage.SetActive(false);
    }

    private IEnumerator DelayNpcMovement()
    {
        yield return new WaitForSeconds(npcMovementDelay);

        if (inspectionPoint == null)
        {
            Debug.LogWarning(
                "ObjectDestroy: Assign an Inspection Point!"
            );
            yield break;
        }

        if (npcAgents == null || npcAgents.Length == 0)
        {
            Debug.LogWarning(
                "ObjectDestroy: No NPCs assigned!"
            );
            yield break;
        }

        foreach (NavMeshAgent agent in npcAgents)
        {
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.SetDestination(inspectionPoint.position);
            }
        }
    }
}
