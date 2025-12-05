using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lumière.Pages.Payment;

public class PaymentIndexModel : PageModel
{
    public string? CurrentPlan { get; set; }
    public string? Message { get; set; }
    public string? CardName { get; set; }
    public string? CardNumberMasked { get; set; }
    public string? Expiry { get; set; }
    public List<BillingItem> BillingHistory { get; set; } = new();

    public void OnGet()
    {
        // Load saved data (in a real app, this would come from a database)
        CurrentPlan = "standard";
        CardName = "John Doe";
        CardNumberMasked = "**** **** **** 4242";
        Expiry = "12/25";

        // Sample billing history
        BillingHistory = new List<BillingItem>
        {
            new() { Date = DateTime.Now.AddDays(-30), Description = "Standard Plan - Monthly", Amount = 14.99m, Status = "Paid" },
            new() { Date = DateTime.Now.AddDays(-60), Description = "Standard Plan - Monthly", Amount = 14.99m, Status = "Paid" },
            new() { Date = DateTime.Now.AddDays(-90), Description = "Basic Plan - Monthly", Amount = 9.99m, Status = "Paid" }
        };
    }

    public IActionResult OnPostSelectPlan(string plan)
    {
        CurrentPlan = plan;
        Message = $"Successfully switched to {plan.ToUpper()} plan!";

        // Reload data
        CardName = "John Doe";
        CardNumberMasked = "**** **** **** 4242";
        Expiry = "12/25";
        BillingHistory = new List<BillingItem>
        {
            new() { Date = DateTime.Now.AddDays(-30), Description = "Standard Plan - Monthly", Amount = 14.99m, Status = "Paid" },
            new() { Date = DateTime.Now.AddDays(-60), Description = "Standard Plan - Monthly", Amount = 14.99m, Status = "Paid" },
            new() { Date = DateTime.Now.AddDays(-90), Description = "Basic Plan - Monthly", Amount = 9.99m, Status = "Paid" }
        };

        return Page();
    }

    public IActionResult OnPostUpdatePayment(string cardName, string cardNumber, string expiry, string cvv)
    {
        // In a real app, this would validate and save the payment method
        CardName = cardName;
        CardNumberMasked = string.IsNullOrEmpty(cardNumber) ? null : "**** **** **** " + cardNumber.Replace(" ", "").Substring(Math.Max(0, cardNumber.Replace(" ", "").Length - 4));
        Expiry = expiry;
        CurrentPlan = "standard";
        Message = "Payment method updated successfully!";

        BillingHistory = new List<BillingItem>
        {
            new() { Date = DateTime.Now.AddDays(-30), Description = "Standard Plan - Monthly", Amount = 14.99m, Status = "Paid" },
            new() { Date = DateTime.Now.AddDays(-60), Description = "Standard Plan - Monthly", Amount = 14.99m, Status = "Paid" },
            new() { Date = DateTime.Now.AddDays(-90), Description = "Basic Plan - Monthly", Amount = 9.99m, Status = "Paid" }
        };

        return Page();
    }

    public class BillingItem
    {
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
