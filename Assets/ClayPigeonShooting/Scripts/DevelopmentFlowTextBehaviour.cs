using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopmentFlowTextBehaviour : MonoBehaviour {

    private UnityEngine.UI.Text textCompo;

	// Use this for initialization
	void Start () {
        this.textCompo = GetComponent<UnityEngine.UI.Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetText(string text)
    {
        textCompo.text = text;
        //textCompo.rectTransform.sizeDelta = new Vector2(textCompo.rectTransform.sizeDelta.x, textCompo.preferredHeight);
    }
}
