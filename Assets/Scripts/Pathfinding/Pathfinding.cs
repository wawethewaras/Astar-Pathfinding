using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pathfinding { 
    public interface Pathfinding
    {

        //void FindPathTest(Transform startPos, Transform endPos);

        void OnPathFound(Vector3[] newPath);

        IEnumerator PathCountDelay();
    

    }
}

