#pragma warning disable
using UnityEngine;
using System.Collections.Generic;

namespace BlackPlagueGames {
	/// <summary>
	/// Attach this to the Tile Object
	/// </summary>
	public class GameTiles : MonoBehaviour {
		public GameObject nodeModelGO;
		public bool autorun = false;
		public bool showNodes = true;
		public bool isInitialized = false;
		public float scaleTime = 0.25F;
		/// <summary>
		/// Size to Scale Object using LeanTween
		/// </summary>
		public float nodeSclaeToSize = 1F;
		public BlackPlagueGames.GameManager gameBoard;
		public LeanTweenType LTweenEaseType = LeanTweenType.easeSpring;
		public List<GameObject> m_neighboarTiles = new List<GameObject>();
		public Vector2 m_coords = Vector2.zero;
		public GameObject linkerGO;
		public List<GameObject> m_linkedTiles;
		private GameObject parameterValues;

		public List<GameObject> NeighboarTiles {
			get {
				return m_neighboarTiles;
			}
		}
		public Vector2 Coordinates {
			get {
				return m_coords;
			}
		}
		public List<GameObject> LinkedTiles {
			get {
				return m_linkedTiles;
			}
		}

		/// <summary>
		/// This will be on the Gametile Object itself.
		/// </summary>
		public void Start() {
			gameBoard = Object.FindObjectOfType<BlackPlagueGames.GameManager>();
			if((gameBoard != null)) {
				LTweenEaseType = gameBoard.LTweenType;
				showNodes = gameBoard.isShowNodes;
				nodeModelGO = GameObject.Find(this.name + "/" + this.transform.GetChild(0).name);
				if((nodeModelGO != null)) {
					nodeModelGO.transform.localScale = Vector3.zero;
					m_coords = new Vector2(base.transform.position.x, base.transform.position.z);
					base.StartCoroutine(timeDelay());
				} else {
					Debug.Log("Error: Cant Find Node GameObject");
				}
			} else {
				Debug.Log("Error (start):  Cant Find GameManager GameObject");
			}
		}

		public void ShowGeometry() {
			if(showNodes) {
				nodeModelGO.GetComponent<UnityEngine.MeshRenderer>().enabled = true;
			} else {
				nodeModelGO.GetComponent<UnityEngine.MeshRenderer>().enabled = false;
			}
			if((nodeModelGO != null)) {
				LeanTween.scale(nodeModelGO, new Vector3(nodeSclaeToSize, 0.25F, nodeSclaeToSize), scaleTime).setEase(LTweenEaseType);
			} else {
				Debug.Log("Error (In ShowGeometry): No GameObject Found");
			}
		}

		public List<GameObject> FindNeighbors(List<GameObject> GameTiles) {
			GameObject foundNeighbor = null;
			List<GameObject> nlist = null;
			nlist = new List<GameObject>();
			foreach(Vector2 loopObject in gameBoard.directions) {
				foundNeighbor = GameTiles.Find((GameObject tempVar) => {
					parameterValues = tempVar;return (parameterValues.GetComponent<BlackPlagueGames.GameTiles>().Coordinates == (loopObject + Coordinates));
				});
				if(((foundNeighbor != null) && !(nlist.Contains(foundNeighbor)))) {
					nlist.Add(foundNeighbor);
				}
			}
			return nlist;
		}

		public System.Collections.IEnumerator timeDelay() {
			yield return new WaitForSeconds(1F);
			m_neighboarTiles = FindNeighbors(gameBoard.tiles);
			if(autorun) {
				InitilizeNode(this);
			}
		}

		public void InitilizeNode(BlackPlagueGames.GameTiles gametile) {
			if(!(isInitialized)) {
				ShowGeometry();
				InitializeNeighbor();
				isInitialized = true;
			}
		}

		public void InitializeNeighbor() {
			this.StartCoroutine(InitilizeNeighborRoutine());
		}

		public System.Collections.IEnumerator InitilizeNeighborRoutine() {
			yield return new WaitForSeconds(0.5F);
			foreach(GameObject loopObject1 in m_neighboarTiles) {
				if(!(m_linkedTiles.Contains(loopObject1))) {
					TileLinker(loopObject1);
					loopObject1.GetComponent<BlackPlagueGames.GameTiles>().InitilizeNode(null);
				}
			}
		}

		public void TileLinker(GameObject targettile) {
			GameObject linkerInstancedGO = null;
			BlackPlagueGames.TileLinker linker = null;
			if((linkerGO != null)) {
				linkerInstancedGO = Object.Instantiate<UnityEngine.GameObject>(linkerGO, this.transform.position, Quaternion.identity);
				linkerInstancedGO.transform.parent = this.transform;
				linker = linkerInstancedGO.GetComponent<BlackPlagueGames.TileLinker>();
				linker.DrawLink(this.transform.position, targettile.transform.position);
			} else {
				Debug.Log("Error (in Gametiles TileLinker) forgot to Link Linker Prefab");
			}
			if(!(m_linkedTiles.Contains(targettile))) {
				m_linkedTiles.Add(targettile);
			}
			if(!(LinkedTiles.Contains(targettile.gameObject))) {
				targettile.GetComponent<BlackPlagueGames.GameTiles>().LinkedTiles.Add(this.gameObject);
			}
		}
	}
}
