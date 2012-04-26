using System;
using Xunit;
using GreasyHands.Schedule;

namespace GreasyHandsTests.Schedule
{
    
    public class TitleParserTests
    {
        private readonly Func<TitleInfo> titleInfoFunc;

        public TitleParserTests()
        {
            titleInfoFunc = () => new TitleInfo();
        }

        [Fact]
        public void test_line_matches_known_publisher()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);

            // Act
            bool result = parser.IsPublisher("MARVEL");
            
            //Assert
            Assert.True(result);
        }

        [Fact]
        public void test_line_matches_unknown_publisher()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);

            // Act
            bool result = parser.IsPublisher("Blah");

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void test_line_matches_valid_title()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);

            // Act
            bool result = parser.IsPublisher("Blah");

            //Assert
            Assert.False(result);            
        }

        [Fact]
        public void test_matchremove_matches_cleans_and_returns_true()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);
            var title = "Test String";

            // Act
            bool result = parser.MatchRemove(ref title, " String");

            //Assert
            Assert.True(result);
            Assert.Equal(title, "Test");
        }

        [Fact]
        public void test_matchremove_nomatch_nochange_and_returns_false()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);
            var title = "Test String";

            // Act
            bool result = parser.MatchRemove(ref title, "nomatch");

            //Assert
            Assert.False(result);
            Assert.Equal(title, "Test String");
        }

        [Fact]
        public void test_title_matches_bad_title_pp_number()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);
            const string title = "STAR TREK ONGOING #2 2ND PTG (PP #994)";

            // Act
            bool result = parser.IsBadTitle(title);

            //Assert
            Assert.True(result); 
        }

        [Fact]
        public void test_title_matches_bad_title_df()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);
            const string title = "DF STAR TREK ONGOING #2 2ND PTG (PP #994)";

            // Act
            bool result = parser.IsBadTitle(title);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void test_title_matches_limited_title()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);
            const string title = "BOMB QUEEN VII #1 (OF 4) (MR)";

            // Act
            bool continuing = parser.IsContinuingTitle(title);
            bool limited = parser.IsLimitedTitle(title);

            //Assert
            Assert.True(limited);
            Assert.False(continuing);
        }

        [Fact]
        public void test_title_matches_continuing_title()
        {
            // Arrange
            var parser = new TitleParser(titleInfoFunc);
            const string title = "GREEN WAKE #7 (MR)";

            // Act
            bool continuing = parser.IsContinuingTitle(title);
            bool limited = parser.IsLimitedTitle(title);

            //Assert
            Assert.True(continuing);
            Assert.False(limited);
        }        
    }
}
