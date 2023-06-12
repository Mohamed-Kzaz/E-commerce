using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Core.Entities.Order_Aggregate
{
    public class Order : BaseEntity
    {
        public Order()
        {

        }

        public Order(string buyerEmail, Address shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            PaymentIntentId = paymentIntentId;
            Items = items;
            SubTotal = subTotal;
        }

        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; } // [ONE]
        public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>(); // [Many]
        public decimal SubTotal { get; set; }

        public decimal GetTotal()
            => SubTotal + DeliveryMethod.Cost;

        public string PaymentIntentId { get; set; }
    }
}
