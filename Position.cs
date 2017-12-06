/*  File:       Position.cs
 *  Creator:    Alexander Semenov
 *  Date:       December 2017 
 *  Location:   Brno, Czech Republic
 *  Project:    GRIP Digital Showcase project - Primitive Minecraft Clone
 *  Desc:       Pure Class used by Game.cs to determine the corect locations of chunks
 *  Usage:      Just include this in the project
 */

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
