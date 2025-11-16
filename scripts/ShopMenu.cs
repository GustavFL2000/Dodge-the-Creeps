using Godot;
using System;
using System.Collections.Generic;

public partial class ShopMenu : Control
{
    [Signal]
    public delegate void CloseShopEventHandler();
    [Signal]
    public delegate void ItemPurchasedEventHandler(string itemName, int price);

    private VBoxContainer _itemList;
    private Control _itemTemplate;

    public int CurrentCoins { get; set; }
    public List<ShopItem> Items { get; set; } = new List<ShopItem>();

    public override void _Ready()
    {
        // Correctly get nodes using their paths from the root of the scene
        _itemList = GetNode<VBoxContainer>("Panel/VBoxContainer");
        _itemTemplate = _itemList.GetNode<Control>("ItemEntry");
        
        // The template is already set to invisible in the scene file,
        // but we ensure it here as well.
        _itemTemplate.Visible = false;

        GetNode<Button>("Panel/CloseButton").Pressed += OnCloseButtonPressed;

        BuildShopUI();
    }

    private void BuildShopUI()
    {
        // Clear previous items, being careful not to delete the template
        foreach (Node child in _itemList.GetChildren())
        {
            // Check by name to be safe, as the template instance might change
            if (child.Name != "ItemEntry")
            {
                child.QueueFree();
            }
        }

        // Create and add an entry for each item
        foreach (var item in Items)
        {
            var entry = (HBoxContainer)_itemTemplate.Duplicate();
            entry.Visible = true;

            entry.GetNode<Label>("ItemNavn").Text = item.Name;
            entry.GetNode<Label>("Pris").Text = item.Price.ToString() + "$";

            var buyButton = entry.GetNode<Button>("Køb");
            buyButton.Pressed += () => OnBuy(item);

            if (CurrentCoins < item.Price)
            {
                buyButton.Disabled = true;
            }

            _itemList.AddChild(entry);
        }
    }

    private void OnBuy(ShopItem item)
    {
        if (CurrentCoins >= item.Price)
        {
            EmitSignal(SignalName.ItemPurchased, item.Name, item.Price);
        }
    }

    private void OnCloseButtonPressed()
    {
        EmitSignal(SignalName.CloseShop);
    }

    // This method can be called by Main.cs after a purchase to update the UI
    public void Refresh(int newCoinTotal)
    {
        CurrentCoins = newCoinTotal;
        BuildShopUI();
    }
}