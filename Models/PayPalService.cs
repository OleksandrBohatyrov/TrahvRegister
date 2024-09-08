using PayPal.Api;
using System.Collections.Generic;
using System.Configuration;

public class PayPalService
{
    // Настройка APIContext с учетными данными PayPal
    private APIContext GetAPIContext()
    {
        var config = ConfigManager.Instance.GetProperties();
        string accessToken = new OAuthTokenCredential(config).GetAccessToken();
        return new APIContext(accessToken);
    }

    public Payment CreatePayment(decimal amount, string returnUrl, string cancelUrl)
    {
        var apiContext = GetAPIContext();

        // Настройка деталей платежа
        var payment = new Payment
        {
            intent = "sale",
            payer = new Payer { payment_method = "paypal" },
            transactions = new List<Transaction>
            {
                new Transaction
                {
                    amount = new Amount
                    {
                        currency = "EUR",
                        total = amount.ToString("F2") // Сумма к оплате
                    },
                    description = "Оплата штрафа через PayPal"
                }
            },
            redirect_urls = new RedirectUrls
            {
                return_url = returnUrl,
                cancel_url = cancelUrl
            }
        };

        // Создаем платеж
        var createdPayment = payment.Create(apiContext);

        return createdPayment;
    }

    // Выполнение платежа после возвращения пользователя с PayPal
    public Payment ExecutePayment(string paymentId, string payerId)
    {
        var apiContext = GetAPIContext();
        var paymentExecution = new PaymentExecution { payer_id = payerId };
        var payment = new Payment { id = paymentId };
        return payment.Execute(apiContext, paymentExecution);
    }
}
