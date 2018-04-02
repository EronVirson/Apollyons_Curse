using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class TileSet : MonoBehaviour {

	public GameObject[] basicTiles = new GameObject[16];
	public GameObject background, lightBackground, windowBackground;
	public GameObject[] specialTiles = new GameObject[0];

	/// <summary>
	/// Instantiates tile of this tileSet, that fits in the given surroundings 
	/// (true indicates there is another tile in that direction).
	/// The tile has the correct rotation to fit in under a tile parent at default rotation.
	/// </summary>
	public Transform SpawnTile(bool up, bool left, bool right, bool down) {
		TileType type = 0;

		//use boolean magic to find correct tile index in basicTiles array.
		if(up)
			type += 8;
		if(down ^ up)
			type += 4;
		if(left ^ right)
			type += 2;
		if(left)
			type += 1;

		//Copy basic tile and it's rotation
		GameObject toReturn = Instantiate(basicTiles[(int)type]);
		toReturn.transform.rotation = basicTiles[(int)type].transform.rotation;

		//new tile is ready; return it.
		return toReturn.transform;

	}

	/// <summary>
	/// Spawn background tile object in default rotation and return it.
	/// Returns null if given tile does not exist.
	/// 1 - standard 
	/// 2 - light
	/// 3 - window
	/// </summary>
	public Transform SpawnBackground(int type) {
		GameObject toReturn;
		switch(type) {
		case 1:
		toReturn = Instantiate(background);
		toReturn.transform.rotation = background.transform.rotation;
		break;
		case 2:
		toReturn = Instantiate(lightBackground);
		toReturn.transform.rotation = lightBackground.transform.rotation;
		break;
		case 3:
		toReturn = Instantiate(windowBackground);
		toReturn.transform.rotation = windowBackground.transform.rotation;
		break;
		default: toReturn = null; break;
		}

		if(toReturn == null)
			return null;
		else
			return toReturn.transform;
	}

	/// <summary>
	/// Spawn special tile object in default rotation and return it.
	/// next should be ModSpecialIndex(index of the old tile + 1).
	/// </summary>
	public Transform SpawnNextSpecial(int next) {
		if(specialTiles.Length == 0)
			return null;

		GameObject toReturn = Instantiate(specialTiles[next]);
		toReturn.transform.rotation = specialTiles[next].transform.rotation;
		return toReturn.transform;
	}

	public int GetNbSpecialTiles() {
		return specialTiles.Length;
	}

	/// <summary>
	/// Enum shows indices that basic tiles should follow in the basicTiles array.
	/// E.g. basicTiles[9] should be a wall piece tile, 
	/// to be surrounded by other tiles on all sides.
	/// </summary>
	public enum TileType {
		HoverBlock = 0,
		HoverPiece = 1,
		HoverCorner_L = 2,
		HoverCorner_R = 3,

		FloorBlock = 4,
		FloorPiece = 5,
		FloorCorner_L = 6,
		FloorCorner_R = 7,

		WallBlock = 8,
		WallPiece = 9,
		WallCorner_L = 10,
		WallCorner_R = 11,

		CeilingBlock = 12,
		CeilingPiece = 13,
		CeilingCorner_L = 14,
		CeilingCorner_R = 15
	}

	public bool BasicTilesValid() {
		if(basicTiles == null)
			return false;
		if(basicTiles.Length != 16)
			return false;
		for(int i = 0; i < 16; i++) {
			if(basicTiles[i] == null)
				return false;
		}
		return true;
	}

	public bool BackgroundTilesValid() {
		return background != null &&
			lightBackground != null &&
			windowBackground != null;
	}

	public bool SpecialTilesValid() {
		if(specialTiles == null) {
			specialTiles = new GameObject[0];
			return true;
		}
		for(int i = 0; i < specialTiles.Length; i++) {
			if(specialTiles[i] == null)
				return false;
		}
		return true;
	}

	public bool Valid() {
		return BasicTilesValid() && 
			BackgroundTilesValid() && 
			SpecialTilesValid();
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(TileSet))]
[CanEditMultipleObjects]
public class TileSetExt : Editor {
	
	public override void OnInspectorGUI() {
		TileSet tileSet = (TileSet)this.target;
		
		if(!tileSet.BasicTilesValid()) {
			GUILayout.Label("Basic tiles not assigned!");
			GUILayout.Label("Require 16 valid tiles.");
			GUILayout.Label("");
		}
		if(!tileSet.BackgroundTilesValid()) {
			GUILayout.Label("Background tiles not assigned!");
			GUILayout.Label("Require valid tile for 3 types.");
			GUILayout.Label("");
		}
		if(!tileSet.BasicTilesValid() || !tileSet.BackgroundTilesValid()) {
			if(GUILayout.Button("Auto fill by name"))
				AutoFillBasicTiles();
			GUILayout.Label("");
		}
		if(!tileSet.SpecialTilesValid()) {
			GUILayout.Label("Special tiles invalid!");
			GUILayout.Label("Require valid special tile set, assign empty objects or reduce array size.");
			GUILayout.Label("");
		}
		if(tileSet.Valid()) {
			GUILayout.Label("Tileset ready!");
		}

		DrawDefaultInspector();
	}

	private void AutoFillBasicTiles() {
		TileSet tileSet = (TileSet)this.target;

		for(int i = 0; i < tileSet.transform.childCount; i++) {
			Transform tile = tileSet.transform.GetChild(i);
			string tileName = tile.name.ToLower();
			tileName = tileName.Replace('.', '_');
			tileName = tileName.Replace(' ', '_');

			//check if name matches basic tile
			for(int j = 0; j < 16; j++) {
				string matchName = ((TileSet.TileType)j).ToString().ToLower();

				if(tileName.Contains(matchName))
					tileSet.basicTiles[j] = tile.gameObject;
			}

			//check if name matches background tiles
			if(tileName.Contains("background")) {
				bool light = tileName.Contains("light");
				bool window = tileName.Contains("window");
				if(!light && !window)
					tileSet.background = tile.gameObject;
				else if(!window)
					tileSet.lightBackground = tile.gameObject;
				else if(!light)
					tileSet.windowBackground = tile.gameObject;
			}
		}
	}
}
#endif