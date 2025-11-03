using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SoundData : ScriptableObject {
    public string id;
    public AudioClip audioClip;
    public float volume;
}
