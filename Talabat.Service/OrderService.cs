using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Spec;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService
            )
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }



        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveyMethodId, Address shippingAddress)
        {
            // 1.Get Basket
            var basket = await _basketRepository.GetBasketAsync(basketId);

            // 2.Get Selected Items at Basket
            var orderItems = new List<OrderItem>();

            if(basket?.Items.Count > 0) 
            {
                foreach (var item in basket.Items)
                {
                    var productsRepo = _unitOfWork.Repository<Product>();

                    if(productsRepo is not null)
                    {
                        var product = await productsRepo.GetByIdAsync(item.Id);

                        if(product is not null)
                        {
                            var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);

                            var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);

                            orderItems.Add(orderItem);
                        }
                    }
                   
                }
            }

            // 3.Calculate SubTotlal
            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);

            // 4.Get Delivery Method
            DeliveryMethod deliveryMethod = new DeliveryMethod();
            var deliveryMethodsRepo = _unitOfWork.Repository<DeliveryMethod>();

            if(deliveryMethodsRepo is not null)
                deliveryMethod = await deliveryMethodsRepo.GetByIdAsync(deliveyMethodId);

            // 5.Create Order
            var spec = new OrderWithPaymentIntentIdSpec(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            if (existingOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);

                await _paymentService.CreateOrUpdatePaymentIntent(basket.Id);
            }
                

            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subtotal, basket.PaymentIntentId);

            var ordersRepo = _unitOfWork.Repository<Order>();
            if(ordersRepo is not null)
            {
                await ordersRepo.Add(order);

                // 6.Save
                var result = await _unitOfWork.Complete();
                if(result > 0)
                    return order;
            }

            return order;
        }

        public async Task<Order?> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
        {
            var spec = new OrderSpecifications(buyerEmail, orderId);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec); 

            if(order is null)
                return null;

            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec); 
            return orders;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

            return deliveryMethods;
        }
    }
}
