using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CraneController : MonoBehaviour
{
    [Header("Crane Mechanical Parts")]
    [Tooltip("The main bridge that moves forward/backward (Z axis).")]
    public Transform gantry; 
    [Tooltip("The cart on the bridge that moves left/right (X axis).")]
    public Transform trolley; 
    [Tooltip("The wire/block that lowers the magnet (Y axis).")]
    public Transform hoist; 
    [Tooltip("The magnet itself that rotates (Y axis rotation).")]
    public Transform magnet; 

    [Header("Physical Controls")]
    [Tooltip("Hinge Joint of the Move Vertical lever.")]
    public HingeJoint verticalLever; 
    [Tooltip("Hinge Joint of the Move Lateral lever.")]
    public HingeJoint lateralLever;

    [Header("Movement Limits")]
    public float minZ = -10f;
    public float maxZ = 10f;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = 0f;
    public float maxY = 10f;

    [Header("Speeds")]
    public float moveSpeed = 2f;
    public float hoistSpeed = 1.5f;
    public float rotationSpeed = 100f;
    
    [Header("Box Management")]
    [Tooltip("The Box prefab to spawn.")]
    public GameObject boxPrefab;
    [Tooltip("Where new boxes should be dropped from.")]
    public Transform boxSpawnPoint;
    public float dumpInterval = 0.5f;
    private bool isDumping = false;
    private float dumpTimer = 0f;

    [Header("Console Adjustment")]
    public float consoleMoveSpeed = 0.5f;
    private bool isConsoleMovingUp = false;
    private bool isConsoleMovingDown = false;

    // Joystick Input States
    private float gantryInput = 0f;
    private float trolleyInput = 0f;
    private float hoistInput = 0f;
    private float targetRotationAngle = 0f;

    private void Update()
    {
        // Continuous Box Dumping
        if (isDumping)
        {
            dumpTimer -= Time.deltaTime;
            if (dumpTimer <= 0f)
            {
                DumpSingleBox();
                dumpTimer = dumpInterval;
            }
        }

        // 0. Console Height Adjustment
        if (isConsoleMovingUp)
        {
            transform.Translate(Vector3.up * consoleMoveSpeed * Time.deltaTime, Space.World);
        }
        if (isConsoleMovingDown)
        {
            transform.Translate(Vector3.down * consoleMoveSpeed * Time.deltaTime, Space.World);
        }

        // 1. Move Gantry (Z Axis) controlled by Joystick_Move
        if (gantry != null)
        {
            if (Mathf.Abs(gantryInput) > 0.1f)
            {
                MoveTarget(gantry, new Vector3(0, 0, gantryInput * moveSpeed * Time.deltaTime), minZ, maxZ, "z");
            }
        }

        // 2. Move Trolley (X Axis) controlled by Joystick_Raise X
        if (trolley != null)
        {
            if (Mathf.Abs(trolleyInput) > 0.1f)
            {
                MoveTarget(trolley, new Vector3(trolleyInput * moveSpeed * Time.deltaTime, 0, 0), minX, maxX, "x");
            }
        }

        // 3. Hoist (Y Axis) controlled by Joystick_Raise Y
        if (hoist != null)
        {
            if (Mathf.Abs(hoistInput) > 0.1f)
            {
                MoveTarget(hoist, new Vector3(0, hoistInput * hoistSpeed * Time.deltaTime, 0), minY, maxY, "y");
            }
        }

        // 4. Magnet Rotation (Y Axis) controlled by Knob
        if (magnet != null)
        {
            if (Mathf.Abs(targetRotationAngle - magnet.localEulerAngles.y) > 0.1f)
            {
                Quaternion targetRot = Quaternion.Euler(0, targetRotationAngle, 0);
                Rigidbody rb = magnet.GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic)
                {
                    rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, rotationSpeed * Time.deltaTime));
                }
                else
                {
                    magnet.localRotation = Quaternion.RotateTowards(magnet.localRotation, targetRot, rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void MoveTarget(Transform target, Vector3 localDelta, float min, float max, string axis)
    {
        Vector3 localPos = target.localPosition;
        localPos += localDelta;
        
        if (axis == "x") localPos.x = Mathf.Clamp(localPos.x, min, max);
        if (axis == "y") localPos.y = Mathf.Clamp(localPos.y, min, max);
        if (axis == "z") localPos.z = Mathf.Clamp(localPos.z, min, max);

        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            rb.MovePosition(target.parent != null ? target.parent.TransformPoint(localPos) : localPos);
        }
        else
        {
            target.localPosition = localPos;
        }
    }

    // --- Public Methods to be called by XR UI ---

    // Called by Joystick_Move (On Y Value Change)
    public void SetVerticalMovement(float value)
    {
        gantryInput = value;
    }

    // Called by Joystick_Raise (On X Value Change)
    public void SetLateralMovement(float value)
    {
        trolleyInput = value;
    }

    // Called by Joystick_Raise (On Y Value Change)
    public void SetHoistMovement(float value)
    {
        hoistInput = value;
    }

    // Called by Up Button (Hover Enter -> true, Hover Exit -> false)
    public void SetHoistingUp(bool value)
    {
        isConsoleMovingUp = value;
    }

    // Called by Down Button (Hover Enter -> true, Hover Exit -> false)
    public void SetHoistingDown(bool value)
    {
        isConsoleMovingDown = value;
    }

    // Called by Magnet Rotate Knob (Value Changed Event)
    public void SetMagnetRotation(float value)
    {
        // XR Knob can output 0 to 1 value. We map it to 0-360 degrees.
        // If your Knob is set to output direct Angles, change this line to targetRotationAngle = value;
        targetRotationAngle = value * 360f;
    }

    // Called by Clear Boxes Button (Select Enter Event)
    public void ClearBoxes()
    {
        GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
        foreach (GameObject box in boxes)
        {
            Destroy(box);
        }
        Debug.Log("CraneController: All boxes cleared.");
    }

    // Called by Dump Boxes Button (Hover Enter -> true, Hover Exit -> false)
    public void SetDumping(bool value)
    {
        isDumping = value;
        if (value) dumpTimer = 0f; // Instantly drop the first box when touched
    }

    private void DumpSingleBox()
    {
        if (boxPrefab != null && boxSpawnPoint != null)
        {
            Instantiate(boxPrefab, boxSpawnPoint.position, Random.rotation);
        }
        else
        {
            Debug.LogWarning("CraneController: Cannot dump boxes because Box Prefab or Spawn Point is missing!");
        }
    }
}
