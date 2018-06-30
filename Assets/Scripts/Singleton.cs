using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astar2DPathFinding.Mika {


public class Singleton<T> : MonoBehaviour where T : Component {

        protected static T instance;
        public static T Instance {
            get {
                if (instance == null) {

                    instance = FindObjectOfType<T>();
                    if (instance == null) {
                        Debug.Log("Could not locate a " + instance.GetType() + ". \n You have to have one in the scene.");
                        Debug.Break();
                    }

                }

                return instance;
            }
        }

        public virtual void Awake() {
            if (instance == null) {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy() {
            instance = null;
        }
    }
}