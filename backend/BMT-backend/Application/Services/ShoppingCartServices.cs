﻿
using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using System.Text.RegularExpressions;
using BMT_backend.Presentation.DTOs;
using BMT_backend.Presentation.Requests;
using System.Data.SqlClient;


namespace BMT_backend.Application.Services
{
    public class ShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

        public async Task<bool> CreateShoppingCartAsync(string userName)
        {
            return await _shoppingCartRepository.CreateShoppingCartAsync(userName);
        }

        public async Task<ShoppingCart> GetShoppingCartAsync(string userId)
        {
            return await _shoppingCartRepository.GetShoppingCartAsync(userId);
        }
    
        public async Task<string> GetCartIdAsync(string userId)
        {
            return await _shoppingCartRepository.GetCartIdAsync(userId);
        }

        public async Task<string> AddProductToCartAsync(string shoppingCartId, string productId)
        {
            return await _shoppingCartRepository.AddProductToCartAsync(shoppingCartId, productId);
        }

        public async Task<bool> ChangeProductQuantityAsync(string shoppingCartId, string productId, int quantity)
        {
            return await _shoppingCartRepository.ChangeProductQuantityAsync(shoppingCartId, productId, quantity);
        }

        public async Task<bool> DeleteProductFromCartAsync(string shoppingCartId, string productId)
        {
            return await _shoppingCartRepository.DeleteProductFromCartAsync(shoppingCartId, productId);
        }

        public async Task<bool> ClearShoppingCartAsync(string shoppingCartId)
        {
            return await _shoppingCartRepository.ClearShoppingCartAsync(shoppingCartId);
        }
    }
}