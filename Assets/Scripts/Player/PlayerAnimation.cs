public class PlayerAnimation
{
    public static Movement movement = new Movement();
    public class Movement
    {
        public string Idle = "Idle";
        public string WalkFront = "Walk Forward";
        public string WalkBack = "Walk Backward";
        public string WalkLeft = "Walk Left";
        public string WalkRight = "Walk Right";
        public string Run = "Run";
        public string JumpStart = "Jump Start";
        public string JumpAir = "Jump Air";
        public string JumpEnd = "Jump End";
    }
}