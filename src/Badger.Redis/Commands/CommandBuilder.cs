using Badger.Redis.DataTypes;
using System.Collections.Generic;
using System.Linq;
using Array = Badger.Redis.DataTypes.Array;

namespace Badger.Redis.Commands
{
    internal class CommandBuilder
    {
        private string _command = "";
        private List<IDataType> _args = new List<IDataType>();

        public CommandBuilder WithCommand(string command)
        {
            _command = command;
            return this;
        }

        public CommandBuilder WithArg(IDataType arg)
        {
            _args.Add(arg);
            return this;
        }

        public CommandBuilder WithArg(string arg)
        {
            _args.Add(BulkString.FromString(arg));
            return this;
        }

        public IDataType Build()
        {
            return new Array(new[] { BulkString.FromString(_command) }.Concat(_args).ToArray());
        }
    }
}