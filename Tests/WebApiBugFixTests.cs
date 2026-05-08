using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;
using WebAPI.IServices;

namespace Tests;

public class UsersControllerAddUserDataTests
{
    private readonly Mock<IUserService> mockUserService;
    private readonly UsersController controller;

    public UsersControllerAddUserDataTests()
    {
        this.mockUserService = new Mock<IUserService>();
        this.controller = new UsersController(this.mockUserService.Object);
    }

    [Fact]
    public async Task AddUserData_ShouldCallAddUserDataAsync_NotUpdate()
    {
        var userData = new UserDataDto { UserId = 1 };
        this.mockUserService.Setup(service => service.AddUserDataAsync(userData))
            .Returns(Task.CompletedTask);

        var result = await this.controller.AddUserData(userData);

        this.mockUserService.Verify(
            service => service.AddUserDataAsync(userData), Times.Once);
        this.mockUserService.Verify(
            service => service.UpdateUserDataAsync(It.IsAny<UserDataDto>()), Times.Never);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task AddUserData_ShouldReturnOk_WhenServiceSucceeds()
    {
        var userData = new UserDataDto { UserId = 42 };
        this.mockUserService.Setup(service => service.AddUserDataAsync(userData))
            .Returns(Task.CompletedTask);

        var result = await this.controller.AddUserData(userData);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateUserData_ShouldCallUpdateUserDataAsync()
    {
        var userData = new UserDataDto { UserId = 1 };
        this.mockUserService.Setup(service => service.UpdateUserDataAsync(userData))
            .Returns(Task.CompletedTask);

        var result = await this.controller.UpdateUserData(userData);

        this.mockUserService.Verify(
            service => service.UpdateUserDataAsync(userData), Times.Once);
        Assert.IsType<OkResult>(result);
    }
}
