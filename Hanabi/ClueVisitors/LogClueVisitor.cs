using System.Text;

namespace Hanabi
{
    public class LogClueVisitor : IClueVisitor
    {
        readonly StringBuilder _stringBuilder = new StringBuilder();

        public bool Visit(ClueAboutNominal clue)
        {
            if (_stringBuilder.Length > 0) _stringBuilder.Append(", ");
            _stringBuilder.Append(clue.Nominal);
            return true;
        }

        public bool Visit(ClueAboutNotNominal clue)
        {
            if (_stringBuilder.Length > 0) _stringBuilder.Append(", ");
            _stringBuilder.Append("Not " + clue.Nominal);
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
