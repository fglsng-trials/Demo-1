using Abstractions.IntermediateCache;
using Abstractions.MessageBroker;
using App.Producer;
using IntermediateCache;
using MessageBroker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<MessageBrokerOptions>(builder.Configuration.GetSection("MessageBroker")); // Configure the message broker options
builder.Services.Configure<IntermediateCacheOptions>(builder.Configuration.GetSection("IntermediateCache")); // Configure the intermediate cache options

builder.Services.AddScoped<IMessageBrokerClient, MessageBrokerClient>(); // Add the intermediate cache client to the services
builder.Services.AddScoped<IIntermediateCacheClient, IntermediateCacheClient>(); // Add the intermediate cache client to the services

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
