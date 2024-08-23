using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BookManager : IBookService
    {
        private readonly IRepositoryManager _manager;
        private readonly ICategoryService _categoryService;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly IDataShaper<BookDto> _shaper;

        public BookManager(IRepositoryManager manager,
            ILoggerService logger,
            IMapper mapper,
            IDataShaper<BookDto> shaper,
            ICategoryService categoryService)
        {
            _manager = manager;
            _logger = logger;
            _mapper = mapper;
            _shaper = shaper;
            _categoryService = categoryService;
        }

        public async Task<BookDto> CreateOneBookAsync(BookDtoForInsertion bookDto)
        {
            var category = await _categoryService
               .GetOneCategoryByIdAsync(bookDto.CategoryId, false);

            var entity = _mapper.Map<Book>(bookDto);
            _manager.Book.CreateOneBook(entity);
            await _manager.SaveAsync();
            return _mapper.Map<BookDto>(entity);
        }

        public async Task DeleteOneBookAsync(int id, bool trackChanges)
        {
            var entity = await GetOneByIdBookAndCheckExists(id, trackChanges);
            _manager.Book.DeleteOneBook(entity);
            await _manager.SaveAsync();
        }

        public async Task<(IEnumerable<ExpandoObject> books, MetaData metaData)>
            GetAllBooksAsync(BookParameters bookParameters,
            bool trackChanges)
        {
            if (!bookParameters.ValidPriceRange)
                throw new PriceOutofRangeBadRequestException();

            var booksWithMetaData = await _manager
                .Book
                .GetAllBooksAsync(bookParameters, trackChanges);

            var booksDto = _mapper.Map<IEnumerable<BookDto>>(booksWithMetaData);

            var shapedData = _shaper.ShapeData(booksDto, bookParameters.Fields);
            return (books: shapedData, metaData: booksWithMetaData.MetaData);
        }

        public async Task<List<Book>> GetAllBooksAsync(bool trackChanges)
        {
           var books =  await _manager.Book.GetAllBooksAsync(trackChanges);
            return books;
        }

        public async Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(bool trackChanges)
        {
            var books = await _manager.Book.GetAllBooksWithDetailsAsync(trackChanges);
            return books;
        }

        public async Task<BookDto> GetOneBookByIdAsync(int id, bool trackChanges)
        {
            var book = await GetOneByIdBookAndCheckExists(id, trackChanges);

            return _mapper.Map<BookDto>(book);
        }

        public async Task<(BookDtoForUpdate bookDtoForUpdate, Book book)> GetOneBookForPatchAsync(int id, bool trackChanges)
        {
            var book = await GetOneByIdBookAndCheckExists(id, trackChanges);

            var bookDtoForUpdate = _mapper.Map<BookDtoForUpdate>(book);

            return (bookDtoForUpdate, book);

        }

        public async Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book)
        {
            _mapper.Map(bookDtoForUpdate, book);
            await _manager.SaveAsync();
        }

        public async Task UpdateOneBookAsync(int id,
            BookDtoForUpdate bookDto,
            bool trackChanges)
        {
            var entity = await GetOneByIdBookAndCheckExists(id, trackChanges);

            entity = _mapper.Map<Book>(bookDto);

            _manager.Book.Update(entity);
            await _manager.SaveAsync();
        }

        private async Task<Book> GetOneByIdBookAndCheckExists(int id, bool trackChanges )
        {
            // check entity 
            var entity = await _manager.Book.GetOneBookByIdAsync(id, trackChanges);
            if (entity is null)
                throw new BookNotFoundException(id);

            return entity;

        }
    }
}
