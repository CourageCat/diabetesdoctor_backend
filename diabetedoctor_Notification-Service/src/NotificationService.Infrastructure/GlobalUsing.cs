// Package
global using System.Text.Json;
global using Confluent.Kafka;
global using System.Reflection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.DependencyInjection;
global using MediatR;
global using NotificationService.Contract.Common.Constants;
global using NotificationService.Contract.EventBus.Abstractions.Message;
global using Microsoft.AspNetCore.Http;
global using FirebaseAdmin.Messaging;

