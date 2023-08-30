using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : BlockCommand
{
    public Quaternion rotation { get; private set; }

    public Rotate(CommandType type, SpecialField commandingField, Quaternion rotation) : base (type, commandingField)
    {
        this.rotation = rotation;
    }
}
