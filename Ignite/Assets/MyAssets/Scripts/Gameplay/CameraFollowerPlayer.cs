﻿using System.Collections;
using UnityEngine;

public class CameraFollowerPlayer : MonoBehaviour {

    public Transform target;
    public Transform startTarget;

    public float smoothSpeed = 0.125f;
    public float smoothStartSpeed = 0.00125f;
    public Vector3 offset;
    public bool look = false;
    public float timeUntilStart = 5f;

    private Vector3 inPos;
    public bool start = true;

    private void Start()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        inPos = smoothedPosition;
        transform.position = startTarget.position;

        StartCoroutine(wait(timeUntilStart));
    }


    void LateUpdate()
    {
        

        if (!start)
        {
            Vector3 desiredPosition = target.position + offset;
            if (Input.GetKey(KeyCode.Mouse1))
            {
                float mx = ((Input.mousePosition.x - Screen.currentResolution.width / 2) * 2) / Screen.currentResolution.width;
                float my = ((Input.mousePosition.y - Screen.currentResolution.height / 2) * 2) / Screen.currentResolution.height;
                PlayerController pc = target.GetComponent<PlayerController>();
                if(pc == null)
                {
                    Debug.LogWarning("No hay PlayerController en target!");
                    return;
                }
                float multi = pc.camDistanceMultiplier;
                desiredPosition = desiredPosition + new Vector3(mx * multi, 3, my * multi);
            }
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            if (look)
            {
                transform.LookAt(target);
            }
        } else
        {
            
            // Starting     
            Vector3 smoothedPosiotion = Vector3.Lerp(transform.position, target.position + offset, smoothStartSpeed);
            transform.position = smoothedPosiotion;
        }
        
        
    }

    IEnumerator wait(float seconds)
    {
        start = true;
        yield return new WaitForSeconds(seconds);
        start = false;
    }

}
