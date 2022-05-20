using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LayerType
{
    Unknown,
    ColordyClassic,
    Maze,
    ColorMaze, 
    ColordyRandomMulti,
    ColorMazeRandom,
    ColordyRandomSingle,
    DislocatedPlatformColordy,
    ColordyCollectables,
    PathOnlyMaze,
    Platform,
};

public enum WallType
{
    None,
    Regular,
    RandomHeight,
};

public enum WallGroundType
{
    None,
    Flat,
    Raised,
    RaisedRandomHeight,
};

/// <summery>
/// Contructor for a layer.
/// Not really ready yet, layer types and apperance still
/// evolving..
/// </summery>
/// <param name="type"> Either Maze, Colordy, Color Maze or Platform so far..</param>
/// <param name="y"> Y Position of the Layer.</param>
/// <param name="startX"> Start Position X for the Player.</param>
/// <param name="startZ"> Start position Z for the Player.</param>
/// <param name="dimensionX"> X dimension of the layer. </param>
/// <param name="dimensionZ"> Z dimension of the layer. </param>
/// <param name="hasWalls"> If true, walls are rendered.</param>
/// <param name="fillAllWithColors"> If true and type is Colordy, there are no gaps between the color tiles..</param>

[System.Serializable]
public class LayerParameters {

	public LayerType type = LayerType.ColordyClassic;
	public int dimensionX = 7;
	public int dimensionZ = 7;
    public int labOffsetX = 0;
    public int labOffsetZ = 0;
    public int numberOfFields = 47;
	public float wallHeightMin = 1f;
	public float wallHeightMax = 1f;
    public int startX = -1, startZ = -1;
    public int endX = -1, endZ = -1;
    public float y = 0;
    public float scale = 1;
    public float wallWidth = 0.1f;
    public Color32 pathColor = Color.green;
}
