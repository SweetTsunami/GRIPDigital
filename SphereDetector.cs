using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generic Detector Script (Detects Generic Faction Objects)
/// </summary>
public class SphereDetector : MonoBehaviour
{
    // Detection parameters
    [SerializeField]
    [Range(0, 1000)]
    private float _detectionRange = 15.0f;
    public float DetectionRange { get { return _detectionRange; } }

    [SerializeField]
    [Range(0, 5)]
    private float _detectionRate = 0.1f;
    public float DetectionRate { get { return _detectionRate; }   }
        
    public List<Chunk> ChunksInRange = new List<Chunk>();
   
    // Debug
    [SerializeField] private bool _displayDetectionRange = true;

    private void Start()
    {
        // Start Automatic Chunk detection 
        InvokeRepeating("DetectChunksObjects", 0.0f, _detectionRate);
    }

    public List<Position> CheckPositionsInRange(List<Position> positions)
    {
        List<Position> positionsInRange = new List<Position>();
        foreach (var pos in positions)
        {// yeah so how do we make them spawn in negative ? or spawn player somewhere in the middle ?
            if (Vector3.Distance(pos.WorldPosition, transform.position) < _detectionRange)
            {
                positionsInRange.Add(pos);
            }
        }
        return positionsInRange;
    }

    /// <summary>
    /// Detects Faction Objects in Range
    /// </summary>
    public void DetectChunksObjects()
    {
        // Clear previous detected characters
        ChunksInRange.Clear();
        
        // Detect all characters in range
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

            //Check if the object has a faction
            var currentObject = hitColliders[i].GetComponent<Chunk>();

            //Organize the detected factions
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