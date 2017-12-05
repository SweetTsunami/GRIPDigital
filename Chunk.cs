/*
*SCRIPT MADE BY ALEXANDER SEMENOV

*USED FOR HIS OWN PEOJECT
*/

using UnityEngine;
using System.Collections;

public class Chunk : MonoBehaviour
{
    /// <summary>
    /// set the X and Z size of the chunk -> it's square
    /// </summary>
    public int ChunkSize = 20;  

    /// <summary>
    /// Array of block prefabs
    /// </summary>
    public GameObject[] BlockPrefab;
    World World;

    public int HeightOffset = 5;
    public int WidthOffset = 5;

    public float Scale = 25.0f;

    /// <summary>
    /// Start CreateChunk coroutine the moment chunk object spawns
    /// </summary>
    private void Awake()
    {        
        StartCoroutine(CreateChunk());
    }
    
    /// <summary>
    /// Creates chunk -> ammount of blocks dendant on ChunkSize
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateChunk()
    {
        // Two for cycles, one for x and z coords
        for (int x = 0; x <= ChunkSize; x++)
        {
            for (int z = 0; z <= ChunkSize; z++)
            {
                // Setup of PerlinNoise function
                var xCoord = WidthOffset + (transform.position.x + x + World.seed) / Scale;
                var yCoord = HeightOffset + (transform.position.z + z + World.seed) / Scale;

                // PerlinNoise dictates the Y position of the block and 
                var y = Mathf.PerlinNoise(xCoord, yCoord);
                // Make the value more impactful
                y = Mathf.RoundToInt(y * 10);
                // Calculate the position of the Block
                var blockPos = new Vector3(transform.position.x + x, y, transform.position.z + z);

                // For cycle to generate different types of cubes base on chance
                for (int i = 0; i < 1/*BlockPrefab.Length*/; i++) //how many layers
                {
                    y--;
                    // Recalculate it to make it under the previous one
                    blockPos = new Vector3(transform.position.x + x - (ChunkSize /2), y, transform.position.z + z - (ChunkSize / 2));

                    // Pick random number
                    int pickBlock = Random.Range(0, 100);
                    if (pickBlock < 50) 
                        CreateBlock(blockPos, BlockPrefab[0]);
                    else if (50 < pickBlock && pickBlock < 70)
                        CreateBlock(blockPos, BlockPrefab[1]);
                    else if (70 < pickBlock && pickBlock < 95)
                        CreateBlock(blockPos, BlockPrefab[2]);
                    else if (95 < pickBlock)
                        CreateBlock(blockPos, BlockPrefab[3]);
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// Creates a block
    /// </summary>
    /// <param name="blockPos">Position of spawned block </param>
    /// <param name="prefab">Prefab of the block to spawn</param>
    void CreateBlock(Vector3 blockPos, GameObject prefab)
    {
        GameObject block = Instantiate(prefab, blockPos, Quaternion.identity);
        block.transform.SetParent(transform, false);
        block.transform.position = blockPos;
    }
}
