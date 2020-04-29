using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameAssets : MonoBehaviour
{

    static GameAssets instance;

    public static GameAssets GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
        menuHighScore.text="Highscore: "+Score.GetHighScore().ToString();
    }

    public Sprite pipeHeadSprite;
    public Transform pfPipeHead;
    public Transform pfPipeBody;
    public Transform pfGround;

    public Text menuHighScore;
    public Text highScore;


    [Serializable]
    public class SoundAudioClip{
      public SoundManager.Sound sound;
      public AudioClip audioClip;
    }

    public SoundAudioClip[] soundAudioClipArray;

}
