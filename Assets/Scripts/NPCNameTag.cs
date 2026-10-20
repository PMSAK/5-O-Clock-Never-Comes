using UnityEngine;

public class NPCNameTag : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;

    private void Start()
    {
        FindPlayerCamera();
    }

    private void LateUpdate()
    {
        FaceCamera();
    }

    private void FindPlayerCamera()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void FaceCamera()
    {
        if (playerCamera == null)
            return;

        Vector3 direction =
            transform.position - playerCamera.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f, 0f);
        }
    }
}


