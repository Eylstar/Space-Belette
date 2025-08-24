using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    void Update()
    {
        transform.position = target.position;
    }
}
