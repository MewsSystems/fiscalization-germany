﻿namespace Mews.Fiscalization.Germany.Model
{
    public sealed class Signature
    {
        public Signature(string value, int counter, string algorithm, string publicKey)
        {
            Value = value;
            Counter = counter;
            Algorithm = algorithm;
            PublicKey = publicKey;
        }

        public string Value { get; }

        public int Counter { get; }

        public string Algorithm { get; }

        public string PublicKey { get; }
    }
}
