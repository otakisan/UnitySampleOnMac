using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalScoreTextBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<UnityEngine.UI.Text>().text = $"Score : {SharedDataAmongScenes.finalScore.ToString()}";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
