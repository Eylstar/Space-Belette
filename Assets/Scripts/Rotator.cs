using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] GameObject obj;
    [SerializeField] Vector3 rotationSpeed = new(0, 0, 10f);
    
    private void Update()
    {
        RotateObject();
    }
    
    private void RotateObject()
    {
        obj.transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
