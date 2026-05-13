using System.Net;
using System.Net.Http;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Moq;
using Moq.Protected;
using WinUI.Services;

namespace Tests;

public sealed class HttpResponseValidationTests
{
    private static HttpClient CreateMockHttpClient(HttpStatusCode statusCode)
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(string.Empty),
            });
        return new HttpClient(handler.Object);
    }

    [Fact]
    public async Task UserServiceProxy_AddUserDataAsync_ThrowsOnServerError()
    {
        var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError);
        var proxy = new UserServiceProxy(httpClient);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => proxy.AddUserDataAsync(new UserDataDto { UserId = 1 }));
    }

    [Fact]
    public async Task UserServiceProxy_AddUserDataAsync_SucceedsOnOk()
    {
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK);
        var proxy = new UserServiceProxy(httpClient);

        await proxy.AddUserDataAsync(new UserDataDto { UserId = 1 });
    }

    [Fact]
    public async Task TrainerDashboardService_SaveWorkoutFeedbackAsync_ThrowsOnServerError()
    {
        var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError);
        var service = new TrainerDashboardService(httpClient);

        var workoutLog = new WorkoutLog
        {
            WorkoutLogId = 1,
            Rating = 4.5,
            TrainerNotes = "Good form",
        };

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.SaveWorkoutFeedbackAsync(workoutLog));
    }

    [Fact]
    public async Task TrainerDashboardService_SaveWorkoutFeedbackAsync_SucceedsOnOk()
    {
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK);
        var service = new TrainerDashboardService(httpClient);

        var workoutLog = new WorkoutLog
        {
            WorkoutLogId = 1,
            Rating = 4.5,
            TrainerNotes = "Good form",
        };

        await service.SaveWorkoutFeedbackAsync(workoutLog);
    }
}
