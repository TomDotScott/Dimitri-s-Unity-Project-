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

        else if (Input.GetButton(GameConstants.GRAPPLE))
        {
// TODO Call rope to grapple point, pull player to grapple point 
        }

        else if (Input.GetButtonUp(GameConstants.GRAPPLE))
        {
            isFiring = false;
            tether.ResetTether();
        }

        tether.gameObject.SetActive(isFiring);
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
            grapplePoint.position = hit.point;
            return true;
        }
        return false;
    }
}
