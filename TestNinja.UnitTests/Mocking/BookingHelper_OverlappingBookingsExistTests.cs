using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TestNinja.Mocking;

namespace TestNinja.UnitTests.Mocking
{
    [TestFixture]
    public class BookingHelper_OverlappingBookingsExistTests
    {
        private Booking _existingBooking;
        private Mock<IBookingRepository> _repository;

        [SetUp]
        public void SetUp()
        {
            _existingBooking = new Booking
            {
                Id = 2,
                ArrivalDate = ArriveOn(2017, 1, 15),
                DepartureDate = DepartOn(2017, 1, 20),
                Reference = "a"
            };

            _repository = new Mock<IBookingRepository>();
            _repository.Setup(br => br.GetActiveBookings(1)).Returns(new List<Booking> { _existingBooking }.AsQueryable);
        }

        [Test]
        public void BookingStartsAndFinishesBeforeAnExistingBooking_ReturnEmptyString()
        {
            var booking = new Booking
            {
                Id = 1,
                ArrivalDate = Before(_existingBooking.ArrivalDate, days: 2),
                DepartureDate = Before(_existingBooking.ArrivalDate)
            };

            var result = BookingHelper.OverlappingBookingsExist(booking, _repository.Object);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void BookingStartsBeforeAndFinishesInTheMiddleOfAnExistingBooking_ReturnExistingBookingsReference()
        {
            var booking = new Booking
            {
                Id = 1,
                ArrivalDate = Before(_existingBooking.ArrivalDate),
                DepartureDate = After(_existingBooking.ArrivalDate)
            };

            var result = BookingHelper.OverlappingBookingsExist(booking, _repository.Object);

            Assert.That(result, Is.EqualTo(_existingBooking.Reference));
        }

        [Test]
        public void BookingStartsBeforeAndFinishesAfterAnExistingBooking_ReturnExistingBookingsReference()
        {
            var booking = new Booking
            {
                Id = 1,
                ArrivalDate = Before(_existingBooking.ArrivalDate),
                DepartureDate = After(_existingBooking.DepartureDate)
            };

            var result = BookingHelper.OverlappingBookingsExist(booking, _repository.Object);

            Assert.That(result, Is.EqualTo(_existingBooking.Reference));
        }

        [Test]
        public void BookingStartsAndFinishesInTheMiddleOfAnExistingBooking_ReturnExistingBookingsReference()
        {
            var booking = new Booking
            {
                Id = 1,
                ArrivalDate = After(_existingBooking.ArrivalDate),
                DepartureDate = Before(_existingBooking.DepartureDate)
            };

            var result = BookingHelper.OverlappingBookingsExist(booking, _repository.Object);

            Assert.That(result, Is.EqualTo(_existingBooking.Reference));
        }

        [Test]
        public void BookingStartsInTheMiddleOfAnExistingBookingButFinishesAfter_ReturnExistingBookingsReference()
        {
            var booking = new Booking
            {
                Id = 1,
                ArrivalDate = After(_existingBooking.ArrivalDate),
                DepartureDate = After(_existingBooking.DepartureDate)
            };

            var result = BookingHelper.OverlappingBookingsExist(booking, _repository.Object);

            Assert.That(result, Is.EqualTo(_existingBooking.Reference));
        }

        [Test]
        public void BookingStartsAndFinishesAfterAnExistingBooking_ReturnExistingBookingsReference()
        {
            var booking = new Booking
            {
                Id = 1,
                ArrivalDate = After(_existingBooking.DepartureDate),
                DepartureDate = After(_existingBooking.DepartureDate)
            };

            var result = BookingHelper.OverlappingBookingsExist(booking, _repository.Object);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void BookingsOverlapButNewBookingIsCancelled_ReturnEmptyString()
        {
            var booking = new Booking
            {
                Id = 1,
                ArrivalDate = After(_existingBooking.DepartureDate),
                DepartureDate = After(_existingBooking.DepartureDate, days: 2),
                Status = "Cancelled"
            };

            var result = BookingHelper.OverlappingBookingsExist(booking, _repository.Object);

            Assert.That(result, Is.Empty);
        }

        private static DateTime Before(DateTime dateTime, int days = 1) => dateTime.AddDays(-days);

        private static DateTime After(DateTime dateTime, int days = 1) => dateTime.AddDays(days);

        private static DateTime ArriveOn(int year, int month, int day) => new(year, month, day, 14, 0, 0);

        private static DateTime DepartOn(int year, int month, int day) => new(year, month, day, 10, 0, 0);
    }
}
