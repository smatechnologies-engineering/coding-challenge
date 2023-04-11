using ElevatorApp;

public class ElevatorControllerTests
{
    private readonly ElevatorController _controller;

    public ElevatorControllerTests()
    {
        _controller = new ElevatorController();
    }

    [Fact]
    public void AddFloorRequest_RequestAdded_ShouldLog()
    {
        // Arrange
        var expectedLog = $"[{DateTime.Now}] Floor 3 Up request added.";

        // Act
        _controller.AddFloorRequest(3, ElevatorDirection.Up);

        // Assert
        Assert.Contains(expectedLog, File.ReadAllText(_controller._logFilePath));
    }

    [Fact]
    public void AddInsideRequest_RequestAdded_ShouldLog()
    {
        // Arrange
        var expectedLog = $"[{DateTime.Now}] Inside request for floor 4 added.";

        // Act
        _controller.AddInsideRequest(4);

        // Assert
        Assert.Contains(expectedLog, File.ReadAllText(_controller._logFilePath));
    }

    [Fact]
    public void MoveElevatorToFloor_NoRequests_ShouldStopAtTheSameFloor()
    {
        // Arrange
        var initialFloor = _controller._sensor.CurrentFloor;
        var expectedFloor = initialFloor;
        var expectedState = ElevatorState.Stopped;
        var expectedLog = $"[{DateTime.Now}] Stopped at floor {initialFloor}.";

        // Act
        _controller.MoveElevatorToFloor(initialFloor);

        // Assert
        Assert.Equal(expectedFloor, _controller._sensor.CurrentFloor);
        Assert.Equal(expectedState, _controller._sensor.State);
        Assert.Contains(expectedLog, File.ReadAllText(_controller._logFilePath));
    }
}
