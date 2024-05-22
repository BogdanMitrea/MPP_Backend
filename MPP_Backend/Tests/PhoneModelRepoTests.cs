using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP_BackEnd;
using MPP_BackEnd.Repositories;

namespace MPP_BackEnd.Tests
{
    [TestClass]
    public class RepositoryPhoneTests
    {
        [TestMethod]
        public void TestAddPhone_ValidPhone_ReturnsId()
        {
            // Arrange
            var phoneModel = new PhoneModel
            {
                Id = -1,
                Name = "TestPhone",
                Producer = "TestProducer",
                Year = 2024,
                Color = "TestColor",
                Memory = 64,
                Photo = "test.jpg"
            };
            var repository = new RepositoryPhone();

            // Act
            int addedPhoneId = repository.AddPhone(phoneModel);

            // Assert
            Assert.AreEqual(repository.GetMaxId(), addedPhoneId + 1); // Assuming IDs are auto-incremented
        }

        [TestMethod]
        public void TestDeletePhone_ExistingId_ReturnsTrue()
        {
            // Arrange
            var repository = new RepositoryPhone();
            int idToDelete = repository.GetMaxId() - 1; // Assuming phone with ID 2 exists
            
            // Act
            bool isDeleted = repository.DeletePhone(idToDelete);

            // Assert
            Assert.IsTrue(isDeleted);
        }
        [TestMethod]
        public void TestDeletePhone_NonexistentId_ReturnsFalse()
        {
            // Arrange
            int nonExistentId = 100;
            var repository = new RepositoryPhone();

            // Act
            bool isDeleted = repository.DeletePhone(nonExistentId);

            // Assert
            Assert.IsFalse(isDeleted);
        }
        [TestMethod]
        public void TestGetAllPhones_ReturnsAllPhones()
        {
            // Arrange
            var repository = new RepositoryPhone();

            // Act
            var allPhones = repository.GetAllPhones().ToList();

            // Assert
            //Assert.AreEqual(10, allPhones.Count);

            Thread addingThread;
            addingThread = new Thread(repository.generatenewPhone)
            {
                IsBackground = true
            };
            
        }
        [TestMethod]
        public void TestGetPhone()
        {
            var phoneModel = new PhoneModel
            {
                Id = -1,
                Name = "TestPhone",
                Producer = "TestProducer",
                Year = 2024,
                Color = "TestColor",
                Memory = 64,
                Photo = "test.jpg"
            };
            var repository = new RepositoryPhone();

            // Act
            int addedPhoneId = repository.AddPhone(phoneModel);

            // Arrange
            int idToGet = repository.GetMaxId() - 1;

            // Act
            PhoneModel phone = repository.GetPhone(idToGet);

            // Assert
            Assert.IsNotNull(phone);
            Assert.AreEqual("TestPhone", phone.Name);
        }

        [TestMethod]
        public void TestUpdatePhone_ExistingId_ReturnsTrue()
        {
            // Arrange
            var phoneModel = new PhoneModel
            {
                Id = 10,
                Name = "TestPhone",
                Producer = "TestProducer",
                Year = 2024,
                Color = "TestColor",
                Memory = 64,
                Photo = "test.jpg"
            };
            var repository = new RepositoryPhone();
            int addedPhoneId = repository.AddPhone(phoneModel);
            // Act
            bool isUpdated = repository.UpdatePhone(addedPhoneId, phoneModel);

            // Assert
            Assert.IsTrue(isUpdated);
            Assert.AreEqual("TestPhone", repository.GetPhone(addedPhoneId).Name);
        }

        [TestMethod]
        public void TestUpdatePhone_NonexistentId_ReturnsFalse()
        {
            // Arrange
            int nonExistentId = 100;
            var phoneModel = new PhoneModel
            {
                Id = 11,
                Name = "TestPhone",
                Producer = "TestProducer",
                Year = 2024,
                Color = "TestColor",
                Memory = 64,
                Photo = "test.jpg"
            };
            var repository = new RepositoryPhone();

            // Act
            bool isUpdated = repository.UpdatePhone(nonExistentId, phoneModel);

            // Assert
            Assert.IsFalse(isUpdated);
        }

        /// <summary>
        /// Fake repo tests
        /// </summary>
        public void TestAddPhone_ValidPhone_ReturnsId2()
        {
            // Arrange
            var phoneModel = new PhoneModel
            {
                Id = 11,
                Name = "TestPhone",
                Producer = "TestProducer",
                Year = 2024,
                Color = "TestColor",
                Memory = 64,
                Photo = "test.jpg"
            };
            var repository = new FakeRepository(new List<PhoneModel>());

            // Act
            int addedPhoneId = repository.AddPhone(phoneModel);

            // Assert
            Assert.AreEqual(11, addedPhoneId); // Assuming IDs are auto-incremented
        }

        [TestMethod]
        public void TestDeletePhone_ExistingId_ReturnsTrue2()
        {
            // Arrange
            var phoneModel = new PhoneModel
            {
                Id = 11,
                Name = "TestPhone",
                Producer = "TestProducer",
                Year = 2024,
                Color = "TestColor",
                Memory = 64,
                Photo = "test.jpg"
            };
            var repository = new FakeRepository(new List<PhoneModel>());

            // Act
            int addedPhoneId = repository.AddPhone(phoneModel);

            // Act
            bool isDeleted = repository.DeletePhone(1);

            // Assert
            Assert.IsTrue(isDeleted);
        }
        [TestMethod]
        public void TestDeletePhone_NonexistentId_ReturnsFalse2()
        {
            // Arrange
            int nonExistentId = 100;
            var repository = new FakeRepository(new List<PhoneModel>());

            // Act
            bool isDeleted = repository.DeletePhone(nonExistentId);

            // Assert
            Assert.IsFalse(isDeleted);
        }
        [TestMethod]
        public void TestGetAllPhones_ReturnsAllPhones2()
        {
            // Arrange
            var phoneModel = new PhoneModel
            {
                Id = 11,
                Name = "TestPhone",
                Producer = "TestProducer",
                Year = 2024,
                Color = "TestColor",
                Memory = 64,
                Photo = "test.jpg"
            };
            var repository = new FakeRepository(new List<PhoneModel>());

            // Act
            int addedPhoneId = repository.AddPhone(phoneModel);

            // Act
            var allPhones = repository.GetAllPhones().ToList();

            // Assert
            Assert.AreEqual(1, allPhones.Count); // Assuming there are 0 initial phones
        }
        [TestMethod]
        public void TestGetPhone2()
        {
            // Arrange
            int idToGet = 1;
            var phoneModel = new PhoneModel
            {
                Id = 11,
                Name = "TestPhone",
                Producer = "TestProducer",
                Year = 2024,
                Color = "TestColor",
                Memory = 64,
                Photo = "test.jpg"
            };
            var repository = new FakeRepository(new List<PhoneModel>());

            // Act
            int addedPhoneId = repository.AddPhone(phoneModel);

            // Act
            PhoneModel phone = repository.GetPhone(idToGet);

            // Assert
            Assert.IsNotNull(phone);
            Assert.AreEqual("TestPhone", phone.Name);
        }

        [TestMethod]
        public void TestUpdatePhone_ExistingId_ReturnsTrue2()
        {
            // Arrange
            var phoneModel = new PhoneModel
            {
                Id = 10,
                Name = "TestPhone",
                Producer = "TestProducer",
                Year = 2024,
                Color = "TestColor",
                Memory = 64,
                Photo = "test.jpg"
            };
            var repository = new FakeRepository(new List<PhoneModel>());
            int addedPhoneId = repository.AddPhone(phoneModel);
            // Act
            bool isUpdated = repository.UpdatePhone(addedPhoneId, phoneModel);

            // Assert
            Assert.IsTrue(isUpdated);
            Assert.AreEqual("TestPhone", repository.GetPhone(addedPhoneId).Name);
        }

        [TestMethod]
        public void TestUpdatePhone_NonexistentId_ReturnsFalse2()
        {
            // Arrange
            int nonExistentId = 100;
            var phoneModel = new PhoneModel
            {
                Id = 11,
                Name = "TestPhone",
                Producer = "TestProducer",
                Year = 2024,
                Color = "TestColor",
                Memory = 64,
                Photo = "test.jpg"
            };
            var repository = new FakeRepository(new List<PhoneModel>());

            // Act
            bool isUpdated = repository.UpdatePhone(nonExistentId, phoneModel);

            // Assert
            Assert.IsFalse(isUpdated);
        }

        [TestMethod]
        public void TestSockets()
        {
            var repository = new RepositoryPhone();
            repository.StartWebSocketServerAsync();
            repository.SendWebSocketMessage("Hatz");
        }
    }
}
