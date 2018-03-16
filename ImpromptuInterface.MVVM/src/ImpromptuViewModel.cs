// 
//  Copyright 2011 Ekon Benefits
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Dynamitey.DynamicObjects;



namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// View Model that uses a Dynamic Implementation to remove boilerplate for Two-Way bound properties and commands to methods. 
    /// If you specific a TInterface it provides a guide to the dynamic properties
    /// </summary>
    /// <typeparam name="TInterfaceContract">The type of the interface.</typeparam>
    [Serializable]
    public class ImpromptuViewModel<TInterfaceContract> : ImpromptuViewModel where TInterfaceContract : class 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuViewModel&lt;TInterface&gt;"/> class.
        /// </summary>
        public ImpromptuViewModel()
        {
            _contract = this.ActLike<TInterfaceContract>(typeof(INotifyPropertyChanged));
     
        }


        private readonly TInterfaceContract _contract;

        /// <summary>
        /// Convenient access to Dynamic Properties but represented by a Static Interface.
        ///  When subclassing you can use Static.PropertyName = x, etc
        /// </summary>
        /// <value>The static.</value>
        [Obsolete("Use Contract Property instead")]
        public TInterfaceContract Static => _contract;

        /// <summary>
        /// Convenient access to Dynamic Properties but represented by a Static Interface.
        ///  When subclassing you can use Contract.PropertyName = x, etc
        /// </summary>
        /// <value>The contract interface.</value>
        public TInterfaceContract Contract => _contract;
    }


    /// <summary>
    /// View Model that uses a Dynamic Implementation to remove boilerplate for Two-Way bound properties and commands to methods
    /// </summary>
    [Serializable]
    public partial class ImpromptuViewModel:BaseDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuViewModel"/> class.
        /// </summary>
        public ImpromptuViewModel()
        {
            LinkedProperties = new Dictionary<string, List<string>>();
        }

        private ImpromptuCommandBinder _commandTrampoline;

        [Obsolete]
        private PropertyDepends _dependencyTrampoline;


        protected readonly IDictionary<string, List<string>> LinkedProperties;
   
        private ISetupViewModel _setup;

        /// <summary>
        /// Convenient access to Dynamic Properties. When subclassing you can use Dynamic.PropertyName = x, etc.
        /// </summary>
        /// <value>The command.</value>
        protected dynamic Dynamic => this;

        /// <summary>
        /// Gets the command for binding. usage: {Binding Command.MethodName} for <code>void MethodName(object parmeter)</code> and optionally <code>bool CanMethodName(object parameter)</code>.
        /// </summary>
        /// <value>The command.</value>
        public virtual dynamic Command => _commandTrampoline 
                                          ?? (_commandTrampoline = new ImpromptuCommandBinder(this, Setup));

        /// <summary>
        /// Gets the EventBinder to bind events to this model.
        /// </summary>
        /// <value>The events.</value>
        public virtual dynamic Events => new EventBinder(this);

        /// <summary>
        /// Locates the View for this ViewModel
        /// NOTE: this hits the IoC container every time, this enables non-singleton views
        /// </summary>
        public virtual dynamic View
        {
            get
            {
                dynamic view = IoC.GetViewFor(this);
                view.DataContext = Dynamic;
                return view;
            }
        }

        /// <summary>
        /// Sets up dependency relations amoung dependenant properties
        /// </summary>
        /// <value>The dependencies.</value>
        [Obsolete("Use Setup.Property instead")]
        public dynamic Dependencies => _dependencyTrampoline 
                                       ?? (_dependencyTrampoline = new PropertyDepends(this));


        /// <summary>
        /// Has Properties to configure view model at setup
        /// </summary>
        /// <value>The setup.</value>
        public ISetupViewModel Setup => _setup ?? (_setup = new SetupTrampoline(this));


        /// <summary>
        /// Properties the changed.
        /// </summary>
        /// <param name="delegate">The @delegate.</param>
        /// <returns></returns>
        public static PropertyChangedEventHandler ChangedHandler(PropertyChangedEventHandler @delegate)
        {
            return @delegate;
        }

        /// <summary>
        /// Subscribe to OnProeprtyChanged notififcations of specific properties
        /// </summary>
        /// <value>The on changed.</value>
        [Obsolete("Use Setup.Property instead")]
        public dynamic OnChanged => (Setup as SetupTrampoline)?.OnChangedTrampoline;

        /// <summary>
        /// Links a property to a dependency.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="dependency">To.</param>
        public void DependencyLink(string property, string dependency)
        {
            if(!LinkedProperties.TryGetValue(dependency,out var tList))
            {
                tList = new List<string>();
                LinkedProperties[dependency] = tList;
            }
            if(!tList.Contains(property))
                tList.Add(property);
        }

        /// <summary>
        /// Unlinks a dependencies.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="dependency">The dependency.</param>
        public void DependencyUnlink(string property, string dependency)
        {
            if (LinkedProperties.TryGetValue(dependency, out var tList))
            {
                tList.Remove(property);
            }
        }


        protected virtual void OnPropertyChanged(string key, HashSet<string> alreadyRaised)
        {
            if (alreadyRaised.Contains(key)) return;
            
            base.OnPropertyChanged(key);

            alreadyRaised.Add(key);

            if (!LinkedProperties.TryGetValue(key, out var tList)) return;
            foreach (var tKey in tList.Distinct())
            {
                OnPropertyChanged(tKey, alreadyRaised);
            }
        }


   

    

        protected override void OnPropertyChanged(string key)
        {
            OnPropertyChanged(key, new HashSet<string>());
        }

        #region Trampoline Classes

      


        #endregion
    }
}
