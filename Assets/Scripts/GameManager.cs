using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public List<GameObject> respawnPoints;
    public List<GameObject> lavaRespawnPoints;
    public PlayerScript player;
    public Slider healthBar;
    public static GameManager GetInstance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start(){
        if (instance == null)
        {
            instance = this;
        }
        player = GameObject.FindObjectOfType<PlayerScript>();
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update(){

#if DEBUG
        if (Input.GetKeyDown(KeyCode.T)){
            if (Time.timeScale == 1.0f){
            Time.timeScale = 0.5f;

            }

            else
            {
                Time.timeScale = 1.0f;
            }
        }
#endif

        UpdateHealthBar();
        if (player.TouchingLava){
            player.Teleport(FindNearestLavaRespawnPoint());
            player.SetAirState(PlayerScript.eAerialState.Grounded);
            player.TouchingLava = false;
        } else if (player.TouchingSpike)
        {
            player.Teleport(FindNearestLavaRespawnPoint());
            player.SetAirState(PlayerScript.eAerialState.Grounded);
            player.TouchingSpike = false;
        }
    }


    void UpdateHealthBar(){
    healthBar.value = player.GetPlayerHealth() / player.GetMaxPlayerHealth();
    }

    public Vector3 getPlayerPosition()
    {
        return player.transform.position;
    }

    Vector2 FindNearestLavaRespawnPoint(){
        // TODO: ACTUALLY IMPLEMENT THIS
        return lavaRespawnPoints[0].transform.position;

    }
}

        
    

