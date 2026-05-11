using Microsoft.AspNetCore.Mvc;

namespace BusinersIA;

public static class Endpoints
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapPost("/getResponse", ([FromBody] ResponseRequest request) =>
        {
            return Results.Ok(new { message = $"Received: {request.Input}" });
        })
        .WithName("GetResponse");
    }
}

public record ResponseRequest(string Input);
