using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Caching
{
    public interface ISignals : IVolatileProvider
    {
        void Trigger<T>(T signal);

        void Trigger(string signal, params object[] args);

        IVolatileToken When<T>(T signal);

        IVolatileToken When(string signal, params object[] args);
    }

    [Feature(Constants.Areas.Core)]
    public class Signals : ISignals
    {
        private readonly IDictionary<object, Token> tokens = new Dictionary<object, Token>();

        public virtual void Trigger<T>(T signal)
        {
            lock (tokens)
            {
                Token token;
                if (tokens.TryGetValue(signal, out token))
                {
                    tokens.Remove(signal);
                    token.Trigger();
                }
            }
        }

        public virtual void Trigger(string signal, params object[] args)
        {
            lock (tokens)
            {
                Token token;
                if (tokens.TryGetValue(signal, out token))
                {
                    tokens.Remove(signal);
                    token.Trigger();
                }

                if (args != null && args.Length > 0)
                {
                    var signalWithArgs = signal + "_" + string.Join("_", args);

                    Token tokenWithArgs;
                    if (tokens.TryGetValue(signalWithArgs, out tokenWithArgs))
                    {
                        tokens.Remove(signalWithArgs);
                        tokenWithArgs.Trigger();
                    }
                }
            }
        }

        public IVolatileToken When<T>(T signal)
        {
            lock (tokens)
            {
                Token token;
                if (!tokens.TryGetValue(signal, out token))
                {
                    token = new Token();
                    tokens[signal] = token;
                }
                return token;
            }
        }

        public IVolatileToken When(string signal, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return When<string>(signal);
            }

            lock (tokens)
            {
                var signalWithArgs = signal + "_" + string.Join("_", args);

                Token token;
                if (!tokens.TryGetValue(signalWithArgs, out token))
                {
                    token = new Token();
                    tokens[signalWithArgs] = token;
                }
                return token;
            }
        }

        private class Token : IVolatileToken
        {
            public Token()
            {
                IsCurrent = true;
            }

            public bool IsCurrent { get; private set; }

            public void Trigger()
            {
                IsCurrent = false;
            }
        }
    }
}