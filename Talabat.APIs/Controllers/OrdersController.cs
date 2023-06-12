using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }


        [ProducesResponseType(typeof(Core.Entities.Order_Aggregate.Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
        {
            var buyerEmai = User.FindFirstValue(ClaimTypes.Email);

            var shippingAddress = _mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);

            var order = await _orderService.CreateOrderAsync(buyerEmai, orderDto.BasketId, orderDto.DeliveryMethodId, shippingAddress);

            if (order is null)
                return BadRequest(new ApiResponse(400));

            return Ok(_mapper.Map<Core.Entities.Order_Aggregate.Order, OrderToReturnDto>(order));
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrderForUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var orders = await _orderService.GetOrdersForUserAsync(buyerEmail);

            return Ok(_mapper.Map<IReadOnlyList<Core.Entities.Order_Aggregate.Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }

        [ProducesResponseType(typeof(Core.Entities.Order_Aggregate.Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var order = await _orderService.GetOrderByIdForUserAsync(email, id);

            if(order is null)
                return NotFound(new ApiResponse(404));

            return Ok(_mapper.Map<Core.Entities.Order_Aggregate.Order, OrderToReturnDto>(order));
        }

        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethods = await _orderService.GetDeliveryMethodsAsync();

            return Ok(deliveryMethods); 
        }
    }
}
