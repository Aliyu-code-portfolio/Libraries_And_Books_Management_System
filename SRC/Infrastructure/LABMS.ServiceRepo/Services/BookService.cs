﻿using AutoMapper;
using LABMS.Application.Common;
using LABMS.Application.DTOs.ForCreation;
using LABMS.Application.DTOs.ForDto;
using LABMS.Application.DTOs.ForUpdate;
using LABMS.Application.Exceptions;
using LABMS.Domain.entities;
using LABMS.ServiceContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LABMS.ServiceRepository.Services
{
    public class BookService : IBookService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        public BookService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }
        public async Task<BookDto> CreateBook(BookForCreation bookCreate)
        {
            var book = _mapper.Map<Book>(bookCreate);
            _repositoryManager.BookRepository.CreateBook(book);
            await _repositoryManager.SaveAsync();
            var bookDto = _mapper.Map<BookDto>(book);
            return bookDto;
        }

        public async Task DeleteBook(int id)
        {
            var book = await _repositoryManager.BookRepository.GetBookByIdAsync(id, false)
                ?? throw new BookNotFoundException(id);
            _repositoryManager.BookRepository.DeleteBook(book);
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<BookDto>> GetAllBookByAuthorAsync(int authorId, bool trackChanges)
        {
            var bookByAuthor = await _repositoryManager.BooksByAuthorRepository
                .GetBook_By_AuthorbyAuthorId(authorId,trackChanges)
                ?? throw new BookNotFoundException(authorId);
            IEnumerable<Book> books = new List<Book>();
            foreach(var item in bookByAuthor)
            {
                var book = await _repositoryManager.BookRepository.GetBookByIdAsync(item.Isbn, trackChanges)
                    ??throw new BookNotFoundException(item.Isbn);
                books.Append(book);
            }
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return booksDto;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync(bool trackChanges)
        {
            var books = await _repositoryManager.BookRepository.GetAllBooksAsync(trackChanges);
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return booksDto;
        }


        public async Task<IEnumerable<BookDto>> GetAllBooksRequestedByLibraryId(int libraryId, bool trackChanges)
        {
            var booksRequested = await _repositoryManager.MemberRequestRepository.
                GetAllMemberRequestByLibraryId(libraryId, trackChanges)
                ?? throw new BookNotFoundException(libraryId);
            IEnumerable<Book> books = new List<Book>();
            foreach(var item in booksRequested)
            {
                var book = await _repositoryManager.BookRepository.GetBookByIdAsync(item.Isbn, trackChanges)
                    ?? throw new BookNotFoundException(item.Isbn);
                books.Append(book);
            }
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return booksDto;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksRequestedByMemberId(int memberId, bool trackChanges)
        {
            var booksRequested = await _repositoryManager.MemberRequestRepository.
                GetAllMemberRequestedByMemberId(memberId, trackChanges)
                ?? throw new BookNotFoundException(memberId);
            IEnumerable<Book> books = new List<Book>();
            foreach (var item in booksRequested)
            {
                var book = await _repositoryManager.BookRepository.GetBookByIdAsync(item.Isbn, trackChanges)
                    ?? throw new BookNotFoundException(item.Isbn);
                books.Append(book);
            }
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return booksDto;
        }


        public async Task<BookDto> GetBooksById(int isbn, bool trackChanges)
        {
            var book = await _repositoryManager.BookRepository.GetBookByIdAsync(isbn, trackChanges);
            var bookDto = _mapper.Map<BookDto>(book);
            return bookDto;
        }

        //Not implemented yet
        public Task<IEnumerable<BookDto>> GetAllBooksRequested(bool trackChanges)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<BookDto>> GetBooksByCategories(int categoryId, int bookId, bool trackChanges)
        {
            throw new NotImplementedException();
        }

        /*public async Task UpdateBook(BookForUpdate bookUpdate)
        {
            var bookToUpdate = await _repositoryManager.BookRepository
                .GetBookByIdAsync(bookUpdate.Isbn, false)
                ?? throw new BookNotFoundException(bookUpdate.Isbn);
            var book = _mapper.Map<Book>(bookUpdate);
            //_repositoryManager.BookRepository.Update
        }*/
    }
}
