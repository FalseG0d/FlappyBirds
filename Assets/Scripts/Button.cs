using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public void Retry()
    {
      SoundManager.PlaySound(SoundManager.Sound.ButtonPush);
      Loader.Load(Loader.Scene.GameScene);
    }

    public void Play()
    {
      SoundManager.PlaySound(SoundManager.Sound.ButtonPush);
      Loader.Load(Loader.Scene.GameScene);
    }

    public void Quit()
    {
      SoundManager.PlaySound(SoundManager.Sound.ButtonPush);
      Application.Quit();
    }

    public void Menu()
    {
      SoundManager.PlaySound(SoundManager.Sound.ButtonPush);
      Loader.Load(Loader.Scene.MainMenu);
    }

}
