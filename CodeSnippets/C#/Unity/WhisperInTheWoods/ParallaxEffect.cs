using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ParallaxEffect : MonoBehaviour
{
    public float Margin;
    public float Layer;
    float x;
    float y;
    float Easing = 0.2f;
    Vector3 pos;
    void Start()
    {
        pos = transform.position;
    }

    void Update()
    {
        UpdatePosition(new Vector3());
    }

    void UpdatePosition(Vector3 position)
    {
        float targetX = Input.mousePosition.x - Screen.width / 2f;
        float dx = targetX - x;
        x += dx * Easing;
        float targetY = Input.mousePosition.y - Screen.height / 2f;
        float dy = targetY - y;
        y += dy * Easing;
        Vector3 direction = new Vector3(x, y, 0f);
        Vector3 depth = new Vector3(0f, 0f, Layer);
        transform.position = pos - direction / 500f * Margin + depth;
    }
}