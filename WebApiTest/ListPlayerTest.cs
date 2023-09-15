// /////////////////////////////////////////////////////////////////////////////
// TESTING AREA
// THIS IS AN AREA WHERE YOU CAN TEST YOUR WORK AND WRITE YOUR TESTS
// /////////////////////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApiTest
{
    public class ListPlayerTest : BaseTestWrapper
    {
        private PlayersServices _playersServices;
        private DataContext _context;
        public override async Task Setup()
        {
            await base.Setup();
        }

        [Test]
        public async Task TestSample()
        {

            var response = await client.GetAsync("/api/player");
            try
            {
                var responseObject = await response.Content.ReadAsStringAsync();
                Assert.That(responseObject, Is.Not.Null);
            }
            catch
            {
                Assert.Fail("Invalid response object");
            }
        }

        [SetUp]
        public void SetUp()
        {
            // Set up your test environment, e.g., using an in-memory database
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryTestDatabase")
                .Options;

            _context = new DataContext(options);
            _context.Database.EnsureCreated();

            // Initialize your service with the test context
            _playersServices = new PlayersServices(_context, new ValidServices());
        }

        [Test]
        public async Task GetAllAsync_ReturnsPlayers()
        {
            // Arrange: Create some sample players and add them to the in-memory database
            var players = new List<Player>
            {
                new Player { Name = "Player1", Position = "Midfielder" },
                new Player { Name = "Player2", Position = "Forward" },
            };
            await _context.Players.AddRangeAsync(players);
            await _context.SaveChangesAsync();

            // Act: Call the method being tested
            var result = await _playersServices.GetAllAsync();

            // Assert: Check if the result is not null and contains the expected number of players
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());

            // Check if the player details are as expected
            var player1 = result.FirstOrDefault(p => p.Name == "Player1");
            var player2 = result.FirstOrDefault(p => p.Name == "Player2");
            Assert.IsNotNull(player1);
            Assert.IsNotNull(player2);
            Assert.AreEqual("Midfielder", player1.Position);
            Assert.AreEqual("Forward", player2.Position);
        }

        [Test]
        public void GetAllAsync_NoPlayers_ThrowsValidationException()
        {
            // Arrange: The database is empty

            // Act and Assert: Check if a ValidationException with the expected message is thrown
            var exception = Assert.ThrowsAsync<ValidationException>(() => _playersServices.GetAllAsync());
            Assert.AreEqual("players", exception.Field);
            Assert.AreEqual("No players found.", exception.InvalidValue);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

    }
}
