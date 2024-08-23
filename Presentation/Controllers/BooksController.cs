using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    //[ApiVersion("1.0")] ServiceExtensions da yazıldı
    [ApiExplorerSettings(GroupName = "v1")]
    [ServiceFilter(typeof (LogFilterAttribute))]
    [ApiController]
    [Route("api/books")]
    //[ResponseCache(CacheProfileName = "5mins")]
    //[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 80)]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;
        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }

        [Authorize]
        [HttpHead]
        [HttpGet(Name = "GetAllBooksAsync")]
        public async Task<IActionResult> GetAllBooksAsync([FromQuery]BookParameters bookParameters)
        {
            var pagedResult = await _manager
                .BookService
                .GetAllBooksAsync(bookParameters, false);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
            return Ok(pagedResult.books);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] int id)
        {
            var book = await _manager
            .BookService
            .GetOneBookByIdAsync(id, false);

            return Ok(book);
        }

        [Authorize]
        [HttpGet("details")]
        public async Task<IActionResult> GetAllBooksWithDetailsAsync()
        {
            return Ok(await _manager
                .BookService
                .GetAllBooksWithDetailsAsync(false));
        }

        [Authorize(Roles = "Editor, Admin")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost(Name = "CreateOneBookAsync")]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {
            var book = await _manager.BookService.CreateOneBookAsync(bookDto);

            return StatusCode(201, book); // CreatedAtRoute()
        }

        [Authorize(Roles = "Editor, Admin")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id,
            [FromBody] BookDtoForUpdate bookDto)
        {
            await _manager.BookService.UpdateOneBookAsync(id, bookDto, false);
            return NoContent(); // 204
        }

        [Authorize(Roles = "Editor, Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
        {
            await _manager.BookService.DeleteOneBookAsync(id, false);
            return NoContent();
        }

        [Authorize(Roles = "Editor, Admin")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] int id,
            [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {

            if (bookPatch is null)
                return BadRequest(); // 400

            var result = await _manager.BookService.GetOneBookForPatchAsync(id, true);

            bookPatch.ApplyTo(result.bookDtoForUpdate, ModelState);

            TryValidateModel(result.bookDtoForUpdate);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            await _manager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);

            return NoContent(); // 204
                                //    Güncelleme verilerini kontrol eder.
                                //    Belirtilen id'ye sahip kitabı alır.
                                //    JSON Patch belgesindeki değişiklikleri kitaba uygular.
                                //    Kitabın güncellenmiş halini doğrular.
                                //    Değişiklikleri veri kaynağında kaydeder.
                                //    Başarılı ise 204 No Content yanıtı döner, başarısız ise uygun hata kodları döner(400 veya 422).
                                //}
        }

        [HttpOptions]
        public IActionResult GetBooksOptions()
        {
            Response.Headers.Add("Allow", "GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS");
            return Ok();
        }
    }
}
