﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TappauMVVM.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TappauMVVM.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Neither the canExecuteMethod nor the executeMethod delegates can be null..
        /// </summary>
        internal static string CommandDelegatesCannotBeNull {
            get {
                return ResourceManager.GetString("CommandDelegatesCannotBeNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to T for Command&lt;T&gt; is not an object nor Nullable..
        /// </summary>
        internal static string CommandInvalidGenericPayloadType {
            get {
                return ResourceManager.GetString("CommandInvalidGenericPayloadType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Property must be a MemberExpression.
        /// </summary>
        internal static string ExpressionExtensions_PropertyMustBeMemberExpression {
            get {
                return ResourceManager.GetString("ExpressionExtensions_PropertyMustBeMemberExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Property to query cannot be null, empty or whitespace.
        /// </summary>
        internal static string ObservableObject_PropertyValueInvalid {
            get {
                return ResourceManager.GetString("ObservableObject_PropertyValueInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is already being observed..
        /// </summary>
        internal static string PropertyExpressionAlreadyObserved {
            get {
                return ResourceManager.GetString("PropertyExpressionAlreadyObserved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Trying to subscribe PropertyChanged listener in object that owns &apos;{0}&apos; property, but the object does not implements INotifyPropertyChanged..
        /// </summary>
        internal static string PropertyObserver_PropertyDoesNotImplementINotifyPropertyChanged {
            get {
                return ResourceManager.GetString("PropertyObserver_PropertyDoesNotImplementINotifyPropertyChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation not supported for the given expression type. Only MemberExpression and ConstantExpression are currently supported..
        /// </summary>
        internal static string PropertyObserverOperationNotSupported {
            get {
                return ResourceManager.GetString("PropertyObserverOperationNotSupported", resourceCulture);
            }
        }
    }
}