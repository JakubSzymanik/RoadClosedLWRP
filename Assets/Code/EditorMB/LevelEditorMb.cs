using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorMb : MonoBehaviour
{
    [SerializeField] private List<Mesh> blockBases;
    [SerializeField] private List<Mesh> blockActives;
    [SerializeField] private List<Mesh> maps;

    public void SetBiome(int id)
    {
        foreach(Block block in FindObjectsOfType<Block>())
        {
                block.transform.GetChild(0).GetComponent<MeshFilter>().mesh = blockBases[id];
                    block.transform.GetChild(1).GetComponent<MeshFilter>().mesh = blockActives[id];
        }
        FindObjectOfType<Map>().transform.GetChild(0).GetComponent<MeshFilter>().mesh = maps[id];
    }

    public void ScreenShot()
    {
        ScreenCapture.CaptureScreenshot("C:\\Users\\Jakub\\Desktop\\" + Time.time + ".png");
    }
}
