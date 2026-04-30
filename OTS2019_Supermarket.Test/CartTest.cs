using NUnit.Framework;
using OTS_Supermarket.Models;
using OTS_Supermarket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS_Supermarket.Test
{
    [TestFixture]
    public class CartTest
    {
        [Test]
        public void AddOneToCart_ShouldAddItemToCart_Success()
        {
            // ARRANGE
            Cart cart = new Cart();
            Monitor monitor = new Monitor();

            // ACT
            cart.AddOneToCart(monitor);
            // ASSERT
            Assert.That(cart.Size, Is.EqualTo(1));
            Assert.That(cart.Amount, Is.EqualTo(100));
        }

        [Test]
        public void AddMultipleToCart_ShouldAddMultipleItemsAndUpdateCounters()
        {
            // ARRANGE
            Cart cart = new Cart();
            Monitor monitor = new Monitor();

            // ACT
            cart.AddMultipleToCart(monitor, 3);

            // ASSERT
            Assert.That(cart.Size, Is.EqualTo(3));
            Assert.That(cart.Amount, Is.EqualTo(300));
            Assert.That(cart.Monitor_counter, Is.EqualTo(3));
        }

        [Test]
        public void AddOneToCart_ThrowsWhenExceedLimit()
        {
            // ARRANGE
            Cart cart = new Cart();
            Monitor m = new Monitor();

            // fill cart to limit
            for (int i = 0; i < 10; i++)
            {
                cart.AddOneToCart(m);
            }

            // ACT
            TestDelegate act = () => cart.AddOneToCart(new Monitor());

            // ASSERT
            Assert.Throws<Exception>(act);
        }

        [Test]
        public void AddMultipleToCart_ThrowsWhenExceedLimit()
        {
            // ARRANGE
            Cart cart = new Cart();
            Monitor m = new Monitor();

            // ACT
            TestDelegate act1 = () => cart.AddMultipleToCart(m, 11);

            // ASSERT - trying to add 11 at once should fail
            Assert.Throws<Exception>(act1);

            // also try when partial fill would overflow
            cart = new Cart();
            cart.AddMultipleToCart(m, 8);
            TestDelegate act2 = () => cart.AddMultipleToCart(m, 3);

            // ASSERT
            Assert.Throws<Exception>(act2);
        }

        [Test]
        public void DeleteAll_ShouldClearCart()
        {
            // ARRANGE
            Cart cart = new Cart();
            cart.AddOneToCart(new Monitor());
            cart.AddOneToCart(new Keyboard());

            // ACT
            cart.DeleteAll();

            // ASSERT
            Assert.That(cart.Size, Is.EqualTo(0));
            Assert.That(cart.Items.Count, Is.EqualTo(0));
            Assert.That(cart.Monitor_counter, Is.EqualTo(0));
            Assert.That(cart.Keyboard_counter, Is.EqualTo(0));
        }

        [Test]
        public void DeleteAll_ThrowsWhenEmpty()
        {
            // ARRANGE
            Cart cart = new Cart();

            // ACT
            TestDelegate act = () => cart.DeleteAll();

            // ASSERT
            Assert.Throws<Exception>(act);
        }

        [Test]
        public void Print_ThrowsOnEmpty()
        {
            // ARRANGE
            Cart cart = new Cart();

            // ACT
            TestDelegate act = () => cart.Print();

            // ASSERT
            Assert.Throws<Exception>(act);
        }

        [Test]
        public void Calculate_WithValidDate_DecreasesBudgetByAmount()
        {
            // ARRANGE
            Cart cart = new Cart();
            cart.Budget = 200;
            cart.AddOneToCart(new Monitor()); // Amount = 100

            string date = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

            // ACT
            cart.Calculate(date);

            // ASSERT
            Assert.That(cart.Budget, Is.EqualTo(100));
        }

        [Test]
        public void Calculate_WrongDateFormat_Throws()
        {
            // ARRANGE
            Cart cart = new Cart();
            string badDate = "10-10-2023";

            // ACT
            TestDelegate act = () => cart.Calculate(badDate);

            // ASSERT
            var ex = Assert.Throws<Exception>(act);
            Assert.That(ex.Message, Does.Contain("Wrong date format"));
        }

        [Test]
        public void Calculate_TodayDate_Throws()
        {
            // ARRANGE
            Cart cart = new Cart();
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            // ACT
            TestDelegate act = () => cart.Calculate(today);

            // ASSERT
            var ex = Assert.Throws<Exception>(act);
            Assert.That(ex.Message, Does.Contain("can't be today's date"));
        }

        [Test]
        public void Calculate_NotEnoughBudget_Throws()
        {
            // ARRANGE
            Cart cart = new Cart();
            cart.Budget = 50; // less than item price
            cart.AddOneToCart(new Monitor()); // 100
            string date = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

            // ACT
            TestDelegate act = () => cart.Calculate(date);

            // ASSERT
            var ex = Assert.Throws<Exception>(act);
            Assert.That(ex.Message, Does.Contain("Not enough budget"));
        }
    }
}
