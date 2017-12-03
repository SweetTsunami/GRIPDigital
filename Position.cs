using UnityEngine;
public class Position
{
    public int Size;

    public Vector3 WorldPosition;
    public Chunk Chunk;

    public Position(int size, Vector3 worldPosition)
    {
        Size = size;
        WorldPosition = worldPosition;
    }
}
