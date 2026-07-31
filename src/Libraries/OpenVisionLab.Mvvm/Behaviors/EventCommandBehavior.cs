using System;
using System.Linq;
using LinqExpression = System.Linq.Expressions.Expression;
using MethodCallExpression = System.Linq.Expressions.MethodCallExpression;
using ParameterExpression = System.Linq.Expressions.ParameterExpression;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace OpenVisionLab.Mvvm.Behaviors
{
    /// <summary>
    /// Attached behavior for replacing simple code-behind event forwarding with ICommand bindings in XAML.
    /// </summary>
    public static class EventCommandBehavior
    {
        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.RegisterAttached(
                "EventName",
                typeof(string),
                typeof(EventCommandBehavior),
                new PropertyMetadata(null, OnEventNameChanged));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(EventCommandBehavior),
                new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(EventCommandBehavior),
                new PropertyMetadata(null));

        public static readonly DependencyProperty PassEventArgsToCommandProperty =
            DependencyProperty.RegisterAttached(
                "PassEventArgsToCommand",
                typeof(bool),
                typeof(EventCommandBehavior),
                new PropertyMetadata(false));

        private static readonly DependencyProperty EventSubscriptionProperty =
            DependencyProperty.RegisterAttached(
                "EventSubscription",
                typeof(EventSubscription),
                typeof(EventCommandBehavior),
                new PropertyMetadata(null));

        public static string GetEventName(DependencyObject target)
        {
            return (string)target.GetValue(EventNameProperty);
        }

        public static void SetEventName(DependencyObject target, string value)
        {
            target.SetValue(EventNameProperty, value);
        }

        public static ICommand GetCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static object GetCommandParameter(DependencyObject target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }

        public static bool GetPassEventArgsToCommand(DependencyObject target)
        {
            return (bool)target.GetValue(PassEventArgsToCommandProperty);
        }

        public static void SetPassEventArgsToCommand(DependencyObject target, bool value)
        {
            target.SetValue(PassEventArgsToCommandProperty, value);
        }

        private static EventSubscription GetEventSubscription(DependencyObject target)
        {
            return (EventSubscription)target.GetValue(EventSubscriptionProperty);
        }

        private static void SetEventSubscription(DependencyObject target, EventSubscription value)
        {
            target.SetValue(EventSubscriptionProperty, value);
        }

        private static void OnEventNameChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            EventSubscription previous = GetEventSubscription(target);
            previous?.Detach();
            SetEventSubscription(target, null);

            string eventName = e.NewValue as string;
            if (string.IsNullOrWhiteSpace(eventName))
            {
                return;
            }

            EventInfo eventInfo = target.GetType().GetEvent(eventName, BindingFlags.Instance | BindingFlags.Public);
            if (eventInfo == null)
            {
                throw new InvalidOperationException($"Event '{eventName}' was not found on '{target.GetType().Name}'.");
            }

            var subscription = new EventSubscription(target, eventInfo);
            subscription.Attach();
            SetEventSubscription(target, subscription);
        }

        private static void InvokeCommand(DependencyObject target, object eventArgs)
        {
            ICommand command = GetCommand(target);
            if (command == null)
            {
                return;
            }

            object parameter = GetPassEventArgsToCommand(target) ? eventArgs : GetCommandParameter(target);
            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }

        private sealed class EventSubscription
        {
            private readonly DependencyObject _target;
            private readonly EventInfo _eventInfo;
            private Delegate _handler;

            public EventSubscription(DependencyObject target, EventInfo eventInfo)
            {
                _target = target;
                _eventInfo = eventInfo;
            }

            public void Attach()
            {
                _handler = CreateHandler(_eventInfo.EventHandlerType);
                _eventInfo.AddEventHandler(_target, _handler);
            }

            public void Detach()
            {
                if (_handler != null)
                {
                    _eventInfo.RemoveEventHandler(_target, _handler);
                    _handler = null;
                }
            }

            private Delegate CreateHandler(Type handlerType)
            {
                MethodInfo invokeMethod = handlerType.GetMethod("Invoke");
                ParameterInfo[] parameters = invokeMethod.GetParameters();
                if (parameters.Length != 2)
                {
                    throw new InvalidOperationException($"Event '{_eventInfo.Name}' must use a two-parameter handler.");
                }

                ParameterExpression[] expressions = parameters
                    .Select(parameter => LinqExpression.Parameter(parameter.ParameterType, parameter.Name))
                    .ToArray();

                MethodInfo invokeCommand = typeof(EventCommandBehavior).GetMethod(
                    nameof(InvokeCommand),
                    BindingFlags.Static | BindingFlags.NonPublic);

                MethodCallExpression body = LinqExpression.Call(
                    invokeCommand,
                    LinqExpression.Constant(_target),
                    LinqExpression.Convert(expressions[1], typeof(object)));

                return LinqExpression.Lambda(handlerType, body, expressions).Compile();
            }
        }
    }
}
