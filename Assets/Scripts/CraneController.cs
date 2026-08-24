using System.Collections.Generic;
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
    [Tooltip("Parent of the boxes that are already placed in the scene at start (not crane-spawned). Their positions get restored on Reset.")]
    public Transform originalBoxesRoot;
    public float dumpInterval = 0.5f;
    private bool isDumping = false;
    private float dumpTimer = 0f;
    private readonly List<GameObject> spawnedBoxes = new List<GameObject>();

    [Header("Console Adjustment")]
    public float consoleMoveSpeed = 0.5f;
    private bool isConsoleMovingUp = false;
    private bool isConsoleMovingDown = false;

    // Joystick Input States
    private float gantryInput = 0f;
    private float trolleyInput = 0f;
    private float hoistInput = 0f;
    private float targetRotationAngle = 0f;

    // Starting state, cached once so Reset can return everything without reloading the scene
    private Vector3 consoleStartPosition;
    private Vector3 gantryStartLocalPos;
    private Vector3 trolleyStartLocalPos;
    private Vector3 hoistStartLocalPos;
    private Quaternion magnetStartLocalRot;
    private readonly List<Transform> originalBoxes = new List<Transform>();
    private readonly List<Vector3> originalBoxStartPositions = new List<Vector3>();
    private readonly List<Quaternion> originalBoxStartRotations = new List<Quaternion>();

    private void Start()
    {
        consoleStartPosition = transform.position;
        if (gantry != null) gantryStartLocalPos = gantry.localPosition;
        if (trolley != null) trolleyStartLocalPos = trolley.localPosition;
        if (hoist != null) hoistStartLocalPos = hoist.localPosition;
        if (magnet != null) magnetStartLocalRot = magnet.localRotation;

        if (originalBoxesRoot != null)
        {
            foreach (Transform box in originalBoxesRoot)
            {
                originalBoxes.Add(box);
                originalBoxStartPositions.Add(box.position);
                originalBoxStartRotations.Add(box.rotation);
            }
        }
    }

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
    // Only removes boxes the crane has dumped - the boxes placed in the scene at start are left alone.
    public void ClearBoxes()
    {
        foreach (GameObject box in spawnedBoxes)
        {
            if (box != null) Destroy(box);
        }
        spawnedBoxes.Clear();
        Debug.Log("CraneController: Crane-spawned boxes cleared.");
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
            GameObject box = Instantiate(boxPrefab, boxSpawnPoint.position, Random.rotation);
            spawnedBoxes.Add(box);
        }
        else
        {
            Debug.LogWarning("CraneController: Cannot dump boxes because Box Prefab or Spawn Point is missing!");
        }
    }

    // Called by the Reset button. Returns the crane, magnet and boxes to how they were
    // at the start of the session, without reloading the scene.
    public void ResetCrane()
    {
        gantryInput = 0f;
        trolleyInput = 0f;
        hoistInput = 0f;
        targetRotationAngle = 0f;
        isDumping = false;
        isConsoleMovingUp = false;
        isConsoleMovingDown = false;
        dumpTimer = 0f;

        transform.position = consoleStartPosition;

        ResetPart(gantry, gantryStartLocalPos);
        ResetPart(trolley, trolleyStartLocalPos);
        ResetPart(hoist, hoistStartLocalPos);

        if (magnet != null)
        {
            Rigidbody magnetRb = magnet.GetComponent<Rigidbody>();
            if (magnetRb != null)
            {
                magnetRb.velocity = Vector3.zero;
                magnetRb.angularVelocity = Vector3.zero;
            }
            magnet.localRotation = magnetStartLocalRot;
        }

        for (int i = 0; i < originalBoxes.Count; i++)
        {
            Transform box = originalBoxes[i];
            if (box == null) continue; // may have been destroyed via the magnet or another interaction

            Rigidbody boxRb = box.GetComponent<Rigidbody>();
            if (boxRb != null)
            {
                // Briefly go kinematic so the physics engine doesn't try to resolve any
                // overlap at the old spot before the teleport takes effect.
                boxRb.velocity = Vector3.zero;
                boxRb.angularVelocity = Vector3.zero;
                boxRb.isKinematic = true;
            }

            box.position = originalBoxStartPositions[i];
            box.rotation = originalBoxStartRotations[i];
            box.gameObject.SetActive(true);

            if (boxRb != null)
            {
                boxRb.isKinematic = false;
            }
        }

        ClearBoxes();
    }

    private void ResetPart(Transform part, Vector3 startLocalPos)
    {
        if (part == null) return;

        Rigidbody rb = part.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        part.localPosition = startLocalPos;
    }
}
