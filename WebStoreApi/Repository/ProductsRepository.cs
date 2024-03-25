using Microsoft.EntityFrameworkCore;
using WebStoreApi.Data;
using WebStoreApi.DTO;
using WebStoreApi.Models;

namespace WebStoreApi.Repository
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly WebStoreDbContext _db;
        private readonly IWebHostEnvironment _webHost;

        public ProductsRepository(WebStoreDbContext db,IWebHostEnvironment webHost)
        {
            _db = db;
            _webHost = webHost;
        }
        public async Task<Product> CreateProduct(ProductDTO product)
        {
            var wwwRoot = _webHost.WebRootPath;
            if (product.ImageFileName is not null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFileName.FileName);
                string productPath = Path.Combine(wwwRoot, @"images/products");

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    product.ImageFileName.CopyTo(fileStream);
                }
                product.ImageUrl = @"/images/products/" + fileName;
            }

            var productModel = new Product()
            {
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                Category = product.Category,
                CreatedAt = DateTime.Now,
                Price = product.Price,
                ImageFileName = product.ImageFileName,
                ImageUrl = product.ImageUrl,
            };

            await _db.Products.AddAsync(productModel);
            await _db.SaveChangesAsync();
            return productModel;
        }


        public async Task<Product> DeleteProduct(Guid id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return null;
            }
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllProduct(int? page, string? search)
        {
            IQueryable<Product> products = _db.Products;

            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(x => x.Name.Contains(search) || x.Description.Contains(search));
            }

            if (page == null || page < 1)
            {
                page = 1;
            }

            int pageSize = 5;
            decimal count = await products.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pageSize);

            var productList = await products.OrderBy(x => x.Id)
                                            .Skip((int)((page - 1) ?? 0) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();

            return productList;
        }


        public async Task<Product> GetProductById(Guid id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product is null)
            {
                throw new ArgumentException("Product not found");
            }
            return product;
        }

        public async Task<Product> UpdateProduct(ProductDTO product, Guid id)
        {
            var UpdatedProduct = await _db.Products.FindAsync(id);
            if (UpdatedProduct is null)
            {
                throw new ArgumentException("Product not found");
            }

            var wwwRoot = _webHost.WebRootPath;
            if (product.ImageFileName is not null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFileName.FileName);
                string productPath = Path.Combine(wwwRoot, @"images/products");


                if (!String.IsNullOrEmpty(product.ImageUrl))
                {
                    // Delete old image
                    var old = Path.Combine(wwwRoot, product.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(old))
                    {
                        System.IO.File.Delete(old);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    product.ImageFileName.CopyTo(fileStream);
                }
                product.ImageUrl = @"/images/products/" + fileName;
            }


            UpdatedProduct.Name = product.Name;
                UpdatedProduct.Description = product.Description;
                UpdatedProduct.Brand = product.Brand;
                UpdatedProduct.Category = product.Category;
                UpdatedProduct.CreatedAt = DateTime.Now;
                UpdatedProduct.Price = product.Price;
                UpdatedProduct.ImageFileName = product.ImageFileName;
           
            await _db.SaveChangesAsync();
            return UpdatedProduct;
        }
    }
}
