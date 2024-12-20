﻿using BMT_backend.Domain.Entities;
using BMT_backend.Application.Interfaces;
using BMT_backend.Infrastructure.Data;
using BMT_backend.Presentation.Requests;
using BMT_backend.Presentation.DTOs;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BMT_backend.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly TagService _tagService;
        private readonly IImageFileService _imageFileService;

        public ProductService(IProductRepository productRepository, TagService tagService, IImageFileService imageFileService)
        {
            _productRepository = productRepository;
            _tagService = tagService;
            _imageFileService = imageFileService;
        }

        public async Task<bool> CreateProduct(Product product)
        {
            ValidateProductData(product);
            var created = await _productRepository.CreateBaseProductAsync(product);
            if (created == null) return false;
            if (product.Type == "NonPerishable")
            {
                created = await _productRepository.CreateNonPerishableProductAsync(product.Id, product.Stock ?? 0);
                if (created == null) return false;
            }
            else if (product.Type == "Perishable")
            {
                created = await _productRepository.CreatePerishableProductAsync(product.Id, product.Limit ?? 0, product.WeekDaysAvailable);
                if (created == null) return false;
            }
            if (product.Tags != null && product.Tags.Count > 0)
            {
                List<string> tagsId = await _tagService.GetTagsIdByTagsName(product.Tags);
                await _tagService.UpdateProductTags(product.Id, tagsId);
            }
            if (product.ImagesFiles != null && product.ImagesFiles.Count > 0)
            {
                await _imageFileService.CreateProductImages(product.Id, product.ImagesFiles);
            }
            return true;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _productRepository.GetProductsAsync();
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task<List<Product>> GetProducsDetailsAsync()
        {
            return await _productRepository.GetProductsDetailsAsync();
        }

        public async Task<List<ProductDevDto>> GetProducsDetailsDevAsync()
        {
            List<Product> products = await GetProducsDetailsAsync();
            List<ProductDevDto> productsDto = new List<ProductDevDto>();
            foreach (Product product in products) {
                ProductDevDto devDto = new ProductDevDto()
                {
                    Name = product.Name,
                    EnterpriseName = product.EnterpriseName,
                    Description = product.Description,
                    Weight = product.Weight,
                    Price = product.Price,
                    Type = product.Type,
                    Tags = product.Tags,
                    ImagesURLs = product.ImagesURLs,
                    Stock = product.Stock,
                    Limit = product.Limit,
                    WeekDaysAvailable = product.WeekDaysAvailable
                };
                productsDto.Add(devDto);
            }
            return productsDto;
        }

        public async Task<Product> GetProductDetailsByIdAsync(string id)
        {
            return await _productRepository.GetProductDetailsByIdAsync(id);
        }

        public async Task<int> GetStock(GetProductStockRequest product)
        {
            if (product.Type == "NonPerishable")
            {
                return await _productRepository.GetNonPerishableStock(product.ProductId);
            }
            else if (product.Type == "Perishable")
            {
                return await _productRepository.GetPerishableStock(product.ProductId, product.Date);
            }
            else
            {
                throw new ArgumentException("Invalid product type or date.");
            }
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            ValidateUpdateProductData(product);
            var updated = await _productRepository.UpdateProductAsync(product);
            if (updated == null) return false;

            List<string> tagsId = await _tagService.GetTagsIdByTagsName(product.Tags);
            await _tagService.UpdateProductTags(product.Id, tagsId);

            if (product.ImagesFiles != null && product.ImagesFiles.Count > 0)
                await _imageFileService.CreateProductImages(product.Id, product.ImagesFiles);

            if (product.Type == "NonPerishable")
            {
                updated = await _productRepository.UpdateNonPerishableDetailsAsync(product.Id, product.Stock ?? 0);
                if (updated == null) return false;
            }
            else if (product.Type == "Perishable")
            {
                updated = await _productRepository.UpdatePerishableDetailsAsync(product.Id, product.WeekDaysAvailable, product.Limit);
                if (updated == null) return false;
            }
            return true;
        }

        public async Task<bool> UpdateStockAsync(string productId, int quantity, string type, string dateString = "")
        {
            if (type == "NonPerishable")
                return await _productRepository.UpdateNonPerishableStockAsync(productId, quantity);
            else if (type == "Perishable" && DateTime.TryParse(dateString, out var date))
                return await _productRepository.UpdatePerishableStockAsync(productId, date.ToString("yyyy-MM-dd"), quantity);
            else
                throw new ArgumentException("Invalid product type or date.");
        }

        private static void ValidateProductData(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
                throw new ArgumentException("El nombre del producto no puede estar vacío.");
            if (string.IsNullOrEmpty(product.Description))
                throw new ArgumentException("La descripción del producto no puede estar vacía.");
            if (product.Price <= 0)
                throw new ArgumentException("El precio del producto no puede ser menor o igual a 0.");
            if (product.Type == "NonPerishable" && (product.Stock <= 0))
                throw new ArgumentException("El stock del producto no puede ser menor o igual a 0.");
            if (string.IsNullOrEmpty(product.Type))
                throw new ArgumentException("El tipo del producto no puede estar vacío.");
            if (product.Type == "Perishable" && product.Limit <= 0)
                throw new ArgumentException("El límite de stock no puede ser menor o igual a 0.");
        }

        private static void ValidateUpdateProductData(Product product)
        {
            if (string.IsNullOrEmpty(product.Id))
                throw new ArgumentException("Se debe proporcionar un Id para modificar un porducto.");
            if (product.Price <= 0)
                throw new ArgumentException("El precio del producto no puede ser menor o igual a 0.");
            if (product.Type == "NonPerishable" && (product.Stock <= 0))
                throw new ArgumentException("El stock del producto no puede ser menor o igual a 0.");
            if (string.IsNullOrEmpty(product.Type))
                throw new ArgumentException("El tipo del producto no puede estar vacío.");
            if (product.Type == "Perishable" && product.Limit <= 0)
                throw new ArgumentException("El límite de stock no puede ser menor o igual a 0.");
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentException("El identificador del producto no puede estar vacío.");

            return await _productRepository.DeleteProductAsync(productId);
        }
    }
}
