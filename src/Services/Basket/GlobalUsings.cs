﻿global using Ardalis.Result;
global using Ardalis.Result.AspNetCore;
global using Bazaar.Basket.Application.DTOs;
global using Bazaar.Basket.Application.IntegrationEvents;
global using Bazaar.Basket.Application.Services;
global using Bazaar.Basket.Application.Validators;
global using Bazaar.Basket.Domain.Entites;
global using Bazaar.Basket.Domain.Exceptions;
global using Bazaar.Basket.Domain.Interfaces;
global using Bazaar.Basket.Infrastructure.Database;
global using Bazaar.Basket.Infrastructure.Repositories;
global using Bazaar.Basket.Web.Messages;
global using Bazaar.BuildingBlocks.EventBus;
global using Bazaar.BuildingBlocks.EventBus.Abstractions;
global using Bazaar.BuildingBlocks.EventBus.Events;
global using Bazaar.BuildingBlocks.EventBusRabbitMQ;
global using Bazaar.BuildingBlocks.JsonAdapter;
global using EntityFrameworkCore.Triggered;
global using FluentValidation;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using RabbitMQ.Client;
