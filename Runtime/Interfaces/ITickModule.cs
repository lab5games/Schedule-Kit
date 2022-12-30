
namespace Lab5Games.Schedules
{
    public interface ITickModule
    {
        int order { get; }

        void Tick(float deltaTime);
    }
}
