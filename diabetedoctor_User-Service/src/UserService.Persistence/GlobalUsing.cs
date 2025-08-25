global using Microsoft.EntityFrameworkCore;
global using System.Linq.Expressions;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using MediatR;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Call
global using UserService.Domain.Abstractions;
global using UserService.Domain.Abstractions.Repositories;
global using UserService.Persistence.Repositories;
global using UserService.Persistence.Interceptors;
global using UserService.Contract.Domain;
global using UserService.Domain.Models;
global using UserService.Domain.Enums;