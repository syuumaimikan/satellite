using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float smoothSpeed = 5f;

    [SerializeField] private Vector3 offset =
        new Vector3(0f, 1f, -10f);

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition =
            target.position + offset;

        Vector3 smoothPosition =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                smoothSpeed * Time.deltaTime
            );

        transform.position = smoothPosition;
    }
}