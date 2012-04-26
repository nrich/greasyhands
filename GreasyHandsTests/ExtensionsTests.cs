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
    }
}
