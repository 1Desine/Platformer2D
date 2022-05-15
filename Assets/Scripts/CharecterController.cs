using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class CharecterController : MonoBehaviour {
    Rigidbody2D _rb;
    SpriteRenderer _sp;
    float inputHorizontal;
    float jumpStamina = 1f;
    float _speed = 500;
    float _jumpForce = 250; 
    float floatingForce = 2;
    float stoppingForce = 5;

    void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _sp = GetComponent<SpriteRenderer>();
    }
    void FixedUpdate() {
        Move();
        FloatingEffect(); // to float above surface and not touch it
    }

    void Update() {
        inputHorizontal = Input.GetAxis("Horizontal");
        Wall_Slide_Climb_Jump(); // responsible for parkour
        CharecterFlip(out var direction);
        Jump();
        StoppingEffect(); // to not slide forever
    }
    // move horizontal and crawl
    void Move() {
            _rb.AddForce(new Vector2(inputHorizontal * _speed, 0));
        }
    // jump, lol
    void Jump() {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        if (Physics2D.Raycast(transform.position,new Vector2(0,-1), 1.7f)) _rb.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);
    }
    // to not scratch flor
    void FloatingEffect() {
        var position = transform.position;
        var up = transform.up;
        
        var floreDistance = Physics2D.Raycast(new Vector2(position.x,position.y-0.9f),up*-1,0.2f);
        if (floreDistance.distance == 0) return;
        _rb.velocity += new Vector2(0, (0.01f / floreDistance.distance) * floatingForce);
    }
    // slide on wall
    void Wall_Slide_Climb_Jump() {
        if (!Input.GetKey(KeyCode.Space)) return;
        CharecterFlip(out var direction);
        
        var position = transform.position;
        var up = transform.up;
        
        var topRaySide = Physics2D.Raycast(new Vector2(position.x + 0.6f * direction, position.y + 0.9f), up*-1, 1);
        var bottomRaySide = Physics2D.Raycast(new Vector2(position.x + 0.6f * direction, position.y),up*-1,0.7f);
        var bottomRay = Physics2D.Raycast(position, up*-1, 1.3f);
        
        // climb
        if (bottomRaySide && !topRaySide) {
            if (bottomRay) return;
            _rb.velocity = new Vector2(0,15);
            return;
        }
        
        /* side jump from wall */
        if (bottomRay) jumpStamina = 1f;
        if (!topRaySide || !bottomRaySide || bottomRay) return;
        jumpStamina += -0.2f * Time.deltaTime;
        if (inputHorizontal < 0 && direction == 1 || inputHorizontal > 0 && direction == -1) { // which direction to jump from wall
            _rb.AddForce(new Vector2(jumpStamina*70*-direction, jumpStamina*210), ForceMode2D.Impulse);
            jumpStamina += -0.05f;
            return;
        }
        if (_rb.velocity.y > -1) return; // to not slow down charecter if its going up
        _rb.velocity = new Vector2(0, -1);
    }
    // flips charecter by x axis
    void CharecterFlip(out int direction) {
        if (_sp.flipX) { direction = -1; } else { direction = 1; }
        if (inputHorizontal < 0) { _sp.flipX = true; } else if (inputHorizontal > 0) _sp.flipX = false;
    }
    // to slowly stop charecter if no move buttons are pressed
    void StoppingEffect() {
        if (inputHorizontal != 0 || Physics2D.Raycast(transform.position, transform.up*-1, 1.3f)) return;
        _rb.velocity += new Vector2(-_rb.velocity.x * stoppingForce * Time.deltaTime,-1) * Time.deltaTime;
    }


    void OnTriggerStay2D(Collider2D coll) {
        if (!coll.gameObject.CompareTag("WallSlide") || !Input.GetKey(KeyCode.Space)) return;
        if (_rb.velocity.y > 1) return;
        
        var velocity = _rb.velocity;
        
        velocity = new Vector2(velocity.x, (velocity.y -10) * Time.deltaTime);
        _rb.velocity = velocity;
    }
}