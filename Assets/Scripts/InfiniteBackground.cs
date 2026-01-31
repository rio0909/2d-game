using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallaxEffect; 

    void Start()
    {
        startpos = transform.position.x;
        // Gets the width of the image for the loop logic
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // 1. Calculate Horizontal Parallax (Left/Right)
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        // 2. MOVE THE BACKGROUND
        // X = Parallax (Scrolls left/right)
        // Y = cam.transform.position.y (LOCKS TO CAMERA! Moves up/down with you)
        transform.position = new Vector3(startpos + dist, cam.transform.position.y, transform.position.z);

        // 3. Loop Logic (Infinite scrolling)
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}