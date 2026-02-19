using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    // How fast the background scrolls to the left
    [SerializeField] private float scrollSpeed = 2f;

    // Original position of the background
    private Vector3 startPos;

    // Width of the sprite to know when to reset position
    private float repeatWidth;

    // Reference to the SpriteRenderer component
    private SpriteRenderer _sr;

    private void Start()
    {
        // Store the starting position of the background
        startPos = transform.position;

        // Get the SpriteRenderer attached to this GameObject
        _sr = GetComponent<SpriteRenderer>();

        // Calculate width of the sprite to know when to reset it
        repeatWidth = _sr.bounds.size.x;
    }

    private void Update()
    {
        // Move the background left at scrollSpeed, frame-rate independent
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // If the background has moved completely off-screen
        if (transform.position.x < startPos.x - repeatWidth)
        {
            // Reset position to the starting point to create a seamless loop
            transform.position = startPos;
        }
    }
}
