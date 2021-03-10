#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using BlackPlagueGames;

namespace BlackPlagueGames {
	/// <summary>
	/// Game Manager
	/// </summary>
	public class GameManager : MonoBehaviour {
		public bool isHex = false;
		[HeaderAttribute("Is this a Pointy Hex (not a flat top Hex)")]
		public bool IsPointyHex = false;
		public bool isDiagMovement = false;
		public bool isAutoStart = true;
		public bool isShowNodes = true;
		public int gridWidthInTiles = 0;
		public int gridHeightInTiles = 0;
		/// <summary>
		/// Tile GameObject
		/// </summary>
		public GameObject tileGO;
		private float tileWidth = 0F;
		private float tileHeight = 0F;
		public float tileGap = 0F;
		public List<GameObject> tiles = new List<GameObject>();
		public Vector2[] directions;
		public LeanTweenType LTweenType = LeanTweenType.easeSpring;
		public GameObject StartTile;

		public void Start() {
			SetTileSize();
			SetTileGap();
			if(isHex) {
				if(IsPointyHex) {
					directions = new Vector2[] { new Vector2(tileWidth, 0F), new Vector2(-(tileWidth), 0F), new Vector2((tileWidth / 2F), (tileHeight * 0.75F)), new Vector2(-((tileWidth / 2F)), -((tileHeight * 0.75F))), new Vector2((tileWidth / 2F), -((tileHeight * 0.75F))), new Vector2(-((tileWidth / 2F)), (tileHeight * 0.75F)) };
				} else {
					directions = new Vector2[] { new Vector2(0F, tileHeight), new Vector2(0F, -(tileHeight)), new Vector2((tileWidth * 0.75F), (tileHeight / 2F)), new Vector2((tileWidth * 0.75F), -((tileHeight / 2F))), new Vector2(-((tileWidth * 0.75F)), (tileHeight / 2F)), new Vector2(-((tileWidth * 0.75F)), -((tileHeight / 2F))) };
				}
			} else if(isDiagMovement) {
				directions = new Vector2[] { new Vector2(tileWidth, 0F), new Vector2(-(tileWidth), 0F), new Vector2(0F, tileWidth), new Vector2(0F, -(tileWidth)), new Vector2(tileWidth, tileWidth), new Vector2(-(tileWidth), -(tileWidth)), new Vector2(-(tileWidth), tileWidth), new Vector2(tileWidth, -(tileWidth)) };
			} else {
				directions = new Vector2[] { new Vector2(tileWidth, 0F), new Vector2(-(tileWidth), 0F), new Vector2(0F, tileWidth), new Vector2(0F, -(tileWidth)) };
			}
			if((tileGO != null)) {
				CreateGrid();
				CreateWalls();
			} else {
				Debug.Log("GameObject not Set (TileGO)");
			}
		}

		/// <summary>
		/// Find Tile size using Bounds of Mesh Renderer
		/// </summary>
		public void SetTileSize() {
			tileWidth = tileGO.GetComponent<UnityEngine.MeshRenderer>().bounds.size.x;
			tileHeight = tileGO.GetComponent<UnityEngine.MeshRenderer>().bounds.size.z;
		}

		/// <summary>
		/// Set Tile Gap
		/// </summary>
		public void SetTileGap() {
			tileWidth = (tileWidth + tileGap);
			tileHeight = (tileHeight + tileGap);
		}

		/// <summary>
		/// Calculate initial Position
		/// </summary>
		public Vector3 CalcInitPos() {
			Vector3 startPos = Vector3.zero;
			startPos = new Vector3((((-(tileWidth) * gridWidthInTiles) / 2F) + (tileWidth / 2F)), 0F, (((gridHeightInTiles / 2F) * tileHeight) - (tileHeight / 2F)));
			return startPos;
		}

		/// <summary>
		/// Calculate World Coordinates
		/// </summary>
		public Vector3 CalcWorldCoords(Vector3 gridPosition) {
			Vector3 initPos = Vector3.zero;
			float offset = 0F;
			float x = 0F;
			float z = 0F;
			initPos = CalcInitPos();
			offset = 0F;
			if(isHex) {
				if(IsPointyHex) {
					if(((gridPosition.z % 2F) != 0F)) {
						offset = (tileWidth / 2F);
					}
					x = ((initPos.x + offset) + (gridPosition.x * tileWidth));
					z = (initPos.z - ((gridPosition.z * tileHeight) * 0.75F));
					return new Vector3(x, 0F, z);
				} else {
					if(((gridPosition.z % 2F) != 0F)) {
						offset = (tileHeight / 2F);
					}
					x = (initPos.z - ((gridPosition.z * tileWidth) * 0.75F));
					z = ((initPos.x + offset) + (gridPosition.x * tileHeight));
					return new Vector3(x, 0F, z);
				}
			} else {
				x = ((gridPosition.x - (gridHeightInTiles / 2F)) * tileWidth);
				z = ((gridPosition.z - (gridWidthInTiles / 2F)) * tileHeight);
				return new Vector3(x, 0F, z);
			}
		}

		/// <summary>
		/// Create Grid
		/// </summary>
		public void CreateGrid() {
			GameObject tileGridGO = null;
			GameObject tile = null;
			Vector3 gridPos = Vector3.zero;
			int counter = 0;
			tileGridGO = this.gameObject;
			for(int index = 0; index < gridHeightInTiles; index += 1) {
				for(int index1 = 0; index1 < gridWidthInTiles; index1 += 1) {
					tile = Object.Instantiate(tileGO) as GameObject;
					gridPos = new Vector3(index1, 0F, index);
					tile.transform.position = CalcWorldCoords(gridPos);
					tile.transform.parent = tileGridGO.transform;
					tile.name = "Tile " + "X: " + index1 + " | " + "Y:" + index;
				}
			}
			GetTileList();
		}

		/// <summary>
		/// create walls/colliders around game board
		/// </summary>
		public void CreateWalls() {
			GameObject wallGO = null;
			wallGO = new GameObject("Wall1");
			wallGO.layer = LayerMask.NameToLayer("WallCollider");
			wallGO.AddComponent<UnityEngine.BoxCollider>().size = new Vector3(((gridWidthInTiles * tileWidth) + 10F), 20F, 0.5F);
			wallGO.transform.position = new Vector3(0F, 4F, ((gridWidthInTiles * tileWidth) / 2F));
			wallGO.transform.parent = this.transform;
			wallGO = new GameObject("Wall2");
			wallGO.layer = LayerMask.NameToLayer("WallCollider");
			wallGO.AddComponent<UnityEngine.BoxCollider>().size = new Vector3(((gridWidthInTiles * tileWidth) + 10F), 20F, 0.5F);
			wallGO.transform.position = new Vector3(((gridWidthInTiles * tileWidth) / 2F), 4F, 0F);
			wallGO.transform.parent = this.transform;
			wallGO.transform.Rotate(new Vector3(0f, 90f, 0f), Space.World);
			wallGO = new GameObject("Wall3");
			wallGO.layer = LayerMask.NameToLayer("WallCollider");
			wallGO.AddComponent<UnityEngine.BoxCollider>().size = new Vector3(((gridWidthInTiles * tileWidth) + 10F), 20F, 0.5F);
			wallGO.transform.position = new Vector3(0F, 4F, -(((gridWidthInTiles * tileWidth) / 2F)));
			wallGO.transform.parent = this.transform;
			wallGO = new GameObject("Wall4");
			wallGO.layer = LayerMask.NameToLayer("WallCollider");
			wallGO.AddComponent<UnityEngine.BoxCollider>().size = new Vector3(((gridWidthInTiles * tileWidth) + 10F), 20F, 0.5F);
			wallGO.transform.position = new Vector3(-(((gridWidthInTiles * tileWidth) / 2F)), 4F, 0F);
			wallGO.transform.parent = this.transform;
			wallGO.transform.Rotate(new Vector3(0f, 90f, 0f), Space.World);
		}

		public void GetTileList() {
			int childCount = 0;
			foreach(GameTiles loopObject in this.transform.GetComponentsInChildren<BlackPlagueGames.GameTiles>()) {
				tiles.Add(loopObject.gameObject);
				childCount = (childCount + 1);
			}
			GetStartTile();
		}

		public void GetStartTile() {
			StartTile = tiles[Random.Range(0, tiles.Count)];
			StartTile.GetComponent<BlackPlagueGames.GameTiles>().autorun = true;
		}
	}
}
