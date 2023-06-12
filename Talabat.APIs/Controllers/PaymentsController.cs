using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentsController> _logger;
        private const string _whSecret = "whsec_2a0e2c32d0cdab86d9dada8362b1c1a2761175d47e010fa4bd8a4de6667fe73a";

        public PaymentsController(
            IPaymentService paymentService,
            IMapper mapper,
            ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize]
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket is null)
            {
                return BadRequest(new ApiResponse(400, "A Problem With Your Basket"));
            }

            return Ok(basket);
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripWebHook()
        {
            //To Get Body Of The Request Will Come From Stripe
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);

            var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

            Order order;

            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                    order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, true);
                    _logger.LogInformation("Payment Is Succeeded : ", paymentIntent.Id);
                    break;

                case Events.PaymentIntentPaymentFailed:
                    order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, false);
                    _logger.LogInformation("Payment Is Failed : ", paymentIntent.Id);
                    break;
            }

            return new EmptyResult();
        }
    }
}
