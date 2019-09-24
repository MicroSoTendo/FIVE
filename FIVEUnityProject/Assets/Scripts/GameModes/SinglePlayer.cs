using FIVE.Robot;

namespace FIVE.GameStates
{
    public class SinglePlayer : GameMode
    {
        void Start()
        {
            RobotManager.CreateRobot();
        }
    }
}
