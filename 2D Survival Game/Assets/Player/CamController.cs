using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [Range(0, 1)]
    public float smoothTime;
    public Transform player;

    private Transform cam;

    public void Start() {
        cam = GetComponent<Transform>();
    }

    public void FixedUpdate() {
        Vector3 pos = cam.position;

        pos.x = Mathf.Lerp(pos.x, player.position.x, smoothTime);
        pos.y = Mathf.Lerp(pos.y, player.position.y, smoothTime);

        cam.position = pos;
    }
}
