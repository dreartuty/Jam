using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public List<AudioSource> sounds = new List<AudioSource>();
    public void PlaySound(int id)
    {
        sounds[id].Play();
    }
}
