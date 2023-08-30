using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialField : MonoBehaviour
{
    public BlockCommand Command { get; protected set; }
    public abstract void OnUse();
    public AudioSource sound;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
    }
}
