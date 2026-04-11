using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthSlider;
    public Image fillImage;
    
    [Header("Color Settings")]
    public Color highHealthColor = Color.green;
    public Color lowHealthColor = Color.red;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        if (healthSlider == null) healthSlider = GetComponentInChildren<Slider>();
    }

    private void LateUpdate()
    {
        // Billboard logic: Luôn hướng về phía Camera chính
        if (_mainCamera != null)
        {
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                             _mainCamera.transform.rotation * Vector3.up);
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthSlider == null) return;

        float fillAmount = currentHealth / maxHealth;
        healthSlider.value = fillAmount;

        // Đổi màu thanh máu dựa trên lượng máu còn lại
        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(lowHealthColor, highHealthColor, fillAmount);
        }
    }
}
