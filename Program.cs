using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using AutoMapper.EquivalencyExpression;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
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

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(baseMessage);
                    Console.ResetColor();
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
                cfg.AddCollectionMappers();
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

    public static class AutoMapperCollectionExtensions{
        public static void MapCollection<TSource, TDest, IDType>(this ResolutionContext context, List<TSource> dtos, List<TDest> databaseEntities, Func<TSource, IDType> sourceId, Func<TDest, IDType> destinationId)
        {
            foreach (var item in dtos)
            {
                if (sourceId(item).Equals(default(IDType)))
                {
                    databaseEntities.Add(context.Mapper.Map<TSource, TDest>(item));
                }
                else
                {
                    var exist = databaseEntities.SingleOrDefault(x => destinationId(x).Equals(sourceId(item)));

                    if (exist != null)
                    {
                        context.Mapper.Map(item, exist);
                    }
                }
            }
        }
    }

    public class Profiles : Profile
    {
        public Profiles()
        {
            
        CreateMap<Order, OrderDTO>();

            CreateMap<OrderDTO, Order>()
                .ForMember(dest => dest.OrderLines, map => map.Ignore())
                .MaxDepth(5)
                .AfterMap((dto, order, ctx) =>
                {
                    ctx.MapCollection(dto.OrderLines, order.OrderLines, lineDto => lineDto.OrderLineId, line => line.OrderLineId);
                });

            CreateMap<OrderLine, OrderLineDTO>()
                .MaxDepth(5);

            CreateMap<OrderLineDTO, OrderLine>()
                .ForMember(dest=> dest.Order, map => map.Ignore())
                .MaxDepth(5);
        }
    }
}
