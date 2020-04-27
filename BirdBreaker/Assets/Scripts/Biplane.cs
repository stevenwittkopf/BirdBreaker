using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biplane : MonoBehaviour {

    public enum RollingState {
        Flying,
        Rolling
    }

    // instance variables set in editor
    public float turnRate;
    public float turnLimit;
    public float speed;
    public float centeringRate;
    public float rollCooldown;
    public float rollRate;
    public Rigidbody2D rigidbody2D;
    public SpriteRenderer spriteRenderer;

    // instance variables set in script
    private float halfWidth;
    private IEnumerator<float?> rollEnumerator;
    public RollingState rollingState;
    public float nextRollTime;

    // Returns a the euler angle z-rotation standardized between -180 and +180 degrees.
    public float ZRotation {
        get {
            return this.transform.eulerAngles.z - 360 * Mathf.Round(this.transform.eulerAngles.z / 360);
        }
    }

    public float HalfWidth {
        get {
            return this.halfWidth;
        }
    }

    // Start is called before the first frame update
    void Start() {
        this.halfWidth = this.spriteRenderer.bounds.size.x / 2;
        this.rollingState = RollingState.Flying;
        this.nextRollTime = 0;
    }

    // Update is called once per frame
    void Update() {

    }

    // Update is called once per fixed physics interval
    void FixedUpdate() {
        if (this.rollingState == RollingState.Flying) {
            this.FlyingMovementUpdate();
        }
        else {
            this.RollingMovementUpdate();
        }
        // Boundary Check
        float bound = GameController.main.WorldHalfWidth - this.HalfWidth;
        if (this.rigidbody2D.velocity.x < 0) {
            if (this.transform.position.x < -bound) {
                this.transform.position = this.transform.position + (-bound - this.transform.position.x) * Vector3.right;
            }
        }
        else if (this.rigidbody2D.velocity.x > 0) {
            if (this.transform.position.x > bound) {
                this.transform.position = this.transform.position + (bound - this.transform.position.x) * Vector3.right;
            }
        }
        // Invoke OnPlaneMoveEvent
        GameController.main.PlaneMoved();
    }

    void FlyingMovementUpdate() {
        // Turns based on user input. +1 for positive rotation (left), -1 for negative rotation (right), 0 for no change
        int turnDirection;
        turnDirection = (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0) - (Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
        if (turnDirection != 0) {
            this.transform.Rotate(Vector3.forward, 360 * this.turnRate * Time.fixedDeltaTime * turnDirection);
            if (Mathf.Abs(this.ZRotation) > this.turnLimit) {
                this.transform.rotation = Quaternion.Euler(0, 0, turnDirection * this.turnLimit);
                
            }
        }
        else {
            turnDirection = (int)-Mathf.Sign(this.ZRotation);
            this.transform.Rotate(Vector3.forward, 360 * this.centeringRate * turnRate * Time.fixedDeltaTime * turnDirection);
            if (turnDirection * this.ZRotation > 0) {
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            turnDirection = 0;
        }
        // Determines x-velocity based on current rotation, drifting left or right according to angle.
        if (Time.fixedTime >= this.nextRollTime && turnDirection != 0 && Input.GetKeyDown(KeyCode.Space)) {
            this.rollingState = RollingState.Rolling;
            this.rigidbody2D.velocity = -turnDirection * this.speed * Vector2.right;
            this.rollEnumerator = this.Roll(turnDirection).GetEnumerator();
            this.nextRollTime = Time.fixedTime + this.rollCooldown;
        }
        else {
            this.rigidbody2D.velocity = -this.ZRotation / this.turnLimit * this.speed * Vector2.right;
        }
    }

    IEnumerable<float?> Roll(float direction) {
        float initialRotation, bound;
        initialRotation = this.ZRotation;
        bound = 360 + this.turnLimit - direction * initialRotation;
        for (float i = 0; i <= bound; i += this.rollRate) {
            yield return initialRotation + direction * i;
        }
        // Ensures you always end up at the turn limit in the given direction
        yield return direction * this.turnLimit;
        // Ceases iteration
        yield return null;
    }

    void RollingMovementUpdate() {
        this.rollEnumerator.MoveNext();
        if (this.rollEnumerator.Current.HasValue) {
            this.transform.rotation = Quaternion.Euler(0, 0, (float)this.rollEnumerator.Current);
        }
        else {
            this.rollingState = RollingState.Flying;
        }
    }
}
