using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ZLibrary.Model;

namespace ZLibrary.Test.Model
{
    [TestClass]
    public class TestLoan
    {
        [TestMethod]
        public void TestCreateLoan()
        {
            var book = new Book();
            var bookId = 2;
            var user = new User();
            var reservation = new Reservation(bookId, user);
            var loan = new Loan(reservation);

            Assert.IsNotNull(loan);
            Assert.AreEqual(reservation, loan.Reservation);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCreateLoanWithNullParameterShouldThrowException()
        {
            var loan = new Loan(null);
        }

        [TestMethod]
        public void TestLoanProperties()
        {
            var book = new Book();
            var bookId = 2;
            var user = new User();
            var reservation = new Reservation(bookId, user);
            var loan = new Loan(reservation);
            var expirationDate = DateTime.Now.AddMonths(3);
            Assert.IsNotNull(loan);
            Assert.AreEqual(reservation, loan.Reservation);
            Assert.AreEqual(LoanStatus.Borrowed, loan.Status);
            Assert.IsNotNull(loan.ExpirationDate);
            Assert.AreEqual(expirationDate.Date, loan.ExpirationDate.Date);
        }

        [TestMethod]
        public void TestLoanChangeStatusProperty()
        {
            var book = new Book();
            var bookId = 2;
            var user = new User();
            var reservation = new Reservation(bookId, user);
            var loan = new Loan(reservation);
            var expirationDate = DateTime.Now.AddMonths(3);
            loan.Status = LoanStatus.Expired;
            Assert.IsNotNull(loan);
            Assert.AreEqual(reservation, loan.Reservation);
            Assert.AreEqual(LoanStatus.Expired, loan.Status);
            Assert.IsNotNull(loan.ExpirationDate);
            Assert.AreEqual(expirationDate.Date, loan.ExpirationDate.Date);
        }
    }
}