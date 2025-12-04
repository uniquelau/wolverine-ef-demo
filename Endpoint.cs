using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Persistence;

namespace wolverine_ef_demo
{
    public record CreateItem(string Name);
    public record ItemResponse(int Id, string Name);

    public class CreateItemEndpoint
    {
        [WolverinePost("items/insert")]
        public static (
            ItemResponse,
            IStorageAction<Item>,
            ProblemDetails) Insert(
            CreateItem command)
        {
            var dbEntity = new Item()
            {
                Name = command.Name,
            };

            return (
                new ItemResponse(dbEntity.Id, dbEntity.Name),
                Storage.Insert(dbEntity),
                WolverineContinue.NoProblems
            );
        }

        [WolverinePost("items/insert-and-publish")]
        public static (
            ItemResponse,
            IStorageAction<Item>,
            ItemCreated,
            ProblemDetails) InsertPublish(
            CreateItem command)
        {
            var dbEntity = new Item()
            {
                Name = command.Name,
            };

            return (
                new ItemResponse(dbEntity.Id, dbEntity.Name),
                Storage.Insert(dbEntity),
                new ItemCreated(),
                WolverineContinue.NoProblems
            );
        }
    }

    // Cascading message...
    public record ItemCreated { }
    public class ItemCreatedHandler
    {
        public static void Handle(ItemCreated command, ILogger logger)
        {
            logger.LogInformation("Handling item created command");
        }
    }
}
