using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine
{
    internal sealed class Capabilities<T>
    {
        private readonly T _options;
        private readonly IEnumerable<Tuple<MemberInfo, object>> _attributes; 
        private readonly Lazy<bool> _anyVerbs;
        private readonly Lazy<bool> _hasVerbHelp;
        private readonly Lazy<bool> _hasHelp;
        private readonly Lazy<bool> _hasParserState; 

        public Capabilities(T options)
        {
            _options = options;

            _attributes = from member in _options.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                          let attributes = member.GetCustomAttributes(true)
                          from attribute in attributes
                          where attribute is ITargetDescriptor
                          select new Tuple<MemberInfo, object>(member, attribute);

            this._anyVerbs = new Lazy<bool>(() => _attributes.Any(a => a.Item2 is VerbOptionAttribute));
            _hasVerbHelp = new Lazy<bool>(() => _attributes.Count(a => a.Item2 is HelpVerbOptionAttribute) == 1);
            _hasHelp = new Lazy<bool>(() => _attributes.Count(a => a.Item2 is HelpOptionAttribute) == 1);
            _hasParserState = new Lazy<bool>(() => _attributes.Count(a => a.Item2 is ParserStateAttribute) == 1);
        }

        public bool AnyVerbs
        {
            get { return _anyVerbs.Value; }
        }

        public bool HasVerbHelp
        {
            get { return _hasVerbHelp.Value; }
        }

        public bool HasHelp
        {
            get { return _hasHelp.Value; }
        }

        public bool HasParserState
        {
            get { return _hasParserState.Value; }
        }
    }
}