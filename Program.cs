using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TestAutoMapCollection.DTOs;
using TestAutoMapCollection.Models;

namespace TestAutoMapCollection
{
    class Program
    {
        private static IMapper _mapper;

        private static void CreateOrder()
        {
            using (var context = new ContextFactory().CreateDbContext(null))
            {
                if (!context.Orders.Any())
                {
                    context.Orders.Add(new Order()
                    {
                        Description = "This is an order",
                        OrderLines = new List<OrderLine>()
                        {
                            new OrderLine()
                            {
                                Description = "This is an order line"
                            },
                            new OrderLine()
                            {
                                Description = "This is another order line"
                            },
                            new OrderLine()
                            {
                                Description = "This is yet annother order line"
                            }
                        }
                    });
                }

                context.SaveChanges();
            }
        }

        private static OrderDTO GetOrderDto()
        {
            OrderDTO orderDto = null;

            using (var context = new ContextFactory().CreateDbContext(null))
            {
                var data = context.Orders
                    .Include(x => x.OrderLines)
                    //.ThenInclude(x => x.Products)
                    .FirstOrDefault();

                orderDto = _mapper.Map<OrderDTO>(data);
            }

            return orderDto;
        }

        private static void SaveOrder(OrderDTO orderDto)
        {
            using (var context = new ContextFactory().CreateDbContext(null))
            {
                var fromDataBase = context.Orders
                    .Include(x => x.OrderLines)
                    .FirstOrDefault(x => x.OrderId == orderDto.OrderId);

                _mapper.Map(orderDto, fromDataBase);

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    var baseMessage = ex.GetBaseException().Message;
                    Console.WriteLine(baseMessage);
                }
            }
        }

        static void Main(string[] args)
        {
            #region Setup That would normally be done by DI

            ServiceCollection services = new ServiceCollection();

            services.AddDbContext<DataContext>(ServiceLifetime.Scoped);

            services.AddEntityFrameworkSqlServer();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<Profiles>();
            });

            _mapper = new Mapper(config);

            // Who needs to properly seed the database? Not me.
            CreateOrder();

            #endregion

            // Get the order
            var orderDto = GetOrderDto();

            //Change the description on the order and its order lines
            orderDto.Description = "This is different.";
            orderDto.OrderLines.ForEach(ol => ol.Description = "This has been changed too.");

            //Save the order back to the database
            SaveOrder(orderDto);
        }
    }

    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<Order, OrderDTO>().ReverseMap();

            CreateMap<OrderLine, OrderLineDTO>().ReverseMap();
        }
    }

    #region Profiles with the AfterMap Used
    //internal class Profiles : Profile
    //{
    //    public Profiles()
    //    {
    //        CreateMap<OrderDTO, Order>()
    //            .ForMember(d => d.OrderLines, opt => opt.Ignore())
    //            .AfterMap(AddOrUpdate)
    //            .ReverseMap();

    //        CreateMap<OrderLine, OrderLineDTO>()
    //            .ReverseMap();
    //    }
    //    private void AddOrUpdate(OrderDTO orderDto, Order order)
    //    {
    //        foreach (var dto in orderDto.OrderLines)
    //        {
    //            if (dto.OrderLineId == 0)
    //            {
    //                //order.OrderLines.Add(Mapper.Map<Order>(dto));
    //                order.OrderLines.Add(new OrderLine(){
    //                    Description = dto.Description,
    //                    OrderId = dto.Order.OrderId
    //                });
    //            }
    //            else
    //            {
    //                var ol = order.OrderLines.SingleOrDefault(x => x.OrderLineId == dto.OrderLineId);

    //                ol.Description = dto.Description;

    //                //Mapper.Map(dto, order.OrderLines.SingleOrDefault(c => c.OrderLineId == dto.OrderLineId));
    //            }
    //        }
    //    }
    //}
    #endregion region
}
