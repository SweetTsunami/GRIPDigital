/*  File:       Game.cs
 *  Creator:    Alexander Semenov
 *  Date:       December 2017 
 *  Location:   Brno, Czech Republic
 *  Project:    GRIP Digital Showcase project - Primitive Minecraft Clone
 *  Desc:       Main game script
 *                 Contains core game functions like World generation
 *                 Generates and handles Chunk generation
 *  Usage:      Put it on the empty GameObject, which is child of the Player object
 *              Put SphereDetector.cs into the same object as this one relies heavily on it
 *              Put GameObject with Chunk.cs into ChunkPrefab
 */

using UnityEngine;

using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Core game class
/// </summary>
public class Game : MonoBehaviour
{
    public string savePath = "/savedGame.dat";
    
    // Keep the track of all chunks
    private List<Chunk> chunks = new List<Chunk>();

    /// <summary>
    /// Lists to keep track of all positions, positions in and outside of the detection range of the player (DetectionRange in SphereDetector)
    /// </summary>
    private List<Position> allPositions = new List<Position>();
    private List<Position> positionsInRange = new List<Position>();
    private List<Position> outOfRange = new List<Position>();

    /// <summary>
    /// Dictionary to store the information about Points and Positions
    /// </summary>
    public Dictionary<Point, Position> PositionDirectory = new Dictionary<Point, Position>();

    // Singleton Game
    public static Game CurrentGame;
    // seed to generate different worlds every time
    public static int Seed;

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
        CurrentGame = this;
        Seed = (int)Network.time;
        FillPositionDirectory();
    }

    /// <summary>
    /// Fill the Directory with positions for chunks
    /// </summary>
    private void FillPositionDirectory()
    {
        // set point size to the size of chunk
        var pointSize = ChunkPrefab.ChunkSize;

        // Nested cycle to fill the GRID with positions, on which chunks are spawned
        for (int x = Mathf.RoundToInt(0 - InitialPositionCount / 2); x < InitialPositionCount ; x++)
        {
            for (int z = Mathf.RoundToInt(0 - InitialPositionCount / 2); z < InitialPositionCount; z++)
            {
                var pointPosition = new Vector3(x * pointSize, 0, z * pointSize);
                // Add the entry to the PositionDirectory
                PositionDirectory.Add(new Point(x, z), new Position(pointSize, pointPosition));
            }
        }
    }

    /// <summary>
    /// Get SphereDetector and start repeating the GenerateChunkPositions (running it in Update() drains FPS like crazy)
    /// </summary>
    private void Start()
    {
        sphereDetector = GetComponent<SphereDetector>();
        InvokeRepeating("HandleChunkPositionsLists", 0, 1);
    }

    /// <summary>
    /// Just key checks for saving/loading the game
    /// </summary>
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    Save();
        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    Load();
        //}
    }

    /// <summary>
    /// Handles the chunks positions lists, so they are filled with proper values, 
    /// </summary>
    private void HandleChunkPositionsLists()
    {
        // Fill AllPositions list with position (Value) entries from PositionDirectory
        foreach (KeyValuePair<Point, Position> entry in PositionDirectory)
        {
            allPositions.Add(entry.Value);
        }

        // Fill PositionsInRange List with filtered out AllPositions List (Sphere detector filters those out)
        positionsInRange = sphereDetector.CheckPositionsInRange(allPositions);
        // Out of range gets filled with all positions except for those outside of the range
        outOfRange = allPositions.Except(positionsInRange).ToList();

        ManageChunks(positionsInRange, outOfRange);
    }

    /// <summary>
    /// Manages chunks :
    /// Instantiates / enables the ones in range
    /// Deactivates the ones outside of range
    /// </summary>
    /// <param name="PositionsInRange">List of all positions in range</param>
    /// <param name="OutOfRange">List of all positions out of range</param>
    private void ManageChunks(List<Position> PositionsInRange, List<Position> OutOfRange)
    {
        // Cycles through positions in range and if there is no chunk, instantiate it, if it's not active, activate it
        foreach (var pos in PositionsInRange)
        {
            // Check if pos has chunk
            var chunk = FindChunkAtPosition(pos);
            if (chunk == null)
            {
                pos.Chunk = Instantiate(ChunkPrefab, pos.WorldPosition, Quaternion.identity);
                chunks.Add(pos.Chunk);
            }
            else
            {
                chunk.gameObject.SetActive(true);
            }
        }

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
    /// ###OBSOLETE, because FindChunkAtPosition, but can come in handy, sometime...
    /// Checks if there is a chunk at point
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

    // ###UNUSED METHODS TO SAVE / LOAD GAME###
    //private Save CreateSaveGameObject()
    //{
    //    Save save = new Save();
    //    foreach (Chunk chunk in Chunks)
    //    {
    //        save.seed = seed;
    //    }
    //    return save;
    //}

    //public void Save()
    //{
    //    Save save = new Save
    //    {
    //        seed = seed
    //    };

    //    Debug.Log("saving game...");
    //    BinaryFormatter bf = new BinaryFormatter();
    //    FileStream file = File.Create(Application.persistentDataPath + savePath);
        
    //    bf.Serialize(file, save);
    //    file.Close();
    //    Debug.Log("game save to " + ( Application.persistentDataPath + savePath));
    //}

    //public void Load()
    //{
    //    Debug.Log("loading game... " + (Application.persistentDataPath + savePath));
    //    if (File.Exists(Application.persistentDataPath))
    //    {
    //        BinaryFormatter bf = new BinaryFormatter();
    //        FileStream file = File.Open(Application.persistentDataPath + savePath, FileMode.Open);
    //        Save save = (Save)bf.Deserialize(file);
    //        file.Close();

    //        RebuildWorld(Chunks, save.SavedChunks);
    //        Chunks.Clear();
    //        // gameObject.transform.parent.position = save.PlayerPos;
    //        Chunks = save.SavedChunks;
    //    }
    //    Debug.Log("Game loaded !");
    //}

    //void RebuildWorld(List<Chunk> originalChunks, List<Chunk> newChunks)
    //{
    //    foreach (Chunk chunk in originalChunks)
    //    {
    //        Destroy(chunk.gameObject);
    //    }
    //    foreach(Chunk chunk in newChunks)
    //    {
    //        ManageChunks(PositionsInRange, OutOfRange);
    //    }
    //}
}