using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class Block : MonoBehaviour
{
    [Header("Values:")]
    [SerializeField] private static float moveSpeed = 5;
    [SerializeField] private static float rotationSpeed = 600;
    [SerializeField] private bool isStatic;
    [SerializeField] private List<Road> roads;

    public bool IsStatic { get { return isStatic; } }
    public bool IsMoving { get; private set; }
    public static int MovingBlockCount { get; private set; } = 0;
    public static readonly Vector3Int[] directions = new Vector3Int[] { new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0) }; 

    private Queue<BlockCommand> currentCommands = new Queue<BlockCommand>();

    [HideInInspector] public MapRoad[] mapRoads = new MapRoad[4]; //-1: null, 0: road, 1: high road ...
    [HideInInspector] public MapRoad[] lastMapRoads = new MapRoad[4];
    [HideInInspector] public Block[] neighbours = new Block[4];
    [HideInInspector] private Block[] lastNeighbours = new Block[4];

    //Roads
    public Road HasRoadFrom(Vector3Int fromDir)
    {
        foreach(Road road in roads)
        {
            foreach(Vector3Int end in road.Ends)
            {
                if (end == new Vector3Int(-fromDir.x, -fromDir.y, fromDir.z))
                    return road;
            }
        }
        return null;
    }

    //Movement
    public void IssueCommands(Queue<BlockCommand> commands)
    {
        currentCommands.Clear();
        currentCommands = commands;

        MovingBlockCount++;
        ExecuteNextCommand();
    }

    public void EnqueueCommand(BlockCommand command)
    {
        if(currentCommands.Count == 0 && !IsMoving)
        {
            currentCommands.Enqueue(command);
            MovingBlockCount++;
            ExecuteNextCommand();
        }
        else
        {
            currentCommands.Enqueue(command);
        }
    }

    private void ExecuteNextCommand()
    {
        if(currentCommands.Count > 0)
        {
            IsMoving = true;
            BlockCommand command = currentCommands.Dequeue();
            switch(command.Type)
            {
                case CommandType.Move:
                    if (command.CommandingField != null)
                        command.CommandingField.OnUse();
                    StartCoroutine(Move((command as Move).direction));
                    break;
                case CommandType.Rotate:
                    RotateRoads((command as Rotate).rotation.eulerAngles.y < 0);
                    command.CommandingField.OnUse();
                    StartCoroutine(Rotate((command as Rotate).rotation));
                    break;
                case CommandType.Stop:
                    command.CommandingField.OnUse();
                    Stop();
                    break;
                case CommandType.Teleport:
                    command.CommandingField.OnUse();
                    Teleport((command as Teleport).teleportTarget);
                    break;
                case CommandType.Destroy:
                    command.CommandingField.OnUse();
                    StartCoroutine(DestroyDelay(0.25f));
                    break;
            }
        }
        else
        {
            IsMoving = false;
            MovingBlockCount--;
            HandleConnectionEffects();
        }
    }

    private IEnumerator Move(Vector2Int direction)
    {
        Vector3 target = transform.localPosition + new Vector3(direction.x, 0.0f, direction.y);
        while (transform.localPosition != target)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, moveSpeed * Time.smoothDeltaTime);
            yield return null;
        }
        ExecuteNextCommand();
    }

    private IEnumerator Rotate(Quaternion deltaRotation)
    {
        ExecuteNextCommand();
        Quaternion target = transform.rotation * deltaRotation;
        while (transform.rotation != target)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void Teleport(Vector3 toPos)
    {
        transform.position = toPos;
        ExecuteNextCommand();
    }

    private void Stop()
    {
        currentCommands.Clear();
        ExecuteNextCommand();
    }

    private void RotateRoads(bool left)
    {
        for (int i = 0; i < roads.Count; i++)
            for (int j = 0; j < roads[i].Ends.Count; j++)
            {
                Vector3Int end = roads[i].Ends[j];

                if (!left)
                {
                    Vector3Int v = new Vector3Int(end.y, -end.x, end.z);
                    roads[i].Ends[j] = v;
                }
                else
                {
                    Vector3Int v = new Vector3Int(-end.y, end.x, end.z);
                    roads[i].Ends[j] = v;
                }
            }
    }

    private IEnumerator DestroyDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        MovingBlockCount--;
        Destroy(this.gameObject);
    }

    //connection effects
    public void SetNeighbours(Block[] neighbours, MapRoad[] mapRoads)
    {
        lastMapRoads = this.mapRoads;
        this.mapRoads = mapRoads;
        lastNeighbours = this.neighbours;
        this.neighbours = neighbours;
    }

    private void HandleConnectionEffects()
    {
        for(int i = 0; i < 4; i++)
        {
            //discard invalid/empty neighbours
            if (neighbours[i] == null || neighbours[i].IsMoving || ContainsArr(ref lastNeighbours, neighbours[i]))
            { }
            else
            {
                //check neighbour connections
                if(this.HasRoadFrom(directions[i] * -1) != null && neighbours[i].HasRoadFrom(directions[i]) != null)
                {
                    EffectsManager.Instance
                    .InstantiateRoadConenctionEffect(
                    transform.position + new Vector3(0.5f * directions[i].x, 0, 0.5f * directions[i].y),
                    Quaternion.Euler(0, directions[i].x == 0 ? 0 : 90, 0));
                }
                else if (this.HasRoadFrom(directions[i] * -1 + new Vector3Int(0,0,1)) != null && neighbours[i].HasRoadFrom(directions[i] + new Vector3Int(0, 0, 1)) != null)
                {
                    EffectsManager.Instance
                    .InstantiateRoadConenctionEffect(
                    transform.position + new Vector3(0.5f * directions[i].x, 0.5f, 0.5f * directions[i].y),
                    Quaternion.Euler(0, directions[i].x == 0 ? 0 : 90, 0));
                }
            }

            //discard invalid map road connections
            if(mapRoads[i] == null || ContainsArr(ref lastMapRoads, mapRoads[i]))
            { }
            else
            {
                //check map road connections
                if(this.HasRoadFrom(directions[i] * -1 + new Vector3Int(0,0, mapRoads[i].End.z)) != null)
                {
                    EffectsManager.Instance
                    .InstantiateRoadConenctionEffect(
                    transform.position + new Vector3(0.5f * directions[i].x, 0.5f * mapRoads[i].End.z, 0.5f * directions[i].y),
                    Quaternion.Euler(0, directions[i].x == 0 ? 0 : 90, 0));
                }
            }
        }
    }

    //utility
    private bool ContainsArr<T>(ref T[] arr, T element)
    {
        foreach(T el in arr)
        {
            if (el != null && el.Equals(element))
                return true;
        }
        return false;
    }

    //Operators
    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    //Editor
#if UNITY_EDITOR
    public void RotateEditor(bool left)
    {
        Transform roadModel = transform.GetChild(2);
        roadModel.localRotation = Quaternion.Euler(roadModel.localRotation.eulerAngles.x, roadModel.localRotation.eulerAngles.y - (left ? 90 : -90), roadModel.localRotation.eulerAngles.z);
        for(int i = 0; i < roads.Count; i++)
            for(int j = 0; j < roads[i].Ends.Count; j++)
            {
                Vector3Int end = roads[i].Ends[j];
                
                if(!left)
                {
                    Vector3Int v = new Vector3Int(end.y, -end.x, end.z);
                    roads[i].Ends[j] = v;
                }
                else
                {
                    Vector3Int v = new Vector3Int(-end.y, end.x, end.z);
                    roads[i].Ends[j] = v;
                }
            }

        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    public void MoveEditor(Vector3Int direction)
    {
        transform.localPosition += direction;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    public void SetStatic()
    {
        SerializedObject obj = new SerializedObject(this);
        obj.Update();
        obj.FindProperty("isStatic").boolValue = !isStatic;
        obj.ApplyModifiedProperties();

        transform.GetChild(1).gameObject.SetActive(!isStatic);
        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
    }
#endif
}
