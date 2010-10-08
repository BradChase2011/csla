﻿//-----------------------------------------------------------------------
// <copyright file="Controller.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Provides methods that respond to HTTP requests</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Provides methods that respond to HTTP requests
  /// in an ASP.NET MVC web site.
  /// </summary>
  public class Controller : System.Web.Mvc.Controller
  {
    /// <summary>
    /// Performs a Save() operation on an
    /// editable business object, with appropriate
    /// validation and exception handling.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="item">The business object to insert.</param>
    /// <param name="forceUpdate">true to force Save() to be an update.</param>
    /// <returns>true the Save() succeeds, false if not.</returns>
    protected virtual bool SaveObject<T>(T item, bool forceUpdate) where T : class, Csla.Core.ISavable
    {
      return SaveObject(item, 
        (i) => UpdateModel(i),
        forceUpdate);
    }

    /// <summary>
    /// Performs a Save() operation on an
    /// editable business object, with appropriate
    /// validation and exception handling.
    /// </summary>
    /// <typeparam name="T">Type of business object.</typeparam>
    /// <param name="item">The business object to insert.</param>
    /// <param name="updateModel">Delegate that invokes the UpdateModel() method.</param>
    /// <param name="forceUpdate">true to force Save() to be an update.</param>
    /// <returns>true the Save() succeeds, false if not.</returns>
    protected virtual bool SaveObject<T>(T item, Action<T> updateModel, bool forceUpdate) where T : class, Csla.Core.ISavable
    {
      try
      {
        ViewData.Model = item;
        if (updateModel != null)
          updateModel(item);
        ViewData.Model = item.Save(forceUpdate);
        return true;
      }
      catch (Csla.DataPortalException ex)
      {
        if (ex.BusinessException != null)
          ModelState.AddModelError("", ex.BusinessException.Message);
        else
          ModelState.AddModelError("", ex.Message);
        return false;
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    /// Loads a property's managed field with the 
    /// supplied value calling PropertyHasChanged 
    /// if the value does change.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="obj">
    /// Object on which to call the method. 
    /// </param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <remarks>
    /// No authorization checks occur when this method is called,
    /// and no PropertyChanging or PropertyChanged events are raised.
    /// Loading values does not cause validation rules to be
    /// invoked.
    /// </remarks>
    protected void LoadProperty<P>(object obj, PropertyInfo<P> propertyInfo, P newValue)
    {
      new ObjectManager().LoadProperty(obj, propertyInfo, newValue);
    }

    private class ObjectManager : Csla.Server.ObjectFactory
    {
      public void LoadProperty<P>(object obj, PropertyInfo<P> propertyInfo, P newValue)
      {
        base.LoadProperty(obj, propertyInfo, newValue);
      }
    }
  }
}