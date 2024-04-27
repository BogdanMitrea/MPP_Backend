using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP_BackEnd.Controllers;
using MPP_BackEnd.Repositories;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace MPP_BackEnd.Tests
{
    [TestClass]
    public class PhoneControllerTests
    {
        [TestMethod]
        public void GetAllPhones_ReturnsAllPhones()
        {
            // Arrange
            var phones = new List<PhoneModel>
            {
                new PhoneModel { Id = 1, Name = "Phone1" },
                new PhoneModel { Id = 2, Name = "Phone2" },
                new PhoneModel { Id = 3, Name = "Phone3" }
            };
            var fakeRepository = new FakeRepository(phones);
            var controller = new PhoneController(fakeRepository);

            // Act
            var result = controller.GetAllPhones();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void GetPhoneById_ReturnsPhone_IfExists()
        {
            // Arrange
            var phones = new List<PhoneModel>
            {
                new PhoneModel { Id = 1, Name = "Phone1" },
                new PhoneModel { Id = 2, Name = "Phone2" },
                new PhoneModel { Id = 3, Name = "Phone3" }
            };
            var fakeRepository = new FakeRepository(phones);
            var controller = new PhoneController(fakeRepository);

            // Act
            var result = controller.GetPhoneById(2) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsInstanceOfType(result.Value, typeof(PhoneModel));
            Assert.AreEqual(2, (result.Value as PhoneModel).Id);
            PhoneModel phone = new PhoneModel { Id = 4, Name = "Phone4" };
            controller.AddNewPhone(phone);
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            controller.UpdatePhone(4, phone);
            controller.UpdatePhone(44, phone);
            controller.DeletePhone(4);
            controller.DeletePhone(44);
            result = controller.GetPhoneById(22) as OkObjectResult;
        }

        // Similarly, you can write tests for other controller actions like AddNewPhone, DeletePhone, UpdatePhone.
    }
}

