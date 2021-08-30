using System;
using System.Threading.Tasks;
using Catalog.Controllers;
using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ApiTests
{
    public class ItemsControllerTests
    {
        private readonly Mock<IItemsRepository> _repositoryStub = new();

        private readonly Random _rand = new();

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            _repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);

            var controller = new ItemsController(_repositoryStub.Object);

            var result = await controller.GetItemAsync(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {
            var expectedItem = CreateRandomItem();

            _repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);

            var controller = new ItemsController(_repositoryStub.Object);

            var result = await controller.GetItemAsync(Guid.NewGuid());

            var resultData = (OkObjectResult)result.Result;

            resultData.Value.Should().BeEquivalentTo(expectedItem);
        }

        [Fact]
        public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
        {
            var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };

            _repositoryStub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(expectedItems);

            var controller = new ItemsController(_repositoryStub.Object);

            var actualItems = await controller.GetItemsAsync();

            actualItems.Should().BeEquivalentTo(expectedItems);
        }


        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            var itemToCreate = new CreateItemDto
            {
                Name = Guid.NewGuid().ToString(),
                Price = _rand.Next(1000),
            };

            var controller = new ItemsController(_repositoryStub.Object);

            var result = await controller.CreateItemAsync(itemToCreate);

            var resultData = (CreatedAtActionResult)result.Result;

            var createdItem = resultData.Value as ItemDto;
            itemToCreate.Should().BeEquivalentTo(
                createdItem,
                options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );
            createdItem.Should().NotBeNull();
            createdItem!.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        {
            Item existingItem = CreateRandomItem();
            _repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);

            var itemId = existingItem.Id;

            var itemToUpdate = new UpdateItemDto
            {
                Name = Guid.NewGuid().ToString(),
                Price = existingItem.Price + 3
            };

            var controller = new ItemsController(_repositoryStub.Object);
            
            var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
        {
            // Arrange
            Item existingItem = CreateRandomItem();
            _repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);

            var controller = new ItemsController(_repositoryStub.Object);

            // Act
            var result = await controller.DeleteItemAsync(existingItem.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        private Item CreateRandomItem() =>
            new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = _rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
    }
}
