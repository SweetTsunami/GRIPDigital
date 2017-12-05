/*
*SCRIPT MADE BY ALEXANDER SEMENOV

*USED FOR HIS OWN PEOJECT
*/
using UnityEngine;

/// <summary>
/// Base player class, to handle player related stuff
/// </summary>
public class Player : MonoBehaviour
{
    // Different modes for player behavior
    enum Mode { destroy, build };
    Mode mode;

    [Header("Character attributes")]
    public float moveSpeed = 5;
    public float jumpSpeed = 8;
    public float gravity = 20;
    // Damage attribute of player's gun
    public float damage = 1;
    // Range of player's gun
    public int range = 10;

    Vector3 moveDir = Vector3.zero;
    Vector2 mouseLook;
    Vector2 mouseSmooth;

    [Header("Mouse setup")]
    public float sensitivity = 5.0f;
    public float smoothing = 2.5f;

    // Transform of the starting point of laser

    [Header("Unity setup")]
    public Transform firePoint;
    public Camera cam;

    [Header("Block setup")]
    public GameObject blockPrefab;
    public GameObject wireframeBlockPrefab;

    private GameObject wireframeBlock;
    private GameObject objectHit;

    [Header("Laser Attributes")]
    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;

    // Storing information about brick, so it can call ReceiveDamage
    private Block targetBlock;

    void Start()
    {
        // lineRenderer = GetComponent<LineRenderer>();
        mode = Mode.destroy;
        //  Cursor.lockState = CursorLockMode.Locked;
        wireframeBlock = Instantiate(wireframeBlockPrefab);
        wireframeBlock.SetActive(false);
    }

    void Update()
    {
        //MovePlayer();
        //MouseLook();
        // Disable laser when it's not used
        lineRenderer.enabled = false;

        if (mode == Mode.build)
        {
            WireframeBlockHandling(wireframeBlock);
            if (Input.GetMouseButtonDown(0))
            {
                TryPlacingBlock();
            }
        }
        if (mode == Mode.destroy)
        {
            // If player pressed / holds left mouse 
            if (Input.GetMouseButton(0))
            {
                TryShooting();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            SwitchModes();
        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //}
    }

    void MovePlayer()
    {
        CharacterController character = GetComponent<CharacterController>();

        if (character.isGrounded)
        {
            moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDir = transform.TransformDirection(moveDir);
            moveDir *= moveSpeed;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDir.y = jumpSpeed;
            }
        }
        moveDir.y -= gravity * Time.deltaTime;
        character.Move(moveDir * Time.deltaTime);
    }

    void MouseLook()
    {
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        mouseSmooth.x = Mathf.Lerp(mouseSmooth.x, mouseDelta.x, 1f / smoothing);
        mouseSmooth.y = Mathf.Lerp(mouseSmooth.y, mouseDelta.y, 1f / smoothing);
        mouseLook += mouseSmooth;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(mouseLook.x, transform.up);
    }

    /// <summary>
    /// Creates raycast to try shooting, if it hits target in range, damages the block
    /// </summary>
    void TryShooting()
    {
        // Create ray towards the center of camera
        RaycastHit hit = AimAtCenterOfCamera();

        if (CheckIfHit(hit))
        {
            objectHit = hit.collider.gameObject;
            // Create lazer towards the point where raycast hit
            Lazer(hit.point);

            // If the hit object is Block, continue
            if (objectHit.CompareTag("Block"))
            {
                // Get Brick script component, so it can receive the message
                Block block = objectHit.GetComponent<Block>();

                // Exception handling
                if (block != null)
                {
                    block.ReceiveDamage(damage);
                }
            }
        }
    }

    void TryPlacingBlock()
    {
        // Create ray towards the center of camera
        RaycastHit hit = AimAtCenterOfCamera();

        if (CheckIfHit(hit))
        {
            objectHit = hit.collider.gameObject;
            PlaceBlock(blockPrefab, GetBlockPos(hit));
        }
    }    

    /// <summary>
    /// Create laser towards target
    /// </summary>
    /// <param name="target">Laser goes from firePoint object towards the Vector3 of target</param>
    void Lazer(Vector3 target)
    {
        // If it's not enabled already, enable it and play impact effect
        if (!lineRenderer.enabled)
        {            
            lineRenderer.enabled = true;
            impactEffect.Play();
        }
        
        // Set positions for line renderer
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target);
        Vector3 direction = firePoint.position - target;
        // Set positions 
        impactEffect.transform.rotation = Quaternion.LookRotation(direction);
        impactEffect.transform.position = target + direction.normalized * 0.5f;        
    }

    void WireframeBlockHandling(GameObject wireframeBlock)
    {
        RaycastHit hit = AimAtCenterOfCamera();

        if (CheckIfHit(hit))
        {
            wireframeBlock.SetActive(true);
            objectHit = hit.collider.gameObject;
            wireframeBlock.transform.position = GetBlockPos(hit);
        }
        else
        {
            wireframeBlock.SetActive(false);
        }
    }

    RaycastHit AimAtCenterOfCamera()
    {
        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Physics.Raycast(ray, out hit, range, 1 << LayerMask.NameToLayer("Environment"));
        return hit;
    }

    bool CheckIfHit(RaycastHit hit)
    {
        if (hit.collider != null)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    Vector3 GetBlockPos(RaycastHit hit)
    {
        Vector3 blockPos = Vector3.zero;

        float xDiff = hit.point.x - hit.transform.position.x;
        float yDiff = hit.point.y - hit.transform.position.y;
        float zDiff = hit.point.z - hit.transform.position.z;

        if (Mathf.Abs(xDiff) == 0.5f)
        {
            blockPos = hit.transform.position + (Vector3.right * xDiff) * 2;
        }
        else if (Mathf.Abs(yDiff) == 0.5f)
        {
            blockPos = hit.transform.position + (Vector3.up * yDiff) * 2;
        }
        else if (Mathf.Abs(zDiff) == 0.5f)
        {
            blockPos = hit.transform.position + (Vector3.forward * zDiff) * 2;
        }
        return blockPos;
    }

    Mode SwitchModes()
    {
        if (mode == Mode.destroy)
        {
            return mode = Mode.build;
        }
        else
        {
            wireframeBlock.SetActive(false);
            return mode = Mode.destroy;
        }
    }

    void PlaceBlock(GameObject blockPrefab, Vector3 blockPos)
    {
        GameObject block = Instantiate(blockPrefab, blockPos, Quaternion.identity);
    }    
}