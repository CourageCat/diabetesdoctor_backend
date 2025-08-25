global using System.Reflection;
global using MediatR;
global using FluentValidation;
global using System.Transactions;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.EntityFrameworkCore;
global using Quartz;

// Call
global using UserService.Contract.Abstractions.Shared;
global using UserService.Domain.Abstractions;
global using UserService.Application.Behaviors;
global using UserService.Contract.Abstractions.Message;
global using UserService.Domain.Abstractions.Repositories;
global using UserService.Domain.Models;
global using UserService.Domain.ValueObjects;
global using UserService.Persistence;
global using UserService.Domain.Enums;
global using UserService.Contract.Domain;
global using UserService.Contract.Services.Patients;
global using UserService.Contract.Enums;
global using UserService.Contract.Helpers;
global using UserService.Contract.DTOs;
global using UserService.Contract.Attributes;
global using UserService.Contract.Common.Messages;
global using UserService.Domain.Events;
