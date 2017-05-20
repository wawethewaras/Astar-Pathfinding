using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {

    private float deltaTime = 0.0f;

	void Update () {
        if(Time.timeScale > 0)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = Mathf.Round(1.0f / deltaTime);

            GetComponent<Text>().text = "FPS: " + fps;
        }

	}
}
