using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Options")]
public class config_Options : ScriptableObject {
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0, 1)] public int musicTrack = 0;

    // Read-only example
    // protected float secretValue = 5f;
    // public float PublicValue { get { return secretValue; } }

}
