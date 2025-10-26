using ConfeccionesAlba_Api.Utils;
using AwesomeAssertions;

namespace ConfeccionesAlbaApiTests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void StringExtensions_GetSha256Hash_ReturnsCorrectHashForEmptyString()
        {
            // Arrange
            var input = string.Empty;

            // Act
            var result = input.GetSha256Hash();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Be("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
        }

        [Test]
        public void StringExtensions_GetSha256Hash_ReturnsCorrectHashForTestString()
        {
            // Arrange
            const string input = "Hello, World!";

            // Act
            var result = input.GetSha256Hash();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Be("dffd6021bb2bd5b0af676290809ec3a53191dd81c7f70a4b28688a362182986f");
        }

        [Test]
        public void StringExtensions_GetSha256Hash_ReturnsDifferentHashForDifferentStrings()
        {
            // Arrange
            const string input1 = "Hello, World!";
            const string input2 = "Hello, World! ";

            // Act
            var result1 = input1.GetSha256Hash();
            var result2 = input2.GetSha256Hash();

            // Assert
            result1.Should().NotBeNullOrEmpty();
            result2.Should().NotBeNullOrEmpty();
            result1.Should().NotBe(result2);
        }

        [Test]
        public void StringExtensions_GetSha256Hash_ReturnsSameHashForSameString()
        {
            // Arrange
            const string input = "Test input";

            // Act
            var result1 = input.GetSha256Hash();
            var result2 = input.GetSha256Hash();

            // Assert
            result1.Should().NotBeNullOrEmpty();
            result2.Should().NotBeNullOrEmpty();
            result1.Should().Be(result2);
        }

        [Test]
        public void StringExtensions_GetSha256Hash_ReturnsConsistentHashLength()
        {
            // Arrange
            const string input1 = "A";
            const string input2 = "A very long string with lots of characters";

            // Act
            var result1 = input1.GetSha256Hash();
            var result2 = input2.GetSha256Hash();

            // Assert
            result1.Should().HaveLength(64); // SHA-256 should always be 64 characters
            result2.Should().HaveLength(64);
        }

        [Test]
        public void StringExtensions_GetSha256Hash_HandlesNullInput()
        {
            // Arrange
            string input = null!;

            // Act & Assert
            Action act = () => input.GetSha256Hash();
            act.Should().Throw<ArgumentNullException>();
        }
    }
}