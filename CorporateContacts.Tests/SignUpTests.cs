using System;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;
using Xobnu.WebUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Collections.Generic;

namespace CorporateContacts.Tests
{
    [TestClass]
    public class SignUpTests
    {
        [TestMethod]
        public void Can_Retrive_Products()
        {
            //Arrange
            Mock<IPlanRepo> mock = new Mock<IPlanRepo>();
            mock.Setup(m => m.Plans).Returns(new Plan[] {
                            new Plan {ID = 1, Name = "P1", Price=9.99M},
                            new Plan {ID = 2, Name = "P2", Price=29.99M},
                            new Plan {ID = 3, Name = "P3", Price=49.99M},
                                                                }.AsQueryable());
            //SignUpController controller = new SignUpController(mock.Object, null, null,null,null,null);

            //Act
          //  IEnumerable<Plan> result = (IEnumerable<Plan>)controller.ProductList().Model;

            //Assert
          //  Assert.AreEqual(3, result.ToList().Count);
        }

        [TestMethod]
        public void Cannot_Save_Accounts_with_errors()
        {
            //Arrange
            //Mock<IAccountRepo> mock = new Mock<IAccountRepo>();
            //mock.Setup(m => m.Accounts).Returns(new Account[] {
            //                new Account {ID = 1, AccountName = "a1"},
            //                new Account {ID = 2, AccountName = "a2"},
            //                new Account {ID = 3, AccountName = "a3"},
            //                                                    }.AsQueryable());
            //SignUpController controller = new SignUpController(null, mock.Object, null,null,null);
            //Account newAccountToBeAdded = new Account {  };
            //controller.ModelState.AddModelError("", "");

            ////Act
            //var result = controller.SaveAccount(newAccountToBeAdded);

            ////Assert
            //mock.Verify(m => m.SaveAccount(newAccountToBeAdded));
        }
    }
}
