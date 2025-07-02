using UnityEngine;

/// <summary>
/// Contains specifications of a certain ship model
/// </summary>
[CreateAssetMenu(menuName = "ModelInfo")]
public class ModelInfo : ScriptableObject
{
    [Tooltip("Ship model name")]
    public string ModelName;
    [Tooltip("Distance at which the camera follows the ship (for different ship sizes)")]
    public float CameraOffset;
    [Tooltip("X: Linear thrust\nY: Vertical thrust\nZ: Longitudinal Thrust")]
    public Vector3 LinearForce;
    [Tooltip("X: Angular thrust:\nX: Pitch\nY: Yaw\nZ: Roll")]
    public Vector3 AngularForce;
}
