using System;
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

    Vector3 campfireRespawnPosition;

    // Start is called before the first frame update
    void Start(){
        if (instance == null)
        {
            instance = this;
        }
        player = GameObject.FindObjectOfType<PlayerScript>();
        UpdateHealthBar();
    }

    public void SetCampfireRespawnPosition(Vector3 position)
    {
        campfireRespawnPosition = position;
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
        if (player.TouchingLava || player.TouchingSpike){
            RespawnPlayer(FindNearestLavaRespawnPoint());
        }
    }


    void UpdateHealthBar(){
    healthBar.value = player.GetPlayerHealth() / player.GetMaxPlayerHealth();
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public PlayerScript GetPlayer()
    {
        return player;
    }

    Vector2 FindNearestLavaRespawnPoint(){
        // TODO: ACTUALLY IMPLEMENT THIS
        return lavaRespawnPoints[0].transform.position;

    }

    public IEnumerator OnPlayerDeath()
    {
        yield return new WaitForSeconds(0.25f);
        RespawnPlayer(campfireRespawnPosition);
    }

    private void RespawnPlayer(Vector3 respawnPosition)
    {
        player.Teleport(respawnPosition);
        player.SetAirState(PlayerScript.eAerialState.Grounded);
        player.TouchingSpike = false;
        player.TouchingLava = false;
        player.IsPlayerDead = false;
    }
}

        
    

