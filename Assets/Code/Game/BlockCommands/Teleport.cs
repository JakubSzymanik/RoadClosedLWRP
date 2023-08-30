using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : BlockCommand
{
    public Vector3 teleportTarget { get; private set; }

    public Teleport(CommandType type, SpecialField commandingField, Vector3 target) : base(type, commandingField)
    {
        this.teleportTarget = target;
    }
}
