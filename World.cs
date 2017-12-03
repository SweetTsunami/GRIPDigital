/*
*SCRIPT MADE BY ALEXANDER SEMENOV

*USED FOR HIS OWN PEOJECT
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class World : MonoBehaviour
{
    public List<Chunk> Chunks = new List<Chunk>();

    public List<Position> AllPositions = new List<Position>();
    public List<Position> PositionsInRange = new List<Position>();

    public Dictionary<Point, Position> PositionDirectory = new Dictionary<Point, Position>();
        
    public static World currentWorld;
    public static int seed;    

    private SphereDetector sphereDetector;
    public Chunk chunkPrefab;
    public int initialPositionCount = 30;

    private void Awake()
    {
        var pointSize = chunkPrefab.chunkSize;
        for (int i = Mathf.RoundToInt(0 - initialPositionCount/2); i < Mathf.RoundToInt(initialPositionCount /2); i++)
        {
            for (int j = Mathf.RoundToInt(0 - initialPositionCount / 2); j < Mathf.RoundToInt(initialPositionCount /2); j++)
            { 
                var pointPosition = new Vector3(i * pointSize, 0, j * pointSize);
                PositionDirectory.Add(new Point(i,j), new Position(pointSize, pointPosition));                
            }
        }
        currentWorld = this;
        seed = (int)Network.time * 10;
    }

    private void Start()
    {
        sphereDetector = GetComponent<SphereDetector>();
    }

    private void Update()
    {
        PopulateWorld();
    }

    private void PopulateWorld()
    {
        foreach (KeyValuePair<Point, Position> entry in PositionDirectory)
        {
            AllPositions.Add(entry.Value);
        }

        PositionsInRange = sphereDetector.CheckPositionsInRange(AllPositions);

        foreach (var pos in PositionsInRange)
        {
            var chunk = FindChunkAtPosition(pos);
            if (chunk == null)
            {
                pos.Chunk = Instantiate(chunkPrefab, pos.WorldPosition, Quaternion.identity);
            } 
        }
    }

    private void DestroyOutOfRange()
    {
        foreach(Position pos in AllPositions)
        {
            
        }
    }

    public Chunk FindChunkAtPoint(Point point)
    {
        Position pos;
        if (PositionDirectory.TryGetValue(point, out pos)) 
        {
            return pos.Chunk;
        }
        return null;
    }

    public Chunk FindChunkAtPosition(Position pos)
    {
        return pos.Chunk;
    }
}
