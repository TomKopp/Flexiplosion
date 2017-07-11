using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommonClassesLib.Model;
using FlexiWallUI.Models;
using FlexiWallUI.ViewModels.Interface;

namespace FlexiWallUI.Commands
{ /// <summary>
  /// Command handling actions regarding different window activities:
  /// - switch visibility of property panel
  /// - switch visibility of help overlay
  /// - toggle fullscreen-mode
  /// - minimize window
  /// - switch visibility of log-window
  /// - close application 
  /// The action to be executed is specified by the appropriate straing value as command parameter.
  /// Possible values and associated actions are stored in a dictionary. In case of an invalid command parameter, 
  /// a <see cref="System.NotImplementedException">NotImplementedException</see> is thrown during execution.
  /// </summary>
    public class ApplicationCommand : ICommand
    {
        /// <summary>
        /// <see cref="IFlexiWallApplicationActions"/> which executes the actions associated with the given command parameter.
        /// </summary>
        private readonly IFlexiWallApplicationActions _vm;

        /// <summary>
        /// Dictionary associating the appropriate actions (implemented in given <see cref="IFlexiWallApplicationActions">IFlexiWallApplicationActions</see> to possible command parameters as key-values.
        /// </summary>
        private readonly Dictionary<String, Action> _actions = new Dictionary<string, Action>();

        /// <summary>
        /// Initialization. Command is enabled and Dictionary is initialized.
        /// </summary>
        /// <param name="vm">object responsible to execute the associated actions for each command parameter.</param>
        public ApplicationCommand(IFlexiWallApplicationActions vm)
        {
            _vm = vm;
            InitActions();
        }

        /// <summary>
        /// Adds the 6 possible parametrs and associated actions from <see cref="IFlexiWallApplicationActions">IFlexiWallApplicationActions</see>-interface.
        /// </summary>
        private void InitActions()
        {
            _actions.Add("PropertyPanelVisibility", () => _vm.TogglePropertyPanelVisibility());
            _actions.Add("Help", () => _vm.ToggleHelp());
            _actions.Add("FullScreen", () => _vm.ToggleFullScreen());
            _actions.Add("Minimize", () => _vm.ToggleAppMinimized());
            _actions.Add("ToggleLog", () => _vm.ToggleLogVisibility());
            _actions.Add("Exit", () => _vm.Exit());
            _actions.Add("SB_Forward", () => _vm.Play(0.1));
            _actions.Add("SB_Backward", () => _vm.Play(-0.1));
            _actions.Add("State_Idle", () => _vm.SwitchAppState(FlexiWallAppState.Idle));
            _actions.Add("State_Menu", () => _vm.SwitchAppState(FlexiWallAppState.SelectAppType));
            _actions.Add("State_Maps", () => _vm.SwitchAppState(FlexiWallAppState.ExploreMaps));
            _actions.Add("State_Company", () => _vm.SwitchAppState(FlexiWallAppState.ExploreCompany));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Exectues the desired Action by matching the paramewter with the keys in the action dictionary. If the aparm is null or not a string 
        /// or there is no appropriate key, an appropriate LogMessage is submitted. Additionally, when the  provided parameter is not contained 
        /// in the dictionary an exception is thrown.
        /// </summary>
        /// <param name="parameter">object parameter containing the string representation of th action to be executed.</param> 
        public void Execute(object parameter)
        {
            var s = parameter as String;

            if (String.IsNullOrWhiteSpace(s))
            {
                Log.LogCommandExecuteFailedNoParam(this);
                return;
            }

            if (_actions.ContainsKey(s))
            {
                _actions[s].Invoke();
                Log.LogCommandSucessfullyExecuted(this, s);
                return;
            }

            Log.LogMessage("Tried to execute " + GetType().FullName + " with invalid parameter. Parameter not found in Dictionary.", LoggingLevel.Warning);
            throw new NotImplementedException("Value " + s + " is not connected with an implemented action.");
        }

        public event EventHandler CanExecuteChanged;
    }
}
