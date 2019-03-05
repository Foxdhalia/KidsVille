using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScene_Sounds : MonoBehaviour
{
    private AudioSource audio;
    // [SerializeField] private AudioClip startGame;
    // [SerializeField] private AudioClip basicClick;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void PlayStartGame(AudioClip soundClip) {
        audio.PlayOneShot(soundClip);
    }


}
