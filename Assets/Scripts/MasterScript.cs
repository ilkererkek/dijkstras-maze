using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MasterScript : MonoBehaviour
{


    public int gridSize = 16;
    public GameObject camera;
    public GameObject tileObject;
    public GameObject[,] tiles;
    public Vector3 objectLocation;
    public GameObject startNode;
    public GameObject endNode;
    public Button button;
    public void Start()
    {
        InitiateTiles();
        button.onClick.AddListener(AStarAlgorithm);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitiateTiles()
    {
        camera.GetComponent<Camera>().orthographicSize = gridSize / 2;
        tiles = new GameObject[gridSize, gridSize];
        var objectLocation = gameObject.transform.position;
        var position = new Vector3(0, 0, 0);
        position.x = objectLocation.x - gridSize / 2f + 0.5f;

        for (int i = 0; i < gridSize; i++)
        {
            position.y = objectLocation.y - gridSize / 2f + 0.5f;
            for (int j = 0; j < gridSize; j++)
            {
                tiles[i, j] = Instantiate(tileObject, position, Quaternion.identity);
                tiles[i, j].GetComponent<TileScript>().x = i;
                tiles[i, j].GetComponent<TileScript>().y = j;
                tiles[i, j].GetComponent<TileScript>().Master = this.gameObject;
                if (j % 2 == i % 2)
                {
                    tiles[i, j].GetComponent<TileScript>().originalColor = Color.grey;
                }

                position.y++;
            }
            position.x++;
        }
    }

    private void setDistances(GameObject orgTile, GameObject targetTile, int[,] distances)
    {
        var tileScript = orgTile.GetComponent<TileScript>();
        var targetScript = targetTile.GetComponent<TileScript>();
        distances[tileScript.x, tileScript.y] = Math.Abs(targetScript.x - tileScript.x) + Math.Abs(targetScript.y - tileScript.x);
    }

    private List<GameObject> GetWalkableTiles(GameObject currentTile, GameObject targetTile, int[,] distances, int[,] costs)
    {
        var possibleTiles = new List<GameObject>();

        if (currentTile.GetComponent<TileScript>().x - 1 >= 0) possibleTiles.Add(tiles[currentTile.GetComponent<TileScript>().x - 1, currentTile.GetComponent<TileScript>().y]);
        if (currentTile.GetComponent<TileScript>().x + 1 < gridSize) possibleTiles.Add(tiles[currentTile.GetComponent<TileScript>().x + 1, currentTile.GetComponent<TileScript>().y]);
        if (currentTile.GetComponent<TileScript>().y - 1 >= 0) possibleTiles.Add(tiles[currentTile.GetComponent<TileScript>().x, currentTile.GetComponent<TileScript>().y - 1]);
        if (currentTile.GetComponent<TileScript>().y + 1 < gridSize) possibleTiles.Add(tiles[currentTile.GetComponent<TileScript>().x, currentTile.GetComponent<TileScript>().y + 1]);


        int initialCost = costs[currentTile.GetComponent<TileScript>().x, currentTile.GetComponent<TileScript>().y];
        possibleTiles.ForEach(tile => distances[tile.GetComponent<TileScript>().x, tile.GetComponent<TileScript>().y] = initialCost + 1);


        possibleTiles.ForEach(tile => setDistances(tile,targetTile, distances));


        return possibleTiles
                .Where(tile => !tile.GetComponent<TileScript>().isBlocked || !tile.GetComponent<TileScript>().isEnd)
                .ToList();
    }


    private void AStarAlgorithm()
    {
        var activeTiles = new List<GameObject>();

        var visitedTiles = new List<GameObject>();
        var distances = new int[gridSize, gridSize];
        var costs = new int[gridSize, gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                distances[i, j] = int.MaxValue;
            }
        }
        distances[startNode.GetComponent<TileScript>().x, startNode.GetComponent<TileScript>().y] = 0;

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                costs[i, j] = int.MaxValue;
            }
        }
        costs[startNode.GetComponent<TileScript>().x, startNode.GetComponent<TileScript>().y] = 0;
        activeTiles.Add(startNode);

        while (activeTiles.Any())
        {
            
            var checkTile = activeTiles.OrderBy(x => distances[x.GetComponent<TileScript>().x, x.GetComponent<TileScript>().y]).First();
            Debug.Log(checkTile.transform.position);
            if (checkTile == endNode)
            {
                Debug.Log("End");
                return;
            }
            visitedTiles.Add(checkTile);
            checkTile.GetComponent<TileScript>().setTraversed();
            activeTiles.Remove(checkTile);

            var walkableTiles = GetWalkableTiles(checkTile, endNode, distances, costs);

            foreach (var walkableTile in walkableTiles)
            {
                if (walkableTile.GetComponent<TileScript>().isBlocked)
                {
                    continue;
                }
                if (visitedTiles.Any(x => x.GetComponent<TileScript>().x == walkableTile.GetComponent<TileScript>().x && x.GetComponent<TileScript>().y == walkableTile.GetComponent<TileScript>().y))
                    continue;

                if (activeTiles.Any(x => x.GetComponent<TileScript>().x == walkableTile.GetComponent<TileScript>().x && x.GetComponent<TileScript>().y == walkableTile.GetComponent<TileScript>().y))
                {
                    var existingTile = activeTiles.First(x => x.GetComponent<TileScript>().x == walkableTile.GetComponent<TileScript>().x && x.GetComponent<TileScript>().y == walkableTile.GetComponent<TileScript>().y);

                    int costDistanceEx = costs[existingTile.GetComponent<TileScript>().x, existingTile.GetComponent<TileScript>().y] + distances[existingTile.GetComponent<TileScript>().x, existingTile.GetComponent<TileScript>().y];
                    int costDistanceCheck = costs[checkTile.GetComponent<TileScript>().x, checkTile.GetComponent<TileScript>().y] + distances[checkTile.GetComponent<TileScript>().x, checkTile.GetComponent<TileScript>().y];

                    if (costDistanceEx > costDistanceCheck)
                    {
                        activeTiles.Remove(existingTile);
                        activeTiles.Add(walkableTile);
                    }
                }
                else
                {
                    activeTiles.Add(walkableTile);
                }
            }

        }




    }
}
