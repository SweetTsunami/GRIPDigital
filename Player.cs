/*
*SCRIPT MADE BY ALEXANDER SEMENOV

*USED FOR HIS OWN PEOJECT
*/
using UnityEngine;

public class Player : MonoBehaviour
{

    public float damage = 1;
    public int range = 10;
    public Transform firePoint;
    public Camera camera;

    [Header("Lazer Attributes")]
    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;

    //public Light impactLight;

    private Brick targetBrick;

    void Start()
    {
    }

    void Update()
    {
        // target = new Vector3(Screen.width, Screen.height);

        lineRenderer.enabled = false;
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);            

            if (Physics.Raycast(ray, out hit) && (Vector3.Distance(transform.position, hit.point) < range))
            {
                Lazer(hit.point);
                GameObject objectHit = hit.collider.gameObject;
                if (objectHit.CompareTag("Brick"))
                {
                    Brick brick = objectHit.GetComponent<Brick>();
                    if (brick != null)
                    {
                        brick.ReceiveDamage(damage);
                    }
                }
            }
        }
    }

    void Lazer(Vector3 target)
    {
        if (!lineRenderer.enabled)
        {            
            lineRenderer.enabled = true;
            impactEffect.Play();
            //impactLight.enabled = true;
        }

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target);
        Vector3 direction = firePoint.position - target;
        impactEffect.transform.rotation = Quaternion.LookRotation(direction);
        impactEffect.transform.position = target + direction.normalized * 0.5f;        
    }
}
