/*  File:       Player.cs
 *  Creator:    Alexander Semenov
 *  Date:       December 2017 
 *  Location:   Brno, Czech Republic
 *  Project:    GRIP Digital Showcase project - Primitive Minecraft Clone
 *  Desc:       Core player script, handles player behavior
 *              such as different modes for destrying and placing blocks
 *  Usage:      Put this into player object and set the values
 *              Create a wireframe Block for block placement preview
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
    // Damage attribute of player's gun
    public float Damage = 1;
    // Range of player's gun
    public int Range = 10;
    
   
    [Header("Unity setup")]
    // Transform of the starting point of laser
    public Transform FirePoint;
    // Camera attached to player object
    public Camera Cam;

    [Header("Block setup")]
    // Blcok to be created by player
    public GameObject BlockPrefab;
    // Block placement preview looks
    public GameObject WireframeBlockPrefab;

    [Header("Laser Attributes")]
    public LineRenderer LineRenderer;
    public ParticleSystem ImpactEffect;

    private GameObject wireframeBlock;
    private GameObject objectHit;

    // Storing information about brick, so it can call ReceiveDamage
    private Block targetBlock;

    private void Start()
    {
        // lineRenderer = GetComponent<LineRenderer>();
        mode = Mode.destroy;
        //  Cursor.lockState = CursorLockMode.Locked;
        wireframeBlock = Instantiate(WireframeBlockPrefab);
        wireframeBlock.SetActive(false);
    }

    private void Update()
    {
        //MovePlayer();
        //MouseLook();
        // Disable laser when it's not used
        LineRenderer.enabled = false;

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
    }
    
    /// <summary>
    /// Creates raycast to try shooting, if it hits target in range, damages the block
    /// </summary>
    private void TryShooting()
    {
        // Create ray towards the center of camera
        RaycastHit hit = AimAtCenterOfCamera();

        if (CheckIfHit(hit))
        {
            objectHit = hit.collider.gameObject;
            // Create lazer towards the point where raycast hit
            Laser(hit.point);

            // If the hit object is Block, continue
            if (objectHit.CompareTag("Block"))
            {
                // Get Brick script component, so it can receive the message
                Block block = objectHit.GetComponent<Block>();

                // Exception handling
                if (block != null)
                {
                    block.ReceiveDamage(Damage);
                }
            }
        }
    }

    /// <summary>
    /// Tries placing block
    /// Places the block it's valid position
    /// </summary>
    private void TryPlacingBlock()
    {
        // Create ray towards the center of camera
        RaycastHit hit = AimAtCenterOfCamera();

        if (CheckIfHit(hit))
        {
            objectHit = hit.collider.gameObject;
            PlaceBlock(BlockPrefab, GetBlockPos(hit));
        }
    }    

    /// <summary>
    /// Create laser towards target
    /// </summary>
    /// <param name="target">Laser goes from firePoint object towards the Vector3 of target</param>
    private void Laser(Vector3 target)
    {
        // If it's not enabled already, enable it and play impact effect
        if (!LineRenderer.enabled)
        {            
            LineRenderer.enabled = true;
            ImpactEffect.Play();
        }
        
        // Set positions for line renderer
        LineRenderer.SetPosition(0, FirePoint.position);
        LineRenderer.SetPosition(1, target);
        Vector3 direction = FirePoint.position - target;
        // Set positions 
        ImpactEffect.transform.rotation = Quaternion.LookRotation(direction);
        ImpactEffect.transform.position = target + direction.normalized * 0.5f;        
    }

    private void WireframeBlockHandling(GameObject wireframeBlock)
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

    /// <summary>
    /// Creates raycast forward from the center of the camera and filters out everything that's not in layer Environment
    /// </summary>
    /// <returns>Return raycast hit information</returns>
    private RaycastHit AimAtCenterOfCamera()
    {
        RaycastHit hit;
        Ray ray = new Ray(Cam.transform.position, Cam.transform.forward);
        Physics.Raycast(ray, out hit, Range, 1 << LayerMask.NameToLayer("Environment"));
        return hit;
    }

    /// <summary>
    /// Edge case handling in case raycast didn't hit anything
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    private bool CheckIfHit(RaycastHit hit)
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

    /// <summary>
    /// Gets correct block position for block placement, so it snaps to grid
    /// </summary>
    /// <param name="hit">Raycast which hits the object</param>
    /// <returns>Returns correct block position snapped to grid</returns>
    private Vector3 GetBlockPos(RaycastHit hit)
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

    /// <summary>
    /// Switches player mode to destroy or build blocks
    /// </summary>
    /// <returns></returns>
    private Mode SwitchModes()
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

    /// <summary>
    /// Creates the block and places it on map
    /// </summary>
    /// <param name="blockPrefab">Block to be created</param>
    /// <param name="blockPos">Position of the block</param>
    private void PlaceBlock(GameObject blockPrefab, Vector3 blockPos)
    {
        GameObject block = Instantiate(blockPrefab, blockPos, Quaternion.identity);
    }    
}