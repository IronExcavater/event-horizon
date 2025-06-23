using System;
using UnityEngine;
using UnityEngine.UI;

public class FuelManager : MonoBehaviour
{
    [Header("Fuel Settings")]
    public Image fuelContainerImage;
    public Image fuelFillImage;
    public float maxFuel = 100f;
    public float currentFuel = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        fuelFillImage.fillAmount = currentFuel / maxFuel;
        LowFuelWarningColor();
        FollowCursor();
    }

    private void LowFuelWarningColor()
    {
        if (currentFuel < maxFuel * 0.25f)
        {
            fuelFillImage.color = Color.red;
        }
        else
        {
            fuelFillImage.color = new Color32(255, 200, 0, 255);
        }
    }

    private Color Color32(int v1, int v2, int v3)
    {
        throw new NotImplementedException();
    }

    private void FollowCursor()
    {
        // This works for Screen Space - Overlay canvases
        transform.position = Input.mousePosition;
    }
    
    public void TakeFuelDamage(float damage)
    {
        currentFuel -= damage;
        if (currentFuel < 0)
        {
            currentFuel = 0;
        }
    }
}
