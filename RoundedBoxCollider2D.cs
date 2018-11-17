

#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Physics 2D/Rounded Box Collider 2D")]

[RequireComponent(typeof(PolygonCollider2D))]
public class RoundedBoxCollider2D : MonoBehaviour {

	public bool topRightCorner = false;
	public bool topLeftCorner = false;
	public bool bottomLeftCorner = false;
	public bool bottomRightCorner = false;
	

	[Range(1, 90)]
	public int smoothness = 5;

	[HideInInspector]
	public float height = 1;

	[HideInInspector]
	public float width = 1;

	[HideInInspector]
	public float radius = .197f, wt, wb;

	[HideInInspector]
	public float trapezoid = .5f;

	[HideInInspector]
	public Vector2 offset, center1, center2, center3, center4;

	float ang = 0;
	List<Vector2> points;

	public Vector2[] getPoints() {
		points = new List<Vector2>();
		points.Clear();

		wt = (width + width) - ((width + width) * trapezoid);   // width top
		wb = (width + width) * trapezoid;                       // width bottom

		// vertices
		Vector2 vTR = new Vector2((wt / 2f), +(height / 2f)); // top right vertex 
		Vector2 vTL = new Vector2((wt / -2f), +(height / 2f)); // top left vertex
		Vector2 vBL = new Vector2((wb / -2f), -(height / 2f)); // bottom left vertex
		Vector2 vBR = new Vector2((wb / 2f), -(height / 2f)); // bottom right vertex


		Vector2 dir = vBL - vTL;
		float hypAngleTL = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // hypertenuse angle top left corner
		hypAngleTL = (hypAngleTL + 360) % 360; // get it between 0-360 range
		hypAngleTL = 360 - hypAngleTL; // use the inside angle
		hypAngleTL /= 2f; // got our adjacent angle
						  ///        
						  ///    adj TL (Top Left)
						  ///    _____
						  ///    \    |
						  ///     \   |
						  ///   h  \  | opp = radius
						  ///       \ |
						  ///        \|
						  /// 
		float adjTL = radius / Mathf.Tan(hypAngleTL * Mathf.Deg2Rad);
		center1 = new Vector3(vTR.x - adjTL, vTR.y - radius, 0);
		center2 = new Vector3(vTL.x + adjTL, vTL.y - radius, 0);



		float hypAngleBL = (180 - hypAngleTL * 2f) / 2f; // hypertenuse angle bottom left corner
														 /// 
														 ///        /|
														 ///       / |
														 ///   h  /  | opp = radius
														 ///     /   |
														 ///    /____|
														 /// 
														 ///     adj BL (Bottom Left)
														 /// 
		float adjBL = radius / Mathf.Tan(hypAngleBL * Mathf.Deg2Rad);
		center3 = new Vector3(vBL.x + adjBL, vBL.y + radius, 0);
		center4 = new Vector3(vBR.x - adjBL, vBR.y + radius, 0);


		// prevent overlapping of the corners
		center1.x = Mathf.Max(0, center1.x);
		center2.x = Mathf.Min(0, center2.x);
		center3.x = Mathf.Min(0, center3.x);
		center4.x = Mathf.Max(0, center4.x);


		// curveTOP angles
		Vector2 tmpDir = vBR - vTR;
		float tmpAng = Mathf.Atan2(tmpDir.y, tmpDir.x) * Mathf.Rad2Deg;
		tmpAng = (tmpAng + 360) % 360;
		float x = vTR.x + adjTL * Mathf.Cos(tmpAng * Mathf.Deg2Rad);
		float y = vTR.y + adjTL * Mathf.Sin(tmpAng * Mathf.Deg2Rad);
		Vector2 startPos = new Vector2(x, y);

		bool canPlot = (Vector2.Distance(startPos, center1) >= radius * .85f) ? true : false;
		if (!canPlot) return null;

		tmpDir = startPos - center1;
		tmpAng = Mathf.Atan2(tmpDir.y, tmpDir.x) * Mathf.Rad2Deg;
		tmpAng = (tmpAng + 360) % 360;

		float t = (tmpAng > 180) ? tmpAng - 360 : tmpAng;

		ang = tmpAng;
		float totalAngle = (t < 0) ? 90f - t : 90f - tmpAng;
		if (topRightCorner)
			calcPoints(center1, totalAngle);
		else
			notCalcPoints(vTR, totalAngle);

		if (topLeftCorner)
			calcPoints(center2, totalAngle);
		else
			notCalcPoints(vTL, totalAngle);


		// curveBottom angles
		tmpDir = vTL - vBL;
		tmpAng = Mathf.Atan2(tmpDir.y, tmpDir.x) * Mathf.Rad2Deg;
		tmpAng = (tmpAng + 360) % 360;
		x = vBL.x + adjBL * Mathf.Cos(tmpAng * Mathf.Deg2Rad);
		y = vBL.y + adjBL * Mathf.Sin(tmpAng * Mathf.Deg2Rad);
		startPos = new Vector2(x, y);

		canPlot = (Vector2.Distance(startPos, center3) >= radius * .9f) ? true : false;
		if (!canPlot) return null;

		tmpDir = startPos - center3;
		tmpAng = Mathf.Atan2(tmpDir.y, tmpDir.x) * Mathf.Rad2Deg;
		tmpAng = (tmpAng + 360) % 360;

		ang = tmpAng;
		totalAngle = 270 - tmpAng;
		if (bottomLeftCorner)
			calcPoints(center3, totalAngle);
		else
			notCalcPoints(vBL, totalAngle);

		if (bottomRightCorner)
			calcPoints(center4, totalAngle);
		else
			notCalcPoints(vBR, totalAngle);

		return points.ToArray();
	}



	void calcPoints(Vector2 ctr, float totAngle) {

		for (int i = 0; i <= smoothness; i++) {
			float a = ang * Mathf.Deg2Rad;
			float x = ctr.x - offset.x + radius * Mathf.Cos(a);
			float y = ctr.y - offset.y + radius * Mathf.Sin(a);

			points.Add(new Vector2(x, y));
			ang += totAngle / smoothness;
		}
		ang -= 90f / smoothness;
	}

	void notCalcPoints(Vector2 vertex, float totAngle) {
		points.Add(vertex);
		for (int i = 0; i <= smoothness; i++) 
			ang += totAngle / smoothness;
		
		ang -= 90f / smoothness;
	}
}
#endif
