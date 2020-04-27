using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    // Singleton static game controller object
    public static GameController main;
    
    // Local variables
    public GameObject player;

    public delegate void GamePlayEventHandler();

    // OnPlaneMove event
    public event GamePlayEventHandler OnPlaneMove;

    public float WorldHalfWidth {
        get;
        private set;
    }

    public void Awake() {
        if (GameController.main == null) {
            GameController.main = this;
        }
        var camera = Camera.main;
        this.WorldHalfWidth = camera.orthographicSize * camera.aspect;
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    // Invokes all functions current subscribed to the OnPlaneMoved event.
    public void PlaneMoved() {
        this.OnPlaneMove?.Invoke();
    }
}
