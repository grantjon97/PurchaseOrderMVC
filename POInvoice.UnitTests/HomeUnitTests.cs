using System;
using System.Collections.Generic;
using POInvoice;
using POInvoice.Controllers;
using POInvoice.ViewModels;
using System.Web;
using System.Web.Mvc;
using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using POInvoice.Data.Domain;
using System.Threading.Tasks;

namespace POInvoice.HomeUnitTests
{
    public class HomeControllerTests
    {
        protected HomeController controller;

        public HomeControllerTests()
        {
            controller = new HomeController();
        }
    }

    [TestClass]
    public class HomeIndexTests : HomeControllerTests
    {
        public HomeIndexTests()
        {
            POInvoice.Data.Persistence.PoInvoice.Test = true;
        }

        [TestMethod]
        public void Index_Pass_CurrentYear()
        {
            var result = controller.Index("2018") as ViewResult;
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(((List<PoFormSearchData>)result.Model).Count > 0);
        }

        [TestMethod]
        public void Index_Pass_FutureYear()
        {
            var result = controller.Index("2020") as ViewResult;
            Assert.IsTrue(((List<PoFormSearchData>)result.Model).Count == 0);
        }

        [TestMethod]
        public void Index_Pass_NotAYearString()
        {
            // An invalid query string such as "xyz" will default to getting
            // the current year's PO forms.
            var result = controller.Index("xyz") as ViewResult;
            Assert.IsTrue(((List<PoFormSearchData>)result.Model).Count > 0);
        }
    }

    [TestClass]
    public class HomeSaveTests : HomeControllerTests
    {
        [TestMethod]
        [ExpectedException(typeof(System.IO.InvalidDataException))]
        public async Task Save_ThrowException_BlankForm()
        {
            var result = await controller.Save(new PoForm { }) as ViewResult;
        }
    }

    [TestClass]
    public class HomeCompleteFormTests : HomeControllerTests
    {
        [TestMethod]
        public void CompleteForm_ThrowException_BadId()
        {
            var result = controller.CompleteForm(Int32.MaxValue) as HttpStatusCodeResult;
            Assert.IsTrue(result.StatusCode == 404);
        }
    }
}
