using StackExchange.Redis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;

        public BasketRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            var basket = await _database.StringGetAsync(basketId);

            return basket.IsNull ? null : JsonSerializer.Deserialize<CustomerBasket>(basket);
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            var SerializedBasket = JsonSerializer.Serialize<CustomerBasket>(basket); 

            var updatedOrCreatedBasket = await _database.StringSetAsync(basket.Id, SerializedBasket, TimeSpan.FromDays(1));

            if (updatedOrCreatedBasket is false)
                return null;

            return await GetBasketAsync(basket.Id);
        }
    }
}
