using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// den h�r koden �r kopierad fr�n ett forum, jag har v�ldigt liten aning om vad varje rad g�r separat, men tillsammans sorterar de alla children ur en parent i bokstavsordning. - Anton
// pga build errors s� kommentifieras koden innan builds, s� det �r med stor sannolikhet att du ser koden som en stor kommentar. - Anton
public class SortChildren : MonoBehaviour
{
	/*[ContextMenu("Sort")]
	public void SortChildrenByName()
	{
		foreach (GameObject obj in Selection.gameObjects)
		{
			List<Transform> children = new List<Transform>();
			for (int i = obj.transform.childCount - 1; i >= 0; i--)
			{
				Transform child = obj.transform.GetChild(i);
				children.Add(child);
				child.parent = null;
			}
			children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });
			foreach (Transform child in children)
			{
				child.parent = obj.transform;
			}
		}
		Debug.Log("Sorted!");

	}*/
}