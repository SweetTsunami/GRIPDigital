/*  File:       Chunk.cs
 *  Creator:    Alexander Semenov
 *  Date:       December 2017 
 *  Location:   Brno, Czech Republic
 *  Project:    GRIP Digital Showcase project - Primitive Minecraft Clone
 *  Desc:       Core script used to generate Blocks
 *  Usage:      Put this into empty object and make prefab from it, put the prefab into Game.cs
 *              Set the parameters to something reasonable
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Chunk : MonoBehaviour
{
    /// <summary>
    /// set the X and Z size of the chunk -> it's square
    /// </summary>
    [Header("World generation parameters")]
    [Range(10,200)]
    public int TerrainSmoothness = 60;
    [Range(10,200)]
    public int TerrainHeightScale = 20;
    [Range(5,100)]
    public int ChunkSize = 20;

    /// <summary>
    /// Array of block prefabs
    /// </summary>
    public GameObject[] BlockPrefabs;

    private List<Vector3> blockPositions;

    Game World;

    /// <summary>
    /// Start SmoothlyGenerateBlocks coroutine the moment chunk object spawns
    /// </summary>
    private void Awake()
    {
        blockPositions = new List<Vector3>();
    }

    private void Start()
    {
        GenerateBlockPositions(blockPositions);

        // ###UNUSED Possible second solution, slower but smoother on FPS than Instantiating everything from start
        // StartCoroutine(SmoothlyGenerateBlocks());

        // Faster than coroutine
        QuicklyGenerateBlocks();
    }

    /// <summary>
    /// Continuosly generate blocks on positions given by list
    /// </summary>
    /// <returns></returns>
    private IEnumerator SmoothlyGenerateBlocks()
    {
        foreach (Vector3 blockPos in blockPositions)
        {
            CreateRandomBlock(blockPos, BlockPrefabs);
            yield return null;
        }
    }

    /// <summary>
    /// Instantly generate all blocks given by the list
    /// </summary>
    private void QuicklyGenerateBlocks()
    {
        foreach (Vector3 blockPos in blockPositions)
        {
            CreateRandomBlock(blockPos, BlockPrefabs);
        }
    }

    /// <summary>
    /// Generates all possible positions for blocks
    /// </summary>
    /// <param name="blockPositions">List of block positions</param>
    private void GenerateBlockPositions(List<Vector3> blockPositions)
    {
        // Nested cycle for X and Z coords
        for (int z = 0; z <= ChunkSize; z++)
        {
            for (int x = 0; x <= ChunkSize; x++)
            {
                // PerlinNoise
                var y = (int)(Mathf.PerlinNoise((transform.position.x + x + Game.Seed) / TerrainSmoothness, (transform.position.z + z + Game.Seed) / TerrainSmoothness) * TerrainHeightScale);
                var blockPos = new Vector3(transform.position.x + x - (ChunkSize / 2), y, transform.position.z + z - (ChunkSize / 2));
                this.blockPositions.Add(blockPos);
            }
        }
    }

    private void CreateRandomBlock(Vector3 blockPos, GameObject[] BlockPrefabs)
    {
        // Pick random number
        int pickBlock = Random.Range(0, 100);
        if (pickBlock < 50)
            CreateBlock(blockPos, this.BlockPrefabs[0]);
        else if (50 <= pickBlock && pickBlock < 70)
            CreateBlock(blockPos, this.BlockPrefabs[1]);
        else if (70 <= pickBlock && pickBlock < 95)
            CreateBlock(blockPos, this.BlockPrefabs[2]);
        else if (95 <= pickBlock)
            CreateBlock(blockPos, this.BlockPrefabs[3]);
    }

    /// <summary>
    /// Creates a block
    /// </summary>
    /// <param name="blockPos">Position of spawned block </param>
    /// <param name="prefab">Prefab of the block to spawn</param>
    private void CreateBlock(Vector3 blockPos, GameObject blockPrefab)
    {
        GameObject block = Instantiate(blockPrefab, blockPos, Quaternion.identity, gameObject.transform);
    }
}