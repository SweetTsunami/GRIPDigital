/*  File:       Block.cs
 *  Creator:    Alexander Semenov
 *  Date:       December 2017 
 *  Location:   Brno, Czech Republic
 *  Project:    GRIP Digital Showcase project - Primitive Minecraft Clone
 *  Desc:       Block handling which basically lets the block be destroyed
 *  Usage:      Put this into Block object
 *              Set health value proportional to expected block resistance
 *              Other values are used purely for texture generation            
 */

using UnityEngine;

public class Block : MonoBehaviour
{
    // Health pool of the brick
    public float Health;

    // This drains CPU like crazy, but fancy -> for real thing use pre-generated textures
    [Header("Texture from noise setup")]
    public bool UseNoise = false;
    public int NoiseSize = 100;
    public float NoiseScale = 50f;

    private void Start()
    {
        if (UseNoise)
        {
            Renderer rend = GetComponent<Renderer>();
            rend.material.mainTexture = GenerateTexture();
        }
    }

    /// <summary>
    /// Brick takes damage and if health is below 0 it dies
    /// </summary>
    /// <param name="damage">ammount of damage taken</param>
    public void ReceiveDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Generates the texture by using noise
    /// </summary>
    /// <returns>Returns generated texture</returns>
    private Texture2D GenerateTexture()
    {
        Texture2D tex = new Texture2D(NoiseSize, NoiseSize);

        // Nested cycle to generate the color for each pixel
        for (int x = 0;x < NoiseSize; x++)
        {
            for (int y = 0; y < NoiseSize; y++)
            {
                Color color = CalculateColor(x, y);
                tex.SetPixel(x, y, color);
            }
        }
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// Calculates the color for noise
    /// </summary>
    /// <param name="x">x coordinate of noise</param>
    /// <param name="y">y coordinate of noise</param>
    /// <returns>Returns the noise changed color</returns>
    private Color CalculateColor(int x, int y)
    {
        // Improved coords for noise
        float xCoord = (float)x / NoiseSize * NoiseScale;
        float yCoord = (float)y / NoiseSize * NoiseScale;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }
}
