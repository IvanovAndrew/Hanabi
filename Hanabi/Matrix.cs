﻿using System;
using System.Diagnostics.Contracts;

namespace Hanabi
{
    public class Matrix
    {
        private readonly int[,] _matrix;
        private readonly IGameProvider _gameProvider;

        public Matrix(IGameProvider provider)
        {
            _gameProvider = provider;
            _matrix = new int[provider.Numbers.Count, provider.Colors.Count];
        }

        public int this[Number number, Color color]
        {
            get { return _matrix[(int) number, _gameProvider.ColorToInt(color)]; }
            set { _matrix[(int)number, _gameProvider.ColorToInt(color)] = value; } 
        }

        public int this[Card card]
        {
            get
            {
                Contract.Requires<ArgumentNullException>(card != null);

                return this[card.Nominal, card.Color];
            }
            set
            {
                Contract.Requires<ArgumentNullException>(card != null);

                this[card.Nominal, card.Color] = value;
            }
        }
    }
}
