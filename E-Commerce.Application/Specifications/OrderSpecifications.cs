using E_Commerce.Domain.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    public class OrderSpecifications:BaseSpecification<Order,Guid>
    {
        public OrderSpecifications(string email):base (o=>o.BuyerEmail==email)
        {
            AddInclude(o => o.DeliveryMethod);
            AddInclude(o => o.Items);
        }

        public OrderSpecifications(Guid id, string email): base(o=>o.Id==id && o.BuyerEmail == email)
        {
            AddInclude(o => o.DeliveryMethod);
            AddInclude(o => o.Items);
        }
    }

}
