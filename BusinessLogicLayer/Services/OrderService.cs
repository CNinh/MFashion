using Azure;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using BusinessLogicLayer.Utilities;
using DataAccessObject.Model;
using Microsoft.EntityFrameworkCore;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<PageResult<OrderResponse>>> GetOrderList(OrderRequest request)
        {
            var response = new BaseResponse<PageResult<OrderResponse>>();
            try
            {
                // Filter
                Expression<Func<Order, bool>> filter = order =>
                    (string.IsNullOrEmpty(request.OrderCode) || order.OrderCode.Contains(request.OrderCode)) &&
                    (!request.Status.HasValue || order.Status == request.Status.Value) &&
                    (!request.MinPrice.HasValue || order.TotalPrice >= request.MinPrice.Value) &&
                    (!request.MaxPrice.HasValue || order.TotalPrice <= request.MaxPrice.Value);

                // Sort
                Expression<Func<Order, object>> OrderByExpression = request.SortBy?.ToLower() switch
                {
                    "ordercode" => o => o.OrderCode,
                    "orderdate" => o => o.OrderDate,
                    "totalprice" => o => o.TotalPrice,
                    _ => o => o.Id
                };

                // Get Paginated data and filter
                (IEnumerable<Order> orders, int totalCount) = await _unitOfWork.OrderRepository.GetPagedAndFilteredAsync(
                    filter,
                    request.PageIndex,
                    request.PageSize,
                    OrderByExpression,
                    request.Descending,
                    null,
                    null
                );

                var order = await _unitOfWork.OrderRepository.Queryable()
                                  .ToListAsync();

                var orderList = order.Select(o => new OrderResponse
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    TotalItem = o.TotalItem,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status
                }).ToList();

                var pageResult = new PageResult<OrderResponse>
                {
                    Data = orderList,
                    TotalCount = totalCount
                };

                response.Success = true;
                response.Data = pageResult;
                response.Message = "Order list retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving order list!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse<DetailResponse>> GetOrderById(int id)
        {
            var response = new BaseResponse<DetailResponse>();
            try
            {
                var order = await _unitOfWork.OrderRepository.Queryable()
                                 .Where(o => o.Id == id)
                                 .Include(o => o.OrderAddress)
                                 .FirstOrDefaultAsync();

                if (order == null)
                {
                    response.Message = "Order not found!";
                    return response;
                }

                var orderDetail = await _unitOfWork.OrderDetailRepository.Queryable()
                                        .Include(od => od.Product)
                                        .Include(od => od.Color)
                                        .Include(od => od.Material)
                                        .Include(od => od.Size)
                                        .Include(od => od.Designs)
                                        .Where(od => od.OrderId == id)
                                        .ToListAsync();

                var detailResponse = orderDetail.Select(od => new OrderDetailResponse
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductName = od.ProductName,
                    Image = od.Image,
                    Color = od.Color.SecondaryColor != null
                            ? od.Color.PrimaryColor + od.Color.SecondaryColor
                            : od.Color.PrimaryColor,
                    Material = od.Material.MaterialType,
                    Size = od.Size.ProductSize,
                    Design = od.Designs.Select(d => d.FileUrl).ToList(),
                    Note = od.Note,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList();

                var addressResponse = new AddressResponse
                {
                    FirstName = order.OrderAddress.FirstName,
                    LastName = order.OrderAddress.LastName,
                    CompanyName = order.OrderAddress.CompanyName,
                    Address = order.OrderAddress.Address,
                    SubAddress = order.OrderAddress.SubAddress,
                    //Country = order.OrderAddress.Country,
                    //State = order.OrderAddress.State,
                    Province = order.OrderAddress.Province,
                    City = order.OrderAddress.City,
                    PostCode = order.OrderAddress.PostCode,
                    PhoneNumber = order.OrderAddress.PhoneNumber,
                    //Email = order.OrderAddress.Email
                };

                var orderResponse = new DetailResponse
                {
                    OrderCode = order.OrderCode,
                    TotalItem = order.TotalItem,
                    TotalPrice = order.TotalPrice,
                    Status = order.Status,
                    OrderDetails = detailResponse,
                    AccountId = order.AccountId,
                    Address = addressResponse
                };

                response.Success = true;
                response.Data = orderResponse;
                response.Message = "Order detail retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving order list!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> CreateOrder(int id, CreateOrderRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var cart = await _unitOfWork.CartRepository.Queryable()
                                .Where(c => c.AccountId == id)
                                .Include(c => c.CartItems)
                                    .ThenInclude(ci => ci.Color)
                                .Include(c => c.CartItems)
                                    .ThenInclude(ci => ci.Size)
                                .Include(c => c.CartItems)
                                    .ThenInclude(ci => ci.Material)
                                .Include(c => c.CartItems)
                                    .ThenInclude(ci => ci.Designs)
                                .FirstOrDefaultAsync();

                if (cart == null)
                {
                    response.Message = "Cart not found !";
                    return response;
                }

                var orderItems = cart.CartItems.ToList();

                var order = new Order
                {
                    OrderCode = GenerateOrderCode(),
                    TotalItem = cart.TotalItem,
                    TotalPrice = cart.TotalPrice,
                    OrderDate = TimeHelper.VietnamTimeZone(),
                    Status = Order.OrderStatus.Waiting,
                    AccountId = id,
                    DeliveryId = request.DeliveryId,
                    OrderAddress = new OrderAddress
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        CompanyName = request.CompanyName,
                        Address = request.Address,
                        SubAddress = request.SubAddress,
                        //Country = request.Country,
                        //State = request.State,
                        Province = request.Province,
                        City = request.City,
                        PostCode = request.PostCode,
                        PhoneNumber = request.PhoneNumber,
                        //Email = request.Email
                    },
                    OrderDetails = new List<OrderDetail>()
                };

                foreach (var item in cart.CartItems)
                {
                    var detail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Image = item.Image,
                        ColorId = item.ColorId,
                        MaterialId = item.MaterialId,
                        SizeId = item.SizeId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };

                    if (item.Designs != null && item.Designs.Any())
                    {
                        foreach (var design in item.Designs)
                        {
                            _unitOfWork.DesignRepository.Attach(design);
                            design.OrderDetail = detail;
                        }
                    }

                    order.OrderDetails.Add(detail);

                    var product = item.Product;
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity;
                        _unitOfWork.ProductRepository.Update(product);
                    }

                    _unitOfWork.CartItemRepository.RemoveRange(cart.CartItems);
                }

                await _unitOfWork.OrderRepository.InsertAsync(order);

                cart.TotalItem = 0;
                cart.TotalPrice = 0;
                _unitOfWork.CartRepository.Update(cart);

                await _unitOfWork.CommitAsync();

                var cartItem = cart.CartItems.FirstOrDefault();

                if (cartItem == null)
                {
                    response.Message = "Item in cart not found!";
                    return response;
                }

                var orderDetail = order.OrderDetails.FirstOrDefault();

                if (orderDetail == null)
                {
                    response.Message = "Item in order not found!";
                    return response;
                }

                if (cartItem.Designs != null && cartItem.Designs.Any())
                {
                    foreach(var desgin in cartItem.Designs)
                    {
                        desgin.OrderDetailId = orderDetail.Id;
                    }
                    await _unitOfWork.CommitAsync();
                }

                response.Success = true;
                response.Message = "Order created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving order list!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> ChangeAddress(int id, AddressRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var address = await _unitOfWork.OrderAddessRepository.Queryable()
                                 .Where(oa => oa.OrderId == id)
                                 .FirstOrDefaultAsync();

                if (address == null)
                {
                    response.Message = "Order not found!";
                    return response;
                }

                address.FirstName = request.FirstName;
                address.LastName = request.LastName;
                address.CompanyName = request.CompanyName;
                address.Address = request.Address;
                address.SubAddress = request.SubAddress;
                //address.Country = request.Country;
                //address.State = request.State;
                address.Province = request.Province;
                address.City = request.City;
                address.PhoneNumber = request.PhoneNumber;
                //address.Email = request.Email;

                _unitOfWork.OrderAddessRepository.Update(address);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Message = "Address updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving order list!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> CancelOrder(int id)
        {
            var response = new BaseResponse();
            try
            {
                var order = await _unitOfWork.OrderRepository.Queryable()
                                .Where(o => o.Id == id)
                                .FirstOrDefaultAsync();

                if (order == null)
                {
                    response.Message = "Order not found!";
                    return response;
                }

                if (order.Status != Order.OrderStatus.Waiting)
                {
                    response.Message = "Invalid status!";
                    return response;
                }

                order.Status = Order.OrderStatus.Cancelled;

                _unitOfWork.OrderRepository.Update(order);

                foreach (var orderItem in order.OrderDetails)
                {
                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(orderItem.ProductId);
                    if (product != null)
                    {
                        product.Quantity += orderItem.Quantity;
                        _unitOfWork.ProductRepository.Update(product);
                    }
                }

                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Message = "Order created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving order list!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> UpdateStatus(int id)
        {
            var response = new BaseResponse();
            try
            {
                var order = await _unitOfWork.OrderRepository.Queryable()
                                .Where(o => o.Id == id)
                                .FirstOrDefaultAsync();

                if (order == null)
                {
                    response.Message = "Order not found!";
                    return response;
                }

                switch (order.Status)
                {
                    case Order.OrderStatus.Waiting:
                        order.Status = Order.OrderStatus.Preparing;
                        _unitOfWork.OrderRepository.Update(order);
                        break;

                    case Order.OrderStatus.Preparing:
                        order.Status = Order.OrderStatus.Delivery;
                        _unitOfWork.OrderRepository.Update(order);
                        break;

                    case Order.OrderStatus.Delivery:
                        order.Status= Order.OrderStatus.Delivered;
                        _unitOfWork.OrderRepository.Update(order);
                        break;
                }
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Message = "Order status updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error updating status!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        private string GenerateOrderCode()
        {
            var currentDay = DateTime.Now.Date;

            var currentDayString = currentDay.ToString("yyMMdd");

            int orderCountInDay = _unitOfWork.OrderRepository.Queryable()
                .Where(o => o.OrderDate.Date == currentDay)
                .Count();

            int nextOrderNumber = orderCountInDay + 1;

            var orderCode = $"{currentDayString}{nextOrderNumber:D4)}";
            return orderCode;
        }
    }
}
