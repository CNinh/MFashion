using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.ModelRequest;
using BusinessLogicLayer.ModelResponse;
using BusinessLogicLayer.Utilities;
using DataAccessObject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public BlogService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<BaseResponse<PageResult<BlogResponse>>> GetAllBlog(BlogRequest request)
        {
            var response = new BaseResponse<PageResult<BlogResponse>>();
            try
            {
                // Filter
                Expression<Func<Blog, bool>> filter = blog =>
                    (string.IsNullOrEmpty(request.Title) || blog.Title.Contains(request.Title)) &&
                    (!request.PublishedBy.HasValue || blog.AccountId == request.PublishedBy.Value) &&
                    (!request.CategoryId.HasValue || blog.CategoryId == request.CategoryId.Value);

                // Sort
                Expression<Func<Blog, object>> orderByExpression = request.SortBy?.ToLower() switch
                {
                    "title" => b => b.Title,
                    _ => b => b.PublishedDate
                };

                // Include entities
                Func<IQueryable<Blog>, IQueryable<Blog>> customQuery = query =>
                    query.Include(b => b.BlogImages);

                // Get paginated data and filter
                (IEnumerable<Blog> blogs, int totalCount) = await _unitOfWork.BlogRepository.GetPagedAndFilteredAsync(
                    filter,
                    request.PageIndex,
                    request.PageSize,
                    orderByExpression,
                    request.Decending,
                    null,
                    customQuery
                );

                var blog = await _unitOfWork.BlogRepository.Queryable()
                                .Include(b => b.BlogImages)
                                .ToListAsync();

                var blogList = blog.Select(b => new BlogResponse
                {
                    Id = b.Id,
                    Category = b.BlogCategory.CategoryName,
                    Title = b.Title
                }).ToList();

                var pageResult = new PageResult<BlogResponse>
                {
                    Data = blogList,
                    TotalCount = totalCount,
                };

                response.Success = true;
                response.Data = pageResult;
                response.Message = "Blog list retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving blog list!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse<BlogDetailResponse>> GetBlogById(int id)
        {
            var response = new BaseResponse<BlogDetailResponse>();
            try
            {
                var blog = await _unitOfWork.BlogRepository.Queryable()
                                .Where(b =>  b.Id == id)
                                .Include(b => b.BlogCategory)
                                .Include(b => b.BlogImages)
                                .Include(b => b.Comments)
                                .FirstOrDefaultAsync();

                if (blog == null)
                {
                    response.Message = "Blog not found!";
                    return response;
                }

                var images = blog.BlogImages.Select(img => img.ImageUrl).ToList();

                var comments = blog.Comments.Select(c => new CommentResponse
                {
                    Id = c.Id,
                    Avatar = c.Avatar,
                    FullName = c.FullName,
                    Content = c.Content
                }).ToList() ?? [];

                var tags = blog.BlogTags.Select(t => new BlogTagResponse
                {
                    Id = t.Id,
                    TagName = t.TagName
                }).ToList() ?? [];

                var blogResponse = new BlogDetailResponse
                {
                    Category = blog.BlogCategory.CategoryName,
                    Title= blog.Title,
                    Content = blog.Content,
                    ImageUrls = images,
                    PublishedBy = blog.PublishedBy,
                    PublishedDate = blog.PublishedDate,
                    Tags = tags,
                    Comments = comments
                };

                response.Success = true;
                response.Data = blogResponse;
                response.Message = "Blog detail retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error retrieving blog!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> CreateBlog(int accountId, CreateBlogRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var account = await _unitOfWork.AccountRepository.Queryable()
                                    .Where(a => a.Id == accountId)
                                    .FirstOrDefaultAsync();

                if (account == null)
                {
                    response.Message = "Invalid user!";
                    return response;
                }

                var images = new List<BlogImage>();

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

                        images.Add(new BlogImage
                        {
                            ImageUrl = uploadResult.Url,
                            PublicId = uploadResult.PublicId,
                            ResourceType = uploadResult.ResourceType
                        });
                    }
                }

                var tags = new List<BlogTag>();

                if (request.TagId != null && request.TagId.Any())
                {
                    tags = await _unitOfWork.BlogTagRepository.Queryable()
                                .Where(b => request.TagId.Contains(b.Id))
                                .ToListAsync();
                }

                var blog = new Blog
                {
                    Title = request.Title,
                    Content = request.Content,
                    PublishedBy = account.LastName + " " + account.FirstName,
                    PublishedDate = TimeHelper.VietnamTimeZone(),
                    AccountId = accountId,
                    CategoryId = request.CategoryId,
                    BlogImages = images,
                    BlogTags = tags                    
                };

                await _unitOfWork.BlogRepository.InsertAsync(blog);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Message = "Blog created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error creating blog!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> UpdateBlog(int id, UpdateBlogRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var blog = await _unitOfWork.BlogRepository.Queryable()
                                .Where(b => b.Id == id)
                                .FirstOrDefaultAsync();

                if (blog == null)
                {
                    response.Message = "Blog not found!";
                    return response;
                }

                blog.Title = request.Title;
                blog.Content = request.Content;
                blog.CategoryId = request.CategoryId;
                blog.PublishedDate = TimeHelper.VietnamTimeZone();

                // Update Images
                if (request.KeepImageIds != null && request.KeepImageIds.Any())
                {
                    var deleteImage = blog.BlogImages
                                      .Where(img => !request.KeepImageIds.Contains(img.Id))
                                      .ToList();

                    foreach (var img in deleteImage)
                    {
                        await _cloudinaryService.DeleteFileAsync(img.PublicId, img.ResourceType);
                        _unitOfWork.BlogImageRepository.Delete(img.Id);
                    }
                }
                else
                {
                    foreach (var img in blog.BlogImages.ToList())
                    {
                        await _cloudinaryService.DeleteFileAsync(img.PublicId, img.ResourceType);
                        _unitOfWork.BlogImageRepository.Delete(img.Id);
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
                            imageFile.ContentType);

                        blog.BlogImages.Add(new BlogImage
                        {
                            ImageUrl = uploadResult.Url,
                            PublicId = uploadResult.PublicId,
                            ResourceType = uploadResult.ResourceType
                        });
                    }
                }

                if (request.TagId != null && request.TagId.Any())
                {
                    var tags = await _unitOfWork.BlogTagRepository.Queryable()
                                    .Where(t => request.TagId.Contains(t.Id))
                                    .ToListAsync();

                    blog.BlogTags = tags;
                }

                _unitOfWork.BlogRepository.Update(blog);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Message = "Blog updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error updating blog!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> DeleteBlog(int id)
        {
            var response = new BaseResponse();
            try
            {
                var blog = await _unitOfWork.BlogRepository.Queryable()
                                .Where(b => b.Id == id)
                                .FirstOrDefaultAsync();

                if (blog == null)
                {
                    response.Message = "Blog not found!";
                    return response;
                }

                foreach (var img in blog.BlogImages.ToList())
                {
                    await _cloudinaryService.DeleteFileAsync(img.PublicId, img.ResourceType);
                }

                if (blog.BlogImages != null && blog.BlogImages.Any())
                {
                    _unitOfWork.BlogImageRepository.RemoveRange(blog.BlogImages);
                }

                _unitOfWork.BlogRepository.Delete(blog);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Message = "Blog deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error deleting blog!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse> Comment(int id, int accountId, CommmentRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var blog = await _unitOfWork.BlogRepository.Queryable()
                                .Where(b => b.Id == id)
                                .FirstOrDefaultAsync();

                if (blog == null)
                {
                    response.Message = "Blog not found!";
                    return response;
                }

                var comment = new Comment
                {
                    BlogId = id,
                    Content = request.Content,
                    AccountId = accountId,
                    Avatar = blog.Account.Avatar,
                    FullName = blog.Account.LastName + " " + blog.Account.FirstName
                };

                await _unitOfWork.BlogRepository.InsertAsync(blog);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Message = "Comment successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error comment on blog!";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
