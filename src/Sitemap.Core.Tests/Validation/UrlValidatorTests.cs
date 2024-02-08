﻿using Sitemap.Core.Validation;

namespace Sitemap.Core.Tests.Validation;

public sealed class UrlValidatorTests
{
    [Fact]
    public void Construct_WithInvalidBaseUrl_ThrowException()
    {
        // act
        var action = () => new UrlValidator(new InvalidBaseUrlProvider());

        // assert
        action.Should().ThrowExactly<InvalidUrlException>();
    }

    [Fact]
    public void Construct_WithSchemeInvalidBaseUrl_ThrowException()
    {
        // act
        var action = () => new UrlValidator(new InvalidSchemeBaseUrlProvider());

        // assert
        action.Should().ThrowExactly<InvalidUrlException>().WithMessage("*https*");
    }

    [Fact]
    public void Validate_WithAbsoluteUrl_ReturnsUri()
    {
        // arrange
        const string Url = "https://example.com/sitemap.xml";
        var validator = new UrlValidator();

        // act
        var result = validator.Validate(Url);

        // assert
        result.ToString().Should().Be(Url);
    }

    [Theory]
    [InlineData("/sitemap.xml")]
    [InlineData("sitemap.xml")]
    public void Validate_WithRelativeUrl_ReturnsUri(string url)
    {
        // arrange
        var validator = new UrlValidator(new TestBaseUrlProvider());

        // act
        var result = validator.Validate(url);

        // assert
        result.ToString().Should().Be("https://example.com/sitemap.xml");
    }

    [Fact]
    public void Validate_WithRelativeUrlAndEmptyBaseUrl_ThrowException()
    {
        // arrange
        const string Url = "sitemap.xml";
        var validator = new UrlValidator();

        // act
        var action = () => validator.Validate(Url);

        // assert
        action.Should().ThrowExactly<InvalidUrlException>();
    }

    [Fact]
    public void Test()
    {
        var url = "/page.html";
        var relativeUri = new Uri(url, UriKind.Relative);

        var baseUrl = new Uri("https://example.com", UriKind.Absolute);

        var result = new Uri(baseUrl, relativeUri);

        result.ToString().Should().Be("https://example.com/page.html");
    }

    private sealed class TestBaseUrlProvider : IBaseUrlProvider
    {
        public Uri BaseUrl => new ("https://example.com", UriKind.Absolute);
    }

    private sealed class InvalidBaseUrlProvider : IBaseUrlProvider
    {
        public Uri BaseUrl => new("/example", UriKind.Relative);
    }

    private sealed class InvalidSchemeBaseUrlProvider : IBaseUrlProvider
    {
        public Uri BaseUrl => new("ftp://example.com", UriKind.Absolute);
    }
}