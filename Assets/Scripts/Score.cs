using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static int GetHighScore(){
      return PlayerPrefs.GetInt("highScore");
    }
    public static void SetHighScore(int highScore){
      if(highScore>GetHighScore()){
        PlayerPrefs.SetInt("highScore",highScore);
      }
    }
}
