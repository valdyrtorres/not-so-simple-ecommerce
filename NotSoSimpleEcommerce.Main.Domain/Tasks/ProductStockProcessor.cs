﻿using Newtonsoft.Json;
using NotSoSimpleEcommerce.MessageHandler.Abstractions;
using NotSoSimpleEcommerce.MessageHandler.Models;
using NotSoSimpleEcommerce.Order.Domain.Events;
using NotSoSimpleEcommerce.Shared.HttpHandlers.Contracts;

namespace NotSoSimpleEcommerce.Main.Domain.Tasks;

public sealed class ProductStockProcessor: IMessageProcessor
{
    private readonly IMainApi _mainApiClient;

    public ProductStockProcessor
    (
        IMainApi mainApiClient
    )
    {
        _mainApiClient = mainApiClient ?? throw new ArgumentNullException(nameof(mainApiClient));
    }

    public async Task ProcessMessageAsync(MessageParams message, CancellationToken cancellationToken)
    {
        var orderCreatedEvent = JsonConvert.DeserializeObject<OrderCreatedEvent>(message.Body);
        if (orderCreatedEvent is null)
            throw new ArgumentNullException(nameof(orderCreatedEvent));
        
        var stockApiResponse = await _mainApiClient.GetStockByProductIdAsync(orderCreatedEvent.ProductId);
        var stockResponse = stockApiResponse.Content;
        if (!stockApiResponse.IsSuccessStatusCode)
            throw new KeyNotFoundException("Sorry, something went wrong while consulting the product stock.");

        if (stockResponse != null)
        {
            var response = await _mainApiClient.UpdateProductStockAsync(orderCreatedEvent.ProductId, new
            {
                orderCreatedEvent.ProductId,
                Quantity = stockResponse.Quantity - orderCreatedEvent.Quantity
            });
        
            if (!response.IsSuccessStatusCode)
                throw new Exception("Sorry, something went wrong while updating the product stock.");
        }
    }
}