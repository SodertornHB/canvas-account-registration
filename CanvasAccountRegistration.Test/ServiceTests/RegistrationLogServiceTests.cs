
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Logic.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace CanvasAccountRegistration.Test
{    
    public class RegistrationLogServiceTests
    {
        private Mock<ILogger<RegistrationLogService>> loggerMock;
        private Mock<IRegistrationLogDataAccess> dataAccessMock;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogger<RegistrationLogService>>();
            dataAccessMock = new Mock<IRegistrationLogDataAccess>();
        }

        [Test]
        public void Initiate_SholdNotBeNull()
        {
            var sut = new RegistrationLogService(loggerMock.Object, dataAccessMock.Object);
            Assert.That(sut,!Is.Null);
        }
        
        [Test]
        public async Task GetRegistrationLog_SholdNotBeNull()
        {
             
        var id3 = 3;
            dataAccessMock.Setup(x => x.Get(It.IsAny<int>())).Returns(Task.FromResult(new RegistrationLog { Id = id3 }));
            var sut = new RegistrationLogService(loggerMock.Object, dataAccessMock.Object);
            var result = await sut.Get(id3);
            Assert.That(sut, !Is.Null);
            Assert.That(id3, Is.EqualTo( result.Id));
            loggerMock.VerifyLoggingExact(LogLevel.Information, $"Fetching entity with id {id3} from data source.");
        }

        [Test]
        public async Task GetAllRegistrationLogs_SholdContainEntities()
        {
             
            var id1 = 1;
            var id2 = 2;
            var id3 = 3;
            var testData = new List<RegistrationLog>
            {
                new RegistrationLog{ Id = id1 },
                new RegistrationLog{ Id = id2 },
                new RegistrationLog{ Id = id3 }
            };
            dataAccessMock.Setup(x => x.GetAll()).Returns(Task.FromResult(testData.AsEnumerable()));
            var sut = new RegistrationLogService(loggerMock.Object, dataAccessMock.Object);
            var result = await sut.GetAll();
            Assert.That(3, Is.EqualTo( result.Count()));
            Assert.That(id1, Is.EqualTo(result.First().Id));
            Assert.That(id3, Is.EqualTo(result.Last().Id));
            loggerMock.VerifyLoggingExact(LogLevel.Information, "Fetching all entities from data source.");
        }

        [Test]
        public async Task InsertRegistrationLog_VerifyInsertIsCalled()
        {
             
            var id1 = 1;
                    var sut = new RegistrationLogService(loggerMock.Object, dataAccessMock.Object);
            await sut.Insert(new RegistrationLog { Id = id1 });
            dataAccessMock.Verify(x => x.Insert(It.Is<RegistrationLog>(y => y.Id == id1)));
            loggerMock.VerifyLoggingContains(LogLevel.Information, "Saving entity");
        }

        [Test]
        public async Task UpdateRegistrationLog_VerifyUpdateIsCalled()
        {
             
        var id1 = 1;
            var sut = new RegistrationLogService(loggerMock.Object, dataAccessMock.Object);
            await sut.Update(new RegistrationLog { Id = id1 });
            dataAccessMock.Verify(x => x.Update(It.Is<RegistrationLog>(y => y.Id == id1)));
            loggerMock.VerifyLoggingContains(LogLevel.Information, "Update entity");
        }

        [Test]
        public async Task DeleteRegistrationLog_VerifyDeleteIsCalled()
        {
             
            var id1 = 1;
            var sut = new RegistrationLogService(loggerMock.Object, dataAccessMock.Object);
            await sut.Delete(id1);
            dataAccessMock.Verify(x => x.Delete(It.Is<int>(y => y == id1)));
            loggerMock.VerifyLoggingExact(LogLevel.Information, $"Deleting entity with id {id1} from data source.");
        }
    }
}
