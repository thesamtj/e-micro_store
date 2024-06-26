﻿using AutoMapper;
using Basket.Application.Requests;
using Basket.Application.Responses;
using Basket.Core.Entities;
using EventBus.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Application.Mappers
{
    public class BasketMappingProfile : Profile
    {
        public BasketMappingProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartResponse>().ReverseMap();            
            CreateMap<ShoppingCartItem, ShoppingCartItemResponse>().ReverseMap();
            CreateMap<ShoppingCartItem, ShoppingCartItemRequest>().ReverseMap();
            CreateMap<BasketCheckout, BasketCheckoutEvent>().ReverseMap();
            CreateMap<BasketCheckoutV2, BasketCheckoutEventV2>().ReverseMap();
        }
    }
}
