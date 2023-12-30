﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Infrastructure.EntityConfigurations
{
    class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> orderItemConfiguration)
        {
            orderItemConfiguration.ToTable("orderItems", OrderDbContext.DEFAULT_SCHEMA);

            orderItemConfiguration.HasKey(o => o.Id);

            orderItemConfiguration.Ignore(b => b.DomainEvents);

            orderItemConfiguration.Property(o => o.Id).ValueGeneratedOnAdd();

            orderItemConfiguration.Property<int>("OrderId").IsRequired();
        }
    }
}