﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public InternalPlayerController internalPlayer;
    [Space]
    public GameObject bullet;
    public Vector3 bulletOffset;
    public GameObject weapon;
    
    private int cargador;
    private NetworkStartPosition[] spawnpoints;

    void Start()
    {
        cargador = internalPlayer.cargador;
        if (isLocalPlayer)
        {
            spawnpoints = FindObjectsOfType<NetworkStartPosition>();
            // Pone al jugador local en verde xd;
            internalPlayer.GetComponent<Renderer>().material.color = Color.green;
        } else
        {
            // I si no lo es en rojo xd
            internalPlayer.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public void RequestCmdFire()
    {
        CmdFire();
    }

    [Command]
    public void CmdFire()
    {
        Quaternion q = weapon.transform.rotation;
        var bulleti = Instantiate(bullet, weapon.transform.position, q);
        Transform t = bulleti.transform;
        t.Translate(bulletOffset);

        NetworkServer.Spawn(bulleti);

        bulleti.SetActive(true);
        
        // Destroy the bullet after 2 seconds
        Destroy(bulleti, 2.0f);
    }

    [ClientRpc]
    public void RpcGetDamage(int damage)
    {
        // Hacer daño SyncVar bla bla bla...
        Debug.Log("jj");
        internalPlayer.GetComponent<Shaker>().GetDamageAnimation();
        internalPlayer.damageThisPlayer(damage);
    }

    [ClientRpc]
    public void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            Vector3 spawnPoint = Vector3.zero;

            if(spawnpoints != null && spawnpoints.Length > 0)
            {
                spawnPoint = spawnpoints[Random.Range(0, spawnpoints.Length)].transform.position;
            }

            transform.position = spawnPoint;
        }

        internalPlayer.gameObject.SetActive(true);

        internalPlayer.GetComponent<PlayerPropieties>().health = PlayerPropieties.maxHealth;
        internalPlayer.GetComponent<PlayerPropieties>().shield = PlayerPropieties.maxShield;
    }
    void Update()
    {
        cargador = internalPlayer.cargador;
        internalPlayer.isLocal = isLocalPlayer;
    }

    public void respawn()
    {
        StartCoroutine(respawnThisPlayer());
    }


    IEnumerator respawnThisPlayer()
    {
        internalPlayer.reloadingBar.gameObject.SetActive(true);
        internalPlayer.reloadingBarInfo.gameObject.SetActive(true);
        internalPlayer.fill.color = new Color(0, 255, 0, 200);

        internalPlayer.reloadingBarInfo.text = "Respawning...";

        for (float i = 0; i < 7; i += 0.01f)
        {
            yield return new WaitForSeconds(0.01f);
            float progress = Mathf.Clamp01(i / 10);
            internalPlayer.reloadingBar.value = progress;
        }

        internalPlayer.reloadingBarInfo.gameObject.SetActive(false);
        internalPlayer.reloadingBar.gameObject.SetActive(false);

        RpcRespawn();
    }

}
