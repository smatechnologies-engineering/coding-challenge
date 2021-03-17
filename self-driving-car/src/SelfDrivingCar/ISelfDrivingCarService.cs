using SelfDrivingCar.Entities;

namespace SelfDrivingCar
{
    public interface ISelfDrivingCarService
    {
        Token Register(TokenRequest request);

        Car GetCar();

        Road GetRoad();

        CarAction DoAction(CarAction action);
    }
}
