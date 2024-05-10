using Kolokwium.Models;
using Kolokwium.DTO;

namespace Kolokwium.Data;

public interface IDataContext
{
    /// <summary>
    /// Pobiernie listy
    /// </summary>
    /// <returns></returns>
    public Task<List<Book>> GetBooksAsync();
    
    /// <summary>
    /// Pobranie po identyfikatorze
    /// </summary>
    /// <param name="idBook"></param>
    /// <returns></returns>
    public Task<Book?> FindProductAsync(int idBook);

    /// <summary>
    /// Pobranie Autora po identyfikatorze
    /// </summary>
    /// <param name="IdAuthor"></param>
    /// <returns></returns>
    public Task<Authors?> FindAuthorAsync(int IdAuthor);
    
    /// <summary>
    /// Zwraca pierwszy element listy obiektów Order, który spełnia warunek lub null jeśli nie ma takiego elementu
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public Task<Book?> FindBookWhereAsync(Func<Book, bool> predicate);

    /// <summary>
    /// Zwraca pirwszy element listy obiektów Author, który spełnia warunek lub null jeśli nie ma takiego elementu
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public Task<Authors?> FindAuthorWarehouseWhere(Func<Authors, bool> predicate);

    /// <summary>
    /// Updates and returns entity, returns null if 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Task<Book> UpdateAsync(Book entity);

    /// <summary>
    /// Inserts new author
    /// </summary>
    /// <param name="newAuthor"></param>
    /// <returns></returns>
    public Task<Authors> InsertAsync(Authors newAuthor);

    /// <summary>
    /// </summary>
    /// <param name="AuthorDTO"></param>
    /// <returns></returns>
    public Task<int> CreateWithProcAsync(AddBookAuthorDTO AuthorDTO);
}