using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astar2DPathFinding.Mika {


    public class Singleton<T> : MonoBehaviour where T : Component {
        private const string dontDestroySCene = "DontDestroy";

        protected static T instance;
        public static T Instance {
            get {
                if (instance == null) {

                    instance = FindObjectOfType<T>();
                    if (instance == null) {
                        Debug.LogError("Singleton" + instance.GetType() + " missing. Created to prevent error.");

                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }

                }

                return instance;
            }
        }

        public virtual void Awake() {
            if (instance == null) {
                instance = this as T;
                DontDestroyOnLoad(this.gameObject);
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