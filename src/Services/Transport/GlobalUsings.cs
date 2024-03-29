﻿global using Ardalis.Result;
global using Ardalis.Result.AspNetCore;
global using Ardalis.Specification;
global using Bazaar.BuildingBlocks.EventBus;
global using Bazaar.BuildingBlocks.EventBus.Abstractions;
global using Bazaar.BuildingBlocks.EventBus.Events;
global using Bazaar.BuildingBlocks.EventBusRabbitMQ;
global using Bazaar.Transport.Application.EventHandling;
global using Bazaar.Transport.Application.IntegrationEvents;
global using Bazaar.Transport.Domain.Constants;
global using Bazaar.Transport.Domain.Entities;
global using Bazaar.Transport.Domain.Interfaces;
global using Bazaar.Transport.Domain.Services;
global using Bazaar.Transport.Domain.Specifications;
global using Bazaar.Transport.Infrastructure.Database;
global using Bazaar.Transport.Infrastructure.Repositories;
global using Bazaar.Transport.ServiceIntegration.EventHandling;
global using Bazaar.Transport.ServiceIntegration.IntegrationEvents;
global using Bazaar.Transport.Web.Messages;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using RabbitMQ.Client;
