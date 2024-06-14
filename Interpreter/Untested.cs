namespace Interpreter
{
    internal class Untested
    {
        ///// <summary>
        /////     Processes the input string.
        ///// </summary>
        ///// <param name="inputString">Input string</param>
        ///// <param name="extension">All Extensions</param>
        //private void ProcessInput(string inputString, ExtensionCommands extension = null)
        //{
        //	(int Status, string Parameter) parameterPart;
        //	List<string> parameter;

        //	//checks if it was an internal Command.
        //	var key = Irt.CheckForKeyWord(inputString, IrtConst.InternCommands);

        //	//Handle Internal Commands
        //	if (key != IrtConst.Error)
        //	{
        //		parameterPart = ProcessParameters(inputString, key, IrtConst.InternCommands);

        //		parameter = parameterPart.Status == IrtConst.ParameterCommand
        //			? Irt.SplitParameter(parameterPart.Parameter, IrtConst.Splitter)
        //			: new List<string> { parameterPart.Parameter };

        //		_irtInternal.HandleInternalCommands(key, parameter, _prompt);
        //	}

        //	if (_com == null)
        //	{
        //		SetError(IrtConst.ErrorNoCommandsProvided);
        //		return;
        //	}

        //	key = Irt.CheckForKeyWord(inputString, _com);

        //	//if Command was not found return error
        //	if (key == IrtConst.Error)
        //	{
        //		SetErrorWithLog(IrtConst.KeyWordNotFoundError, _inputString);
        //		return;
        //	}

        //	parameterPart = ProcessParameters(inputString, key, _com);

        //	parameter = parameterPart.Status == 1
        //		? Irt.SplitParameter(parameterPart.Parameter, IrtConst.Splitter)
        //		: new List<string> { parameterPart.Parameter };

        //	//check for Parameter Overload
        //	var check = Irt.CheckOverload(_com[key].Command, parameter.Count, _com);

        //	if (check == null)
        //	{
        //		SetErrorWithLog(IrtConst.SyntaxError);
        //		return;
        //	}

        //	//add optional Extension data
        //	SetResult((int)check, parameter, extension);
        //}
    }
}