using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    [SerializeField] private Text scoreLabel;
    [SerializeField] private SettingsPopup settingsPopup;

    private int _score;

    void Awake() {
        Messenger.AddListener(GameEvent.ENEMY_HIT, OnEnemyHit);
    }

    void OnDestroy() {
        Messenger.RemoveListener(GameEvent.ENEMY_HIT, OnEnemyHit);
    }

	// Use this for initialization
	void Start () {
        _score = 0;
        scoreLabel.text = _score.ToString();

        settingsPopup.Close();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void OnEnemyHit() {
        _score += 1;
        scoreLabel.text = _score.ToString();
    }

    public void OnOpenSettings() {
        settingsPopup.Open();
    }

    public void OnPointerDown() {
        Debug.Log("down");
    }
}
