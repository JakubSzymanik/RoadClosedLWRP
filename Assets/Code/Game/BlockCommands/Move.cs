using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : BlockCommand
{
    public Vector2Int direction { get; private set; }

    public Move(CommandType type, SpecialField commandingField, Vector2Int direction) : base(type, commandingField)
    {
        this.direction = direction;
    }
}
