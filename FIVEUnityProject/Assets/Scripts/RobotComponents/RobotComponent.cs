using Mirror;

namespace FIVE.RobotComponents
{
    public class RobotComponent : NetworkBehaviour
    {
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            base.OnDeserialize(reader, initialState);
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            return base.OnSerialize(writer, initialState);
        }
    }
}
