using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Dynamitey;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Trampoline object to give access to methods as Commands of original viewmodal
    /// </summary>
    public class ImpromptuCommandBinder : DynamicObject
    {
        private readonly object _parent;

        private readonly Dictionary<string, ImpromptuRelayCommand> _commands =
            new Dictionary<string, ImpromptuRelayCommand>();

        private readonly ISetupViewModel _setup;

        internal ImpromptuCommandBinder(object viewModel, ISetupViewModel setup = null)
        {
            _parent = viewModel;
            _setup = setup;
        }


#if SILVERLIGHT5

        /// <summary>
        /// Gets the custom Type.
        /// </summary>
        /// <returns></returns>
        public Type GetCustomType()
        {
            return this.GetDynamicCustomType();
        }
#endif

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this[binder.Name];
            return true;
        }

        /// <summary>
        /// Gets the <see cref="ImpromptuInterface.MVVM.ImpromptuRelayCommand"/> with the specified key.
        /// </summary>
        /// <value></value>
        public ImpromptuRelayCommand this[String key]
        {
            get
            {
                ImpromptuRelayCommand result;

                if (!_commands.TryGetValue(key, out result))
                {

                    var tCanExecute = $"Can{key}";
                    var members = Dynamic.GetMemberNames(_parent);
                    if (members.Contains(tCanExecute))
                    {
                        result = new ImpromptuRelayCommand(_parent, key, _parent, tCanExecute,_setup);
                    }
                    else
                    {
                        result = new ImpromptuRelayCommand(_parent, key,_setup);
                    }
                    _commands[key] = result;
                }
                return result;
            }
        }
    }
}