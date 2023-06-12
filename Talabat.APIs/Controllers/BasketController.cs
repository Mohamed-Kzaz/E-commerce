using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        // Get And Recreate
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);

            return basket ?? new CustomerBasket(id);
        }

        // Update And Create For First Time
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var createdOrUpdatedbasket = await _basketRepository.UpdateBasketAsync(mappedBasket);

            if (createdOrUpdatedbasket is null)
                return BadRequest(new ApiResponse(400));

            return createdOrUpdatedbasket;
        }


        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
           return await  _basketRepository.DeleteBasketAsync(id);
        }
    }
}
