using System;
using Xunit;
using GreasyHands.Schedule;

namespace GreasyHandsTests.Schedule
{
    public class ScheduleParserTests
    {
        /*
        private readonly Func<TitleInfo> titleInfoFunc;

        public ScheduleParserTests()
        {
            titleInfoFunc = () => new TitleInfo();
        }
         * */

        [Fact]
        public void test_line_matches_shipping_date_text_long()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping This Week:  June 30, 2010";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 30));
        }

        [Fact]
        public void test_line_matches_shipping_date_mmddyy_long()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping This Week:  06/30/10";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 30));
        }


        [Fact]
        public void test_line_matches_shipping_date_mdyy_long()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping This Week:  6/3/10";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 3));
        }

        [Fact]
        public void test_line_matches_shipping_date_mdyyyy_long()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping This Week:  6/3/2010";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 3));
        }


        [Fact]
        public void test_line_matches_shipping_date_mddyyyy_long()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping This Week:  6/30/2010";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 30));
        }

        [Fact]
        public void test_line_matches_shipping_date_mddyy_long()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping This Week:  6/30/10";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 30));
        }


        [Fact]
        public void test_line_matches_shipping_date_text_short()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping June 30, 2010";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 30));
        }

        [Fact]
        public void test_line_matches_shipping_date_mmddyy_short()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping 06/30/10";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 30));
        }


        [Fact]
        public void test_line_matches_shipping_date_mdyy_short()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping 6/3/10";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 3));
        }

        [Fact]
        public void test_line_matches_shipping_date_mdyyyy_short()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping 6/3/2010";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 3));
        }


        [Fact]
        public void test_line_matches_shipping_date_mddyyyy_short()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping 6/30/2010";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 30));
        }

        [Fact]
        public void test_line_matches_shipping_date_mddyy_short()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping 6/30/10";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 30));
        }

        [Fact]
        public void test_line_matches_shipping_date_nextweek_long()
        {
            // Arrange
            var parser = new ScheduleParser(null, null);
            const string line = "Shipping Next Week:  June 30, 2010";

            // Act
            DateTime shippingDate = parser.ShippingDate(line);

            //Assert
            Assert.False(shippingDate == DateTime.MinValue);
            Assert.True(shippingDate == new DateTime(2010, 6, 30));
        }
    }
}
