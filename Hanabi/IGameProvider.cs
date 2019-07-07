using System.Collections.Generic;

namespace Hanabi
{
    public interface IGameProvider
    {
        Matrix CreateEmptyMatrix();
        Matrix CreateFullDeckMatrix();

        IReadOnlyList<Color> Colors { get; }
        IReadOnlyList<Rank> Ranks { get; }

        int ColorToInt(Color color);
        int GetMaximumScore();
    }
}
