using GreasyHands.DAL.Container;
using GreasyHands.Search;
using GreasyHands.Search.Matcher;
using Xunit;

namespace GreasyHandsTests.Search
{
    public class SearchMatcherTests
    {
        [Fact]
        public void test_title_in_filename_matches_query_title_and_number()
        {
            // Arrange
            var query = new Query {Num = 1, Title = "Action Comics"};

            const string filename = "Action Comics 01 (2011) (two covers) (Minutemen-DTs).cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.None, MatchTitle.Exact);

            //Assert
            Assert.True(result);                  
        }

        [Fact]
        public void test_title_in_filename_matches_query_title_but_not_number()
        {
            // Arrange
            var query = new Query {Num = 2, Title = "Action Comics"};

            const string filename = "Action Comics 01 (2011) (two covers) (Minutemen-DTs).cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.None, MatchTitle.Exact);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void test_title_in_filename_matches_query_number_but_not_title()
        {
            // Arrange
            var query = new Query {Num = 1, Title = "Actionish Comics"};

            const string filename = "Action Comics 01 (2011) (two covers) (Minutemen-DTs).cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.None, MatchTitle.Exact);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void test_title_in_filename_matches_neither_query_number_or_title()
        {
            // Arrange
            var query = new Query {Num = 2, Title = "Actionish Comics"};

            const string filename = "Action Comics 01 (2011) (two covers) (Minutemen-DTs).cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.None, MatchTitle.Exact);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void test_title_in_filename_matches_query_title_and_number_and_not_cover_to_cover()
        {
            // Arrange
            var query = new Query { Num = 1, Title = "Action Comics" };

            const string filename = "Action Comics 01 (2011) (two covers) (c2c) (Minutemen-DTs).cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.Ignore, MatchTitle.Exact);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void test_title_in_filename_matches_query_title_and_number_and_cover_to_cover()
        {
            // Arrange
            var query = new Query { Num = 1, Title = "Action Comics" };

            const string filename = "Action Comics 01 (2011) (two covers) (c2c) (Minutemen-DTs).cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.Only, MatchTitle.Exact);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void test_title_in_filename_matches_query_title_and_number_and_not_cover_to_cover_only()
        {
            // Arrange
            var query = new Query { Num = 1, Title = "Action Comics" };

            const string filename = "Action Comics 01 (2011) (two covers) (Minutemen-DTs).cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.Only, MatchTitle.Exact);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void test_title_in_filename_matches_limited_series_with_brackets()
        {
            // Arrange
            var query = new Query { Num = 3, Title = "Villains For Hire" };
            const string filename = "Villains For Hire 3 (of 4) (2012)(FB-DCP)(C2C).cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.None, MatchTitle.Exact);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void test_title_in_filename_matches_limited_series_no_brackets()
        {
            // Arrange
            var query = new Query { Num = 3, Title = "Villains For Hire" };
            const string filename = "Villains For Hire 3 of 4 (2012)(FB-DCP)(C2C).cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.None, MatchTitle.Exact);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void test_title_with_year_match_year()
        {
            // Arrange
            var query = new Query { Num = 3, Title = "Villains For Hire", Year = "2012"};
            const string filename = "Villains For Hire 3 of 4 (2012)(FB-DCP)(C2C).cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.None, MatchTitle.Exact);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void test_title_with_year_not_match_year()
        {
            // Arrange
            var query = new Query { Num = 3, Title = "Villains For Hire", Year = "2011" };
            const string filename = "Villains For Hire 3 of 4 (2012)(FB-DCP)(C2C).cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.None, MatchTitle.Exact);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void test_title_with_no_year_match_year()
        {
            // Arrange
            var query = new Query { Num = 3, Title = "Villains For Hire", Year = "" };
            const string filename = "Villains For Hire 3 of 4 (2012)(FB-DCP)(C2C).cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.None, MatchTitle.Exact);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void test_filename_with_no_year()
        {
            // Arrange
            var query = new Query { Num = 7, Title = "Blue Beetle", Year = "" };
            const string filename = "Blue Beetle 7.cbz";

            var searchMatcher = new SearchMatcher();

            // Act
            var result = searchMatcher.MatchFilename(query, filename, C2CPreference.None, MatchTitle.Exact);

            // Assert
            Assert.True(result);
        }
    }
}
