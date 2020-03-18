using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class Bird : MonoBehaviour
{
    Rigidbody2D birdRigidbody2D;
    const float JUMP_AMOUNT = 100f;
    void Awake()
    {
        birdRigidbody2D = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) 
        {
            Jump();
        }
        
    }

    void Jump()
    {
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //CMDebug.TextPopMouse("Dead!!");

    }
}
