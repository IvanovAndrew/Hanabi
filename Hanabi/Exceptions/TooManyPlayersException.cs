namespace Hanabi.Exceptions
{
    public class TooManyPlayersException : WrongPlayerCountException
    {
        public TooManyPlayersException() : base("Too many players")
        {

        }
    }
}
