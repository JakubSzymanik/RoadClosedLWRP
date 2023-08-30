using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : SpecialField
{
    [SerializeField] private Vector2Int moveDir;
    [SerializeField] private MeshRenderer renderer;

    public override void OnUse()
    {
        sound.Play();
    }

    void Start()
    {
        this.Command = new Move(CommandType.Move, this, moveDir);
    }

    private void Update()
    {
        renderer.material.SetTextureOffset("_BaseMap", Vector2.down * Time.time * 0.5f);
        //renderer.material.mainTextureOffset = Vector2.up * Time.time * 1000;
    }
}
