using UnityEngine;

/// <summary>
/// Position class to store the information about positions (Vector3) 
/// And also store information about chunk on this position
/// </summary>
public class Position
{
    // size of the position 
    public int Size;

    // position's location in world
    public Vector3 WorldPosition;

    // Information about chunk
    public Chunk Chunk;

    // Basic constructor
    public Position(int size, Vector3 worldPosition)
    {
        Size = size;
        WorldPosition = worldPosition;
    }
}
