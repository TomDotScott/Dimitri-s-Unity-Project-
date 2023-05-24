using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookLauncher : MonoBehaviour
{
    [SerializeField] private Transform grapplePoint;
    [SerializeField] private float maxGrappleRange;
    [SerializeField] private LayerMask grappleLayers;
    private bool isFiring;
    [SerializeField] private TetherScript tether;
    [SerializeField] private PlayerScript player;
    [SerializeField] private float pullForce;
    [SerializeField] private float minimumPullDistance;
    [SerializeField] private float minimumMagnitude;
    [SerializeField] private float maximumMagnitude;
    private bool hitWall;


    // Start is called before the first frame update
    void Start()
    {
        isFiring = false;
        tether.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(GameConstants.GRAPPLE))
        {
            isFiring = Fire();
        }

        else if (Input.GetButtonUp(GameConstants.GRAPPLE))
        {
            ResetHook();
        }

        tether.gameObject.SetActive(isFiring);
    }

    private void FixedUpdate()
    {
        if (isFiring && tether.GetTetherState() == TetherScript.TetherState.Straight)
        {
            PullTowardsTarget();
        }
    }



    private bool Fire()
    {
        PlayerScript.FacingDirection playerDirection = player.GetFacingDirection();
        Vector2 shootDirection; 
        if (playerDirection == PlayerScript.FacingDirection.Left)
        {
            shootDirection = Vector2.left;
        }
        else
        {
            shootDirection = Vector2.right;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, shootDirection, maxGrappleRange, grappleLayers);
        Debug.DrawRay(transform.position, shootDirection * 1000, Color.green, 1);
        if (Vector2.Distance(hit.point, transform.position) < maxGrappleRange && hit.collider != null)
        {
            hitWall = hit.transform.CompareTag(GameConstants.WALL_TAG);
            grapplePoint.position = hit.point;
            return true;
        }
        return false;
    }

    private void PullTowardsTarget()
    {
        Vector2 direction = grapplePoint.position - transform.position;
        float distance = direction.magnitude;
        float forceMagnitude = Mathf.Clamp(distance * pullForce, minimumMagnitude, maximumMagnitude);
        Debug.Log(distance * pullForce);
        Vector2 force = direction.normalized * forceMagnitude;

        Rigidbody2D playerRigidbody = player.gameObject.GetComponent<Rigidbody2D>();
        playerRigidbody.AddForce(force);
        if (distance < minimumPullDistance && hitWall == false)
        {
            ResetAndStop();
        }
    }

    private void ResetHook()
    {
        isFiring = false;
        tether.ResetTether();
    }

    private void ResetAndStop()
    {
        ResetHook();
        Rigidbody2D playerRigidbody = player.gameObject.GetComponent<Rigidbody2D>();
        playerRigidbody.velocity = Vector2.zero;
    }
}
