using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Camera follow speed
    public float followSpeed = 2f;
    public Transform target;

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position;
            targetPosition.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
