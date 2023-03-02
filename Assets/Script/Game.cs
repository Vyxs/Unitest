using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoadType
{
    Straight,
    Left,
    Right,
    Both
}

public class Game : MonoBehaviour
{
    [SerializeField]
    List<GameObject> houseList;

    [SerializeField]
    GameObject roadBoth;

    [SerializeField]
    GameObject roadLeft;

    [SerializeField]
    GameObject roadStraight;

    [SerializeField]
    int viewDistance = 15;

    private readonly Quaternion defRotation = Quaternion.Euler(0, 90, 0);
    private const float roadLength = 6f;
    private const float roadWidth = 7f;
    
    private long zPosition = 0;

    private readonly Queue<List<GameObject>> partsQueue = new();

    void Start()
    {
        AddDefaultPart();
    }

    void Update()
    {
        Vector3 pos = CameraManager.Instance.GetPosition();

        if (pos.z > zPosition - (viewDistance - 1) * roadLength)
        {
            AddPart();
            RemoveLastPart();
        }
    }

    void AddDefaultPart()
    {
        for (int i = 0; i < viewDistance; i++) {
            AddPart();
        }
    }

    void AddPart()
    {
        RoadType type = GetRandomRoadType();
        List<GameObject> houses = SpawnHouses(type, zPosition);
        GameObject road = SpawnRoad(type, new Vector3(0, 0, zPosition));

        houses.Add(road);
        partsQueue.Enqueue(houses);
        zPosition += (long) roadLength;
    }

    void RemoveLastPart()
    {
        partsQueue.Dequeue().ForEach(o => Destroy(o));
    }

    RoadType GetRandomRoadType()
    {
        return Random.Range(0, 6) switch
        {
            1 => RoadType.Left,
            2 => RoadType.Right,
            3 => RoadType.Straight,
            _ => RoadType.Both,
        };
    }

    GameObject SpawnRoad(RoadType type, Vector3 position)
    {
        GameObject roadToSpawn = roadStraight;
        Quaternion roadRotation = defRotation;

        switch (type)
        {
            case RoadType.Left:
                roadToSpawn = roadLeft;
                break;
            case RoadType.Right:
                roadToSpawn = roadLeft;
                roadRotation = Quaternion.Euler(0, 270, 0);
                break;
            case RoadType.Both:
                roadToSpawn = roadBoth;
                break;
            default:
                break;
        }
        return Instantiate(roadToSpawn, position, roadRotation);
    }

    List<GameObject> SpawnHouses(RoadType type, long x)
    {
        List<GameObject> houses = new();
        int rnd = Random.Range(0, houseList.Count);
        int rnd2 = Random.Range(0, houseList.Count);

        switch (type)
        {
            case RoadType.Left:
                houses.Add(Instantiate(houseList[rnd], new Vector3(-roadWidth, 0, x), defRotation));
                break;
            case RoadType.Right:
                houses.Add(Instantiate(houseList[rnd], new Vector3(roadWidth, 0, x), Quaternion.Euler(0, 270, 0)));
                break;
            case RoadType.Both:
                houses.Add(Instantiate(houseList[rnd], new Vector3(-roadWidth, 0, x), defRotation));
                houses.Add(Instantiate(houseList[rnd2], new Vector3(roadWidth, 0, x), Quaternion.Euler(0, 270, 0)));
                break;
            default: break;
        }
        return houses;
    }
}
