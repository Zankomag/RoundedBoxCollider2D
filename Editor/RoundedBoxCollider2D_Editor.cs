
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoundedBoxCollider2D))]
public class RoundedBoxCollider2D_Editor : Editor {

	RoundedBoxCollider2D rb;
	PolygonCollider2D polyCollider;
	Vector2 off;

	void OnEnable() {
		rb = (RoundedBoxCollider2D)target;

		polyCollider = rb.GetComponent<PolygonCollider2D>();
		if (polyCollider == null) {
			polyCollider = rb.gameObject.AddComponent<PolygonCollider2D>();
		}

		Vector2[] pts = rb.getPoints();
		if (pts != null) polyCollider.points = pts;
	}

	public override void OnInspectorGUI() {
		GUI.changed = false;
		DrawDefaultInspector();

		float lesser = (rb.width > rb.height) ? rb.height : rb.width;
		lesser /= 2f;
		lesser = Mathf.Round(lesser * 100f) / 100f;
		rb.radius = Mathf.Clamp(rb.radius, 0f, lesser);

		if (GUI.changed || !off.Equals(polyCollider.offset)) {
			Vector2[] pts = rb.getPoints();
			if (pts != null) polyCollider.points = pts;
		}

		off = polyCollider.offset;
	}

}


