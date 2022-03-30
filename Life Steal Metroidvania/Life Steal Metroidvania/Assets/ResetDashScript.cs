using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetDashScript : MonoBehaviour
{
    public PlayerScript player;
    private float countdown;
    public float activeTime;
    private bool visible = true;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(visible);
        countdown = activeTime;
    }

    // Update is called once per frame
    void Update(){

        // Debug.Log(countdown);

        if (visible == false){
            countdown -= Time.deltaTime;
            if (countdown <= 0){
                visible = true;
                // gameObject.SetActive(visible);
                transform.localScale = new Vector3(1, 1, 1);
                countdown = activeTime;
            }
        } 
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.tag == "Player"){
            player.ResetDash();
            visible = false;
            transform.localScale = new Vector3(0, 0, 0);
            // gameObject.SetActive(visible);
        }
    }
}

