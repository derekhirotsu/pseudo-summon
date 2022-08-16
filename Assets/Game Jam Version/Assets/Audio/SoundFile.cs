using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SoundFile")]
public class SoundFile : ScriptableObject
{
    [SerializeField] public AudioClip audioClip;
    [SerializeField] public string audioName;

    [Header("Playback Parameters")]
    [SerializeField] public float volumeScale;
    [SerializeField] public bool randomizePitch;
    [SerializeField] public float minPitch;
    [SerializeField] public float maxPitch;
}
