/*
*SCRIPT MADE BY ALEXANDER SEMENOV

*USED FOR HIS OWN PEOJECT
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class World : MonoBehaviour
{
    // Keep the track of all chunks
    public List<Chunk> Chunks = new List<Chunk>();

    /// <summary>
    /// Lists to keep track of all positions, positions in and outside of the detection range of the player (DetectionRange in SphereDetector)
    /// </summary>
    public List<Position> AllPositions = new List<Position>();
    public List<Position> PositionsInRange = new List<Position>();
    public List<Position> OutOfRange = new List<Position>();

    /// <summary>
    /// Dictionary to store the information about Points and Positions
    /// </summary>
    public Dictionary<Point, Position> PositionDirectory = new Dictionary<Point, Position>();
        
    // Singleton World
    public static World currentWorld;
    // seed to generate different worlds every time
    public static int seed;    

    // Reference to SphereDetector so we can use it's DetectionRange
    private SphereDetector sphereDetector;

    // Chunk prefab to instantiate
    public Chunk ChunkPrefab;

    // Ammount of initialy stored positions to have some Positions to generate Chunks
    public int InitialPositionCount = 30;

    /// <summary>
    /// Fill the PositionDirectory with positions, set the current world singleton and set the seed by time (Pseudo random)
    /// </summary>
    private void Awake()
    {
        // Singleton
        currentWorld = this;
        seed = (int)Network.time * 10;

        // set point size to the size of chunk
        var pointSize = ChunkPrefab.ChunkSize;

        // Nested cycle to fill the GRID with positions, on which chunks are spawned
        for (int x = Mathf.RoundToInt(0 - InitialPositionCount/2); x < Mathf.RoundToInt(InitialPositionCount /2); x++)
        {
            for (int z = Mathf.RoundToInt(0 - InitialPositionCount / 2); z < Mathf.RoundToInt(InitialPositionCount /2); z++)
            { 
                var pointPosition = new Vector3(x * pointSize, 0, z * pointSize);
                // Add the entry to the PositionDirectory
                PositionDirectory.Add(new Point(x,z), new Position(pointSize, pointPosition));                
            }
        }
    }

    /// <summary>
    /// Get SphereDetector and start repeating the PopulateWorld (running it in Update() drains FPS like crazy)
    /// </summary>
    private void Start()
    {
        sphereDetector = GetComponent<SphereDetector>();
        InvokeRepeating("PopulateWorld", 0, 1);
    }
    
    /// <summary>
    /// "Populatates" the world with chunks, if their positions are in range, if they are not, disable them, so they won't have to be created again
    /// </summary>
    private void PopulateWorld()
    {
        // Fill AllPositions list with position (Value) entries from PositionDirectory
        foreach (KeyValuePair<Point, Position> entry in PositionDirectory)
        {
            AllPositions.Add(entry.Value);
        }

        // Fill PositionsInRange List with filtered out AllPositions List (Sphere detector filters those out)
        PositionsInRange = sphereDetector.CheckPositionsInRange(AllPositions);

        // Cycles through positions in range and if there is no chunk, instantiate it, if it's not active, activate it
        foreach (var pos in PositionsInRange)
        {
            // Check if pos has chunk
            var chunk = FindChunkAtPosition(pos);
            if (chunk == null)
            {
                pos.Chunk = Instantiate(ChunkPrefab, pos.WorldPosition, Quaternion.identity);
            }
            else
            {
                chunk.gameObject.SetActive(true);
            }
        }

        // Out of range gets filled with all positions except for those outside of the range
        OutOfRange = AllPositions.Except(PositionsInRange).ToList();

        // Cycle through positions outside of range and deactivate chunks outside of range
        foreach (var pos in OutOfRange)
        {
            // Check if there is a chunk at position
            var chunk = FindChunkAtPosition(pos);
            if (chunk)
            {
                chunk.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Checks if there is a chunk at position ###OBSOLETE, because FindChunkAtPosition, but can come in handy, sometime...
    /// </summary>
    /// <param name="point">Point on the grid</param>
    /// <returns>Return Chunk or null</returns>
    public Chunk FindChunkAtPoint(Point point)
    {
        // storing position to get it's value from PositionDirectory
        Position pos;
        // Getting value of Position by the Point Key
        if (PositionDirectory.TryGetValue(point, out pos)) 
        {
            return pos.Chunk;
        }
        return null;
    }

    /// <summary>
    /// Checks if Position contains Chunk
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns>Returns chunk or null</returns>
    public Chunk FindChunkAtPosition(Position pos)
    {
        return pos.Chunk;
    }
}
