using System;
using Xunit;
using lab4;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab4Tests
{
    public class MusicCatalogTests
    {
        [Fact]
        public void AddComposition_ShouldAddToCatalog()
        {
            // Arrange
            using (var context = new MusicCatalogContext())
            {
                var controller = new MusicCatalogController(context);
                var composition = new MusicComposition { Artist = "Artist1", Title = "Title1" };

                // Act
                var result = controller.AddComposition(composition);

                // Assert
                Assert.IsType<OkResult>(result);
            }
        }
        
        [Fact]
        public void GetAllCompositions_ShouldReturnAllCompositions()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MusicCatalogContext>()
                .UseInMemoryDatabase(databaseName: "GetAllCompositions_Test_Database")
                .Options;

            using (var context = new MusicCatalogContext(options))
            {
                var controller = new MusicCatalogController(context);
                var composition1 = new MusicComposition { Artist = "Artist1", Title = "Title1" };
                var composition2 = new MusicComposition { Artist = "Artist2", Title = "Title2" };
                context.Catalog.AddRange(composition1, composition2);
                context.SaveChanges();

                // Act
                var result = controller.GetAllCompositions();
                var resultList = Assert.IsType<OkObjectResult>(result.Result).Value as List<MusicComposition>;

                // Assert
                Assert.NotNull(resultList);
                Assert.Equal(2, resultList.Count);
                Assert.Contains(composition1, resultList);
                Assert.Contains(composition2, resultList);
            }
        }

        [Fact]
        public void SearchCompositions_ShouldReturnMatchingResults()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MusicCatalogContext>()
                .UseInMemoryDatabase(databaseName: "SearchCompositions_Test_Database")
                .Options;

            using (var context = new MusicCatalogContext(options))
            {
                var controller = new MusicCatalogController(context);
                var composition1 = new MusicComposition { Artist = "Artist1", Title = "Title1" };
                var composition2 = new MusicComposition { Artist = "Artist2", Title = "Title2" };
                context.Catalog.AddRange(composition1, composition2);
                context.SaveChanges();

                // Act
                var result = controller.SearchCompositions("Artist");
                var resultList = Assert.IsType<OkObjectResult>(result.Result).Value as List<MusicComposition>;

                // Assert
                Assert.NotNull(resultList);
                Assert.Equal(2, resultList.Count);
                Assert.Contains(composition1, resultList);
                Assert.Contains(composition2, resultList);
            }
        }
    }
}