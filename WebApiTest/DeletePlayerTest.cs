// /////////////////////////////////////////////////////////////////////////////
// TESTING AREA
// THIS IS AN AREA WHERE YOU CAN TEST YOUR WORK AND WRITE YOUR TESTS
// /////////////////////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Internal;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApiTest
{
    public class DeletePlayerTest : BaseTestWrapper
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

            var response = await client.DeleteAsync("/api/player/1");
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
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryTestDatabase")
                .Options;

            _context = new DataContext(options);
            _context.Database.EnsureCreated();
            _playersServices = new PlayersServices(_context, new ValidServices());
        }


        [Test]
        public async Task DeletePlayerAsync_ValidId_ReturnsDeletedPlayer()
        {
            var player = new Player
            {
                Name = "Player1",
                Position = "Midfielder"
            };
            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();

            var result = await _playersServices.DeletePlayerAsync(player.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(player.Id, result.Id);
            Assert.AreEqual("Player1", result.Name);
            Assert.AreEqual("Midfielder", result.Position);

            var deletedPlayer = await _context.Players.FindAsync(player.Id);
            Assert.IsNull(deletedPlayer);
        }

        [Test]
        public void DeletePlayerAsync_InvalidId_ThrowsValidationException()
        {
            int invalidId = 999;

            var exception = Assert.ThrowsAsync<ValidationException>(() => _playersServices.DeletePlayerAsync(invalidId));
            Assert.AreEqual("Player not found", exception.Field);
            Assert.AreEqual("", exception.InvalidValue);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up resources and dispose of the in-memory database
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
