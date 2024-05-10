using Kolokwium.Data;
using Kolokwium.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium.Controllers;

[ApiController]
[Route("api/books")]
public class BookController : ControllerBase
{
    private readonly IDataContext _dataContext;

    public BookController(IDataContext dataContext)
    {
        dataContext = dataContext;
    }

    [HttpGet("{id}/authors")]
    public ActionResult<BookDTO> GetBookAuthors(int id)
    {
        var bookDTO = _dataContext.FindAuthorAsync(id);
        if (bookDTO == null)
        {
            return NotFound();
        }
        return Ok(bookDTO);
    }

    [HttpPost]
    public ActionResult<BookDTO> AddBook(BookDTO bookDTO)
    {
        var savedBookDTO = _dataContext.AddBook;
        return CreatedAtAction(nameof(GetBookAuthors), new { id = savedBookDTO.Id }, savedBookDTO);
    }
}

