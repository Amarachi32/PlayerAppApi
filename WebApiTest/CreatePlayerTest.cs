// /////////////////////////////////////////////////////////////////////////////
// TESTING AREA
// THIS IS AN AREA WHERE YOU CAN TEST YOUR WORK AND WRITE YOUR TESTS
// /////////////////////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApi.DTO;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Services;
using WebApiTest.Base;

namespace WebApiTest
{

    public class CreatePlayerTest : BaseTestWrapper
    {
        private PlayersServices _playersServices;
        private DataContext _context;

        [Test]
        public async Task TestSample()
        {
            Player player = new()
            {
                Name = "player name",
                Position = "defender",
                PlayerSkills = new()
                {
                    new() { Skill = "attack", Value = 60 },
                    new() { Skill = "speed", Value = 80 },
                }
            };

            var response = await client.PostAsJsonAsync("/api/player", player);
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

            // Initialize your service with the test context
            _playersServices = new PlayersServices(_context, new ValidServices());
        }

        [Test]
        public async Task CreatePlayerAsync_ValidPlayer_ReturnsCreatedPlayer()
        {
            // Arrange: Create a valid player DTO
            var createPlayerDto = new CreatePlayerDto
            {
                Name = "Player1",
                Position = "Midfielder",
                PlayerSkills = new List<PlayerSkillDto>
                {
                    new PlayerSkillDto { Skill = "Speed", Value = 80 },
                    new PlayerSkillDto { Skill = "Dribbling", Value = 75 }
                }
            };
            var result = await _playersServices.CreatePlayerAsync(createPlayerDto);
            Assert.IsNotNull(result);
            Assert.AreEqual("Player1", result.Name);
            Assert.AreEqual("Midfielder", result.Position);
            Assert.IsNotNull(result.PlayerSkills);
            Assert.AreEqual(2, result.PlayerSkills.Count);
        }

        [Test]
        public void CreatePlayerAsync_InvalidPlayerDto_ThrowsValidationException()
        {

            CreatePlayerDto createPlayerDto = null;
            var exception = Assert.ThrowsAsync<ValidationException>(() => _playersServices.CreatePlayerAsync(createPlayerDto));
            Assert.AreEqual("request", exception.Field);
            Assert.AreEqual("Invalid request data.", exception.InvalidValue);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
