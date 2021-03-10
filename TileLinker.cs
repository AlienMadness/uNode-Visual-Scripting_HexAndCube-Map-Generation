#pragma warning disable
using UnityEngine;
using System.Collections.Generic;

namespace BlackPlagueGames {
	/// <summary>
	/// To Draw the links between tiles
	/// </summary>
	public class TileLinker : MonoBehaviour {
		public float borderWidth = 0.02F;
		public float lineWidth = 0.3F;
		public float scaleTime = 0.5F;
		public LeanTweenType LTweenEaseType = LeanTweenType.easeSpring;

		public void DrawLink(Vector3 startPos, Vector3 endPos) {
			Vector3 dirVector = Vector3.zero;
			float zScale = 0F;
			Vector3 newScale = Vector3.zero;
			this.transform.localScale = new Vector3(lineWidth, 1F, 0F);
			dirVector = (endPos - startPos);
			zScale = (dirVector.magnitude - borderWidth);
			newScale = new Vector3(lineWidth, 1.7F, zScale);
			this.transform.rotation = Quaternion.LookRotation(dirVector);
			this.transform.position = (startPos + (this.transform.forward * borderWidth));
			LeanTween.scale(this.gameObject, newScale, scaleTime).setEase(LTweenEaseType);
		}
	}
}
