// Package
global using System.Text.Json;
global using Confluent.Kafka;
global using System.Reflection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.DependencyInjection;
global using MediatR;
// Assembly
global using MediaService.Contract.EventBus.Abstractions;
global using MediaService.Contract.EventBus.Abstractions.Message;
global using MediaService.Contract.Infrastructure.Services;
global using MediaService.Contract.Settings;
global using MediaService.Infrastructure.EventBus;
global using MediaService.Infrastructure.EventBus.Kafka;
global using MediaService.Infrastructure.Services;

