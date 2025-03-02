using Player;
using UnityEngine;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    private Slider slider;
    private PlayerMovement player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        slider = GetComponent<Slider>();
        slider.maxValue = player.Health;
        slider.minValue = 0;
        UpdateHealth();
    }

    public void UpdateHealth() { 
        slider.value = player.Health;
    }
}
