using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    /// <summary>
    /// ProductsController responsible for GET/POST for managing products
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AdventureWorks2019Context _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly QueueClient _queueClient;


        public ProductsController(AdventureWorks2019Context context, BlobServiceClient blobServiceClient, QueueClient queueClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
            _queueClient = queueClient;
        }

        /// <summary>
        /// GET: api/Products
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            return await _context.Product.ToListAsync();
        }

        /// <summary>
        /// GET: api/Products/5
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        /// <summary>
        /// Upload files to Azure blob, and add a notification with all necessary information to Azure queue
        /// </summary>
        [HttpPost("file")]
        public async Task<IActionResult> UploadFiles(IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("awfilecontainer");
            var blobClient = containerClient.GetBlobClient(file.FileName);
            await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });

            await _queueClient.CreateIfNotExistsAsync();
            if (await _queueClient.ExistsAsync())
            {
                await _queueClient.SendMessageAsync($"Save file {file.FileName} to azure blob filename {blobClient.Name} with metadata - ContentType {file.ContentType}.");
            }

            return NoContent();
        }

        /// <summary>
        /// PUT: api/Products/5
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}
