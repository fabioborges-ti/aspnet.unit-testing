using Bongo.Core.Services.IServices;
using Bongo.Models.Model;
using Bongo.Models.Model.VM;
using Bongo.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Bongo.Web.Tests
{
    [TestFixture]
    public class RoomBookingControllerTests
    {
        private readonly Mock<IStudyRoomBookingService> _studyRoomBookingService = new();

        private RoomBookingController _bookingController;

        [SetUp]
        public void SetUp()
        {
            _bookingController = new RoomBookingController(_studyRoomBookingService.Object);
        }

        [Test]
        public void IndexPage_CallRequest_VerifyGetAllInvoked()
        {
            _bookingController.Index();

            _studyRoomBookingService
                .Verify(x => x.GetAllBooking(), Times.Once());
        }

        [Test]
        public void BookRoomCheck_ModelStateInvalid_ReturnView()
        {
            _bookingController.ModelState.AddModelError("test", "test");

            var result = _bookingController.Book(new StudyRoomBooking());

            var viewResult = result as ViewResult;

            Assert.AreEqual("Book", viewResult?.ViewName);
        }

        [Test]
        public void BookRoomCheck_NotSuccessful_NoRoomCode()
        {
            _studyRoomBookingService
                .Setup(x => x.BookStudyRoom(It.IsAny<StudyRoomBooking>()))
                .Returns(new StudyRoomBookingResult
                {
                    Code = StudyRoomBookingCode.NoRoomAvailable,
                });

            var result = _bookingController.Book(new StudyRoomBooking());

            var viewResult = result as ViewResult;

            Assert.AreEqual("No Study Room available for selected date", viewResult?.ViewData["Error"]);
        }
    }
}
