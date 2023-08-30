using System.Collections.Generic;
using UnityEngine;
using UniRx;

using UnityEditor; ////// for gizmos

public class Map : MonoBehaviour
{
    [Header("Values:")]
    [SerializeField] private Vector2Int mapSize = new Vector2Int();

    [Header("References:")]
    [SerializeField] private List<Block> startingBlocks;
    [SerializeField] private List<SpecialField> startingSpecialFields;
    [SerializeField] private List<MapRoad> startingMapRoads;

    public System.IObservable<Unit> lvlCompletedStream { get { return lvlCompletedSubject; } }
    private Subject<Unit> lvlCompletedSubject = new Subject<Unit>();
    public System.IObservable<Unit> moveCompletedStream { get { return moveCompletedSubject; } }
    private Subject<Unit> moveCompletedSubject = new Subject<Unit>();
    public System.IObservable<Unit> moveStartedStream { get { return moveStartedSubject; } }
    private Subject<Unit> moveStartedSubject = new Subject<Unit>();

    private Block[,] blocks;
    private SpecialField[,] specialFields;
    private MapRoad[] mapRoads;

    private bool moved = false;
    private bool blocksMovingLastFrame = false;
    public bool BlocksMoving { get { return Block.MovingBlockCount > 0; } }
    public int worldID;

    public static Map instance { get; private set; } //for cars to access map

    public void Move(Vector2Int direction)
    {
        MoveBlocks(direction);
        moved = true;
    }

    //MonoBehaviour
    private void Update()
    {
        if(blocksMovingLastFrame && !BlocksMoving)
        {
            if(worldID == 5)
                UseConveyorBelts(); //only conveyor belts
            else
                UsePostMoveFields(); //all special fields combined, could be split like conveyors
            moveCompletedSubject.OnNext(new Unit());
            blocksMovingLastFrame = false;
        }
        else if (BlocksMoving)
            blocksMovingLastFrame = true;

        if(moved && !BlocksMoving)
        {
            moved = false;
            if(CheckLevelCompleted())
            {
                lvlCompletedSubject.OnNext(new Unit());
                this.enabled = false;
            }
        }
    }

    private void Start()
    {
        instance = this;

        PlaceBlocks(mapSize.x, mapSize.y);
        PlaceSpecialFields(mapSize.x, mapSize.y);
        PlaceMapRoads();
    }

    //Setup
    private void PlaceBlocks(int mapSizeX, int mapSizeY)
    {
        blocks = new Block[mapSizeX, mapSizeY];
        foreach(Block block in startingBlocks)
        {
            Vector2Int pos = new Vector2Int((int)(block.transform.position.x - 0.5f), (int)(block.transform.position.z - 0.5f));
            blocks[pos.x, pos.y] = block;
        }
        startingBlocks.Clear();
    }

    private void PlaceSpecialFields(int mapSizeX, int mapSizeY)
    {
        specialFields = new SpecialField[mapSizeX, mapSizeY];
        foreach(SpecialField field in startingSpecialFields)
        {
            Vector2Int pos = new Vector2Int((int)(field.transform.position.x - 0.5f), (int)(field.transform.position.z - 0.5f));
            specialFields[pos.x, pos.y] = field;
        }
        startingSpecialFields.Clear();
    }

    private void PlaceMapRoads()
    {
        mapRoads = new MapRoad[startingMapRoads.Count];
        startingMapRoads.CopyTo(mapRoads);
        startingMapRoads.Clear();
    }

    //Gameplay
    private bool CheckLevelCompleted()
    {
        //check if all mapRoads have neighbours
        foreach(MapRoad mapRoad in mapRoads)
        {
            Vector2Int target = mapRoad.MapPosition + new Vector2Int(mapRoad.End.x, mapRoad.End.y);
            if (blocks[target.x, target.y] == null)
                return false; //if no, no point in checking further
        }

        //if yes, check road from one of them
        RoadCompletionTester completionTester = new RoadCompletionTester();
        return completionTester.CheckIfCompleted(ref blocks, ref mapRoads);
    }

    private void MoveBlocks(Vector2Int direction)
    {
        int startX, endX, deltaX, startY, endY, deltaY;
        if(direction.x > 0)
        {
            startX = mapSize.x - 1;
            endX = -1;
            deltaX = -1;
        }
        else
        {
            startX = 0;
            endX = mapSize.x;
            deltaX = 1;
        }
        if (direction.y > 0)
        {
            startY = mapSize.y - 1;
            endY = -1;
            deltaY = -1;
        }
        else
        {
            startY = 0;
            endY = mapSize.y;
            deltaY = 1;
        }

        bool blocksMoved = false;

        for (int x = startX; x != endX; x += deltaX)
            for(int y = startY; y != endY; y += deltaY)
            {
                if (blocks[x, y] == null || blocks[x, y].IsStatic)
                    continue;
                int distance = 0;
                Queue<BlockCommand> commands = GetMoveList(x, y, direction, ref distance);
                if (commands.Count == 0)
                    continue;

                blocks[x, y].IssueCommands(commands);

                if(distance != 0) //if distance != 0 - move block on the map
                {
                    Vector2Int target = new Vector2Int(x, y) + direction * distance;
                    blocks[target.x, target.y] = blocks[x, y];
                    blocks[x, y] = null;
                    blocksMoved = true;
                }
            }

        if (blocksMoved)
        {
            moveStartedSubject.OnNext(new Unit());
            for (int x = 0; x < mapSize.x; x++)
                for (int y = 0; y < mapSize.y; y++)
                {
                    if (blocks[x, y] == null)
                        continue;

                    Block[] neighbours = new Block[4];
                    MapRoad[] mapRoadNeighbours = new MapRoad[4];
                    for(int i = 0; i < 4; i++)
                    {
                        Vector2Int dir = (Vector2Int)Block.directions[i];
                        if(x + dir.x >= 0 && x + dir.x < mapSize.x && y + dir.y >= 0 && y + dir.y < mapSize.y)
                        {
                            //if pos to check is inside map
                            neighbours[i] = blocks[x + dir.x, y + dir.y];
                        }
                        else
                        {
                            //else if outside map
                            MapRoad mr = FindMapRoadAt(new Vector2Int(x + dir.x, y + dir.y));
                            if(mr != null)
                            {
                                mapRoadNeighbours[i] = mr;
                            }
                        }
                    }
                    blocks[x, y].SetNeighbours(neighbours, mapRoadNeighbours);
                }
        }
    }

    private Queue<BlockCommand> GetMoveList(int x, int y, Vector2Int direction, ref int distance)
    {
        distance = 0;
        Queue<BlockCommand> commands = new Queue<BlockCommand>();
        Vector2Int target = new Vector2Int(x, y) + direction;
        while(target.x >= 0 && target.x < mapSize.x && target.y >= 0 && target.y < mapSize.y && 
            blocks[target.x, target.y] == null)
        {
            commands.Enqueue(new Move(CommandType.Move, null, direction));

            SpecialField sField = specialFields[target.x, target.y];
            if (sField != null && sField.GetType() != typeof(Teleporter) && sField.GetType() != typeof(WreckingBall) && sField.GetType() != typeof(ConveyorBelt))
            {
                commands.Enqueue(specialFields[target.x, target.y].Command);
            }

            distance++;
            target += direction;

            if (sField != null && sField.GetType() == typeof(Stopper))
                break;
        }
        return commands;
    }

    private void UsePostMoveFields()
    {
        List<int> usedTeleporterIndexes = new List<int>(); 
        for(int x = 0; x < mapSize.x; x++)
        {
            for(int y = 0; y < mapSize.y; y++)
            {
                SpecialField field = specialFields[x, y];
                if(field != null && field.GetType() == typeof(Teleporter) && blocks[x,y] != null && !usedTeleporterIndexes.Contains((field as Teleporter).teleporterIndex))
                { //teleporter
                    blocks[x, y].EnqueueCommand(field.Command);
                    Vector2Int otherMapPos = new Vector2Int(
                            Mathf.FloorToInt((field as Teleporter).otherTeleporter.position.x),
                            Mathf.FloorToInt((field as Teleporter).otherTeleporter.position.z));

                    Swap(ref blocks[x, y], ref blocks[otherMapPos.x, otherMapPos.y]);

                    if(blocks[x,y] != null)
                    {
                        blocks[x, y].EnqueueCommand((field as Teleporter).otherTeleporter.GetComponent<Teleporter>().Command);
                    }
                    usedTeleporterIndexes.Add((field as Teleporter).teleporterIndex);
                }
                else if(field != null && field.GetType() == typeof(WreckingBall) && blocks[x,y] != null)
                { //wrecking ball
                    blocks[x, y].EnqueueCommand(field.Command);
                    blocks[x, y] = null;
                }
                else if(field != null && field.GetType() == typeof(ConveyorBelt) && blocks[x,y] != null)
                {
                    blocks[x, y].EnqueueCommand(field.Command);
                    blocks[x + (field.Command as Move).direction.x, y + (field.Command as Move).direction.y] = blocks[x,y];
                    blocks[x, y] = null;
                }
            }
        }
    }

    private void UseConveyorBelts()
    {
        bool blocksMoved = true;
        while(blocksMoved)
        {
            blocksMoved = false;
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    SpecialField field = specialFields[x, y];
                    if (field != null && field.GetType() == typeof(ConveyorBelt) && blocks[x, y] != null)
                    {
                        Vector2Int target = new Vector2Int(x + (field.Command as Move).direction.x, y +(field.Command as Move).direction.y);
                        if (target.x >= mapSize.x || 
                            target.y >= mapSize.y || 
                            target.x < 0 ||
                            target.y < 0 ||
                            blocks[target.x, target.y] != null)
                            continue;
                        //move block physically
                        blocks[x, y].EnqueueCommand(field.Command);
                        //move block on map
                        blocks[target.x, target.y] = blocks[x, y];
                        blocks[x, y] = null;
                        blocksMoved = true;
                    }
                }
            }
        }
    }

    //utility
    private MapRoad FindMapRoadAt(Vector2Int position)
    {
        foreach(MapRoad mr in mapRoads)
        {
            if (mr.MapPosition == position)
                return mr;
        }
        return null;
    }

    private void Swap<T>(ref T objA, ref T objB)
    {
        T temp = objA;
        objA = objB;
        objB = temp;
    }

    //cars
    public Block GetBlock(int x, int y)
    {
        return blocks[x, y];
    }

#if UNITY_EDITOR
    //Editor
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;

        for (int i = -1; i <= 5; i++)
            for (int j = -1; j <= 5; j++)
            {
                style.normal.textColor = Color.red;
                Handles.Label(new Vector3(i + 0.5f, 0, j + 0.5f), "x: " + i, style);
                style.normal.textColor = Color.blue;
                Handles.Label(new Vector3(i + 0.5f, 0, j + 0.5f), "\n" + "y: " + j, style);
            }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero + Vector3.up * 0.01f, Vector3.forward * mapSize.x + Vector3.up * 0.01f);
        Gizmos.DrawLine(Vector3.forward * mapSize.y + Vector3.up * 0.01f, Vector3.forward * mapSize.y + Vector3.right * mapSize.x + Vector3.up * 0.01f);
        Gizmos.DrawLine(Vector3.forward * mapSize.y + Vector3.right * mapSize.x + Vector3.up * 0.01f, Vector3.right * mapSize.x + Vector3.up * 0.01f);
        Gizmos.DrawLine(Vector3.right * mapSize.x + Vector3.up * 0.01f, Vector3.zero + Vector3.up * 0.01f);
    }
#endif
}
