using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public Sprite pipeHeadSprite;

}
