using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Domain.Enums;

namespace PaymentContext.Test.Entities
{
    [TestClass]
    public class StudentTests
    {
        [TestMethod]
        public void ShouldReturnErrorWhenHasActiveSubscription()
        {
            var name = new Name("Bruce", "Wayne");
            var doc = new Document("12345678912", EDocumentType.CPF);
            var email = new Email("batman@dc.com");
            var student = new Student(name, doc, email);
            var subscription = new Subscription(null);
            var payment = new PayPalPayment("123456", "6", DateTime.Now, DateTime.Now.AddDays(5), 10, 10, "Wayne Corp.", doc, null, email);
            Assert.Fail();
        }

        [TestMethod]
        public void ShouldReturnErrorWhenSubscriptionHasNoPayment()
        {
            var name = new Name("Bruce", "Wayne");
            var doc = new Document("12345678912", EDocumentType.CPF);
            var email = new Email("batman@dc.com");
            var student = new Student(name, doc, email);
            Assert.Fail();
        }

        [TestMethod]
        public void ShouldReturnSuccessWhenHasNoActiveSubscription()
        {
            Assert.Fail();
        }
    }
}
