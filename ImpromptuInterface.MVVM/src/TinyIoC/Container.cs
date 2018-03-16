﻿using System;
using System.Collections.Generic;
using Dynamitey.DynamicObjects;

namespace ImpromptuInterface.MVVM.TinyIoC
{
    /// <summary>
    /// An IContainer wrapping a TinyIoC container for ImpromptuInterface.MVVM
    /// </summary>
    public sealed class Container : IContainer
    {
        private readonly dynamic _container;
        private readonly Dictionary<Type, string> _viewLookup = new Dictionary<Type, string>();
        private readonly FluentStringLookup _viewStringLookup;
        private readonly FluentStringLookup _viewModelStringLookup;

        /// <summary>
        /// Default ctor, requires a TinyIoCContainer
        /// </summary>
        /// <param name="container"></param>
        public Container(dynamic container)
        {
            _container = container;
            _viewStringLookup = new FluentStringLookup(GetView);
            _viewModelStringLookup = new FluentStringLookup(GetViewModel);
        }

        /// <summary>
        /// Gets an exported value of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>()
             where T : class
        {
            return _container.Resolve<T>();
        }

        /// <summary>
        /// Gets an exported value by export name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic Get(string name)
        {
            return _container.Resolve(typeof(object), name);
        }

        /// <summary>
        /// Gets a list of exported values of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetMany<T>()
             where T : class
        {
            return _container.ResolveAll<T>();
        }

        /// <summary>
        /// Gets a list of exported values by export name
        /// NOTE: TinyIoC does not support this operation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetMany(string name)
        {
            throw new NotSupportedException("TinyIoC does not support this operation!");
        }

        /// <summary>
        /// Gets a View of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic GetView(string name)
        {
            return Get(name + IoC.View);
        }

        public dynamic View
        {
            get { return _viewStringLookup; }
        }

        /// <summary>
        /// Gets a ViewModel of the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic GetViewModel(string name)
        {
            return Get(name + IoC.ViewModel);
        }

        public dynamic ViewModel
        {
            get { return _viewModelStringLookup; }
        }

        /// <summary>
        /// Gets the View for specified ViewModel
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public dynamic GetViewFor(dynamic viewModel)
        {
            string name;
            if (_viewLookup.TryGetValue(viewModel.GetType(), out name))
            {
                return GetView(name);
            }

            throw new Exception("View not found!");
        }

        /// <summary>
        /// Registers a View/ViewModel pair with the container
        /// </summary>
        /// <param name="name"></param>
        /// <param name="viewType"></param>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        public IContainer AddView(string name, Type viewType, Type viewModelType)
        {
            _container.Register(typeof(object), viewType, name + IoC.View);
            _container.Register(typeof(object), viewModelType, name + IoC.ViewModel);
            _viewLookup[viewModelType] = name;
            return this;
        }
    }
}
