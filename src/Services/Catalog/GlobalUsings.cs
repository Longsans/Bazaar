global using Ardalis.Result;
global using Ardalis.Result.AspNetCore;
global using Ardalis.Specification;
global using Bazaar.BuildingBlocks.EventBus;
global using Bazaar.BuildingBlocks.EventBus.Abstractions;
global using Bazaar.BuildingBlocks.EventBus.Events;
global using Bazaar.BuildingBlocks.EventBusRabbitMQ;
global using Bazaar.BuildingBlocks.JsonAdapter;
global using Bazaar.Catalog.Application.EventHandling;
global using Bazaar.Catalog.Application.IntegrationEvents;
global using Bazaar.Catalog.Application.Services;
global using Bazaar.Catalog.Application.Specifications;
global using Bazaar.Catalog.Domain.Entities;
global using Bazaar.Catalog.Domain.Enums;
global using Bazaar.Catalog.Domain.Exceptions;
global using Bazaar.Catalog.Domain.Interfaces;
global using Bazaar.Catalog.Infrastructure.Database;
global using Bazaar.Catalog.Infrastructure.Repositories;
global using Bazaar.Catalog.Web.Messages;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using RabbitMQ.Client;
global using System.Text.Json.Serialization;
