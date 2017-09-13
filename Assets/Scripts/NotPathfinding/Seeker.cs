using UnityEngine;

[RequireComponent(typeof(CountPath))]
public class Seeker2 : MonoBehaviour {
    private CountPath counter;

    void Start() {
        counter = GetComponent<CountPath>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            counter.FindPath(transform, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}