/**
 * HPBar Class
 * -----------
 * This class represents an HP bar with a background and a filled HP value.
 * It provides a method to update the HP bar based on the current and maximum HP values.
 * 
 * Public Variables:
 * - background: The Image component representing the background of the HP bar.
 * - hp: The Image component representing the current HP value to be displayed as a red bar.
 * 
 * Methods:
 * - UpdateHPBar(float currentHP, float maxHP): Updates the HP bar based on the current and maximum HP values.
 * 
 * Usage:
 * Attach this script to a GameObject with Image components representing the HP bar in a Unity scene.
 * Customize the public variables in the Unity Editor for specific HP bar properties.
 * Call the UpdateHPBar method to update the HP bar based on the current and maximum HP values.
 */
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    // The Image component representing the background of the HP bar.
    public Image background;

    // The Image component representing the current HP value to be displayed as a red bar.
    public Image hp;

    /**
     * UpdateHPBar Method
     * -------------------
     * Updates the HP bar based on the current and maximum HP values.
     * 
     * Parameters:
     * - currentHP: The current HP value.
     * - maxHP: The maximum HP value.
     */
    public void UpdateHPBar(float currentHP, float maxHP)
    {
        Debug.Log("HP Bar updated.");

        // Calculate the fill amount based on the current and maximum HP values.
        float fillAmount = currentHP / maxHP;

        // Update the fill amount of the HP bar.
        hp.fillAmount = fillAmount;
    }
}
