using Xunit;
using GreasyHands;

namespace GreasyHandsTests
{
    public class ExtensionsTests
    {
        [Fact]
        public void test_fuzzy_title_matches_removed_the()
        {
            // Arrange
            const string left = "The Amazing Spider-Man";
            const string right = "Amazing Spiderman";

            // Act
            var result = left.FuzzyTitle() == right.FuzzyTitle();

            //Assert
            Assert.True(result);            
        }

        [Fact]
        public void test_fuzzy_title_matches_ampersand_and()
        {
            // Arrange
            const string left = "Billy Batson And The Magic Of Shazam";
            const string right = "Billy Batson & The Magic of Shazam!";

            // Act
            var result = left.FuzzyTitle() == right.FuzzyTitle();

            //Assert
            Assert.True(result);
        }
    }
}
