using UnityEngine;

namespace Astar2DPathFinding.Mika {

    [RequireComponent(typeof(CountPath))]
    public class SeekerController : MonoBehaviour {

        CountPath counter;

        void Start() {
            counter = GetComponent<CountPath>();
        }

        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                counter.FindPath(transform, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }


    }
}