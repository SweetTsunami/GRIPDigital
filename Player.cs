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
    // Damage attribute of player's gun
    public float damage = 1;
    // Range of player's gun
    public int range = 10;
    // Transform of the starting point of laser
    public Transform firePoint;

    public Camera cam;

    [Header("Laser Attributes")]
    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;    

    // Storing information about brick, so it can call ReceiveDamage
    private Block targetBlock;
    
    void Update()
    {
        // Disable laser when it's not used
        lineRenderer.enabled = false;

        // If player pressed / holds left mouse 
        if (Input.GetMouseButton(0))
        {
            TryShooting();
        }
    }

    /// <summary>
    /// Creates raycast to try shooting, if it hits target in range, damages the block
    /// </summary>
    void TryShooting()
    {
        // Create ray towards the center of camera
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // If raycast hits something and it's within range of gun
        if (Physics.Raycast(ray, out hit) && (Vector3.Distance(transform.position, hit.point) < range))
        {
            // Create lazer towards the point where raycast hit
            Lazer(hit.point);
            // Remember the hit object
            GameObject objectHit = hit.collider.gameObject;

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
}
