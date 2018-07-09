using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astar2DPathFinding.Mika { 
    public interface Pathfinding {
        void OnPathFound(Vector2[] newPath);
    
    }
}

