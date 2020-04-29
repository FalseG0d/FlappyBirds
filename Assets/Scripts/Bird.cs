using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class Bird : MonoBehaviour
{
    public event EventHandler OnDeath;
    public event EventHandler OnStart;
    Rigidbody2D birdRigidbody2D;
    static Bird instance;

    State state;
    enum State{
      Waiting,
      Alive,
      Dead
    };
    const float JUMP_AMOUNT = 100f;
    void Awake()
    {
        instance=this;
        birdRigidbody2D = GetComponent<Rigidbody2D>();
        birdRigidbody2D.bodyType=RigidbodyType2D.Static;
        state=State.Waiting;
    }
    public static Bird GetInstance(){
      return instance;
    }
    // Update is called once per frame
    void Update()
    {
        switch(state){
          default:
          case State.Waiting:
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                state=State.Alive;
                birdRigidbody2D.bodyType=RigidbodyType2D.Dynamic;
                if(OnStart!=null)OnStart(this,EventArgs.Empty);
                Jump();
            }
          break;
          case State.Alive:
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Jump();
            }
          break;
          case State.Dead:
          break;
        }
    }

    void Jump()
    {
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        SoundManager.PlaySound(SoundManager.Sound.BirdDead);
        birdRigidbody2D.bodyType=RigidbodyType2D.Static;

        if(OnDeath!=null){
          OnDeath(this,EventArgs.Empty);
        }
    }
}
