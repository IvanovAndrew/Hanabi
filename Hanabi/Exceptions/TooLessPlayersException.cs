namespace Hanabi.Exceptions
{
    public class TooLessPlayersException : WrongPlayerCountException
    {
        public TooLessPlayersException() : base("Too less players")
        {

        }
    }
}
