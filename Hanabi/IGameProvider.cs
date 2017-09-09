﻿using System.Collections.Generic;

namespace Hanabi
{
    public interface IGameProvider
    {
        Matrix CreateEmptyMatrix();
        Matrix CreateFullDeckMatrix();

        IReadOnlyList<Color> Colors { get; }
        IReadOnlyList<Nominal> Nominals { get; }

        int ColorToInt(Color color);
        int GetMaximumScore();
    }
}
