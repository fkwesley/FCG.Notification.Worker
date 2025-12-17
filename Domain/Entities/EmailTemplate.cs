namespace FCG.Notification.Worker.Domain.Entities
{
    public class EmailTemplate
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public static EmailTemplate GetTemplateById(string templateId)
        {
            return templateId switch
            {
                "OrderReceived" => new EmailTemplate
                {
                    Id = "OrderReceived",
                    Subject = "FiapCloudGames - Order Received",
                    Body = "Your order {orderId} has been received."
                },
                "AwaitingPayment" => new EmailTemplate
                {
                    Id = "AwaitingPayment",
                    Subject = "FiapCloudGames - Order Awaiting Payment",
                    Body = "Your order {orderId} is awaiting payment."
                },
                "PaymentReceived" => new EmailTemplate
                {
                    Id = "PaymentReceived",
                    Subject = "FiapCloudGames - Payment Received",
                    Body = "Your payment for {orderId} has been received."
                },
                _ => throw new ArgumentException("Invalid template ID")
            };
        }
    }
}