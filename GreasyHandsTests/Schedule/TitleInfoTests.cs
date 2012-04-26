using System;
using GreasyHands.Schedule;
using Xunit;

namespace GreasyHandsTests.Schedule
{
    public class TitleInfoTests
    {
        private readonly Func<TitleInfo> titleInfoFunc;

        public TitleInfoTests()
        {
            titleInfoFunc = () => new TitleInfo();
        }

        [Fact]
        public void test_title_removes_code_and_sets_code_and_title()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);

            // Act
            var result = parser.BuildTitleInfo("NOV110019\tBOMB QUEEN VII #1 (OF 4) (MR)", null, DateTime.Now);

            //Assert
            Assert.Equal("Bomb Queen VII", result.Name);
            Assert.Equal("NOV110019", result.Code);
        }

        [Fact]
        public void test_title_is_mature_and_sets_mature_flag_true()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);

            // Act
            var result = parser.BuildTitleInfo("NOV110019\tBOMB QUEEN VII #1 (OF 4) (MR)", null, DateTime.Now);

            //Assert
            Assert.True(result.Mature);
        }

        [Fact]
        public void test_title_is_not_mature_and_sets_mature_flag_false()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);

            // Act
            var result = parser.BuildTitleInfo("NOV110019\tBOMB QUEEN VII #1 (OF 4)", null, DateTime.Now);

            //Assert
            Assert.False(result.Mature);
        }

        [Fact]
        public void test_title_uppercase_roman_numeral()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);

            // Act
            var result = parser.BuildTitleInfo("NOV110019\tBOMB QUEEN VII #1 (OF 4)", null, DateTime.Now);

            //Assert
            Assert.Equal("Bomb Queen VII", result.Name);
        }

        [Fact]
        public void test_title_is_oneshot_and_sets_oneshot_flag_true()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);

            // Act
            var result = parser.BuildTitleInfo("NOV110019\tBOMB QUEEN VII #1 (ONE SHOT)", null, DateTime.Now);

            //Assert
            Assert.True(result.OneShot);
        }

        [Fact]
        public void test_title_is_not_oneshot_and_sets_oneshot_flag_false()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);

            // Act
            var result = parser.BuildTitleInfo("NOV110019\tBOMB QUEEN VII #1 (OF 4)", null, DateTime.Now);

            //Assert
            Assert.False(result.OneShot);
        }

        [Fact]
        public void test_continuing_title_parses_name_num()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);

            // Act
            var result = parser.BuildTitleInfo("NOV110019\tDC COMICS PRESENTS SUPERMAN SECRET IDENTITY #2", null, DateTime.Now);

            //Assert
            Assert.Equal(result.Name, "DC Comics Presents Superman Secret Identity");
            Assert.Equal(result.Num, 2);
        }

        [Fact]
        public void test_lmited_title_parses_name_num_limited()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);

            // Act
            var result = parser.BuildTitleInfo("NOV110019\tMOUSE GUARD BLACK AXE #3 (OF 6)", null, DateTime.Now);

            //Assert
            Assert.Equal(result.Name, "Mouse Guard Black Axe");
            Assert.Equal(result.Num, 3);
            Assert.Equal(result.Limited, 6);
        }
    }
}
