/*
*SCRIPT MADE BY ALEXANDER SEMENOV

*USED FOR HIS OWN PEOJECT
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    public int chunkSize = 20;  

    public GameObject[] blockPrefab;
    
    public int heightOffset = 5;
    public int widthOffset = 5;

    public float scale = 25.0f;

    private void Awake()
    {        
        StartCoroutine(CreateChunk());
    }
    
    IEnumerator CreateChunk()
    {
        for (int x = 0; x <= chunkSize; x++)
        {
            for (int z = 0; z <= chunkSize; z++)
            {
                var xCoord = widthOffset + (transform.position.x + x + World.seed) / scale;
                var yCoord = heightOffset + (transform.position.z + z + World.seed) / scale;
                var value = Mathf.PerlinNoise(xCoord, yCoord);
                value = Mathf.RoundToInt(value * 10);
                var blockPos = new Vector3(transform.position.x + x, value, transform.position.z + z);
                for (int i = 0; i < 1/*blockPrefab.Length*/; i++) //how many layers
                {
                    value--;
                    blockPos = new Vector3(transform.position.x + x, value, transform.position.z + z);
                    int pickBlock = Random.Range(0, 100);
                    if (pickBlock < 50) 
                        CreateBlock(blockPos, blockPrefab[0]);
                    else if (50 < pickBlock && pickBlock < 70)
                        CreateBlock(blockPos, blockPrefab[1]);
                    else if (70 < pickBlock && pickBlock < 95)
                        CreateBlock(blockPos, blockPrefab[2]);
                    else if (95 < pickBlock)
                        CreateBlock(blockPos, blockPrefab[3]);
                }
            }
            yield return null;
        }
    }

    void CreateBlock(Vector3 blockPos, GameObject prefab)
    {
        GameObject block = Instantiate(prefab, blockPos, Quaternion.identity);
        block.transform.SetParent(transform, false);
        block.transform.position = blockPos;
    }


    //private void OnCollisionExit(Collision col)
    //{
    //    Debug.Log(this + " chunk exited detector ");
    //    if (col.gameObject.GetComponent<SphereDetector>())
    //    {
    //        Destroy(gameObject);
    //    }
    //}
}
