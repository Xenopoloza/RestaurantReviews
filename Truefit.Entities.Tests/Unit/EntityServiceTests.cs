﻿using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Truefit.Entities.Models;
using Truefit.Entities.Repositories;

namespace Truefit.Entities.Tests.Unit
{
    [TestFixture]
    public class EntityServiceTests
    {
        private Mock<ICityRepository> _cityRepository;
        private Mock<IEntityRepository> _entityRepository;
        private IEntityService _entityService;

        [SetUp]
        public void Setup()
        {
            this._cityRepository = new Mock<ICityRepository>();
            this._entityRepository = new Mock<IEntityRepository>();
            this._entityService = new EntityService(this._cityRepository.Object, this._entityRepository.Object);
        }

        [Test]
        public async Task GetCity_Is_Repo_Passthrough()
        {
            var guid = Guid.NewGuid();
            var expected = new CityModel();

            this._cityRepository.Setup(x => x.GetByGuid(guid)).ReturnsAsync(expected);
            var actual = await this._entityService.GetCity(guid);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task GetAllCities_Is_Repo_Passthrough()
        {
            var expected = new[] { new CityModel() };

            this._cityRepository.Setup(x => x.GetAll()).ReturnsAsync(expected);
            var actual = await this._entityService.GetAllCities();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task GetEntity_Is_Repo_Passthrough()
        {
            var guid = Guid.NewGuid();
            var expected = new EntityModel();

            this._entityRepository.Setup(x => x.GetByGuid(guid)).ReturnsAsync(expected);
            var actual = await this._entityService.GetEntity(guid);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task GetEntities_Lowercases_Type()
        {
            var cityId = Guid.NewGuid();
            var type = "LOWER";
            var expected = "lower";

            await this._entityService.GetEntities(cityId, type);

            this._entityRepository.Verify(x => x.GetByCityAndType(cityId, expected));
        }

        [Test]
        public async Task AddUserEntity_Should_Set_NeedsApproved_To_True()
        {
            var entity = new EntityModel();
            await this._entityService.AddUserEntity(entity);
            this._entityRepository.Verify(x => x.Insert(It.Is<EntityModel>(e => e.NeedsReviewed)));
        }

        [Test]
        public async Task AddUserEntity_Should_Set_IsActive_To_False()
        {
            var entity = new EntityModel();
            await this._entityService.AddUserEntity(entity);
            this._entityRepository.Verify(x => x.Insert(It.Is<EntityModel>(e => !e.IsActive)));
        }

        [Test]
        public async Task AddUserEntity_Should_Set_New_Guid()
        {
            var guid = Guid.NewGuid();
            var entity = new EntityModel {Guid = guid};
            await this._entityService.AddUserEntity(entity);
            this._entityRepository.Verify(x => x.Insert(It.Is<EntityModel>(e => e.Guid != guid)));
        }
    }
}
