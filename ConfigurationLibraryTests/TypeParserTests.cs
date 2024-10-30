using ConfigurationLibrary.Common;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace ConfigurationLibraryTests
{
    [TestFixture]
    public class TypeParserTests
    {
        private TypeParser _typeParser;

        [SetUp]
        public void Setup()
        {
            _typeParser = new TypeParser();
        }

        [Test]
        public void Parse_ShouldThrowInvalidCastException_WhenTypeIsNotSupported()
        {
            // Arrange
            string value = "Test";

            // Act
            Action act = () => _typeParser.Parse<DateTime>(value);

            // Assert
            act.Should().Throw<InvalidCastException>()
               .WithMessage("Unsupported type: System.DateTime");
        }

        [TestCase("123", typeof(int), 123)]
        [TestCase("123.45", typeof(double), 123.45)]
        [TestCase("true", typeof(bool), true)]
        [TestCase("False", typeof(bool), false)]
        [TestCase("test string", typeof(string), "test string")]
        public void Parse_ShouldReturnValue_WhenValueIsGivenType(string value, Type targetType, object expected)
        {
            // Act
            var result = InvokeParse(value, targetType);

            // Assert
            result.Should().Be(expected);
        }

        [TestCase("Test", typeof(int))]
        [TestCase("Test", typeof(double))]
        [TestCase("Test", typeof(bool))]
        public void Parse_ShouldThrowInvalidCastException_WhenValueCannotBeParsedToTargetType(string value, Type targetType)
        {
            // Act
            Action act = () => InvokeParse(value, targetType);

            // Assert
            act.Should().Throw<InvalidCastException>()
               .WithMessage($"Unable to parse '{value}' to type {targetType}");
        }

        private object InvokeParse(string value, Type targetType)
        {
            if (targetType == typeof(int))
            {
                return _typeParser.Parse<int>(value);
            }
            else if (targetType == typeof(double))
            {
                return _typeParser.Parse<double>(value);
            }
            else if (targetType == typeof(bool))
            {
                return _typeParser.Parse<bool>(value);
            }
            else
            {
                return _typeParser.Parse<string>(value);
            }

        }
    }
}
