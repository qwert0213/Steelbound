using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemCount : MonoBehaviour
{
    private int count = 0;
    private Text Text;

    public int Count => count;
    protected void Awake()
    {
        Text = GetComponent<Text>();
    }
    protected void Update()
    {
        Text.text = Convert.ToString(count);
    }
    public void AddQuantity(int quantityToAdd) {
        count += quantityToAdd;
    }
}
