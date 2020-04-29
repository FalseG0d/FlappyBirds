﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public enum Sound{
      BirdJump,
      BirdDead,
      BirdScore,
      ButtonPush,
    }
    public static void PlaySound(Sound sound){
      GameObject gameObject=new GameObject("Sound",typeof(AudioSource));
      AudioSource audioSource=gameObject.GetComponent<AudioSource>();
      audioSource.PlayOneShot(GetAudioClip(sound));
    }
    private static AudioClip GetAudioClip(Sound sound){
      foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.GetInstance().soundAudioClipArray) {
        if(soundAudioClip.sound==sound){
          return soundAudioClip.audioClip;
        }
      }
      return null;
    }
}
