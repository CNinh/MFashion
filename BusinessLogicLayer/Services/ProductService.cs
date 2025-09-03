using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using BusinessLogicLayer.Utilities;
using DataAccessObject.Model;
using LinqKit;
using MailKit.Net.Imap;
using Microsoft.EntityFrameworkCore;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
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
                        HexValue = c.SecondaryHex != null
                                   ? c.PrimaryHex + "/" + c.SecondaryHex
                                   : c.PrimaryHex,
                        ThemeColor = c.SecondaryColor != null
                                     ? c.PrimaryColor + "/" + c.SecondaryColor
                                     : c.PrimaryColor
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

                var image = product.ProductImages.Select(img => img.ImageUrl).ToList();

                var color = await _unitOfWork.ColorRepository.GetAllAsync();

                var colorResponse = color.Select(c => new ColorResponse
                {
                    Id = c.Id,
                    HexValue = c.SecondaryHex != null
                                   ? c.PrimaryHex + "/" + c.SecondaryHex
                                   : c.PrimaryHex,
                    ThemeColor = c.SecondaryColor != null
                                     ? c.PrimaryColor + "/" + c.SecondaryColor
                                     : c.PrimaryColor
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

                var tagResponse = product.Tags.Select(t => new TagResponse
                {
                    Id = t.Id,
                    TagName = t.TagName
                }).ToList();

                var productResponse = new ProductResponse
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Rate = avgRate,
                    TotalRate = totalRate,
                    TotalSold = totalSold,
                    Description = product.Description,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    SKU = product.SKU,
                    Status = product.Status,
                    AccountId = product.AccountId,
                    CategoryId = product.CategoryId,
                    CategoryName = product.ProductCategory.CategoryName,
                    ImageUrls = image,
                    Colors = colorResponse,
                    Materials = materialResponse,
                    Sizes = sizeResponse,
                    Tags = tagResponse
                };

                response.Success = true;
                response.Data = productResponse;
                response.Message = "Product detail retrived successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error fetching product detail!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> GetCreateOption()
        {
            var response = new BaseResponse();
            try
            {
                var statusResponse = Enum.GetValues(typeof(Product.ProductStatus))
                    .Cast<Product.ProductStatus>()
                    .Select(s => new StatusResponse
                    {
                        Id = (int)s,
                        Status = s.ToString()
                    }).ToList();

                var category = await _unitOfWork.CategoryRepository.GetAllAsync();

                var categoryResponse = category.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                }).ToList();

                var color = await _unitOfWork.ColorRepository.GetAllAsync();

                var colorResponse = color.Select(cl => new ColorResponse
                {
                    Id = cl.Id,
                    HexValue = cl.SecondaryHex != null
                               ? cl.PrimaryHex + "/" + cl.SecondaryHex
                               : cl.PrimaryHex,
                    ThemeColor = cl.SecondaryColor != null
                                 ? cl.PrimaryColor + "/" + cl.SecondaryColor
                                 : cl.PrimaryColor
                }).ToList();

                var size = await _unitOfWork.SizeRepository.GetAllAsync();

                var sizeResponse = size.Select(s => new SizeResponse
                {
                    Id = s.Id,
                    ProductSize = s.ProductSize
                }).ToList();

                var tag = await _unitOfWork.TagRepository.GetAllAsync();

                var tagResponse = tag.Select(t => new TagResponse
                {
                    Id = t.Id,
                    TagName = t.TagName
                }).ToList();

                var optionsResponse = new OptionsResponse
                {
                    Statuses = statusResponse,
                    Categories = categoryResponse,
                    Colors = colorResponse,
                    Sizes = sizeResponse,
                    Tags = tagResponse
                };

                response.Success = true;
                response.Data = optionsResponse;
                response.Message = "option list retrieved successfully.";
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving option list!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> CreateProduct(int accountId, CreateProductRequest request)
        {
            var response = new BaseResponse();
            try
            {
                if (request == null)
                {
                    response.Message = "Product information is required!";
                    return response;
                }

                var productImages = new List<ProductImage>();

                if (request.Images != null && request.Images.Any())
                {
                    foreach (var imageFile in request.Images)
                    {
                        using var stream = imageFile.OpenReadStream();
                        var uploadResult = await _cloudinaryService.UploadFileAsync(
                            stream,
                            imageFile.FileName,
                            imageFile.ContentType
                        );

                        productImages.Add(new ProductImage
                        {
                            ImageUrl = uploadResult.Url,
                            PublicId = uploadResult.PublicId,
                            ResourceType = uploadResult.ResourceType
                        });
                    }
                }

                var colors = new List<Color>();

                if (request.ColorId != null && request.ColorId.Any())
                {
                    colors = await _unitOfWork.ColorRepository.Queryable()
                                            .Where(c => request.ColorId.Contains(c.Id))
                                            .ToListAsync();
                }

                var sizes = new List<Size>();
                if (request.SizeId != null && request.SizeId.Any())
                {
                    sizes = await _unitOfWork.SizeRepository.Queryable()
                                            .Where(s => request.SizeId.Contains(s.Id))
                                            .ToListAsync();
                }

                var tags = new List<Tag>();

                if (request.TagId != null && request.TagId.Any())
                {
                    tags = await _unitOfWork.TagRepository.Queryable()
                                            .Where(t => request.TagId.Contains(t.Id))
                                            .ToListAsync();
                }

                Product.ProductStatus status;

                if (request.Quantity > 0)
                {
                    if (request.Status != Product.ProductStatus.OutOfStock &&
                        request.Status != Product.ProductStatus.OnBackOrder)
                    {
                        response.Message = "Status has to be In stock or On sale!";
                        return response;
                    }
                    status = request.Status;
                }
                else
                {
                    if (request.Status != Product.ProductStatus.InStock &&
                        request.Status != Product.ProductStatus.OnSale)
                    {
                        response.Message = "Status has to be Out of stock or On back order!";
                        return response;
                    }
                    status = request.Status;
                }

                var product = new Product
                {
                    ProductName = request.ProductName,
                    Quantity = request.Quantity,
                    Price = request.Price,
                    Description = request.Description,
                    SKU = GenerateSku(request.CategoryId),
                    Status = status,
                    AccountId = accountId,
                    CategoryId = request.CategoryId,
                    ProductImages = productImages,
                    Colors = colors,
                    Sizes = sizes,
                    Tags = tags,
                    CreateAt = TimeHelper.VietnamTimeZone()
                };

                await _unitOfWork.ProductRepository.InsertAsync(product);
                await _unitOfWork.CommitAsync();

                var imageResponse = product.ProductImages.Select(img => img.ImageUrl).ToList();

                var colorResponse = product.Colors.Select(c => new ColorResponse
                {
                    Id = c.Id,
                    HexValue = c.SecondaryHex != null
                               ? c.PrimaryHex + "/" + c.SecondaryHex
                               : c.PrimaryHex,
                    ThemeColor = c.SecondaryColor != null
                                 ? c.PrimaryColor + "/" + c.SecondaryColor
                                 : c.PrimaryColor
                }).ToList();

                var sizeResponse = product.Sizes.Select(s => new SizeResponse
                {
                    Id = s.Id,
                    ProductSize = s.ProductSize
                }).ToList();

                var tagResponse = product.Tags.Select(t => new TagResponse
                {
                    Id = t.Id,
                    TagName = t.TagName,
                }).ToList();

                var productResponse = new CreateProductResponse
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    Description = product.Description,
                    SKU = product.SKU,
                    Status = product.Status,
                    CategoryId = product.CategoryId,
                    AccountId = product.AccountId,
                    CreateAt = TimeHelper.VietnamTimeZone(),
                    ImageUrls = imageResponse,
                    Colors = colorResponse,
                    Sizes = sizeResponse,
                    Tags = tagResponse
                };

                response.Success = true;
                response.Data = product;
                response.Message = "Product created successfully.";
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
                var product = await _unitOfWork.ProductRepository.Queryable()
                              .Where(p => p.Id == id)
                              .Include(p => p.ProductImages)
                              .Include(p => p.Tags)
                              .FirstOrDefaultAsync();

                if (product == null)
                {
                    response.Message = "Product not found!";
                    return response;
                }

                if (request == null)
                {
                    response.Message = "Product detail has to be provided to update!";
                    return response;
                }

                Product.ProductStatus status;

                if (request.Quantity > 0)
                {
                    if (request.Status != Product.ProductStatus.OutOfStock &&
                        request.Status != Product.ProductStatus.OnBackOrder)
                    {
                        response.Message = "Status has to be In stock or On sale!";
                        return response;
                    }
                    status = request.Status;
                }
                else
                {
                    if (request.Status != Product.ProductStatus.InStock &&
                        request.Status != Product.ProductStatus.OnSale)
                    {
                        response.Message = "Status has to be Out of stock or On back order!";
                        return response;
                    }
                    status = request.Status;
                }

                product.ProductName = request.ProductName;
                product.Quantity = request.Quantity;
                product.Price = request.Price;
                product.Description = request.Description;
                product.Status = status;
                product.CategoryId = request.CategoryId;

                // Update images
                if (request.KeepImageIds != null && request.KeepImageIds.Any())
                {
                    var deleteImage = product.ProductImages
                                      .Where(img => !request.KeepImageIds.Contains(img.Id))
                                      .ToList();

                    foreach (var img in deleteImage)
                    {
                        await _cloudinaryService.DeleteFileAsync(img.PublicId, img.ResourceType);
                        _unitOfWork.ProductImageRepository.Delete(img.Id);
                    }
                }
                else
                {
                    foreach (var img in product.ProductImages.ToList())
                    {
                        await _cloudinaryService.DeleteFileAsync(img.PublicId, img.ResourceType);
                        _unitOfWork.ProductImageRepository.Delete(img.Id);
                    }
                }

                if (request.Images != null && request.Images.Any())
                {
                    foreach (var imageFile in request.Images)
                    {
                        using var stream = imageFile.OpenReadStream();
                        var uploadResult = await _cloudinaryService.UploadFileAsync(
                            stream,
                            imageFile.FileName,
                            imageFile.ContentType
                        );

                        product.ProductImages.Add(new ProductImage
                        {
                            ImageUrl = uploadResult.Url,
                            PublicId = uploadResult.PublicId,
                            ResourceType = uploadResult.ResourceType
                        });
                    }
                }


                if (request.ColorId != null && request.ColorId.Any())
                {
                    var colors = await _unitOfWork.ColorRepository.Queryable()
                                    .Where(c => request.ColorId.Contains(c.Id))
                                    .ToListAsync();

                    product.Colors = colors;
                }

                if (request.SizeId != null && request.SizeId.Any())
                {
                    var sizes = await _unitOfWork.SizeRepository.Queryable()
                                    .Where(s => request.SizeId.Contains(s.Id))
                                    .ToListAsync();

                    product.Sizes = sizes;
                }
                
                if (request.TagId != null && request.TagId.Any())
                {
                    var tags = await _unitOfWork.TagRepository.Queryable()
                                     .Where(t => request.TagId.Contains(t.Id))
                                     .ToListAsync();

                    product.Tags = tags;
                }

                _unitOfWork.ProductRepository.Update(product);
                await _unitOfWork.CommitAsync();

                var imageResponse = product.ProductImages.Select(img => img.ImageUrl).ToList();

                var colorResponse = product.Colors.Select(c => new ColorResponse
                {
                    Id = c.Id,
                    HexValue = c.SecondaryHex != null
                               ? c.PrimaryHex + "/" + c.SecondaryHex
                               : c.PrimaryHex,
                    ThemeColor = c.SecondaryColor != null
                                 ? c.PrimaryColor + "/" + c.SecondaryColor
                                 : c.PrimaryColor
                }).ToList();

                var sizeResponse = product.Sizes.Select(s => new SizeResponse
                {
                    Id = s.Id,
                    ProductSize = s.ProductSize
                }).ToList();

                var tagResponse = product.Tags.Select(t => new TagResponse
                {
                    Id = t.Id,
                    TagName = t.TagName
                }).ToList();

                var productResponse = new UpdateProductResponse
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    Description = product.Description,
                    SKU = product.SKU,
                    Status = product.Status,
                    CategoryId = product.CategoryId,
                    ImageUrls = imageResponse,
                    Colors = colorResponse,
                    Sizes = sizeResponse,
                    Tags = tagResponse
                };

                response.Success = true;
                response.Data = productResponse;
                return response;
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
                var product = await _unitOfWork.ProductRepository.Queryable()
                                    .Where(p => p.Id == id)
                                    .Include(p => p.ProductImages)
                                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    response.Message = "Product not found!";
                    return response;
                }

                foreach (var img in product.ProductImages.ToList())
                {
                    await _cloudinaryService.DeleteFileAsync(img.PublicId, img.ResourceType);
                }

                if (product.ProductImages != null && product.ProductImages.Any())
                {
                    _unitOfWork.ProductImageRepository.RemoveRange(product.ProductImages);
                }

                _unitOfWork.ProductRepository.Delete(product.Id);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Message = "Product deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error deleting product!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        #region
        private string GenerateSku(int categoryId)
        {
            var category = _unitOfWork.CategoryRepository.Queryable()
                    .Where(c => c.Id == categoryId)
                    .FirstOrDefault();

            if (category == null)
            {
                throw new Exception("Category not found");
            }

            var prefix = RemoveDiacritics(category.CategoryName)
                            .ToUpper()
                            .Substring(0, Math.Min(3, category.CategoryName.Length));

            var currentDay = TimeHelper.VietnamTimeZone();

            var currentDayString = currentDay.ToString("yyyyMMdd");
            
            var skuInDay = _unitOfWork.ProductRepository.Queryable()
                .Where(p => p.CreateAt.Date == currentDay)
                .Count();

            return $"{prefix}-{currentDayString}-{skuInDay + 1:D3}";
        }

        private string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder();

            foreach(var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        #endregion
    }
}
