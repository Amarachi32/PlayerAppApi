// /////////////////////////////////////////////////////////////////////////////
// TESTING AREA
// THIS IS AN AREA WHERE YOU CAN TEST YOUR WORK AND WRITE YOUR TESTS
// /////////////////////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Services;
using WebApiTest.Base;

namespace WebApiTest
{
    public class ProcessTeamTest : BaseTestWrapper
    {

        private TeamServices _teamServices;
        private DataContext _context;

        [Test]
        public async Task TestSample()
        {
            List<TeamProcessViewModel> requestData = new List<TeamProcessViewModel>()
            {
                new TeamProcessViewModel()
                {
                    Position = "defender",
                    MainSkill = "speed",
                    NumberOfPlayers = "1"
                }
            };

            var response = await client.PostAsJsonAsync("/api/team/process", requestData);
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
            _teamServices = new TeamServices(_context, new ValidServices());
        }

        [Test]
        public async Task SelectTeamsAsync_ReturnsSelectedPlayers()
        {
            // Arrange: Create some sample players and add them to the in-memory database
            var players = new List<Player>
            {
                new Player { Name = "Player1", Position = "Midfielder", PlayerSkills = new List<PlayerSkill> { new PlayerSkill { Skill = "Speed", Value = 80 } } },
                new Player { Name = "Player2", Position = "Forward", PlayerSkills = new List<PlayerSkill> { new PlayerSkill { Skill = "Speed", Value = 90 } } },
                new Player { Name = "Player3", Position = "Midfielder", PlayerSkills = new List<PlayerSkill> { new PlayerSkill { Skill = "Strength", Value = 75 } } },
            };
            await _context.Players.AddRangeAsync(players);
            await _context.SaveChangesAsync();

            var requirements = new List<TeamRequirement>
            {
                new TeamRequirement { Position = "Midfielder", MainSkill = "Speed", NumberOfPlayers = 1 },
                new TeamRequirement { Position = "Forward", MainSkill = "Speed", NumberOfPlayers = 1 },
            };
            // Act: Call the method being tested
            var result = await _teamServices.SelectTeamsAsync(requirements);

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
        public void SelectTeamsAsync_NoPlayers_ThrowsValidationException()
        {
            // Arrange: The database is empty
            var requirements = new List<TeamRequirement>
            {
                new TeamRequirement { Position = "Midfielder", MainSkill = "Speed", NumberOfPlayers = 1 },
            };

            // Act and Assert: Check if a ValidationException with the expected message is thrown
            var exception = Assert.ThrowsAsync<ValidationException>(() => _teamServices.SelectTeamsAsync(requirements));
            Assert.AreEqual("position", exception.Field);
            Assert.AreEqual("Insufficient number of players for position: Midfielder", exception.InvalidValue);
        }
    }
}
