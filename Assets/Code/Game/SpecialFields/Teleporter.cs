using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : SpecialField
{
    [SerializeField] public int teleporterIndex;
    [SerializeField] public Transform otherTeleporter;
    [SerializeField] private ParticleSystem ps;

    private void Start()
    {
        this.Command = new Teleport(CommandType.Teleport, this, new Vector3(otherTeleporter.position.x, 0, otherTeleporter.position.z));
    }

    public override void OnUse()
    {
        sound.Play();
        ps.Play();
    }
}
