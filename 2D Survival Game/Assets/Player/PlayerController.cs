using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public bool grounded;
    
    private Rigidbody2D rb;
    private Animator anim;

    private float horizontal;
    public bool hit;

    [HideInInspector]
    public Vector2 spawnPos;
    public TerrainGeneration terrGen;
    public Vector2Int mousePos;

    public void Spawn() {
        GetComponent<Transform>().position = spawnPos;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D col) {
        if (col.CompareTag("Ground"))
            grounded = true;
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(col.CompareTag("Ground"))
            grounded = false;
    }

    private void FixedUpdate() {
        horizontal = Input.GetAxis("Horizontal");
        float jump = Input.GetAxisRaw("Jump");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new(horizontal * moveSpeed, rb.velocity.y);

        hit = Input.GetMouseButton(0);
        if(hit) {
            terrGen.RemoveTile(mousePos.x, mousePos.y);
        }

        if (horizontal > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (horizontal < 0)
            transform.localScale = new Vector3(1, 1, 1);

        if ((vertical > 0.1f || jump > 0.1f)) {
            if(grounded)
                movement.y = jumpForce;
        }

        rb.velocity = movement;
    }

    private void Update() {
        mousePos.x = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - 0.5f);
        mousePos.y = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - 0.5f);

        anim.SetFloat("Horizontal", horizontal);
        anim.SetBool("Hit", hit);
    }
}
