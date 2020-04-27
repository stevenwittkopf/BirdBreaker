using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour {

    public SpriteRenderer spriteRenderer;
    public float turnRate;

    // Start is called before the first frame update
    void Start() {
        this.spriteRenderer.color = new Color(1f, 1f, 1f, 0.75f);
        // Subscribe to OnPlaneMove event
        GameController.main.OnPlaneMove += this.OnPlaneMove;
    }

    // Update is called once per frame
    void Update() {
        this.transform.Rotate(Vector3.forward, this.turnRate);
    }

    // Moves the propellor to where the player is going to be after its location is updated.
    void OnPlaneMove() {
        var player = GameController.main.player.GetComponent<Biplane>();
        this.transform.position = (player.transform.position.x + player.rigidbody2D.velocity.x * Time.fixedDeltaTime - this.transform.position.x) * Vector3.right + this.transform.position;
    }
}
