using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotor : SpecialField
{
    [SerializeField] private Vector3 rotation;

    public override void OnUse()
    {
        sound.Play();
    }

    void Start()
    {
        this.Command = new Rotate(CommandType.Rotate, this, Quaternion.Euler(rotation));
    }
}
