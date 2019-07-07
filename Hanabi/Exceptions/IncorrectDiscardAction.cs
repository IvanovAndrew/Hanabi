namespace Hanabi.Exceptions
{
    public class IncorrectDiscardActionException : HanabiException
    {
        public IncorrectDiscardActionException() : base("You can discard only own card")
        {

        }
    }
}
