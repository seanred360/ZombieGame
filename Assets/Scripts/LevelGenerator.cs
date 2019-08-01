using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour {

    public NavMeshSurface surface;
	public int width = 10;
	public int height = 10;

	public GameObject wall;
	public GameObject player;

	private bool playerSpawned = false;

	// Use this for initialization
	void Awake () {
		GenerateLevel();
        surface.BuildNavMesh();
	}
	
	// Create a grid based level
	void GenerateLevel()
	{
		// Loop over the grid
		for (int x = 0; x <= width; x+=2)
		{
			for (int y = 0; y <= height; y+=2)
			{
                if (Random.value > .9f)
                {
                    // Spawn a wall
                    Vector3 pos = new Vector3(x - width / 2f, 1f, y - height / 2f);
                    //Instantiate(orePrefab, pos, Quaternion.identity, transform);
                } 
                // Should we place a wall?
                if (Random.value > .7f)
				{
					// Spawn a wall
					Vector3 pos = new Vector3(x - width / 2f, 1f, y - height / 2f);
					Instantiate(wall, pos, Quaternion.identity, transform);
				} else if (!playerSpawned) // Should we spawn a player?
				{
					// Spawn the player
					Vector3 pos = new Vector3(x - width / 2f, 1.25f, y - height / 2f);
					Instantiate(player, pos, Quaternion.identity);
                    playerSpawned = true;
				}
			}
		}
        //Object[] foundOre = GameObject.FindObjectsOfType<Ore>();
        //spawnedOre = new Ore[foundOre.Length];
        //for (int i = 0; i < foundOre.Length; i++)
        //{
        //    spawnedOre[i] = (Ore)foundOre[i];
        //}

        //spawnedOre[1].oreType = OreType.Bomb;
    }

}
