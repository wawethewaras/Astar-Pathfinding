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
        //counter.FindPath(transform, target.position);
        pos = transform.position;
        StartCoroutine(loop());
    }

    void Update()
    {
        //    if (autoCountPath && target != null)
        //    {
        //        counter.FindPath(transform, target.position);
        //    }
        //    else {
        if (Input.GetMouseButtonDown(0)) {
                counter.FindPath(transform, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }


        //    }
    }

    Vector2 pos;

    void movedline() {
        Debug.DrawLine(transform.position, pos, Color.red, 50);
        pos = transform.position;
    }
    IEnumerator loop() {
        while (true) {
            movedline();
            yield return new WaitForSeconds(0.1f);
        }
    }
}