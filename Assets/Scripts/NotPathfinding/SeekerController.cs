using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CountPath))]
public class SeekerController : MonoBehaviour {

    CountPath counter;
    public Transform target;

    public bool autoCountPath;

    void Start () {
        counter = GetComponent<CountPath>();
    }

    void Update () {
        if (autoCountPath && target != null)
        {
            counter.FindPath(transform, target.position);
        }
        else {
            if (Input.GetMouseButtonDown(0)) {
                counter.FindPath(transform, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }
}