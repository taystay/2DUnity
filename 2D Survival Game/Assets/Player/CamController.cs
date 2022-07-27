using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [Range(0, 1)]
    public float smoothTime;
    public Transform player;

    [HideInInspector]
    public int worldSize;
    private float orthoSize;

    public void Spawn(Vector3 pos) {
        GetComponent<Transform>().position = pos;
        orthoSize = GetComponent<Camera>().orthographicSize;
    }

    public void FixedUpdate() {
        Vector3 pos = GetComponent<Transform>().position;

        pos.x = Mathf.Lerp(pos.x, player.position.x, smoothTime);
        pos.y = Mathf.Lerp(pos.y, player.position.y, smoothTime);

        pos.x = Mathf.Clamp(pos.x, 0 + (orthoSize * 2.05f), worldSize - (orthoSize * 2.05f));

        GetComponent<Transform>().position = pos;
    }
}
