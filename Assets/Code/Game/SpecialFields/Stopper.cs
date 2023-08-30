using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopper : SpecialField
{
    private ParticleSystem particle;

    public override void OnUse()
    {
        sound.Play();
        particle.Play();
    }

    void Start()
    {
        this.Command = new BlockCommand(CommandType.Stop, this);
        particle = this.GetComponent<ParticleSystem>();
    }
}
