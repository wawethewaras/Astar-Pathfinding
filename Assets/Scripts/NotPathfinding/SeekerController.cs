using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CountPath))]
public class SeekerController : MonoBehaviour {

    CountPath counter;
    public Transform target;
    public bool readyToCountPath;

    public bool autoCountPath;

    void Start () {
        counter = GetComponent<CountPath>();
        readyToCountPath = true;
    }

    // Update is called once per frame
    void Update () {
        if (autoCountPath && readyToCountPath)
        {
            counter.FindPath(transform, target);
        }
        else {
            if (Input.GetButtonDown("Jump"))
            {
                counter.FindPath(transform, target);
            }
        }
    }
}
