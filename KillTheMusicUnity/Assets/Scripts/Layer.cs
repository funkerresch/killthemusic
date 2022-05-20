using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;
using System.Linq;

/// <summery>
/// This class holds all the data for one layer and the 
/// methods for random level generation 
/// and path finding.
/// </summery>
///

public enum Direction
{
	North,
	NorthEast,
	NorthWest,
	South,
	SouthEast,
	SouthWest,
	West,
	East
};

public enum CoordinateState
{
	none, // outside bounds
	free,
	occupied
};

public enum ElementType
{
	Unknown,
	None,
	Way,
	Wall
};

[System.Serializable]
public class MazeElement1
{
	public static int WAY = 0;
	public static int WALL = -1;
	public static int GOAL = -3;
	public static int START = 1;
	public static int PATH = 2;
	public static int NONE = 10;

	public int x;
	public int z;

	public enum State
	{
		none, // outside bounds
		free,
		occupied
	}

	public State state = State.free;
	public Color32 color;
    public bool colorAssigned = false;
	public float wayHeight = 1f;
	public float groundWallHeight = 0.7f;
	public float wallHeight = 2f;
	public int type = WALL;

	[System.NonSerialized]
	public GameObject ground = null;
	[System.NonSerialized]
	public GameObject northWall = null;
	[System.NonSerialized]
	public GameObject southWall = null;
	[System.NonSerialized]
	public GameObject westWall = null;
	[System.NonSerialized]
	public GameObject eastWall = null;
	[System.NonSerialized]
	public GameObject collectable = null;

	public MazeElement1(int type, int x, int z)
    {
		this.type = type;
		this.x = x;
		this.z = z;
    }	
}

[System.Serializable]
public class Layer {
    public LayerParameters parameters;
	public GameObject parent = null;

	[System.NonSerialized]
	public MazeElement1[,] maze1;

	[System.NonSerialized]
	public GameObject[] floorPlate;

	[System.NonSerialized]
	public int[,] path = null;

	[System.NonSerialized]
	public bool init = false;

	public bool initSharedColorMaterials = false;
	public Material[] sharedColorMaterials = new Material[25];

	[System.NonSerialized]
	public Material pathMat;
	[System.NonSerialized]
	public Material wallMat;
	[System.NonSerialized]
	public Material groundWall;

	public Material path2GoalMat;

	[System.NonSerialized]
	public Material ColorTextureMaterial;

	public bool timeOut = false;

	[System.NonSerialized]
	public List<Vector2Int> pathList;

	[System.NonSerialized]
	private int minNumberOfMoves;
	private int coordinates2OccupyWithColors = 25;

	RandomTileColor colorArray;
	public int collectableCount = 0;

	public static int numberOfCollectables = 25;

	public Layer(GameObject parent, LayerParameters parameters)
	{
		this.parameters = parameters;
		this.parent = parent;
		this.parent.transform.localScale = Vector3.one;
		colorArray = new RandomTileColor(25);
		floorPlate = new GameObject[25];
		SetMaterialDefaults();
		Generate();
	}

	private bool CoordinateIsWithinLayerBounds(int x, int z)
	{
		bool b = true;
		if (x < 0 || x >= parameters.dimensionX || z < 0 || z >= parameters.dimensionZ)
			b = false;

		return b;
    }

	private int GetAdjoiningElementType1(Direction d, int x, int z)
	{
		if (d == Direction.North)
		{
			if (z + 1 < parameters.dimensionZ)
				return maze1[x, z + 1].type;
			else
				return MazeElement1.NONE;
		}

		if (d == Direction.South)
		{
			if (z - 1 >= 0)
				return maze1[x, z - 1].type;
			else
				return MazeElement1.NONE;
		}

		if (d == Direction.West)
		{
			if (x - 1 >= 0)
				return maze1[x - 1, z].type;
			else
				return MazeElement1.NONE;
		}

		if (d == Direction.East)
		{
			if (x + 1 < parameters.dimensionX)
				return maze1[x + 1, z].type;
			else
				return MazeElement1.NONE;
		}
		return MazeElement1.NONE;
	}

	public Vector2Int GetAdjoiningCoordinate(Direction d, int x, int z)
	{
		Vector2Int coordinate = new Vector2Int(-1, -1);
		if (d == Direction.North)
		{
			if (z + 1 < parameters.dimensionZ)
			{
				coordinate.x = x;
				coordinate.y = z + 1;
				return coordinate;
			}
		}

		if (d == Direction.NorthWest)
		{
			if (z + 1 < parameters.dimensionZ && x - 1 >= 0)
			{
				coordinate.x = x - 1;
				coordinate.y = z + 1;
				return coordinate;
			}
		}

		if (d == Direction.NorthEast)
		{
			if (z + 1 < parameters.dimensionZ && x + 1 < parameters.dimensionX)
			{
				coordinate.x = x + 1;
				coordinate.y = z + 1;
				return coordinate;
			}
		}

		if (d == Direction.South)
		{
			if (z - 1 >= 0)
			{
				coordinate.x = x;
				coordinate.y = z - 1;
				return coordinate;
			}
		}

		if (d == Direction.SouthWest)
		{
			if (z - 1 >= 0 && x - 1 >= 0)
			{
				coordinate.x = x - 1;
				coordinate.y = z - 1;
				return coordinate;
			}
		}

		if (d == Direction.SouthEast)
		{
			if (z - 1 >= 0 && x + 1 < parameters.dimensionX)
			{
				coordinate.x = x + 1;
				coordinate.y = z - 1;
				return coordinate;
			}
		}

		if (d == Direction.West)
		{
			if (x - 1 >= 0)
			{
				coordinate.x = x - 1;
				coordinate.y = z;
				return coordinate;
			}
		}

		if (d == Direction.East)
		{
			if (x + 1 < parameters.dimensionX)
			{
				coordinate.x = x + 1;
				coordinate.y = z;
				return coordinate;
			}
		}

		return coordinate;
	}

	private MazeElement1.State GetAdjoiningCoordinateState1(Direction d, int x, int z)
	{
		Vector2Int coordinate = GetAdjoiningCoordinate(d, x, z);
		if (coordinate.x != -1)
			return maze1[coordinate.x, coordinate.y].state;
		else
            return MazeElement1.State.none;
	}

	bool AdjoingCoordinatesAreFree(int x, int z)
	{
		bool b = true;

		foreach (Direction d in (Direction[])Enum.GetValues(typeof(Direction)))
		{
			if (GetAdjoiningCoordinateState1(d, x, z) != MazeElement1.State.free)
				b = false;
		}
		return b;
	}

	void OccupyAdjoiningCoordinates(int x, int z, int depth)
	{
		if (depth > 0)
		{
			if (CoordinateIsWithinLayerBounds(x, z))
				maze1[x, z].state = MazeElement1.State.occupied;

			foreach (Direction d in (Direction[])Enum.GetValues(typeof(Direction)))
			{
				Vector2Int coordinate = GetAdjoiningCoordinate(d, x, z);
				if (CoordinateIsWithinLayerBounds(coordinate.x, coordinate.y))
				{
					maze1[coordinate.x, coordinate.y].state = MazeElement1.State.occupied;
					OccupyAdjoiningCoordinates(coordinate.x, coordinate.y, depth - 1);
				}
			}
		}
	}

	void GenerateDislocatedColordy(int startX = -1, int startZ = -1)
	{
		maze1 = new MazeElement1[parameters.dimensionX, parameters.dimensionZ];

		for (int i = 0; i < parameters.dimensionX; i++)
			for (int j = 0; j < parameters.dimensionZ; j++)
				maze1[i, j] = new MazeElement1(MazeElement1.NONE, i, j);

		System.Random rand = new System.Random();
		int coordinates2Occupy = 25;
		
		int currentX;
		int currentZ;

		while(coordinates2Occupy > 0)
		{
			currentX = rand.Next(1, parameters.dimensionX-1);
			currentZ = rand.Next(1, parameters.dimensionZ-1);

			if (AdjoingCoordinatesAreFree(currentX, currentZ))
			{
				OccupyAdjoiningCoordinates(currentX, currentZ, 1);
				coordinates2Occupy--;
				maze1[currentX, currentZ].type = MazeElement1.WAY;
			}
		}
	}

	public void ScalePlayground(float scale)
	{
		if (scale > 0)
		{
			Vector3 xzScale = new Vector3();
			xzScale = parent.transform.localScale;
			xzScale.x = scale;
			xzScale.z = scale;
			parent.transform.localScale = xzScale;
		}
    }

	public void LoadSharedColorMaterials()
	{
		for (int i = 0; i < 25; i++)
		{
			string material = "COLORMAT" + i;
			sharedColorMaterials[i] = Resources.Load("Materials/" + material, typeof(Material)) as Material;
			//Debug.Log(material);
		}
	}

	public void SetPath2GoalMaterial(string material)
	{
		path2GoalMat = Resources.Load("Materials/" + material, typeof(Material)) as Material;
	}

	public void SetGroundMaterial(string material)
	{
        pathMat = Resources.Load("Materials/" + material, typeof(Material)) as Material;
	}

	public void SetWallMaterial(string material)
	{
		wallMat = Resources.Load("Materials/" + material, typeof(Material)) as Material;
	}

	public void SetGroundWallMaterial(string material)
	{
		groundWall = Resources.Load("Materials/" + material, typeof(Material)) as Material;
	}

	public void SetMaterialDefaults()
	{
		//SetGroundMaterial("MARBLEGROUND");
		SetGroundMaterial("SIMPLEBLACK");
		SetWallMaterial("SIMPLEPURPLE");
		SetGroundWallMaterial("SIMPLEBLACK");
		SetPath2GoalMaterial("SIMPLETEAL2");
	}	

	public void Generate()
    {
		LoadSharedColorMaterials();

        if (parameters.type == LayerType.Maze)
        {
			GenerateMaze1(parameters.startX, parameters.startZ);
			pathList = new List<Vector2Int>();
            findShortestPath(parameters.startX, parameters.startZ);
			AttachGameObjects();
			AssignMaterials();
		}

		if (parameters.type == LayerType.ColorMaze)
		{
			GenerateMaze1(parameters.startX, parameters.startZ);
			pathList = new List<Vector2Int>();
			findShortestPath(parameters.startX, parameters.startZ);
			AttachGameObjects();
			AssignMaterials();
			AssignColors();
		}

		if (parameters.type == LayerType.ColorMazeRandom)
		{
			GenerateMaze1(parameters.startX, parameters.startZ);
			pathList = new List<Vector2Int>();
			findShortestPath(parameters.startX, parameters.startZ);
			AssignColors();
			AttachGameObjects();
			AttachCollectableObjects();
			AssignMaterials();			
		}

		if (parameters.type == LayerType.ColordyClassic)
        {
			CheckColordyClassicDimensions();
			GeneratePlatform(parameters.startX, parameters.startZ);
			AttachGameObjects();
			AssignMaterials();
			AssignColors();
		}

		if (parameters.type == LayerType.ColordyRandomMulti)
		{
			GeneratePlatform(parameters.startX, parameters.startZ);
			AttachGameObjects();
			AssignMaterials();
			AssignColors();
		}

		if (parameters.type == LayerType.Platform)
        {
            GeneratePlatform(parameters.startX, parameters.startZ);
			AttachGameObjects();
			AssignMaterials();
		}

		if (parameters.type == LayerType.DislocatedPlatformColordy)
		{
			GenerateDislocatedColordy();
			AttachGameObjects();
			AssignMaterials();
			AssignColors();
		}

		if (parameters.type == LayerType.ColordyCollectables)
		{
			GeneratePlatform((int)Mathf.Floor(parameters.dimensionX/2), 1);
			AttachGameObjects();
			AssignMaterials();
			AssignColors();
			AttachCollectableObjects();
		}
	}

	/// <summery>
	/// Assings random wall heights.
	/// </summery>

	public void AssignWallHeights(float min, float max)
	{
		for (int i = 0; i < parameters.dimensionX; i++) {
			for (int j = 0; j < parameters.dimensionZ; j++) {

				float height = UnityEngine.Random.value;
				float range = max - min;
				height *= range;
				height += min;
				maze1 [i, j].wallHeight = height;
			}
		}
	}

	public void AssignMaterials()
	{
		Renderer rend;
		for (int x = 0; x < parameters.dimensionX; x++)
		{
			for (int z = 0; z < parameters.dimensionZ; z++)
			{
				if (maze1[x, z].type == MazeElement1.WALL)
				{
					if (wallMat)
					{
						if (maze1[x, z].northWall)
						{
							rend = maze1[x, z].northWall.GetComponent<Renderer>();
							rend.material = wallMat;
						}

						if (maze1[x, z].southWall)
						{
							rend = maze1[x, z].southWall.GetComponent<Renderer>();
							rend.material = wallMat;
						}

						if (maze1[x, z].westWall)
						{
							rend = maze1[x, z].westWall.GetComponent<Renderer>();
							rend.material = wallMat;
						}

						if (maze1[x, z].eastWall)
						{
							rend = maze1[x, z].eastWall.GetComponent<Renderer>();
							rend.material = wallMat;
						}
					}
				}
				else
				{
					if (pathMat)
					{
						if (maze1[x, z].ground)
						{
							rend = maze1[x, z].ground.GetComponent<Renderer>();
							rend.material = pathMat;
						}
					}
				}
			}
		}
	}

	public void CreateClassicColordyTexture()
	{
		int colorIndex = 0;
		// Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
		var texture1 = new Texture2D(5, 5, TextureFormat.ARGB32, true, true);
		texture1.wrapMode = TextureWrapMode.Clamp;
		texture1.filterMode = FilterMode.Point;

		for (int x = 0; x < 5; x++)
		{
			for (int z = 0; z < 5; z++)
			{	
				texture1.SetPixel(x, z, colorArray.AtIndex(colorIndex % 25));
				colorIndex++;	
			}
		}

		texture1.Apply();
		ColorTextureMaterial = new Material(Shader.Find("Standard"));
		ColorTextureMaterial.mainTexture = texture1;
	}

	public void CreateColorMazeRandomTexture()
	{
		// Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
		var texture1 = new Texture2D(parameters.dimensionX - 2, parameters.dimensionZ - 2, TextureFormat.ARGB32, true, true);
		texture1.wrapMode = TextureWrapMode.Clamp;
		texture1.filterMode = FilterMode.Point;

		for (int x = 1; x < parameters.dimensionX - 1; x++)
		{
			for (int z = 1; z < parameters.dimensionZ - 1; z++)
			{
			/*	if (maze1[x, z].colorAssigned && (maze1[x,z].type == MazeElement1.WAY))
				{
					texture1.SetPixel(x-1, z-1, maze1[x,z].color);
				}
				else
			*/
					texture1.SetPixel(x-1, z-1, Color.black);
			}
		}

		// Apply all SetPixel calls
     
		texture1.Apply();
		// connect texture to material of GameObject this script is attached to
		ColorTextureMaterial = new Material(Shader.Find("Standard"));
		ColorTextureMaterial.mainTexture = texture1;
	}

	public void CreateColorTexture()
	{
		int colorIndex = 0;
		// Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
		var texture1 = new Texture2D(parameters.dimensionX-2, parameters.dimensionZ-2, TextureFormat.ARGB32, true, true);
		texture1.wrapMode = TextureWrapMode.Clamp;
		texture1.filterMode = FilterMode.Point;

		for (int x = 0; x < parameters.dimensionX-2; x++)
		{
			for (int z = 0; z < parameters.dimensionZ-2; z++)
			{
				if (maze1[x, z].type == MazeElement1.WAY)
				{
					texture1.SetPixel(x, z, colorArray.AtIndex(colorIndex % 25));
					colorIndex++;
				}
				else
					texture1.SetPixel(x, z, Color.white);
			}
		}

		// Apply all SetPixel calls
		texture1.Apply();
		// connect texture to material of GameObject this script is attached to
		ColorTextureMaterial = new Material(Shader.Find("Standard"));
		ColorTextureMaterial.mainTexture = texture1;
	}

	private void CheckColordyClassicDimensions()
	{
		if (parameters.dimensionX != parameters.dimensionZ)
		{
			parameters.dimensionX = 7;
			parameters.dimensionZ = 7;
        }

		if ((parameters.dimensionX - 2) % 5 != 0)
		{
			parameters.dimensionX = 7;
			parameters.dimensionZ = 7;
		}

		if ((parameters.dimensionZ - 2) % 5 != 0)
		{
			parameters.dimensionX = 7;
			parameters.dimensionZ = 7;
		}
	}

	public void AssignColors2Neighbours(int x, int z, int stride, Color color)
	{
		for (int i = x; i < (x + stride); i++) {
			for (int j = z; j < (z + stride); j++) {
				maze1[i, j].color = color;
				maze1[i, j].colorAssigned = true;
			}
        }
    }

	public void AssignColors()
	{
		int colorIndex = 0;
		int stride = (parameters.dimensionX - 2) / 5;

		if (parameters.type == LayerType.ColordyClassic)
		{	
			for (int x = 1; x < parameters.dimensionX - 1; x+=stride)
			{
				for (int z = 1; z < parameters.dimensionZ - 1; z+=stride)
				{
				    AssignColors2Neighbours(x, z, stride, colorArray.AtIndex(colorIndex % 25));
					colorIndex++;
				}
			}
		}

		if (parameters.type == LayerType.ColorMazeRandom)
		{
			System.Random rand = new System.Random();
			coordinates2OccupyWithColors = 25;

			int x;
			int z;

			while (coordinates2OccupyWithColors > 0)
			{
				x = rand.Next(1, parameters.dimensionX - 1);
				z = rand.Next(1, parameters.dimensionZ - 1);

				if ((!maze1[x, z].colorAssigned) && (maze1[x, z].type == MazeElement1.WAY))
				{
					maze1[x, z].color = colorArray.AtIndex(colorIndex % 25);
					maze1[x, z].colorAssigned = true;
					colorIndex++;
					coordinates2OccupyWithColors--;
				}
			}
		}

		if (parameters.type == LayerType.ColorMaze)
		{
			for (int x = 0; x < parameters.dimensionX; x++)
			{
				for (int z = 0; z < parameters.dimensionZ; z++)
				{
					if (maze1[x, z].type == MazeElement1.WAY)
					{
						maze1[x, z].color = colorArray.AtIndex(colorIndex % 25);
						if (maze1[x, z].ground)
						{
							Renderer rend = maze1[x, z].ground.GetComponent<Renderer>();
							rend.material.SetColor("_Color", maze1[x, z].color);
							rend.material.SetColor("_EmissionColor", maze1[x, z].color);
						}
						maze1[x, z].colorAssigned = true;

						colorIndex++;
					}
				}
			}
		}	

		if (parameters.type == LayerType.ColordyRandomMulti || parameters.type == LayerType.ColordyCollectables)
		{
			System.Random rand = new System.Random();
			int coordinates2Occupy = 25;

			int x;
			int z;

			while (coordinates2Occupy > 0)
			{
				x = rand.Next(1, parameters.dimensionX - 1);
				z = rand.Next(1, parameters.dimensionZ - 1);

				if (!maze1[x, z].colorAssigned && maze1[x, z].type == MazeElement1.WAY )
				{
					maze1[x, z].color = colorArray.AtIndex(colorIndex % 25);
					/*if (!maze1[x, z].ground)
						AttachGroundCube(x, z);
					Renderer rend = maze1[x, z].ground.GetComponent<Renderer>();
					rend.material.SetColor("_Color", maze1[x, z].color);
					rend.material.SetColor("_EmissionColor", maze1[x, z].color);*/
					maze1[x, z].colorAssigned = true;
					colorIndex++;
					coordinates2Occupy--;
				}
			}
		}

		if (parameters.type == LayerType.Unknown)
		{
			

		}
	}

	private void SetElementHeight(GameObject element, float height)
	{
		Vector3 scale = element.transform.localScale;
		scale.y = height;
		element.transform.localScale = scale;

		Vector3 position = element.transform.position;
		if (height > 1f)
			position.y = (height - parameters.y) * 0.5f;
		else
			position.y = parameters.y;

		element.transform.position = position;
	}

	public void SetGroundWallHeight(float height)
	{
		if (maze1 == null)
			return;

		for (int x = 0; x < parameters.dimensionX; x++)
		{
			for (int z = 0; z < parameters.dimensionZ; z++)
			{
				if (maze1[x, z] != null)
				{
					if (maze1[x, z].type == MazeElement1.WALL)
					{
						if (maze1[x, z].ground)
							SetElementHeight(maze1[x, z].ground, height);
					}
				}
			}
		}
	}

	public void SetWallHeight(float height)
	{
		if (maze1 == null)
			return;

		for (int x = 0; x < parameters.dimensionX; x++)
		{
			for (int z = 0; z < parameters.dimensionZ; z++)
			{
				if (maze1[x, z] != null)
				{
					if (maze1[x, z].type == MazeElement1.WALL)
					{
						if (maze1[x, z].northWall)
							SetElementHeight(maze1[x, z].northWall, height);

						if (maze1[x, z].southWall)
							SetElementHeight(maze1[x, z].southWall, height);

						if (maze1[x, z].westWall)
							SetElementHeight(maze1[x, z].westWall, height);

						if (maze1[x, z].eastWall)
							SetElementHeight(maze1[x, z].eastWall, height);
					}
				}
			}
		}
	}

	public Material GetMaterialByColor(Color color)
	{
		for (int i = 0; i < 25; i++)
		{
			if (sharedColorMaterials[i].color == color)
				return sharedColorMaterials[i];
		}
		return sharedColorMaterials[0];
	}

	public void GenerateCollectableCube(int x, int z)
	{
		float renderPositionX = x + parameters.labOffsetX;
		float renderPositionZ = z + parameters.labOffsetZ;

		if (maze1[x, z].colorAssigned)
		{
			maze1[x, z].collectable = GameObject.CreatePrimitive(PrimitiveType.Cube);
			//maze1[x, z].collectable.transform.hideFlags = HideFlags.HideInHierarchy;
			maze1[x, z].collectable.transform.position = new Vector3(renderPositionX, parameters.y+1f, renderPositionZ);
			maze1[x, z].collectable.transform.localScale = new Vector3(1f, 1f, 1f);
			maze1[x, z].collectable.transform.parent = parent.transform;

			//maze1[x, z].collectable.GetComponent<BoxCollider>().isTrigger = false;


			maze1[x, z].collectable.tag = "CollectableCube";
			BoxCollider col = maze1[x, z].collectable.GetComponent<BoxCollider>();
			//col.isTrigger = true;
			Renderer rend = maze1[x, z].collectable.GetComponent<Renderer>();
			rend.sharedMaterial = GetMaterialByColor(maze1[x, z].color);
			//rend.material.SetColor("_Color", maze1[x, z].color);
			//rend.material.SetColor("_EmissionColor", maze1[x, z].color);
		}
	}

	public void AttachGroundCube(int x, int z)
	{
		float renderPositionX = x + parameters.labOffsetX;
		float renderPositionZ = z + parameters.labOffsetZ;

		maze1[x, z].ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//maze1[x, z].ground.transform.hideFlags = HideFlags.HideInHierarchy;
		maze1[x, z].ground.transform.position = new Vector3(renderPositionX, parameters.y, renderPositionZ);
		maze1[x, z].ground.transform.localScale = new Vector3(1f, maze1[x, z].wayHeight, 1f);
		maze1[x, z].ground.transform.parent = parent.transform;		
	}

	public void AttachWallCube(int x, int z)
	{
		float renderPositionX = x + parameters.labOffsetX;
		float renderPositionZ = z + parameters.labOffsetZ;
		Vector3 p = new Vector3();

		maze1[x, z].ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//maze1[x, z].ground.transform.hideFlags = HideFlags.HideInHierarchy;
		maze1[x, z].ground.transform.position = new Vector3(renderPositionX, parameters.y, renderPositionZ);
		maze1[x, z].ground.transform.localScale = new Vector3(1f, maze1[x, z].groundWallHeight, 1f);
		maze1[x, z].ground.transform.parent = parent.transform;
		maze1[x, z].ground.tag = "BaseWall";

		Renderer rend = maze1[x, z].ground.GetComponent<Renderer>();
		rend.sharedMaterial = path2GoalMat;

		foreach (Direction d in (Direction[])Enum.GetValues(typeof(Direction)))
		{
			if (d == Direction.North || d == Direction.South || d == Direction.West || d == Direction.East)
			{
				if (GetAdjoiningElementType1(d, x, z) == MazeElement1.WALL)
				{
                    if (d == Direction.North)
                    {
                        maze1[x, z].northWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
						maze1[x, z].northWall.tag = "NorthWall";
						//maze1[x, z].northWall.transform.hideFlags = HideFlags.HideInHierarchy;
						maze1[x, z].northWall.transform.parent = parent.transform;

                        p.x = renderPositionX - parameters.wallWidth * 0.5f;
                        p.y = parameters.y;
                        p.z = renderPositionZ + 0.25f;

                        maze1[x, z].northWall.transform.position = p;
                        maze1[x, z].northWall.transform.localScale = new Vector3(parameters.wallWidth, maze1[x, z].wallHeight, 0.5f);

                    }

                    if (d == Direction.South)
                    {
                        maze1[x, z].southWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
						maze1[x, z].southWall.tag = "SouthWall";
						//maze1[x, z].southWall.transform.hideFlags = HideFlags.HideInHierarchy;
						maze1[x, z].southWall.transform.parent = parent.transform;

                        p.x = renderPositionX - parameters.wallWidth * 0.5f;
                        p.y = parameters.y;
                        p.z = renderPositionZ - 0.25f;

                        maze1[x, z].southWall.transform.position = p;
                        maze1[x, z].southWall.transform.localScale = new Vector3(parameters.wallWidth, maze1[x, z].wallHeight, 0.5f);
                    }

                    if (d == Direction.West)
                    {
                        maze1[x, z].westWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
						maze1[x, z].westWall.tag = "WestWall";
						//maze1[x, z].westWall.transform.hideFlags = HideFlags.HideInHierarchy;
						maze1[x, z].westWall.transform.parent = parent.transform;

                        p.x = renderPositionX - 0.25f;
                        p.y = parameters.y;
                        p.z = renderPositionZ - parameters.wallWidth * 0.5f;

                        maze1[x, z].westWall.transform.position = p;
                        maze1[x, z].westWall.transform.localScale = new Vector3(0.5f, maze1[x, z].wallHeight, parameters.wallWidth);
                    }

                    if (d == Direction.East)
                    {
                        maze1[x, z].eastWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
						maze1[x, z].eastWall.tag = "EastWall";
						//maze1[x, z].eastWall.transform.hideFlags = HideFlags.HideInHierarchy;
						maze1[x, z].eastWall.transform.parent = parent.transform;

                        p.x = renderPositionX + 0.25f;
                        p.y = parameters.y;
                        p.z = renderPositionZ - parameters.wallWidth * 0.5f;

                        maze1[x, z].eastWall.transform.position = p;
                        maze1[x, z].eastWall.transform.localScale = new Vector3(0.5f, maze1[x, z].wallHeight, parameters.wallWidth);
                    }
                }
			}
		}	
	}

	/// <summery> Generates Primitive Cube elements for the layer.
	/// </summery>

	public void AttachGameObjects()
	{
		if (parameters.type == LayerType.ColordyClassic)
		{
			float renderPositionX = parameters.dimensionX * 0.5f -0.5f;
			float renderPositionZ = parameters.dimensionZ * 0.5f -0.5f;

			floorPlate[0] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			floorPlate[0].transform.position = new Vector3(renderPositionX, parameters.y, renderPositionZ);
			floorPlate[0].transform.localScale = new Vector3(parameters.dimensionX - 2, 1f, parameters.dimensionZ - 2);
			Renderer rend = floorPlate[0].GetComponent<Renderer>();
			CreateClassicColordyTexture();
			rend.material = ColorTextureMaterial;

			//maze1[x, z].ground.transform.hideFlags = HideFlags.HideInHierarchy;

			floorPlate[0].transform.parent = parent.transform;

			for (int x = 0; x < parameters.dimensionX; x++)
			{
				for (int z =0; z < parameters.dimensionZ; z++)
				{
					if (maze1[x, z].type == MazeElement1.WALL)
						AttachWallCube(x, z);
				}
			}
		}

        else if (parameters.type == LayerType.ColorMazeRandom)
		{
			float renderPositionX = (parameters.dimensionX-2) * 0.5f + 0.5f;
			float renderPositionZ = (parameters.dimensionZ-2) * 0.5f +0.5f;

			floorPlate[0] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			floorPlate[0].transform.position = new Vector3(renderPositionX, parameters.y, renderPositionZ);
			floorPlate[0].transform.localScale = new Vector3(parameters.dimensionX - 2, 1f, parameters.dimensionZ - 2);
			Renderer rend = floorPlate[0].GetComponent<Renderer>();
			CreateColorMazeRandomTexture();
			rend.material = ColorTextureMaterial;
			floorPlate[0].transform.Rotate(0, 180, 0);

			//maze1[x, z].ground.transform.hideFlags = HideFlags.HideInHierarchy;

			floorPlate[0].transform.parent = parent.transform;

			for (int x = 0; x < parameters.dimensionX; x++)
			{
				for (int z = 0; z < parameters.dimensionZ; z++)
				{
					if (maze1[x, z].type == MazeElement1.WALL)
						AttachWallCube(x, z);
				}
			}
		}

		else if (parameters.type == LayerType.ColordyCollectables)
		{
			for (int i = 0; i < 25; i++)
			{
				float renderPositionX = (parameters.dimensionX - 2f) * 0.5f + 0.5f;
				float renderPositionZ = (9 - 2f) * 0.5f + 0.5f + (i * 9);

				floorPlate[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				floorPlate[i].transform.position = new Vector3(renderPositionX, parameters.y, renderPositionZ);
				floorPlate[i].transform.localScale = new Vector3(parameters.dimensionX - 2,1f, 9);
				Renderer rend = floorPlate[i].GetComponent<Renderer>();
				//CreateColorTexture();
				rend.material = pathMat;

				//maze1[x, z].ground.transform.hideFlags = HideFlags.HideInHierarchy;

				floorPlate[i].transform.parent = parent.transform;
			}

			for (int x = 0; x < parameters.dimensionX; x++)
			{
				for (int z = 1; z < parameters.dimensionZ-1; z++)
				{
					if (maze1[x, z].type == MazeElement1.WALL)
						AttachWallCube(x, z);
				}
			}
		}
		else
		{
			for (int x = 0; x < parameters.dimensionX; x++)
			{
				for (int z = 0; z < parameters.dimensionZ; z++)
				{
					if (maze1[x, z].type == MazeElement1.WALL)
						AttachWallCube(x, z);
					else
						AttachGroundCube(x, z);
				}
			}
		}
	}

	public void AttachCollectableObjects()
	{
		collectableCount = 0;
		for (int x = 1; x < parameters.dimensionX-1; x++)
		{
			for (int z = 1; z < parameters.dimensionZ-1; z++)
			{
				if (maze1[x, z].colorAssigned)
					GenerateCollectableCube(x, z);
			}
		}
	}

	/// <summery>
	/// Generates a random maze of dimension X and Z.
	/// </summery>

	void SetStartingPoint(int startX, int startZ)
	{
		System.Random rand = new System.Random();

		if (startX == -1)
		{
			parameters.startX = rand.Next(parameters.dimensionX);
			while (parameters.startX % 2 == 0)
			{
				parameters.startX = rand.Next(parameters.dimensionX);
			}
		}
		else
			parameters.startX = startX;

        if (startZ == -1)
        {
            parameters.startZ = rand.Next(parameters.dimensionZ);
            while (parameters.startZ % 2 == 0)
            {
                parameters.startZ = rand.Next(parameters.dimensionZ);
            }
        }
        else
            parameters.startZ = startZ;
    }

	/// <summery>
	/// Recursion for the maze generation.
	/// </summery>

	private void recursion1(int x, int z)
	{
		int[] directions = new int[] { 1, 2, 3, 4 };

		parameters.endX = x;
		parameters.endZ = z;

		Shuffle(directions);

		for (int i = 0; i < directions.Length; i++)
		{
			switch (directions[i])
			{
				case 1: // Up
						//　Whether 2 cells up is out or not
					if (x - 2 <= 0)
						continue;
					if (maze1[x - 2, z].type != MazeElement1.WAY)
					{
						maze1[x - 2, z].type = MazeElement1.WAY;
						maze1[x - 1, z].type = MazeElement1.WAY;
						recursion1(x - 2, z);
					}
					break;
				case 2: // Right
						// Whether 2 cells to the right is out or not
					if (z + 2 >= parameters.dimensionZ - 1)
						continue;
					if (maze1[x, z + 2].type != MazeElement1.WAY)
					{
						maze1[x, z + 2].type = MazeElement1.WAY;
						maze1[x, z + 1].type = MazeElement1.WAY;
						recursion1(x, z + 2);
					}
					break;
				case 3: // Down
						// Whether 2 cells down is out or not
					if (x + 2 >= parameters.dimensionX - 1)
						continue;
					if (maze1[x + 2, z].type != MazeElement1.WAY)
					{
						maze1[x + 2, z].type = MazeElement1.WAY;
						maze1[x + 1, z].type = MazeElement1.WAY;
						recursion1(x + 2, z);
					}
					break;
				case 4: // Left
						// Whether 2 cells to the left is out or not
					if (z - 2 <= 0)
						continue;
					if (maze1[x, z - 2].type != MazeElement1.WAY)
					{
						maze1[x, z - 2].type = MazeElement1.WAY;
						maze1[x, z - 1].type = MazeElement1.WAY;
						recursion1(x, z - 2);
					}
					break;
			}
		}
	}

	void GenerateMaze1(int startX = -1, int startZ = -1)
	{
		maze1 = new MazeElement1[parameters.dimensionX, parameters.dimensionZ];

		for (int i = 0; i < parameters.dimensionX; i++)
			for (int j = 0; j < parameters.dimensionZ; j++)
				maze1[i, j] = new MazeElement1(MazeElement1.WALL, i, j);

		SetStartingPoint(startX, startZ);

		maze1[parameters.startX, parameters.startZ].type = MazeElement1.WAY;

		recursion1(parameters.startX, parameters.startZ);

		init = true;
		maze1[parameters.startX, parameters.startZ].type = MazeElement1.START;
		maze1[parameters.endX, parameters.endZ].type = MazeElement1.GOAL;
	}

	/// <summery>
	/// Generates a platform of dimension X and Z.
	/// </summery>

	void GeneratePlatform (int startX = -1, int startZ = -1)
	{
		SetStartingPoint(startX, startZ);

		maze1 = new MazeElement1[parameters.dimensionX, parameters.dimensionZ];

		for (int i = 0; i < parameters.dimensionX; i++)
			for (int j = 0; j < parameters.dimensionZ; j++)
				maze1[i, j] = new MazeElement1(MazeElement1.WAY, i, j);


		for (int x = 0; x < parameters.dimensionX; x++) {
			maze1 [x, 0].type = MazeElement1.WALL;
			maze1 [x, parameters.dimensionZ -1].type = MazeElement1.WALL;
		}

		for (int z = 0; z < parameters.dimensionZ; z++) {
			maze1 [0, z].type = MazeElement1.WALL;
			maze1 [parameters.dimensionX -1, z].type = MazeElement1.WALL;
		}
	
		maze1 [parameters.startX, parameters.startZ].type = MazeElement1.START;

        parameters.endX = UnityEngine.Random.Range (1, parameters.dimensionX - 2);
        parameters.endZ = UnityEngine.Random.Range (1, parameters.dimensionZ - 2);

		maze1 [parameters.endX, parameters.endZ].type = MazeElement1.GOAL;
	}

	/// <summery>
	/// Fisher Yates Shuffle.
	/// Changes maze generation direction randomly for
	/// every recursion step.
	/// </summery>

	private void Shuffle<T> (T[] array)
	{
		System.Random _random = new System.Random ();
		for (int i = array.Length; i > 1; i--) {
			// Pick random element to swap.
			int j = _random.Next (i); // 0 <= j <= i-1
			// Swap.
			T tmp = array [j];
			array [j] = array [i - 1];
			array [i - 1] = tmp;
		}
		Thread.Sleep (10);
	}

	/// <summery>
	/// Possible moves for the path finding algorithm
	/// </summery>

	int[][] moves = {
		new int[] { -1, 0 },
		new int[] { 0, -1 },
		new int[] { 0, 1 },
		new int[] { 1, 0 } };

	/// <summery>
	/// Validates new position.
	/// </summery>

	public bool IsValidPos(int newX, int newZ)
	{
		if (newX < 0) return false;
		if (newZ < 0) return false;
		if (newX >= parameters.dimensionX) return false;
		if (newZ >= parameters.dimensionZ) return false;
		return true;
	}

	bool IsValidPos(int[,] array, int newX, int newZ)
	{
		if (newX < 0) return false;
		if (newZ < 0) return false;
		if (newX >= parameters.dimensionX) return false;
		if (newZ >= parameters.dimensionZ) return false;
		return true;
	}

	/// <summery>
	/// Recursion for finding the shortest path from
	/// start to goal.
	/// </summery>

	public int Move(int[,] arrayTemp, List<Vector2Int> listTmp, int x, int z, int count, ref int lowest)
	{
		int[,] testPath = arrayTemp.Clone () as int[,];
		List<Vector2Int> pathListTmp = new List<Vector2Int>();
		foreach (Vector2Int item in listTmp)
			pathListTmp.Add (new Vector2Int (item.x, item.y));
		
		int value = testPath[x, z];

		if (value >= 1)
		{			
			foreach (var movePair in moves)
			{
				int newX = x + movePair[0];
				int newZ = z + movePair[1];
				if (IsValidPos(testPath,  newX, newZ))
				{
					int testValue = testPath[newX, newZ];
					if (testValue == 0)
					{
						int[,] tmp = testPath.Clone () as int[,];
						List<Vector2Int> tmp2 = new List<Vector2Int>();
						foreach (Vector2Int item in pathListTmp)
							tmp2.Add (new Vector2Int (item.x, item.y));
						
						tmp[newX, newZ] = value + 1;
						tmp2.Add (new Vector2Int (newX, newZ));
						Move(tmp, tmp2, newX, newZ, count + 1, ref lowest);

					}
					else if (testValue == -3)
					{
						if (count + 1 < lowest)
						{
							lowest = count + 1;
							path = testPath.Clone() as int[,];

							foreach (Vector2Int item in pathListTmp)
								pathList.Add (new Vector2Int (item.x, item.y));

							Debug.Log ("Found new shortest path");
						}
						return 1;
					}
				}
			}
		}
		return -1;
	}

	public void removeFirst()
	{
		pathList.RemoveAt (0);
	}

	public void addAtFront(int x, int z)
	{
		pathList.Insert(0, new Vector2Int(x, z));
	}

	public void findShortestPath(int x, int z)
	{	
		pathList.Clear ();
		int[,] mazeCopyAsIntArray = new int[parameters.dimensionX, parameters.dimensionZ];
		for (int xx = 0; xx < parameters.dimensionX; xx++)
		{
			for (int zz = 0; zz < parameters.dimensionZ; zz++)
				mazeCopyAsIntArray[xx, zz] = maze1[xx, zz].type;
        }

		mazeCopyAsIntArray[parameters.startX, parameters.startZ] = MazeElement1.WAY;
        parameters.startX = x;
        parameters.startZ = z;
		mazeCopyAsIntArray[x, z] = MazeElement1.START;
		minNumberOfMoves = int.MaxValue;
		Move (mazeCopyAsIntArray, pathList, x, z, 0, ref minNumberOfMoves);		
	}
}
	