using UnityEngine;

/// <summary>
/// Move an object using velocity
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class MoveWithVelocity : MonoBehaviour
{
    [Tooltip("The speed at which the object is moved")]
    public float speed = 1.0f;

    [Tooltip("Controls the direction of movement")]
    public Transform origin = null;

    private Vector3 inputVelocity = Vector3.zero;
    private Rigidbody rigidBody = null;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ApplyVelocity();
    }

    private void ApplyVelocity()
    {
        Vector3 targetVelocity = inputVelocity * speed;
        targetVelocity = origin.TransformDirection(targetVelocity);

        Vector3 velocityChange = targetVelocity - rigidBody.velocity;
        rigidBody.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    [Header("Invert Controls (Check if reversed)")]
    public bool invertX = false;
    public bool invertY = false;
    public bool invertZ = false;

    public void SetRightVelocity(float value)
    {
        inputVelocity.x = invertX ? -value : value;
    }

    public void SetForwardVelocity(float value)
    {
        inputVelocity.z = invertZ ? -value : value;
    }

    public void SetUpVelocity(float value)
    {
        inputVelocity.y = invertY ? -value : value;
    }

    private void OnValidate()
    {
        if (!origin)
            origin = transform;
    }
}
