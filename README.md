# A* pathfinding for Unity

2D focused pathfinding asset for Unity3D.  [Blog to preview.](https://elementalecho.wordpress.com/2017/05/17/a-pathfinding-for-unity2d/)

## Features
* Finding path from point A to Point B
* Moving unit from point A to Point B
* Dynamic collider check to create smoother paths.
* Randomizive when paths are calculated
* Calculate new path only when target moves
* Option to prevent corner cutting
* Weighted paths
* Show calculated path
* Updating nodes to check if they are walkable (Useful grids that are changing, but this is really expensive)
* Option to change heuristics (Can change algorithm work like standard A*, Dijkstra's or greedy best first)
* Use a Binary Heap to maintain your open list.


### Bugs

* Sometimes cuts corners
* If seeker node is out of bounds, it causes error
* If target is unreachable, game lags because it tries to search the whole grid.


### ToDo

* Improved inspector
* Option to run pathfinding on other thread
* React to other units (ex. Avoids other units moving in path=
* Recalculate path if colliders around unit are changing



## Authors

* **Mika Savolainen** - [@WaweTheWaras](https://twitter.com/WaweTheWaras)



## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details


