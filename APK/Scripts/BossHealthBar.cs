using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    // Reference to the UI slider that visually represents the boss HP
    public Slider slider;

    void Awake()
    {
        // Automatically grab the Slider component attached to this GameObject
        slider = GetComponent<Slider>();
    }

    // Sets the slider's maximum health and initializes its value. Also ensures the health bar becomes visible.
    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;  // Set the full value bar
        slider.value = maxHealth;     // Start full
        gameObject.SetActive(true);   // Make sure the bar is visible
    }

    // Updates the slider to match the boss's current health.
    public void SetHealth(int health)
    {
        slider.value = health;
    }

    // Hides the health bar when boss is dead or not present.
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
