using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseMap : MonoBehaviour
{
    Dictionary<short, GameObject> tileset;
    Dictionary<short, GameObject> tileGroups;
    [SerializeField] PrefabListSO availableTiles;
    short baseTileAmount = -1;
    [SerializeField] int mapWidth = 16;
    [SerializeField] int mapHeight = 9;

    List<List<int>> noiseGrid = new List<List<int>>();
    List<List<GameObject>> tileGrid = new List<List<GameObject>>();

    [SerializeField] float magnification = 7.0f; //Recommended between 4-20
    [SerializeField] int xOffset = 0; //<- +>
    [SerializeField] int yOffset = 0; //v- +^
   
    void Start(){
        CreateTileSet();
        CreateTileGroups();
        GenerateMap();
    }

    void CreateTileSet(){
        tileset = new Dictionary<short, GameObject>();
        for (short i = 0; i < availableTiles.gameObjectList.Count; i++){
            if (availableTiles.gameObjectList[i].name == availableTiles.gameObjectList[0].name){
                baseTileAmount++;
            }
            tileset.Add(i,availableTiles.gameObjectList[i]);
        }
    }

    void CreateTileGroups(){
        tileGroups = new Dictionary<short, GameObject>();
        for (short i = 0; i < availableTiles.gameObjectList.Count; i++){
            var _gameObject = availableTiles.gameObjectList[i];
            GameObject tileGroup = new GameObject(_gameObject.name);
            tileGroup.transform.parent = gameObject.transform;
            tileGroup.transform.localPosition = Vector3.zero;
            tileGroups.Add(i,tileGroup);
        }
    }

    void GenerateMap(){
        for (int x = 0; x < mapWidth; x++){
            noiseGrid.Add(new List<int>());
            tileGrid.Add(new List<GameObject>());
            for (int y = 0; y < mapHeight; y++){
                short tileID = GetIDUsingPerlin(x, y);
                noiseGrid[x].Add(tileID);
                CreateTile(tileID,x,y);
            }
        }
    }
    
    short GetIDUsingPerlin(int x, int y){
        float rawPerlin = Mathf.PerlinNoise(
            (x - xOffset) / magnification,
            (y - yOffset) / magnification
        );
        float clampedPerlin = Mathf.Clamp01(rawPerlin); //Sometimes perlin gives <0 or >1 so we clamp it.
        float scalePerlin = clampedPerlin * tileset.Count;
        if (scalePerlin == tileset.Count){
            scalePerlin -= 1;
        }
        return (short)Mathf.FloorToInt(scalePerlin);
    }
    void CreateTile(short tileID, int x, int y){
        var z = tileID;
        if (tileGroups[tileID].name == tileGroups[0].name){
            z = 0;
            
        }
        else{
           z =(short) Mathf.Max(1,tileID -baseTileAmount);
        }
        
        GameObject tilePrefab = tileset[tileID];
        GameObject tileGroup = tileGroups[tileID];
        GameObject tile = Instantiate(tilePrefab, tileGroup.transform);

        tile.name = $"TileX{x}Y{y}";
        tile.transform.localPosition = new Vector3(x, y, z);
        tileGrid[x].Add(tile);
    }
    
}