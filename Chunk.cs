/*
*SCRIPT MADE BY ALEXANDER SEMENOV
*USED FOR HIS OWN PEOJECT
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Chunk : MonoBehaviour
{
    /// <summary>
    /// set the X and Z size of the chunk -> it's square
    /// </summary>
    public int ChunkSize = 20;
    /// <summary>
    /// Array of block prefabs
    /// </summary>
    public GameObject[] BlockPrefabs;
    public List<Vector3> BlockPositions;
    World World;
    public int TerrainSmoothness = 60;
    public int TerrainHeightScale = 20;
    /// <summary>
    /// Start CreateChunk coroutine the moment chunk object spawns
    /// </summary>
    private void Awake()
    {
        BlockPositions = new List<Vector3>();
    }
    void Start()
    {
        GenerateBlockPositions(BlockPositions);
        // StartCoroutine(CreateChunk());
        foreach (Vector3 blockPos in BlockPositions)
        {
            CreateRandomBlock(blockPos, BlockPrefabs);
        }
    }

    /// <summary>
    /// Creates chunk -> ammount of blocks dendant on ChunkSize
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateChunk()
    {
        foreach (Vector3 blockPos in BlockPositions)
        {
            CreateRandomBlock(blockPos, BlockPrefabs);
            yield return null;
        }
    }

    void GenerateBlockPositions(List<Vector3> blockPositions)
    {
        for (int z = 0; z <= ChunkSize; z++)
        {
            for (int x = 0; x <= ChunkSize; x++)
            {
                var y = (int)(Mathf.PerlinNoise((transform.position.x + x + World.seed) / TerrainSmoothness, (transform.position.z + z + World.seed) / TerrainSmoothness) * TerrainHeightScale);
                var blockPos = new Vector3(transform.position.x + x - (ChunkSize / 2), y, transform.position.z + z - (ChunkSize / 2));
                BlockPositions.Add(blockPos);
            }
        }
    }

    void CreateRandomBlock(Vector3 blockPos, GameObject[] BlockPrefabs)
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
    void CreateBlock(Vector3 blockPos, GameObject blockPrefab)
    {
        GameObject block = Instantiate(blockPrefab, blockPos, Quaternion.identity, gameObject.transform);
    }
}