using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float maxGroundDistance;
    [SerializeField] private float jumpHeight;
    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGrounded())
        {
            Debug.Log("Grounded;)");
            if (Input.GetButtonDown(GameConstants.JUMP))
            {
                rb.velocity =
                     new Vector2(rb.velocity.x, jumpHeight);
            }
        } else {
            Debug.Log("Airborne");
        }
    }

    private bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector3.down * maxGroundDistance, Color.magenta, 0.25f);
        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(transform.position, Vector3.down, maxGroundDistance);
        return raycastHits.Any(hit => hit.collider.gameObject.CompareTag("Ground"));
    }
}
