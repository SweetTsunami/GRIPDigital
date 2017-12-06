/*  File:       SphereDetector.cs
 *  Creator:    Alexander Semenov
 *  Date:       December 2017 
 *  Location:   Brno, Czech Republic
 *  Project:    GRIP Digital Showcase project - Primitive Minecraft Clone
 *  Desc:       Spherical detector used by Game.cs to properly handle the
 *              Chunks within and out of view range
 *  Usage:      Put this onto same object as Game.cs      
 *              Set values, be careful with detection range as Chunk and thus Block generation
 *              relies on it, if you set it too high, too many Blocks will be generated.
 */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sphere detector script, for finding chunks and possibly other things
/// </summary>
public class SphereDetector : MonoBehaviour
{
    // Detection parameters
    // how far
    [SerializeField]
    [Range(0, 100)]
    private float _detectionRange = 15.0f;
    public float DetectionRange { get { return _detectionRange; } }

    // how often
    [SerializeField]
    [Range(0, 5)]
    private float _detectionRate = 0.1f;
    public float DetectionRate { get { return _detectionRate; }   }
        
    // list of chunks in the range
    public List<Chunk> ChunksInRange = new List<Chunk>();
   
    // Debug
    [SerializeField] private bool _displayDetectionRange = true;

    private void Start()
    {
        // Start Automatic Chunk detection 
        InvokeRepeating("DetectChunksObjects", 0.0f, _detectionRate);
    }

    /// <summary>
    /// Checks positions in range
    /// </summary>
    /// <param name="positions">List of positions</param>
    /// <returns>Returns the list of positions in range</returns>
    public List<Position> CheckPositionsInRange(List<Position> positions)
    {
        // List of positions in range (empty)
        List<Position> positionsInRange = new List<Position>();

        // cycles thorugh positions to fill the List positionsInRange
        foreach (var pos in positions)
        {
            // if the position is within detection range, add it to the list
            if (Vector3.Distance(pos.WorldPosition, transform.position) < _detectionRange)
            {
                positionsInRange.Add(pos);
            }
        }

        // return the list with positions
        return positionsInRange;
    }

    /// <summary>
    /// Detects Chunks Range
    /// </summary>
    public void DetectChunksObjects()
    {
        // Clear previous detected chunks
        ChunksInRange.Clear();

        // Detect all chunks in range
        var hitColliders = Physics.OverlapSphere(transform.position, _detectionRange);
        var i = 0;
        while (i < hitColliders.Length)
        {
            //Ignore self object
            if (hitColliders[i].gameObject == gameObject)
            {
                // Move to the next object
                i++;
                continue;
            }

            //Check if the object has a chunk
            var currentObject = hitColliders[i].GetComponent<Chunk>();

            //Organize the detected chunks
            if (currentObject != null)
            {
                ChunksInRange.Add(currentObject);
            }

            // Move to the next object
            i++;
        }
    }
    /// <summary>
    /// Executed on the draw gizmos event
    /// </summary>
    private void OnDrawGizmos()
    {
        // Shows the detection range in editor
        if (_displayDetectionRange)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);
        }
    }
}