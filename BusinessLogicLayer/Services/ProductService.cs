using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using DataAccessObject.Model;
using LinqKit;
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
    public class ProductService : IProductService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly CloudinaryService _cloudinaryService;

        public ProductService(UnitOfWork unitOfWork, CloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<BaseResponse<PageResult<ProductListResponse>>> GetProductList(ProductListRequest request)
        {
            var response = new BaseResponse<PageResult<ProductListResponse>>();
            try
            {
                // Filter
                Expression<Func<Product, bool>> filter = product =>
                    (string.IsNullOrEmpty(request.ProductName) || product.ProductName.Contains(request.ProductName)) &&
                    (!request.MinPrice.HasValue || product.Price >= request.MinPrice.Value) &&
                    (!request.MaxPrice.HasValue || product.Price <= request.MaxPrice.Value);

                var account = await _unitOfWork.AccountRepository.GetByIdAsync(request.UserId);

                // Check user
                if ((account != null && account.RoleId == 1) || (account != null && account.RoleId == 2))
                {
                    // Display all product
                }
                else
                {
                    filter = filter.And(p => p.Quantity > 0); // Display in stock product
                }

                // Sort
                Expression<Func<Product, object>> orderByExpression = request.SortBy?.ToLower() switch
                {
                    "productname" => p => p.ProductName,
                    "price" => p => p.Price,
                    "createat" => p => p.CreateAt,
                    _ => p => p.Id
                };

                // Include entities
                Func<IQueryable<Product>, IQueryable<Product>> customQuery = query =>
                    query.Include(p => p.ProductImages)
                         .Include(p => p.Colors);

                // Get paginated data and filter
                (IEnumerable<Product> products, int totalCount) = await _unitOfWork.ProductRepository.GetPagedAndFilteredAsync(
                    filter,
                    request.PageIndex,
                    request.PageSize,
                    orderByExpression,
                    request.Descending,
                    null,
                    customQuery
                );

                var productList = products.Select(p => new ProductListResponse
                {
                    Id = p.Id,
                    ImageUrl = p.ProductImages.First().ImageUrl,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Colors = p.Colors.Select(c => new ColorResponse
                    {
                        Id = c.Id,
                        ThemeColor = c.ThemeColor
                    }).ToList()
                }).ToList();

                var pageResult = new PageResult<ProductListResponse>
                {
                    Data = productList,
                    TotalCount = totalCount
                };

                response.Success = true;
                response.Data = pageResult;
                response.Message = "Product list fetched successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error fetching product list!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> GetProductById(int id)
        {
            var response = new BaseResponse();
            try
            {
                var product = await _unitOfWork.ProductRepository.Queryable().
                                    Where(p => p.Id == id)
                                    .Include(p => p.ProductImages)
                                    .Include(p => p.ProductCategory)
                                    .Include(p => p.Colors)
                                    .Include(p => p.Sizes)
                                    .Include(p => p.Materials)
                                    .Include(p => p.Tags)
                                    .Include(p => p.Deliveries)
                                    .AsSplitQuery()
                                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    response.Message = "Product not found!";
                    return response;
                }

                var orderDetail = await _unitOfWork.OrderDetailRepository.Queryable()
                                    .Where(od => od.ProductId == product.Id &&
                                           od.Order.Status == Order.OrderStatus.Delivered)
                                    .Include(od => od.Order)
                                        .ThenInclude(o => o.Reviews)
                                        .ThenInclude(r => r.ReviewImages)
                                    .ToListAsync();

                var reviews = orderDetail.SelectMany(po => po.Order.Reviews).ToList();

                // calculate average rate
                var avgRate = reviews.Any() ? Math.Round(reviews.Average(r => r.Rate), 1) : 0;

                // calculate total rate
                var totalRate = reviews.Sum(r => r.Rate);

                // calculate total sold
                var totalSold = orderDetail.Sum(od => od.Quantity);

                var reviewResponse = reviews.Select(r => new ReviewResponse
                {
                    Rate = r.Rate,
                    Name = r.Account.LastName + "" + r.Account.FirstName,
                    Avatar = r.Account.Avatar,
                    Comment = r.Comment,
                    CreateAt = r.CreateAt,
                    Images = r.ReviewImages!.FirstOrDefault()?.ImageUrl != null
                             ? new List<string> { r.ReviewImages.FirstOrDefault()?.ImageUrl! }
                             : new List<string>()
                }).ToList();

                var color = await _unitOfWork.ColorRepository.GetAllAsync();

                var colorResponse = color.Select(c => new ColorResponse
                {
                    Id = c.Id,
                    ThemeColor = c.ThemeColor
                }).ToList();

                var delivery = await _unitOfWork.DeliveryRepository.GetAllAsync();

                var deliveryResponse = delivery.Select(d => new DeliveryResponse
                {
                    Id = d.Id,
                    Period = d.Period
                }).ToList();

                var size = await _unitOfWork.SizeRepository.GetAllAsync();

                var sizeResponse = size.Select(s => new SizeResponse
                {
                    Id = s.Id,
                    ProductSize = s.ProductSize
                }).ToList();

                var material = await _unitOfWork.MaterialRepository.GetAllAsync();

                var materialResponse = material.Select(m => new MaterialResponse
                {
                    Id = m.Id,
                    MaterialType = m.MaterialType
                }).ToList();

                var tag = await _unitOfWork.TagRepository.GetAllAsync();

                var tagResponse = tag.Select(t => new TagResponse
                {
                    Id = t.Id,
                    TagName = t.TagName
                }).ToList();
            }
            catch(Exception ex )
            {
                response.Success = false;
                response.Message = "Error fetching product detail!";
                response.Errors.Add(ex.Message);
            }

            return response; 
        }

        public async Task<BaseResponse> CreateProduct(CreateProductRequest request)
        {
            var response = new BaseResponse();
            try
            {

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error creating product!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> UpdateProduct(int id, UpdateProductRequest request)
        {
            var response = new BaseResponse();
            try
            {

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error updating product!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> DeleteProduct(int id)
        {
            var response = new BaseResponse();
            try
            {

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error deleting product!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
