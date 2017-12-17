using System.Text;

namespace Hanabi
{
    public class LogClueVisitor : IClueVisitor
    {
        readonly StringBuilder _stringBuilder = new StringBuilder();

        public bool Visit(ClueAboutRank clue)
        {
            if (_stringBuilder.Length > 0) _stringBuilder.Append(", ");
            _stringBuilder.Append(clue.Rank);
            return true;
        }

        public bool Visit(ClueAboutNotRank clue)
        {
            if (_stringBuilder.Length > 0) _stringBuilder.Append(", ");
            _stringBuilder.Append("Not " + clue.Rank);
            return true;
        }

        public bool Visit(ClueAboutColor clue)
        {
            if (_stringBuilder.Length > 0) _stringBuilder.Append(", ");
            _stringBuilder.Append(clue.Color);
            return true;
        }

        public bool Visit(ClueAboutNotColor clue)
        {
            if (_stringBuilder.Length > 0) _stringBuilder.Append(", ");
            _stringBuilder.Append("Not " + clue.Color);
            return true;
        }

        public string Build()
        {
            return _stringBuilder.ToString();
        }
    }
}
