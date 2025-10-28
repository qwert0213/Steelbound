using NUnit.Framework;
using UnityEngine;

public class ItemCountEditTests
{
    [Test]
    public void AddQuantity_IncreasesCount()
    {
        var go = new GameObject();
        var item = go.AddComponent<ItemCount>();
        item.AddQuantity(5);
        Assert.AreEqual(5, item.Count);
        item.AddQuantity(3);
        Assert.AreEqual(8, item.Count);
        Object.DestroyImmediate(go);
    }
}
