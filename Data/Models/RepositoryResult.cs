using Microsoft.AspNetCore.Http.HttpResults;

namespace Data.Models;

public class RepositoryResult<T>
{
    public bool Succeeded { get; set; }

    public int StatusCode { get; set; }

    public string? ErrorMessage { get; set; }

    public T? Result { get; set; }

    public static RepositoryResult<T> Ok(T? result)
    {
        return new RepositoryResult<T>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = result
        };
    }
    public static RepositoryResult<T> Created(T? result)
    {
        return new RepositoryResult<T>
        {
            Succeeded = true,
            StatusCode = 201,
            Result = result
        };
    }
    public static RepositoryResult<T> NoContent()
    {
        return new RepositoryResult<T>
        {
            Succeeded = true,
            StatusCode = 204
        };
    }
    public static RepositoryResult<T> BadRequest(string errorMessage)
    {
        return new RepositoryResult<T>
        {
            Succeeded = false,
            StatusCode = 400,
            ErrorMessage = errorMessage
        };
    }
    public static RepositoryResult<T> NotFound(string errorMessage)
    {
        return new RepositoryResult<T>
        {
            Succeeded = false,
            StatusCode = 404,
            ErrorMessage = errorMessage
        };
    }
    public static RepositoryResult<T> Errror(string errorMessage)
    {
        return new RepositoryResult<T>
        {
            Succeeded = false,
            StatusCode = 500,
            ErrorMessage = errorMessage
        };
    }
}
