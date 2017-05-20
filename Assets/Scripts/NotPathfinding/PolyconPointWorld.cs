using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyconPointWorld : MonoBehaviour {

    public PolygonCollider2D poly;

	// Use this for initialization
	void Start () {
        foreach (Vector2 poin in poly.points) {
            Vector2 position = (Vector2)transform.position + Vector2.Scale( poin , (Vector2)transform.localScale);
            print(position);

        }

    }
	
	
}
