using System.Text;

namespace Hanabi
{
    public class LogClueVisitor : IClueVisitor
    {
        readonly StringBuilder _stringBuilder = new StringBuilder();

        public bool Visit(ClueAboutRank clueAboutRank)
        {
            if (_stringBuilder.Length > 0) _stringBuilder.Append(", ");
            _stringBuilder.Append(clueAboutRank.Rank);
            return true;
        }

        public bool Visit(ClueAboutNotRank clueAboutNotRank)
        {
            if (_stringBuilder.Length > 0) _stringBuilder.Append(", ");
            _stringBuilder.Append("Not " + clueAboutNotRank.Rank);
            return true;
        }

        public bool Visit(ClueAboutColor clueAboutColor)
        {
            if (_stringBuilder.Length > 0) _stringBuilder.Append(", ");
            _stringBuilder.Append(clueAboutColor.Color);
            return true;
        }

        public bool Visit(ClueAboutNotColor clueAboutNotColor)
        {
            if (_stringBuilder.Length > 0) _stringBuilder.Append(", ");
            _stringBuilder.Append("Not " + clueAboutNotColor.Color);
            return true;
        }

        public string Build()
        {
            return _stringBuilder.ToString();
        }
    }
}
