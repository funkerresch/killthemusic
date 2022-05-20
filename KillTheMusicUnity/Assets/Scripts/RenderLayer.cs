using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine.SceneManagement;

/// <summery>
/// Singleton Class GameState for saving the global Game State.
/// Holds all data of the currently loaded level including
/// </summery>
///

public class CamSettings
{
	public Vector3 offset;
	public float xRotation;
	public float fov;

	public CamSettings(Vector3 offset, float xRotation, float fov)
	{
		this.offset = offset;
		this.xRotation = xRotation;
		this.fov = fov;
    }
}

[ExecuteInEditMode]
public class RenderLayer : MonoBehaviour
{
	public static RenderLayer state = null;
    public Layer layer;
	public LayerParameters parameters = null;
	public Vector3 playerPosition;
	public Camera overviewCam;
	public Camera playerCam;
	public GameObject player;
	public GameObject mazeParent = null;
	public bool showPath = false;

	public int dimensionX = 7;
	public int dimensionZ = 7;
	public float scale = 1f;
	public float groundWallHeight = 0.5f;
	public float wallHeight = 1f;
	public LayerType layerType = LayerType.ColordyClassic;
	public bool moveCollectables = true;
	private float timer;
	private float rate = 0.3f;
	private GameObject[] Collectables;
	private System.Random randomCollectable = new System.Random();
	private System.Random randomDirection = new System.Random();

	public CamSettings[] camSettings;
	public CamSettings currentCamSettings;

	CamSettings camSettings0 = new CamSettings(new Vector3(0, 19f, -16f), 50.5f, 12f);
	CamSettings camSettings1 = new CamSettings(new Vector3(0, 4.8f, -16f), 12.78f, 14f);
	CamSettings camSettings2 = new CamSettings(new Vector3(0, 2.5f, -17f), 5f, 12f);
	CamSettings camSettings3 = new CamSettings(new Vector3(0, 2.5f, -16f), 10f, 10f);

	Vector3 north = new Vector3(0, 0, 1f);
	Vector3 south = new Vector3(0, 0, -1f);
	Vector3 east = new Vector3(1f, 0, 0);
	Vector3 west = new Vector3(-1f, 0, 0);

	Vector3[] directions = new Vector3[4];

	/// <summery> Awake instatiates the singleton
	/// If static member state is already initialized, the 
	/// class will destory itself. 
	/// Otherwise vars will be initialized and current level will be loaded. 
	/// </summery>

	private void InitMazeParent()
	{
		mazeParent = GameObject.FindGameObjectsWithTag("MazeParent")[0];
	}

	private void Awake()
	{
		rate = 0.3f;

		if (parameters == null)
			parameters = new LayerParameters();

		Generate();
		InitMazeParent();

		playerPosition = new Vector3 (0, 0, 0);
		state = this;
		camSettings = new CamSettings[10];
		InitCameratSetteings();
		Collectables = GameObject.FindGameObjectsWithTag("CollectableCube");
		directions[0] = north; directions[1] = south; directions[2] = east; directions[3] = west;
	}

	private bool PositionIsOccopiedByCollectable(Vector3 pos)
	{
		foreach (GameObject o in Collectables)
		{
			if (pos.x == o.transform.position.x && pos.z == o.transform.position.z)
				return true;

		}
		return false;
	}

	private void Update()
	{
		if (!Application.isPlaying)
			return;

		Vector3 newPlace = new Vector3();
		Color lerpedColor = Color.black;
		Color targetColor = parameters.pathColor;
		lerpedColor = Color.Lerp(Color.black, targetColor, Mathf.PingPong(Time.time, 1));
		if (showPath)
			layer.path2GoalMat.color = lerpedColor;
		//Debug.Log("Test");
		if (moveCollectables == true)
		{
			//Debug.Log("Test");
			timer += Time.deltaTime;

			if (Countdown.timeElapsed > 5)
				rate = 0.1f;

			if (Countdown.timeElapsed > 10)
				rate = 0.02f;

			if (Countdown.timeElapsed > 20)
				rate = 0.001f;

			if (Countdown.timeElapsed > 30)
				rate = 0.0001f;

			if (timer > rate)
			{
				int tmpX, tmpY;
				int index = randomCollectable.Next(0, Layer.numberOfCollectables);
				int direction = randomDirection.Next(0, 4);

				//Debug.Log(index + "    " + direction);

				newPlace = Collectables[index].transform.position + directions[direction];
				tmpX = (int)newPlace.x;
				tmpY = (int)newPlace.z;

				if (layer.IsValidPos(tmpX, tmpY) == true)
				{
					//Debug.Log("TYPE: " + (int)layer.maze1[tmpX, tmpY].type);
					if (layer.maze1[tmpX, tmpY] != null)
					{
						if (layer.maze1[tmpX, tmpY].type == 0)
						{
							if (PositionIsOccopiedByCollectable(newPlace) == false)
								Collectables[index].transform.position = newPlace;

						}
						else
						{
							direction = randomDirection.Next(0, 4);
							newPlace = Collectables[index].transform.position + directions[direction];
							tmpX = (int)newPlace.x;
							tmpY = (int)newPlace.z;

							if (layer.IsValidPos(tmpX, tmpY) == true)
							{
								if (layer.maze1[tmpX, tmpY].type == 0)
								{
									if (PositionIsOccopiedByCollectable(newPlace) == false)
										Collectables[index].transform.position = newPlace;
								}

							}
						}
					}
				}
				timer -= rate;

			}
		}
	}

	private void InitCameratSetteings()
	{
		camSettings[0] = camSettings0;
		camSettings[1] = camSettings1;
		camSettings[2] = camSettings2;
		camSettings[3] = camSettings3;
		currentCamSettings = camSettings[0];
	}

	public static RenderLayer GetInstance()
	{
		return state;
    }

	public void DestroyMaze()
	{
		if(mazeParent == null)
			InitMazeParent();

		var tempList = mazeParent.transform.Cast<Transform>().ToList();		

		foreach (var child in tempList)
			DestroyImmediate(child.gameObject);
	}

	public void SetCameratStartPositions()
	{
		//Vector3 overviewCamPosition = new Vector3(parameters.dimensionX * 0.5f, overviewCam.transform.position.y, overviewCam.transform.position.z);
		//overviewCam.transform.position = overviewCamPosition;
		//Vector3 playerPosition = player.transform.position;
		//playerPosition.x = parameters.startX;
		//playerPosition.z = parameters.startZ;
		//playerPosition.y = parameters.y + 1f;
		//player.transform.position = playerPosition;
		//playerCam.transform.position = playerPosition + currentCamSettings.offset;
	}

	public void SetPlayerCameraPosition(int index)
	{
		//Vector3 playerPosition = player.transform.position;
		//playerPosition.x = parameters.startX;
		//playerPosition.z = parameters.startZ;
		//playerPosition.y = parameters.y + 1f;
		//player.transform.position = playerPosition;

		//currentCamSettings = camSettings[0];
		//playerCam.transform.position = playerPosition + currentCamSettings.offset;
	}

	public void Generate()
	{
		DestroyMaze();
	
		parameters.dimensionX = dimensionX;
		parameters.dimensionZ = dimensionZ;
		parameters.type = layerType;

		layer = new Layer(gameObject, parameters);
		SetCameratStartPositions();
		SetScale(scale);
		SetGroundWallHeight(groundWallHeight);
		SetWallHeight(wallHeight);

		var scene = SceneManager.GetActiveScene();
#if UNITY_EDITOR
		if (Application.isPlaying == false)
			EditorSceneManager.MarkSceneDirty(scene);
#endif
	}

	public void SetDimensionX(string dimX)
	{
		int number;

		if (int.TryParse(dimX, out number))
		{
			dimensionX = number;
		}
	}

	public void SetDimensionZ(string dimZ)
	{
		int number;

		if (int.TryParse(dimZ, out number))
		{
			dimensionZ = number;
		}
	}

	public void UpdateScaling()
	{
		Vector3 newScaling = new Vector3();
		newScaling = mazeParent.transform.localScale;
		newScaling.x = scale;
		newScaling.z = scale;
		mazeParent.transform.localScale = newScaling;
	}

	public void SetScale(float scale)
	{
		if (scale > 0)
		{
			this.scale = scale;
			layer.ScalePlayground(this.scale);
			Vector3 playerPosition = new Vector3();
			playerPosition = player.transform.position;
			playerPosition.x = parameters.startX * this.scale;
			playerPosition.z = parameters.startZ * this.scale;
			player.transform.position = playerPosition;

			Vector3 overviewCamPosition = new Vector3(parameters.dimensionX * 0.5f * this.scale, overviewCam.transform.position.y, overviewCam.transform.position.z);
			overviewCam.transform.position = overviewCamPosition;

			Vector3 playerCamPosition = new Vector3(player.transform.position.x, player.transform.position.y + 19f, player.transform.position.z - 16f);
			playerCam.transform.position = playerCamPosition;
		}
	}

	public void ShowPath()
	{
		if (layer.pathList != null)
		{
			foreach (Vector2Int v in layer.pathList)
			{
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.transform.parent = mazeParent.transform;
				cube.transform.position = (new Vector3(v.x, 0.15f, v.y));
				Renderer rend = cube.GetComponent<Renderer>();
				rend.sharedMaterial = layer.path2GoalMat;
				rend.sharedMaterial.SetColor("_Color", parameters.pathColor);
				rend.sharedMaterial.SetColor("_EmissionColor", parameters.pathColor);
				cube.tag = "Path";
			}
		}
	}

	public void HidePath()
	{
		var tempList = mazeParent.transform.Cast<Transform>().ToList();

		foreach (var child in tempList)
		{
			if(child.tag == "Path")
				DestroyImmediate(child.gameObject);
		}
	}

	public void UpdateGroundWallHeight()
	{
		layer.SetGroundWallHeight(this.groundWallHeight);	
	}

	public void UpdateWallHeight()
	{
		layer.SetWallHeight(this.wallHeight);
	}

	public void SetGroundWallHeight(float height)
	{
		if (height >= 0)
		{
			this.groundWallHeight = height;
			layer.SetGroundWallHeight(height);
		}
	}

	public void SetWallHeight(float height)
	{
		if (height >= 0)
		{
			this.wallHeight = height;
			layer.SetWallHeight(height);
		}
	}
}

