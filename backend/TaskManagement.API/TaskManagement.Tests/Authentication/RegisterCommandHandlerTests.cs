using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Authentication.Commands.Register;
using TaskManagement.Domain.Identity;
using Microsoft.Extensions.Logging;


namespace TaskManagement.Tests.Authentication;


public class RegisterCommandHandlerTests
{

    private readonly Mock<IEmailService> _emailMock;


    public RegisterCommandHandlerTests()
    {
        _emailMock = new Mock<IEmailService>();
    }

    private Mock<IApplicationUrlService> GetApplicationUrlServiceMock()
    {
        var mock = new Mock<IApplicationUrlService>();
        mock.SetupGet(x => x.FrontendBaseUrl).Returns("http://localhost:5173");
        return mock;
    }

    private Mock<ILogger<RegisterCommandHandler>> GetLoggerMock()
    {
        return new Mock<ILogger<RegisterCommandHandler>>();
    }



    private Mock<UserManager<ApplicationUser>> GetUserManagerMock()
    {
        return new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
    }



    // 1. Duplicate Email Test

    [Fact]
    public async Task Should_Return_Error_When_Email_Exists()
    {

        var userManager = GetUserManagerMock();


        userManager
            .Setup(x => x.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(new ApplicationUser());


        var appUrlMock = GetApplicationUrlServiceMock();
        var loggerMock = GetLoggerMock();

        var handler = new RegisterCommandHandler(
            userManager.Object,
            _emailMock.Object,
            appUrlMock.Object,
            loggerMock.Object);



        var result = await handler.Handle(
            new RegisterCommand
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                Username = "test",
                Password = "Password@123",
                ConfirmPassword = "Password@123"
            },
            CancellationToken.None);



        result.Succeeded.Should().BeFalse();

        result.Message
            .Should()
            .Be("Email already exists.");
    }



    // 2. Duplicate Username Test


    [Fact]
    public async Task Should_Return_Error_When_Username_Exists()
    {

        var userManager = GetUserManagerMock();


        userManager
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser)null);



        userManager
            .Setup(x => x.FindByNameAsync("test"))
            .ReturnsAsync(new ApplicationUser());



        var appUrlMock = GetApplicationUrlServiceMock();
        var loggerMock = GetLoggerMock();

        var handler = new RegisterCommandHandler(
            userManager.Object,
            _emailMock.Object,
            appUrlMock.Object,
            loggerMock.Object);



        var result = await handler.Handle(
            new RegisterCommand
            {
                Email = "test@test.com",
                Username = "test",
                Password = "Password@123"
            },
            CancellationToken.None);



        result.Succeeded.Should().BeFalse();

        result.Message
            .Should()
            .Be("Username already exists.");
    }





    // 3. Identity Create Failed Test


    [Fact]
    public async Task Should_Return_Error_When_User_Creation_Fails()
    {

        var userManager = GetUserManagerMock();


        userManager
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser)null);


        userManager
            .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser)null);



        userManager
            .Setup(x => x.CreateAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<string>()))
            .ReturnsAsync(
                IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Password is invalid"
                    }));



        var appUrlMock = GetApplicationUrlServiceMock();
        var loggerMock = GetLoggerMock();

        var handler = new RegisterCommandHandler(
            userManager.Object,
            _emailMock.Object,
            appUrlMock.Object,
            loggerMock.Object);



        var result = await handler.Handle(
            new RegisterCommand
            {
                Email = "test@test.com",
                Username = "test",
                Password = "Password@123"
            },
            CancellationToken.None);



        result.Succeeded.Should().BeFalse();

        result.Message
            .Should()
            .Contain("Password is invalid");
    }






    // 4. Successful Registration Test


    [Fact]
    public async Task Should_Register_User_Successfully()
    {

        var userManager = GetUserManagerMock();


        userManager
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser)null);


        userManager
            .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser)null);



        userManager
            .Setup(x => x.CreateAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<string>()))
            .ReturnsAsync(
                IdentityResult.Success);



        userManager
            .Setup(x => x.GenerateEmailConfirmationTokenAsync(
                It.IsAny<ApplicationUser>()))
            .ReturnsAsync("token123");



        _emailMock
            .Setup(x => x.SendHtmlAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);



        var appUrlMock = GetApplicationUrlServiceMock();
        var loggerMock = GetLoggerMock();

        var handler = new RegisterCommandHandler(
            userManager.Object,
            _emailMock.Object,
            appUrlMock.Object,
            loggerMock.Object);



        var result = await handler.Handle(
            new RegisterCommand
            {
                FirstName = "Abdul",
                LastName = "Moiz",
                Email = "test@test.com",
                Username = "abdul",
                Password = "Password@123",
                ConfirmPassword = "Password@123"
            },
            CancellationToken.None);



        result.Succeeded.Should().BeTrue();

        result.RequiresEmailVerification
            .Should()
            .BeTrue();
    }

}