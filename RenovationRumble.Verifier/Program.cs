namespace RenovationRumble.Verifier
{
    using Logic.Serialization;
    using Microsoft.AspNetCore.Mvc;
    using RenovationRumble.Logic.Data;
    using Data;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var serializer = new JsonSerializer(isHumanReadable: false);

            CatalogDataModel catalog;
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Catalog.json");
                if (!File.Exists(path))
                    throw new FileNotFoundException("Catalog.json not found next to the app.", path);

                var json = File.ReadAllText(path);
                catalog = serializer.Deserialize<CatalogDataModel>(json);
            }

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapPost("/verify", ([FromBody] object body) =>
                {
                    var raw = body.ToString();
                    if (string.IsNullOrWhiteSpace(raw))
                    {
                        return Results.BadRequest(new VerifyResponseDataModel
                        {
                            Status = VerifyStatus.InvalidInput,
                            Message = "Empty body."
                        });
                    }

                    VerifyRequestDataModel request;
                    try
                    {
                        request = serializer.Deserialize<VerifyRequestDataModel>(raw);
                    }
                    catch (Exception e)
                    {
                        return Results.BadRequest(new VerifyResponseDataModel
                        {
                            Status = VerifyStatus.InvalidInput,
                            Message = $"Invalid JSON: {e.Message}"
                        });
                    }

                    var result = Verifier.Verify(catalog, request);
                    return Results.Ok(result);
                })
                .Produces<VerifyResponseDataModel>()
                .Produces<VerifyResponseDataModel>(StatusCodes.Status400BadRequest)
                .WithName("VerifyRun")
                .WithOpenApi()
                .WithSummary("Verify match replay")
                .WithDescription("""
                                 Processes a match replay with commands and a claimed score.
                                 Returns a result indicating whether the score is valid, if the game ended,
                                 or if there were simulation errors.

                                 Uses the custom JsonSerializer to support command union types.
                                 """);

            app.Run();
        }
    }
}