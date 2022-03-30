using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Vector2> respawnPoints;
    public PlayerScript player;
    public Slider healthBar;

    // Start is called before the first frame update
    void Start(){
        player = GameObject.FindObjectOfType<PlayerScript>();
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update(){
        UpdateHealthBar();
    }


    void UpdateHealthBar(){
    healthBar.value = player.GetPlayerHealth() / player.GetMaxPlayerHealth();
    }
}

        
    

