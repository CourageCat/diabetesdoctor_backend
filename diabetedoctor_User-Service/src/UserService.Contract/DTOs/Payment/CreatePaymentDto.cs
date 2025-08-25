namespace UserService.Contract.DTOs.Payment;

public sealed class CreatePaymentDto
{
    public long OrderCode { get; set; }
    public string Description { get; set; }
    public List<ItemDto> Items { get; set; }
    public string CancelUrl { get; set; }
    public string ReturnUrl { get; set; }

    public CreatePaymentDto(long orderCode, string description, List<ItemDto> items, string cancelUrl, string returnUrl)
    {
        OrderCode = orderCode;
        Description = description;
        Items = items;
        CancelUrl = cancelUrl;
        ReturnUrl = returnUrl;
    }
}

