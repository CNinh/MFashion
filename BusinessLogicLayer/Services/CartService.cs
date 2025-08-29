using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using DataAccessObject.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public CartService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<BaseResponse> CreateCart(int id)
        {
            var response = new BaseResponse();
            try
            {
                var existingCart = await _unitOfWork.CartRepository.Queryable()
                                        .Where(c => c.AccountId == id)
                                        .FirstOrDefaultAsync();

                if (existingCart != null)
                {
                    response.Message = "This account already has cart!";
                    return response;
                }

                var cart = new Cart
                {
                    TotalItem = 0,
                    TotalPrice = 0,
                    AccountId = id
                };

                await _unitOfWork.CartRepository.InsertAsync(cart);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Data = cart;
                response.Message = "Cart created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error creating cart!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> GetCart(int id)
        {
            var respone = new BaseResponse();
            try
            {
                var cart = await _unitOfWork.CartRepository.Queryable()
                                .Where(c => c.AccountId == id)
                                .Include(c => c.CartItems)
                                .FirstOrDefaultAsync();

                if (cart == null)
                {
                    respone.Message = "Cart not found!";
                    return respone;
                }

                var cartItems = await _unitOfWork.CartItemRepository.Queryable()
                                    .Where(ci => ci.CartId == cart.Id)
                                    .Include(ci => ci.Color)
                                    .Include(ci => ci.Delivery)
                                    .Include(ci => ci.Material)
                                    .Include(ci => ci.Size)
                                    .Include(ci => ci.Designs)
                                    .AsSplitQuery()
                                    .ToListAsync();

                var cartResponse = new CartResponse
                {
                    Id = cart.Id,
                    TotalItem = cart.TotalItem,
                    TotalPrice = cart.TotalPrice,
                    AccountId = id,
                    VoucherId = null,
                    CartItems = cartItems.Select(ci => new CartItemResponse
                    {
                        Id = ci.Id,
                        CartId = ci.CartId,
                        ProductId = ci.ProductId,
                        ProductName = ci.ProductName,
                        Image = ci.Image,
                        Color = ci.Color.ThemeColor,
                        Delivery = ci.Delivery.DeliveryType,
                        Material = ci.Material.MaterialType,
                        Size = ci.Size.ProductSize,
                        Files = ci.Designs.Select(ci => ci.FileUrl).ToList(),
                        Quantity = ci.Quantity,
                        UnitPrice = ci.UnitPrice,
                        Total = ci.Total
                    }).ToList()
                };

                respone.Success = true;
                respone.Data = cartResponse;
                respone.Message = "Cart retrieved successfully.";
            }
            catch (Exception ex)
            {
                respone.Success = false;
                respone.Message = "Error fetching cart!";
                respone.Errors.Add(ex.Message);
            }

            return respone;
        }

        public async Task<BaseResponse> AddToCart(int id, AddToCartRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var cart = await _unitOfWork.CartRepository.Queryable()
                                .Where(c => c.AccountId == id)
                                .Include(c => c.CartItems)
                                .FirstOrDefaultAsync();

                if (cart == null)
                {
                    response.Message = "Cart not found!";
                    return response;
                }

                var product = await _unitOfWork.ProductRepository.Queryable()
                                    .Where(p => p.Id == request.ProductId && (
                                           p.Status == Product.ProductStatus.OnSale ||
                                           p.Status == Product.ProductStatus.InStock))
                                    .Include(p => p.ProductImages)
                                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    response.Message = "Invalid product!";
                    return response;
                }

                if (product.Quantity < request.Quantity)
                {
                    response.Message = "Insufficient existing product!";
                    return response;
                }

                CartItem? cartItem = null;

                if (request.Files == null || !request.Files.Any())
                {
                    cartItem = await _unitOfWork.CartItemRepository.Queryable()
                        .Where(ci => ci.CartId == cart.Id
                            && ci.ProductId == product.Id
                            && ci.ColorId == request.ColorId
                            && ci.MaterialId == request.MaterialId
                            && ci.SizeId == request.SizeId
                            && ci.DeliveryId == request.DeliveryId
                            && !ci.Designs.Any())
                        .FirstOrDefaultAsync();
                }

                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = request.ProductId,
                        ProductName = product.ProductName,
                        Image = product.ProductImages.FirstOrDefault()!.ImageUrl,
                        ColorId = request.ColorId,
                        MaterialId = request.MaterialId,
                        SizeId = request.SizeId,
                        DeliveryId = request.DeliveryId,
                        Quantity = request.Quantity,
                        UnitPrice = product.Price,
                        Total = request.Quantity * product.Price
                    };

                    await _unitOfWork.CartItemRepository.InsertAsync(cartItem);
                    await _unitOfWork.CommitAsync();

                    if (request.Files?.Any() == true)
                    {
                        if (request.Files.Count > 10)
                        {
                            response.Message = "Maximum upload 10 files!";
                            return response;
                        }

                        foreach (var file in request.Files)
                        {
                            using var stream = file.OpenReadStream();
                            var uploadResult = await _cloudinaryService.UploadFileAsync(
                                stream,
                                file.FileName,
                                file.ContentType
                            );

                            var design = new Design
                            {
                                CartItemId = cartItem.Id,
                                FileUrl = uploadResult.Url,
                                PublicId = uploadResult.PublicId,
                                ResourceType = uploadResult.ResourceType
                            };

                            await _unitOfWork.DesignRepository.InsertAsync(design);
                        }
                    }
                }
                else
                {
                    if (product.Quantity < cartItem.Quantity + request.Quantity)
                    {
                        response.Message = "Insufficient existing product!";
                        return response;
                    }

                    cartItem.Quantity += request.Quantity;
                    cartItem.Total = cartItem.Quantity * product.Price;
                    _unitOfWork.CartItemRepository.Update(cartItem);
                }
                
                cart.TotalItem = cart.CartItems.Sum(ci => ci.Quantity);
                cart.TotalPrice = cart.CartItems.Sum(ci => ci.Total);

                _unitOfWork.CartRepository.Update(cart);
                await _unitOfWork.CommitAsync();

                var cartItems = await _unitOfWork.CartItemRepository.Queryable()
                                    .Where(ci => ci.CartId == cart.Id)
                                    .Include(ci => ci.Color)
                                    .Include(ci => ci.Delivery)
                                    .Include(ci => ci.Material)
                                    .Include(ci => ci.Size)
                                    .Include(ci => ci.Designs)
                                    .AsSplitQuery()
                                    .AsNoTracking()
                                    .ToListAsync();

                var cartResponse = new CartResponse
                {
                    Id = cart.Id,
                    TotalItem = cart.TotalItem,
                    TotalPrice = cart.TotalPrice,
                    AccountId = id,
                    VoucherId = null,
                    CartItems = cartItems.Select(ci => new CartItemResponse
                    {
                        Id = ci.Id,
                        CartId = ci.CartId,
                        ProductId = ci.ProductId,
                        ProductName = ci.ProductName,
                        Image = ci.Image,
                        Color = ci.Color.ThemeColor,
                        Delivery = ci.Delivery.DeliveryType,
                        Material = ci.Material.MaterialType,
                        Size = ci.Size.ProductSize,
                        Files = ci.Designs.Select(d => d.FileUrl).ToList(),
                        Quantity = ci.Quantity,
                        UnitPrice = ci.UnitPrice,
                        Total = ci.Quantity * ci.UnitPrice
                    }).ToList()
                };

                response.Success = true;
                response.Data = cartResponse;
                response.Message = "Product added to cart.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error adding product to cart!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> UpdateQuantity(int id, UpdateQuantityRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var cart = await _unitOfWork.CartRepository.Queryable()
                                            .Where(c => c.AccountId == id)
                                            .FirstOrDefaultAsync();
                if (cart == null)
                {
                    response.Message = "Cart not found!";
                    return response;
                }

                var cartItem = await _unitOfWork.CartItemRepository.Queryable()
                                    .Where(ci => ci.Id == request.ItemId)
                                    .FirstOrDefaultAsync();

                if (cartItem == null)
                {
                    response.Message = "Item not found!";
                    return response;
                }

                var product = await _unitOfWork.ProductRepository.GetByIdAsync(cartItem.ProductId);

                if (product == null)
                {
                    response.Message = "Invalid product!";
                    return response;
                }

                if (product.Quantity < request.Quantity)
                {
                    response.Message = "Insfficient product quantity!";
                    return response;
                }

                int oldQuantity = cartItem.Quantity;
                decimal oldTotalPrice = cartItem.Total;

                cartItem.Quantity = request.Quantity;
                cartItem.Total = request.Quantity * product.Price;

                cart.TotalItem += cartItem.Quantity - oldQuantity;
                cart.TotalPrice += cartItem.Total - oldTotalPrice;

                _unitOfWork.CartItemRepository.Update(cartItem);
                _unitOfWork.CartRepository.Update(cart);
                await _unitOfWork.CommitAsync();

                var cartItems = await _unitOfWork.CartItemRepository.Queryable()
                                    .Where(ci => ci.CartId == cart.Id)
                                    .Include(ci => ci.Color)
                                    .Include(ci => ci.Delivery)
                                    .Include(ci => ci.Material)
                                    .Include(ci => ci.Size)
                                    .Include(ci => ci.Designs)
                                    .AsSplitQuery()
                                    .AsNoTracking()
                                    .ToListAsync();

                var cartResponse = new CartResponse
                {
                    Id = cart.Id,
                    TotalItem = cart.TotalItem,
                    TotalPrice = cart.TotalPrice,
                    AccountId = id,
                    VoucherId = null,
                    CartItems = cartItems.Select(ci => new CartItemResponse
                    {
                        Id = ci.Id,
                        CartId = ci.CartId,
                        ProductId = ci.ProductId,
                        ProductName = ci.ProductName,
                        Image = ci.Image,
                        Color = ci.Color.ThemeColor,
                        Delivery = ci.Delivery.DeliveryType,
                        Material = ci.Material.MaterialType,
                        Size = ci.Size.ProductSize,
                        Files = ci.Designs.Select(d => d.FileUrl).ToList(),
                        Quantity = ci.Quantity,
                        UnitPrice = ci.UnitPrice,
                        Total = ci.Quantity * ci.UnitPrice
                    }).ToList()
                };

                response.Success = true;
                response.Data = cartResponse;
                response.Message = "Product quantity updated.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error updating product quantity!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> RemoveFromCart(int id, RemoveProductRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var cart = await _unitOfWork.CartRepository.Queryable()
                                .Where(c => c.AccountId == id)
                                .FirstOrDefaultAsync();

                if (cart == null)
                {
                    response.Message = "Cart not found!";
                    return response;
                }

                var cartItem = await _unitOfWork.CartItemRepository.Queryable()
                                    .Where(ci => ci.Id == request.ItemId)
                                    .Include(ci => ci.Designs)
                                    .FirstOrDefaultAsync();

                if (cartItem == null)
                {
                    response.Message = "Item not found!";
                    return response;
                }

                foreach(var file in cartItem.Designs.ToList())
                {
                    await _cloudinaryService.DeleteFileAsync(file.PublicId, file.ResourceType);
                }

                if (cartItem.Designs != null && cartItem.Designs.Any())
                {
                    _unitOfWork.DesignRepository.RemoveRange(cartItem.Designs);
                    await _unitOfWork.CommitAsync();
                }

                cart.TotalItem -= cartItem.Quantity;
                cart.TotalPrice -= cartItem.Total;

                _unitOfWork.CartRepository.Update(cart);
                _unitOfWork.CartItemRepository.Delete(request.ItemId);
                await _unitOfWork.CommitAsync();

                var cartItems = await _unitOfWork.CartItemRepository.Queryable()
                                    .Where(ci => ci.CartId == cart.Id)
                                    .Include(ci => ci.Color)
                                    .Include(ci => ci.Delivery)
                                    .Include(ci => ci.Material)
                                    .Include(ci => ci.Size)
                                    .Include(ci => ci.Designs)
                                    .AsSplitQuery()
                                    .AsNoTracking()
                                    .ToListAsync();

                var cartResponse = new CartResponse
                {
                    Id = cart.Id,
                    TotalItem = cart.TotalItem,
                    TotalPrice = cart.TotalPrice,
                    AccountId = id,
                    VoucherId = null,
                    CartItems = cartItems.Select(ci => new CartItemResponse
                    {
                        Id = ci.Id,
                        CartId = ci.CartId,
                        ProductId = ci.ProductId,
                        ProductName = ci.ProductName,
                        Image = ci.Image,
                        Color = ci.Color.ThemeColor,
                        Delivery = ci.Delivery.DeliveryType,
                        Material = ci.Material.MaterialType,
                        Size = ci.Size.ProductSize,
                        Files = ci.Designs.Select(d => d.FileUrl).ToList(),
                        Quantity = ci.Quantity,
                        UnitPrice = ci.UnitPrice,
                        Total = ci.Quantity * ci.UnitPrice
                    }).ToList()
                };

                response.Success = true;
                response.Data = cartResponse;
                response.Message = "Product removed from cart.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error removing product!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> ClearItem(int id)
        {
            var response = new BaseResponse();
            try
            {
                var cart = await _unitOfWork.CartRepository.Queryable()
                                .Where(c => c.AccountId == id)
                                .Include(c => c.CartItems)
                                    .ThenInclude(ci => ci.Designs)
                                .FirstOrDefaultAsync();

                if (cart == null)
                {
                    response.Message = "Cart not found!";
                    return response;
                }

                if (cart.CartItems != null && cart.CartItems.Any())
                {
                    foreach (var file in cart.CartItems.SelectMany(ci => ci.Designs).ToList())
                    {
                        await _cloudinaryService.DeleteFileAsync(file.PublicId, file.ResourceType);
                    }

                    var designs = cart.CartItems.SelectMany(ci => ci.Designs).ToList();
                    if (designs.Any())
                    {
                        _unitOfWork.DesignRepository.RemoveRange(designs);
                        await _unitOfWork.CommitAsync();
                    }

                    _unitOfWork.CartItemRepository.RemoveRange(cart.CartItems);
                }

                cart.TotalItem = 0;
                cart.TotalPrice = 0;

                _unitOfWork.CartRepository.Update(cart);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Data = new List<CartItemResponse>();
                response.Message = "Items in cart cleared.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error clearing items!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
