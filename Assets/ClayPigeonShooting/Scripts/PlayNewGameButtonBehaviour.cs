using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNewGameButtonBehaviour : MonoBehaviour {

    public string newGameSceneName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void HandleClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(this.newGameSceneName);
    }
}
