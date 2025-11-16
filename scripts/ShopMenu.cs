using Godot;
using System;
using System.Collections.Generic;

public partial class ShopMenu : Control
{
    private VBoxContainer _itemList;
    private Control _itemTemplate;

    // Here is your item list
    public List<ShopItem> Items = new List<ShopItem>()
    {
        new ShopItem("Sværd", 50),
        new ShopItem("Skjold", 30),
        new ShopItem("Potion", 10)
    };

    public override void _Ready()
    {
        // Get nodes via code
        _itemList = GetNode<VBoxContainer>("VBoxContainer");
        _itemTemplate = _itemList.GetNode<Control>("ItemEntry");

        // Hide template
        _itemTemplate.Visible = false;

        // Build shop UI
        BuildShopUI();
    }

    private void BuildShopUI()
    {
        foreach (var item in Items)
        {
            var entry = (Control)_itemTemplate.Duplicate();
            entry.Visible = true;

            // Set text
            entry.GetNode<Label>("ItemNavn").Text = item.Name;
            entry.GetNode<Label>("Pris").Text = item.Price.ToString();

            // Connect button via code
            var buyButton = entry.GetNode<Button>("Køb");
            buyButton.Pressed += () => OnBuy(item);

            _itemList.AddChild(entry);
        }
    }

    private void OnBuy(ShopItem item)
    {
        GD.Print($"Du købte: {item.Name} for {item.Price} guld.");
    }
}
