using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Dapper;
using MusicaApi;
using System.Linq;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;

[Route("api/[controller]")]
[ApiController]
public class MusicController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public MusicController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Retorna uma lista de músicas.", Description = "Esta operação retorna uma lista de todas as músicas disponíveis.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Retorna uma lista de músicas.")]
 
    public IActionResult Get()
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var query = "SELECT * FROM Musica";
            var musicas = connection.Query<Musica>(query);

            return Ok(musicas);
        }
    }

    [HttpGet("{nomeMusica}")]
    [SwaggerOperation(Summary = "Retorna uma música pelo nome.", Description = "Esta operação retorna uma música com base no nome fornecido.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Retorna a música encontrada.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Música não encontrada.")]
    public IActionResult Get(string nomeMusica)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var query = "SELECT * FROM Musica WHERE NomeMusica = @NomeMusica";
            var musicas = connection.Query<Musica>(query, new { NomeMusica = nomeMusica });

            if (musicas == null || musicas.Count() == 0)
            {
                return NotFound("Música não encontrada");
            }

            return Ok(musicas);
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Insere uma nova música.", Description = "Esta operação permite inserir uma nova música na lista.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Música criada com sucesso.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos.")]
    public IActionResult Post([FromBody] Musica musica)
    {
        if (musica == null)
        {
            return BadRequest("Dados inválidos.");
        }

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var query = "INSERT INTO Musica (Autor, Genero, AlbumDiretorio, MusicaDiretorio, NomeMusica) VALUES (@Autor, @Genero, @AlbumDiretorio, @MusicaDiretorio, @NomeMusica)";
            connection.Execute(query, musica);
        }

        return CreatedAtAction("Get", new { id = musica.Id }, musica);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Exclui uma música pelo ID.", Description = "Esta operação permite excluir uma música com base no ID fornecido.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Música excluída com sucesso.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Música não encontrada.")]
    public IActionResult Delete(int id)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var query = "DELETE FROM Musica WHERE Id = @Id";
            connection.Execute(query, new { Id = id });

            return NoContent();
        }
    }
}
