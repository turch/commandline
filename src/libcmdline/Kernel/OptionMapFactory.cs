using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using CommandLine.Extensions;

namespace CommandLine.Kernel
{
    internal class OptionMapFactory<T>
    {
        private readonly ParserSettings _settings;

        public OptionMapFactory(
            ParserSettings settings)
        {
            _settings = settings;
        }

        //public OptionMap CreateOptionMap(T options)
        //{
        //    var list = new OptionPropertyQuery().SelectProperties(options.GetType());

        //    var map = new OptionMap(_settings);

        //    foreach (OptionProperty prop in list)
        //    {
        //        map[prop.UniqueName] = prop;
        //    }

        //    map.RawOptions = options;
        //    return map;
        //}

        //public OptionMap CreateVerbOptionMap(T options)
        //{
        //    var verbs = new VerbOptionPropertyQuery().SelectProperties(options.GetType());

        //    var map = new OptionMap(_settings);

        //    var guard = new ThrowingVerbOptionParameterLessCtorGuard();

        //    foreach (OptionProperty verb in verbs)
        //    {
        //        guard.Execute(verb);

        //        map[verb.UniqueName] = verb;
        //    }

        //    map.RawOptions = options;
        //    return map;
        //}

        public OptionMap Create(
            T options,
            IOptionPropertyGuard guard,
            IEnumerable<IProperty> properties)
        {
            var map = new OptionMap(_settings);

            foreach (OptionProperty prop in properties)
            {
                guard.Execute(prop);

                map[prop.UniqueName] = prop;
            }

            map.RawOptions = options;
            return map;
        }
    }
}
