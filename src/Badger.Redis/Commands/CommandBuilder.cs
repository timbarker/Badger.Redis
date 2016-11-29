using Badger.Redis.Types;
using System.Collections.Generic;
using System.Linq;

namespace Badger.Redis.Commands
{
    internal class CommandBuilder
    {
        private string _command = "";
        private readonly List<IRedisType> _args = new List<IRedisType>();

        public CommandBuilder WithCommand(string command)
        {
            _command = command;
            return this;
        }

        public CommandBuilder WithArg(IRedisType arg)
        {
            _args.Add(arg);
            return this;
        }

        public CommandBuilder WithArg(string arg)
        {
            _args.Add(RedisBulkString.FromString(arg));
            return this;
        }

        public IRedisType Build()
        {
            return new RedisArray(new[] { RedisBulkString.FromString(_command) }.Concat(_args).ToArray());
        }
    }
}