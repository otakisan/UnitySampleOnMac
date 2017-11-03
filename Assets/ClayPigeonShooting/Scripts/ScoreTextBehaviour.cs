using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTextBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.clayManger = ClayManagerBehaviour.GetClayManager();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private ClayManagerBehaviour clayManger;
    public void UpdateScore(string groupName, CompleteAction action)
    {
        
        bool valid = this.clayManger.IsActionValid(groupName, action);
        Debug.Log(action.ToString() + (valid ? "妥当" : "不正"));

        var scoreTextLabel = GetComponent<UnityEngine.UI.Text>();
        int score = 0;
        if (int.TryParse(scoreTextLabel.text, out score))
        {
            scoreTextLabel.text = (score + (action == CompleteAction.Hit ? 100 : 30) * (valid ? 1 : -1)).ToString();
        }
    }

    public int GetScore()
    {
        var scoreTextLabel = GetComponent<UnityEngine.UI.Text>();
        int score = 0;
        int.TryParse(scoreTextLabel.text, out score);
        return score;
    }

    public static ScoreTextBehaviour Instance
    {
        get
        {
            return GameObject.Find("ScoreText").GetComponent<ScoreTextBehaviour>();
        }
    }

}
