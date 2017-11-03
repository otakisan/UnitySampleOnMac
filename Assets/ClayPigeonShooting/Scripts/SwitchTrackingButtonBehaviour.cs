using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrackingButtonBehaviour : MonoBehaviour {

    private UnityEngine.UI.Button button;
    private Color initialNormalColor;

	// Use this for initialization
	void Start () {
        this.button = GetComponent<UnityEngine.UI.Button>();
        this.initialNormalColor = this.button.colors.normalColor;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void HandleClick()
    {
        var compo = Camera.main.GetComponent<ShooterViewCameraBehaviour>();
        compo.trackingNearestTarget = !compo.trackingNearestTarget;

        this.SetButtonFaceColor(compo.trackingNearestTarget);
    }

    private void SetButtonFaceColor(bool isOn)
    {
        var red = Color.red;
        red.a = 0.5f;
        UnityEngine.UI.ColorBlock cb = button.colors;
        cb.normalColor = isOn ? red : this.initialNormalColor;
        cb.highlightedColor = isOn ? red : this.initialNormalColor;
        this.button.colors = cb;
        this.button.GetComponentInChildren<UnityEngine.UI.Text>().color = isOn ? Color.white : Color.black;
    }
}
