using System;
namespace Hanabi
{
    public class HanabiException : Exception
    {
        public HanabiException(string message) : base(message)
        { }
    }
}
