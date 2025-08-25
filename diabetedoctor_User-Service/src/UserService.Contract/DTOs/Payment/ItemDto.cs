namespace UserService.Contract.DTOs.Payment;

public sealed class ItemDto
{
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }

    public ItemDto(string itemName, int quantity, int price)
    {
        ItemName = itemName;
        Quantity = quantity;
        Price = price;
    }
}
